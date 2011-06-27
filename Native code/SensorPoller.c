// You might need to change this depending on the location of the header file
#include <RLP.h>
#include <lpc23xx.h>

#if 1
#define RLP_ADDRESS		0x40000440
#define RLP_SIZE		0x000027FC

typedef int bool;
typedef unsigned char byte;

#define P0MASK   (1 << 0)
#define P1MASK   (1 << 1)
#define P2MASK   (1 << 2)
#define P3MASK   (1 << 3)
#define P4MASK   (1 << 4)
#define P5MASK   (1 << 5)
#define P6MASK   (1 << 6)
#define P7MASK   (1 << 7)
#define P8MASK   (1 << 8)
#define P9MASK   (1 << 9)
#define P10MASK  (1 << 10)
#define P11MASK  (1 << 11)
#define P12MASK  (1 << 12)
#define P13MASK  (1 << 13)
#define P14MASK  (1 << 14)
#define P15MASK  (1 << 15)
#define P16MASK  (1 << 16)
#define P17MASK  (1 << 17)
#define P18MASK  (1 << 18)
#define P19MASK  (1 << 19)
#define P20MASK  (1 << 20)
#define P21MASK  (1 << 21)
#define P22MASK  (1 << 22)
#define P23MASK  (1 << 23)
#define P24MASK  (1 << 24)
#define P25MASK  (1 << 25)
#define P26MASK  (1 << 26)
#define P27MASK  (1 << 27)
#define P28MASK  (1 << 28)
#define P29MASK  (1 << 29)
#define P30MASK  (1 << 30)
#define P31Mask  (1 << 31)

#define DI { /* RLPext->Interrupt.GlobalInterruptDisable(); */ }
#define EI { /* RLPext->Interrupt.GlobalInterruptEnable(); */ }


#define TIMERFREQ 18000000
#define TICKS_PER_US 18
#define TICKS_PER_S TIMERFREQ
#define CM_PER_METER 100


#define CARRIER_FREQUENCY_HZ 40000L 
#define CARRIER_PERIOD_TICKS (TICKS_PER_S / CARRIER_FREQUENCY_HZ)

#define USEC(x) (TICKS_PER_US * x)


bool expired(unsigned long expireTime)
{
	int diff =(expireTime - T0TC);
	return (diff < 0);
}

void delay_until(unsigned long expireTime)
{
	while (!expired(expireTime)) ;
}

unsigned long p0InfraredMask;
unsigned long p1InfraredMask;
unsigned long p2InfraredMask;
unsigned long ultrasoundMask;

enum eState { WAITING_FOR_INITIAL_STATE = 0, 
		WAITING_FOR_START_TRANSITION = 1,
		WAITING_FOR_END_TRANSITION= 2,
		WAIT_COMPLETED = 3
};

#define INFRARED_COUNT 16
#define ULTRASOUND_COUNT 4
#define SENSOR_COUNT (INFRARED_COUNT + ULTRASOUND_COUNT)
#define PORT_COUNT 3

unsigned long lastPortReading[PORT_COUNT];		// the last reading on this port.
unsigned long activeMasks[PORT_COUNT];			// the masks of pins we're actively looking for timings on
unsigned long samplingMasks[PORT_COUNT];		// the masks of pins we're looking to begin sampling on.
unsigned long lastSampleTime;

byte sampling[SENSOR_COUNT];		// 0 => not started, 1=>waiting for start transition, 2=>waiting for end transition
unsigned long startTimes[SENSOR_COUNT];
unsigned long endTimes[SENSOR_COUNT];
byte remainingToCapture;
byte remainingToSample;

#define soundSpeedAt0C  331.29f
#define soundSpeedPerDegree  0.606f
float speedOfSound = soundSpeedAt0C + soundSpeedPerDegree * 25;

const unsigned long Masks[SENSOR_COUNT] = 
{
	P8MASK, P9MASK, P10MASK, P11MASK,	// the port 0 bits, start at 0
	P0MASK, P1MASK, P4MASK, P8MASK,	// the port 1 ultrasound bits, start at 4
	P22MASK, P23MASK, P24MASK, P25MASK, P26MASK, P27MASK, P28MASK, P29MASK, // the port 1 IR bits, start at 8
	P0MASK, P1MASK, P2MASK, P7MASK		// the port 2 IR bits, start at 16
};

const unsigned long ActiveStateMask[SENSOR_COUNT] = 
{
	0, 0, 0, 0,
	P0MASK, P1MASK, P4MASK, P8MASK,
	0, 0, 0, 0, 0, 0, 0, 0,
	0, 0, 0, 0
};

const unsigned long InactiveStateMask[SENSOR_COUNT] = 
{
	P8MASK, P9MASK, P10MASK, P11MASK,	// the port 0 bits, start at 0
	0, 0, 0, 0,	// the port 1 ultrasound bits, start at 4
	P22MASK, P23MASK, P24MASK, P25MASK, P26MASK, P27MASK, P28MASK, P29MASK, // the port 1 IR bits, start at 8
	P0MASK, P1MASK, P2MASK, P7MASK		// the port 2 IR bits, start at 16
};

const byte portIndexes[SENSOR_COUNT] = 
{
	0, 0, 0, 0,
	1, 1, 1, 1,
	1, 1, 1, 1, 1, 1, 1, 1,
	2, 2, 2, 2
};


const byte portFirstIndexes[PORT_COUNT+1] = 
{
	0, 4, 16, 20
};

// this stores the mapping to in order infrared sensors.
const byte irMap[INFRARED_COUNT] = 
{
	0, 17, 1, 16, 3, 18, 2, 19,
	15, 14, 13, 12, 11, 10, 9, 8
};

// this stores the mapping to in order ultrasound sensors.
const byte usoundMap[ULTRASOUND_COUNT] = 
{
	5, 4, 7, 6
};

void one_time_sensor_init()
{
}

void process_readings()
{
	unsigned long scratch[PORT_COUNT];
	unsigned long tmpMask;
	int e, i, j;

	lastSampleTime = T0TC;
	scratch[0] = FIO0PIN;
	scratch[1] = FIO1PIN;
	scratch[2] = FIO2PIN;
	

	if (remainingToSample)
	{
		// see if we've hit the initial state first...
		for (i = 0; i<SENSOR_COUNT;i++)
		{
			if (sampling[i] != WAITING_FOR_INITIAL_STATE) continue;
			j = portIndexes[i];
			tmpMask = Masks[i];
			if (samplingMasks[j] & tmpMask) // check to see if we match the ith sensor
			{
				// this is a port we should be looking at.
				if ((scratch[j] & tmpMask) == InactiveStateMask[i])
				{
					// we can begin sampling this item.
					remainingToSample--;
					activeMasks[j] |= tmpMask;
					lastPortReading[j] |= InactiveStateMask[i];
					sampling[i] = WAITING_FOR_START_TRANSITION;
				}
			}
		}
	}

	for (j = 0; j<PORT_COUNT && remainingToCapture;j++)
	{
		scratch[j] &= activeMasks[j];
		
		if (scratch[j] != lastPortReading[j])
		{
			e = portFirstIndexes[j+1];
			// something changed on this port
			for (i= portFirstIndexes[j];i<e;i++)
			{
				tmpMask = Masks[i];
				if (sampling[i]==WAITING_FOR_START_TRANSITION)
				{
					if ((tmpMask & scratch[j]) == ActiveStateMask[i])
					{
						// we've transitioned to the end time.
						startTimes[i] = lastSampleTime;
						sampling[i] = WAITING_FOR_END_TRANSITION;
					}
				}
				else if (sampling[i] == WAITING_FOR_END_TRANSITION)
				{
					if ((tmpMask & scratch[j]) == InactiveStateMask[i])
					{
						sampling[i]  = WAIT_COMPLETED;
						endTimes[i] = lastSampleTime;
						activeMasks[j] &= ~tmpMask;		// mark this one as done.
						remainingToCapture--;
					}
				}
			}
			lastPortReading[j] = scratch[j];	// store for next time.
		}
	}
}

int acquire_data(byte *resultsArray, void **args, unsigned int argsCount, unsigned int *argSize)
{
	// generalArray[0] on input might be temp in degrees C (if we use it)
	// generalArray must be 20 bytes long or longer.
	// on return [0..15] filled with number of carrier frequency pulses on IR sensors
	//           [16..19] filled with cms from ultrasound sensors
	byte *results = resultsArray;
	int i,j;
	unsigned long expiryTime;
	p0InfraredMask = (P12MASK-1)& ~(P8MASK-1);		// pins 8-11 inclusive
	p1InfraredMask = (P30MASK-1)& ~(P22MASK-1);	// pins 22-29 inclusive;
	p2InfraredMask = (P0MASK | P1MASK | P2MASK | P7MASK);
	ultrasoundMask = (P0MASK | P1MASK | P4MASK | P8MASK);

	DI
		FIO1DIR = FIO1DIR | ultrasoundMask;
	EI

	FIO1SET = ultrasoundMask;	// pulse high
	expiryTime = T0TC + USEC(10);	// stash expiry time.

	// do some more processing, initialization
	for (i=0;i<SENSOR_COUNT;i++)
	{
		startTimes[i] = endTimes[i] = 0;
		sampling[i] = WAITING_FOR_INITIAL_STATE;
	}
	samplingMasks[0] = p0InfraredMask;
	samplingMasks[1] = p1InfraredMask;
	samplingMasks[2] = p2InfraredMask;	// start acquiring infrared.

	lastPortReading[0] = lastPortReading[1] = lastPortReading[2];

	remainingToCapture = 20;
	remainingToSample = 20;
		
#if 0
	while (!expired(expiryTime))
	{
		process_readings();		
		// this will process some ir values while we're waiting for the
		// ultrasound to get set up.
	}
#endif


	FIO1CLR = ultrasoundMask;	// pulse low.
	DI
		FIO1DIR = FIO1DIR & ~ultrasoundMask;
	EI

	expiryTime += USEC(38000L);	// we allow everything up to 38 msec to 
	samplingMasks[1] |= ultrasoundMask;	// start waiting for ultrasound value.

	while (!expired(expiryTime) && remainingToCapture)
	{
		process_readings();	
	}

	// finish processing.
	for (i = 0; i<INFRARED_COUNT;i++)
	{
		j = irMap[i];
		if (sampling[j] != WAIT_COMPLETED) *results++ = 0;
		else
		{
			expiryTime = endTimes[j] - startTimes[j];
			*results++ = expiryTime/ CARRIER_PERIOD_TICKS;
		}
	}

	for (i=0;i<ULTRASOUND_COUNT;i++)
	{
		j = usoundMap[i];
		if (sampling[j] != WAIT_COMPLETED) *results++ = 255;
		else
		{
			expiryTime = endTimes[j] - startTimes[j];
			*results++ = (byte)
				(expiryTime * speedOfSound * CM_PER_METER / TICKS_PER_S / 2);
		}
	}

	return 0;
}
#endif


int Acquire_test(unsigned long  *generalArray, void **args, unsigned int argsCount, unsigned int *argSize)
{
	int t0 = T0TC;
	unsigned long expiryTime = t0 + USEC(20);	// stash expiry time.

	while (!expired(expiryTime));
	
	return T0TC - t0;
}

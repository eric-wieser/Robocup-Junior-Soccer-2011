/*
 *	Copyright (C) GHI Electronics, LLC.  All rights reserved.
 *	RLP support files.
 *  This file should not be changed under any condition.
 */

#ifndef _USER_RLP_H_
#define _USER_RLP_H_

// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!   The user must set these settings according to the used platform    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// ChipworkX
//#define RLP_ADDRESS		0xA0000000
//#define RLP_SIZE		0x001FB3FC

// EMX
//#define RLP_ADDRESS		0xA0F00000
//#define RLP_SIZE		0x000FFFFC

// USBizi
//#define RLP_ADDRESS		0x40000440
//#define RLP_SIZE		0x000027FC
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

// Definitions
#define RLP_TRUE  1
#define RLP_FALSE 0

// GPIO Definitions
#define RLP_GPIO_NONE  0xFFFFFFFF
#define RLP_GPIO_CWX_PORTA(x) x			// ChipworkX
#define RLP_GPIO_CWX_PORTB(x) (x+32)	// ChipworkX
#define RLP_GPIO_CWX_PORTC(x) (x+64)	// ChipworkX

// GPIO Interrupt Edge
#define RLP_GPIO_INT_NONE        0
#define RLP_GPIO_INT_EDGE_LOW    1
#define RLP_GPIO_INT_EDGE_HIGH   2
#define RLP_GPIO_INT_EDGE_BOTH   3
#define RLP_GPIO_INT_LEVEL_HIGH  4 
#define RLP_GPIO_INT_LEVEL_LOW   5 

// GPIO Resistor State 
#define RLP_GPIO_RESISTOR_DISABLED 0
#define RLP_GPIO_RESISTOR_PULLDOWN 1 // Not supported on ChipworkX hardware
#define RLP_GPIO_RESISTOR_PULLUP 2

// RLP Extensions	////////////////////////////////////////////////////////////////////////////////////////////////
typedef void (*RLP_CALLBACK_FPN)( void* arg );
typedef void (*RLP_GPIO_INTERRUPT_SERVICE_ROUTINE)( unsigned int Pin, unsigned int PinState, void* Param );

#define RLP_TASK_DATA_SIZE	32

typedef struct 
{
	unsigned int __data[RLP_TASK_DATA_SIZE/4];
} RLP_Task;


typedef struct
{
	unsigned int GlitchFilterEnable;
	unsigned int IntEdge;
	unsigned int ResistorState;
}RLP_InterruptInputPinArgs;

typedef struct 
{
	unsigned int magic;					// Defined below
	unsigned int firmwareVersion;		
	unsigned int magicSize;
	struct
	{
		unsigned int (*Install)(unsigned int Irq_Index, RLP_CALLBACK_FPN ISR, void* ISR_Param );
		unsigned int (*Uninstall)( unsigned int Irq_Index );
		unsigned int (*Disable)( unsigned int Irq_Index );
		unsigned int (*Enable)( unsigned int Irq_Index );
		unsigned int (*IsEnabled)( unsigned int Irq_Index );
		unsigned int (*IsPending)( unsigned int Irq_Index );
		unsigned int (*GlobalInterruptDisable)();
		unsigned int (*GlobalInterruptEnable)();
		unsigned int (*IsGlobalInterruptEnabled)();

	} Interrupt;

	struct
	{
		void		 (*EnableOutputMode)( unsigned int Pin, unsigned int InitialState );
		unsigned int (*EnableInputMode) ( unsigned int Pin, unsigned int ResistorState);
		unsigned int (*EnableInterruptInputMode) (unsigned int Pin, RLP_InterruptInputPinArgs *args, RLP_GPIO_INTERRUPT_SERVICE_ROUTINE ISR, void* ISR_Param);
		unsigned int (*ReadPin)( unsigned int Pin);
		void		 (*WritePin)( unsigned int Pin,  unsigned int PinState );
		unsigned int (*ReservePin)( unsigned int Pin, unsigned int reserve );
		unsigned int (*IsReserved)( unsigned int Pin);
	} GPIO;
	
	void* (*malloc)(unsigned int len);
	void (*free)(void *ptr);
	void (*Delay)(unsigned int microSeconds);

	void* (*malloc_CustomHeap)(unsigned int len);
	void (*free_CustomHeap)(void *ptr);

	struct
	{
		void (*Initialize)(RLP_Task *task, RLP_CALLBACK_FPN taskCallback, void* arg, unsigned int isKernelMode);
		void (*Schedule)(RLP_Task *task);
		void (*ScheduleTimeOffset)(RLP_Task *task, unsigned int timeOffset_us);
		void (*Abort)(RLP_Task *task);
		unsigned int (*IsScheduled)(RLP_Task *task);

	} Task;

	void (*PostManagedEvent)(unsigned int data);

} RLPext_T;

#define RLP_EXT_MAGIC		0x73DE1BEA
#define RLPext (*((RLPext_T**)(RLP_ADDRESS + RLP_SIZE)))
#define RLP_MAGIC_SIZE (sizeof(RLPext_T))
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#endif	// _USER_RLP_H_


using System;
using Microsoft.SPOT;

namespace Technobotts.Hardware
{
	class SyncedUltrasonicSensors
	{
		public UltrasonicSensor[] Sensors { get; private set; }
		private long[] pulseStart;
		private long[] pulseEnd;
		public Double[] Readings { get; private set; }
		private long _timeout = 50;

		public SyncedUltrasonicSensors(params UltrasonicSensor[] sensors)
		{
			Sensors = sensors;
			pulseStart = new long[Sensors.Length];
			pulseEnd = new long[Sensors.Length];
		}

		protected void SendPings()
		{
			for (int i = 0; i < Sensors.Length; i++)
			{
				pulseStart[i] = 0;
				pulseEnd[i] = 0;
			}
			/*
			for (int i = 0; i < Sensors.Length; i++)
			{
				Sensors[i].InnerPort.Active = true;
				Sensors[i].InnerPort.Write(true);
			}
			for (int i = 0; i < Sensors.Length; i++)
			{
				Sensors[i].InnerPort.Write(false);
				Sensors[i].InnerPort.Active = false;
			}*/
			Sensors[1].SendPing();
		}

		protected double[] AwaitResponses()
		{
			long cancelWait = System.Environment.TickCount + _timeout;

			int noBounce = Sensors.Length;
			int j = 0;
			while (noBounce > 2 /*&& (j++ % 100 != 90000 || System.Environment.TickCount < cancelWait)*/)
			{
				for (int i = 0; i < Sensors.Length; i++)
				{
					if (pulseEnd[i] != 0) continue;

					bool high = Sensors[i].InnerPort.Read();

					if (pulseStart[i] == 0 && high) pulseStart[i] = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
					else if (pulseStart[i] != 0 && !high)
					{
						pulseEnd[i] = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
						noBounce--;
					}
				}
			}

			double[] readings = new double[Sensors.Length];
			for (int i = 0; i < Sensors.Length; i++)
			{
				readings[i] = Sensors[i].CalculateDistance(pulseEnd[i] - pulseStart[i]);
			}
			return readings;
		}

		public void Update()
		{
			SendPings();
			Readings = AwaitResponses();
		}

		public DistanceUnits Unit
		{
			set
			{
				foreach (UltrasonicSensor sensor in Sensors)
				{
					sensor.Unit = value;
				}
			}
		}
	}
}

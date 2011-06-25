using System;
using System.Threading;
using System.Diagnostics;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;

namespace Technobotts.Hardware
{
	public delegate void SignalBounce(long delay); 

	public enum DistanceUnits
	{
		mm,
		cm,
		dm,
		m,
		feet,
		inch,
		yard
	}

	public class UltrasonicSensor
	{
		public TristatePort InnerPort;

		const double _soundSpeed = 331.3;
		const double _soundSpeedPerDegree = 0.606;
		double _metresPerTick;

		private long _timeout = 50;

		public UltrasonicSensor(Cpu.Pin pin)
		{
			Unit = DistanceUnits.m;
			OperatingTemperature = 20;
			InnerPort = new TristatePort(pin, false, false, Port.ResistorMode.Disabled);

			int initTick = TechnobottsRTC.Ticks;
			Thread.Sleep(50);
			Debug.Print("" + TechnobottsRTC.TickDiffSeconds(initTick));

		}

		public void SendPing()
		{
			//Set it to an output
			InnerPort.Active = true;

			//Send a quick HIGH pulse
			InnerPort.Write(true);
			InnerPort.Write(false);

			// Set it as an input
			InnerPort.Active = false;
		}


		private double _timoutSecs = 0.050;
		protected double AwaitResponse()
		{
			int initTick = TechnobottsRTC.Ticks;
			int highTime = 0;
			while (TechnobottsRTC.TickDiffSeconds(initTick) < _timoutSecs)
			{
				if (InnerPort.Read())
				{	// is the level high?  If so store the tick it started.
					if (highTime == 0)
					{
						highTime = TechnobottsRTC.Ticks;
					}
				}
				else
				{

					if (highTime != 0)
					{
						return TechnobottsRTC.TickDiffSeconds(highTime);
					}
				}
			}
			return DoubleEx.NaN;	// timeout		
		}

		internal Double CalculateDistance(double seconds)
		{
			return Convert(seconds * _metresPerTick / 2, Unit);
		}

#if true
		protected long AwaitResponseOLD()
		{
			long cancelWait = System.Environment.TickCount + _timeout;

			long pulseStart = 0;
			long pulseEnd = 0;

			while (System.Environment.TickCount < cancelWait)
			{
				bool high = InnerPort.Read();

				if (pulseStart == 0 && high) pulseStart = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
				else if (pulseStart != 0 && !high)
				{
					pulseEnd = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
					break;
				}
			}

			return pulseEnd - pulseStart;
		}
#else
		public long AwaitResponse()
		{
    		long pulseStart = 0;
    		long pulseEnd = 0;
    
    		while (true)
    		{
				bool high = InnerPort.Read();

    			if (pulseStart == 0 && high)
    				pulseStart = System.DateTime.Now.Ticks;
    			else if (pulseStart != 0 && !high)
    			{
    				pulseEnd = System.DateTime.Now.Ticks;
    				break;
    			}
    		}
    
    		return pulseEnd - pulseStart;
		}
#endif

		internal Double CalculateDistanceOLD(long timeInterval)
		{
			return timeInterval < 0 ? DoubleEx.NaN : Convert(timeInterval * _metresPerTick / 2, Unit);
		}

		/// <summary>
		/// Return the Ping))) sensor's reading in millimeters.
		/// </summary>
		/// <param name="usedefault">Set true to return value in the unit specified by the "Unit" property.
		/// Set false to return value in mm.</param>
		public double GetDistance()
		{
			SendPing();
			return CalculateDistance(AwaitResponse());
		}

		/// <summary>
		/// Convert the millimeters into other units.
		/// </summary>
		/// <param name="metres">The Ping))) sensor's m reading.</param>
		/// <param name="outputUnit">The desired output unit.</param>
		public double Convert(double metres, DistanceUnits outputUnit)
		{
			double result = metres;

			switch (outputUnit)
			{
				case DistanceUnits.mm:
					result = metres * 1000F;
					break;
				case DistanceUnits.cm:
					result = metres * 100F;
					break;
				case DistanceUnits.dm:
					result = metres * 10F;
					break;
				case DistanceUnits.inch:
					result = metres * 39.3700787;
					break;
				case DistanceUnits.feet:
					result = metres * 3.2808399;
					break;
				case DistanceUnits.yard:
					result = metres * 1.0936133;
					break;
			}

			return result;
		}

		public DistanceUnits Unit { get; set; }
		
		private double _operatingTemperature = 0;
		public double OperatingTemperature {
			set {
				_operatingTemperature = value;
				double speed = _soundSpeed + _soundSpeedPerDegree * _operatingTemperature;
				_metresPerTick = speed; /// TimeSpan.TicksPerSecond;
			}
			get {
				return _operatingTemperature;
			}
		}
	}
}
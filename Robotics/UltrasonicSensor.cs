using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Technobotts.Robotics
{
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
        TristatePort _port;

        const double _soundSpeed = 331.3;
		const double _soundSpeedPerDegree = 0.606;
		double _metresPerTick;

		public UltrasonicSensor(Cpu.Pin pin)
        {
			Unit = DistanceUnits.m;
			OperatingTemperature = 20;
            _port = new TristatePort(pin, false, false, Port.ResistorMode.Disabled);
        }

        /// <summary>
        /// Return the Ping))) sensor's reading in millimeters.
        /// </summary>
        /// <param name="usedefault">Set true to return value in the unit specified by the "Unit" property.
        /// Set false to return value in mm.</param>
        public double GetDistance()
        {
            long t1, t2;

            // Set it to an putput
            _port.Active = true;

            //Send a quick HIGH pulse
            _port.Write(true);
            _port.Write(false);

            // Set it as an input
            _port.Active = false;  

			//Wait till port is high
            while (!_port.Read());
            t1 = System.DateTime.Now.Ticks;

			//Wait till port is low
            while (_port.Read()); 
            t2 = System.DateTime.Now.Ticks;

			long deltaT = t2 - t1;

			return Convert(deltaT / 2 * _metresPerTick, Unit);
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
				_metresPerTick = speed / TimeSpan.TicksPerSecond;
			}
			get {
				return _operatingTemperature;
			}
		}
    }
}
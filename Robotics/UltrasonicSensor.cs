using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

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

    public class Ping
    {
        TristatePort _port;

        const double _soundSpeed = 331.3;
		const double _soundSpeedPerDegrees = 0.606;
		double _metresPerTick;
 
        public Ping(Cpu.Pin pin)
        {
			Unit = DistanceUnits.mm
            _port = new TristatePort(pin, false, false, ResistorModes.Disabled);
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

            return Convert((t2 - t1) * _metresPerTick, Unit);
        }

        /// <summary>
        /// Convert the millimeters into other units.
        /// </summary>
        /// <param name="millimeters">The Ping))) sensor's mm reading.</param>
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
                    result = meters * 39.3700787;
                    break;
                case DistanceUnits.feet:
                    result = meters * 3.2808399;
                    break;
                case DistanceUnits.yard:
                    result = meters * 1.0936133;
                    break;
            }

            return result;
        }

        public DistanceUnits Unit { get; set; }
		
		private double _operatingTemperature = 20;
		public double OperatingTemperature {
			set {
				_operatingTemperature = value;
				double speed = _soundSpeed + _soundSpeedPerDegree * _operatingTemperature;
				_distancePerTick = speed / TimeSpan.TicksPerSecond;
			}
			get {
				return _operatingTemperature;
			}
		}
    }
}
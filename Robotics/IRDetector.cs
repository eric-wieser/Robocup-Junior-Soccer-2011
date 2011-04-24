using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using Technobotts.Hardware;

namespace Technobotts.Robotics
{
	class IRDetector : PwmIn, IIntensityDetector
	{
		public IRDetector(FEZ_Pin.Interrupt pin) : base(pin) { }

		public double Intensity {
			get {
				return 1 - DutyCycle;
			}
		}
	}
}

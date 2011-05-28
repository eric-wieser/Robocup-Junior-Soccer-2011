using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Technobotts.Hardware
{
	class HiTechnicCompassSensor : I2CDevice
	{
		public const ushort DefaultAddress = 0x02;

		private const byte HeadingRegister = 0x44;

		private const byte CommandRegister = 0x41;
		private const byte CalibrateMode = 0x43;
		private const byte MeasurementMode = 0x00;

		private static I2CTransaction[] _readHeading = new I2CTransaction[] {
			CreateWriteTransaction(new byte[] {HeadingRegister}),
			CreateReadTransaction(new byte[2]),
		};

		private static I2CTransaction[] _startCalibration = new I2CTransaction[] {
			CreateWriteTransaction(new byte[] {CommandRegister, CalibrateMode})
		};

		private static I2CTransaction[] _stopCalibration = new I2CTransaction[] {
			CreateWriteTransaction(new byte[] {CommandRegister, MeasurementMode})
		};

		public HiTechnicCompassSensor(ushort address = DefaultAddress) :
			base(new Configuration(address, 10))
		{ }

		public double Heading
		{
			get
			{
				if (Calibrating) throw new InvalidOperationException("Can't read while Calibrating");
				int transferred = Execute(_readHeading, 500);
				if (transferred != 0)
				{
					byte[] data = _readHeading[1].Buffer;
					return data[0] | data[1] << 8;
				}
				else
					return Double.NaN;
			}
		}

		private bool _calibrating = false;
		public bool Calibrating
		{
			get { return _calibrating; }
			set
			{
				if (value && !_calibrating)
					_calibrating = Execute(_startCalibration, 500) != 0;
				else if (!value && _calibrating)
					_calibrating = Execute(_stopCalibration, 500) == 0;
			}
		}
	}
}

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Technobotts.Geometry;
using Technobotts.Robotics;

namespace Technobotts.Hardware
{
	public class HMC6352 : I2CDevice, AngleFinder
	{
		public const ushort DefaultAddress = 0x21;
		public const int ClockSpeed = 100;

		#region Enum constants
			public enum Command
			{
				WriteEEPROM = 'w',
				ReadEEPROM = 'r',
				WriteRAM = 'G',
				ReadRAM = 'g',
				Sleep = 'S',
				Wake = 'W',
				UpdateOffsets = 'O',
				StartCalibration = 'C',
				EndCalibration = 'E',
				SaveMode = 'L',
				CalculateHeading = 'A'
			}

			public enum EEPROMLocation
			{
				SlaveAddress = 0x00,
				MagnetometerXOffsetMSB = 0x01,
				MagnetometerXOffsetLSB = 0x02,
				MagnetometerYOffsetMSB = 0x03,
				MagnetometerYOffsetLSB = 0x04,
				TimeDelay = 0x05,
				MeasurementCount = 0x06,
				SoftwareVersion = 0x07,
				OperationMode = 0x08
			}

			public enum RAMLocation
			{
				OperationMode = 0x74,
				Output = 0x4E
			}
		#endregion

		public class OperationConfiguration
		{
			static readonly uint[] MeasurementRates = new uint[] { 1, 5, 10, 20 };

			public enum OperationMode
			{
				Standby = 0x00,
				Query = 0x01,
				Continuous = 0x02
			}

			public OperationConfiguration()
			{
				Mode = OperationMode.Standby;
			}

			public OperationMode Mode { get; set; }
			public bool PeriodicOffsetUpdate { get; set; }

			private int _measurementRate = 0;
			public uint MeasurementRate
			{
				set
				{
					for (int i = 0; i < MeasurementRates.Length; i++)
					{
						if (value == MeasurementRates[i])
						{
							_measurementRate = i;
							return;
						}
					}
					throw new ArgumentOutOfRangeException("value", "Must be a supported measurement rate");
				}
				get
				{
					return MeasurementRates[_measurementRate];
				}
			}
			public byte toI2CData()
			{
				if (Mode == OperationMode.Continuous)
					return (byte)(_measurementRate << 5 | (PeriodicOffsetUpdate ? 1 : 0) << 4 | (byte)Mode);
				else
					return (byte)Mode;
			}
		}

		private OperationConfiguration _config;
		public OperationConfiguration OperationConfig {
			get { return _config; }
			set {
				writeRAM(RAMLocation.OperationMode, value.toI2CData());
				_config = value;
			}
		}

		public HMC6352(ushort address = DefaultAddress) :
			base(new Configuration(address, ClockSpeed))
		{
			OperationConfig = new OperationConfiguration();
		}

		#region Reading sensor output

		public enum OutputModeType
		{
			Heading = 0x00,
			RawMagnetometerX = 0x01,
			RawMagnetometerY = 0x02,
			MagnetometerX = 0x03,
			MagnetometerY = 0x04
		}

		private OutputModeType _outputMode = OutputModeType.Heading;
		public OutputModeType OutputMode
		{
			get { return _outputMode; }
			private set
			{
				if (value != _outputMode)
				{
					writeRAM(RAMLocation.Output, (byte)value);
					_outputMode = value;
				}
			}
		}

		public double Heading
		{
			get
			{
				OutputMode = OutputModeType.Heading;

				return getOutput() / 10.0;
			}
		}

		public double Angle
		{
			get
			{
				return Heading / 180.0 * System.Math.PI;
			}
		}

		public Vector MagnetometerReading
		{
			get
			{
				OutputMode = OutputModeType.MagnetometerX;
				double x = getOutput();
				OutputMode = OutputModeType.MagnetometerY;
				double y = getOutput();

				return new Vector(x, y);
			}
		}
		public Vector RawMagnetometerReading
		{
			get
			{
				OutputMode = OutputModeType.RawMagnetometerX;
				double x = getOutput();
				OutputMode = OutputModeType.RawMagnetometerY;
				double y = getOutput();

				return new Vector(x, y);
			}
		}

		private I2CTransaction[] outputTransaction = new I2CTransaction[] {
			CreateReadTransaction(new byte[2])
		};


		protected short getOutput()
		{
			if (OperationConfig.Mode == OperationConfiguration.OperationMode.Standby)
				SendCommand(Command.CalculateHeading);

			byte[] rx = outputTransaction[0].Buffer;
			int transferred = Execute(outputTransaction, 500);

			if (OperationConfig.Mode == OperationConfiguration.OperationMode.Query)
				SendCommand(Command.CalculateHeading);

			if (transferred != 0)
				return (short)(rx[0] << 8 | rx[1]);
			else
				throw new I2CException();
		}

		private I2CTransaction[] commandTransaction = new I2CTransaction[] {
			CreateWriteTransaction(new byte[1])
		};

		public void SendCommand(Command command)
		{
			commandTransaction[0].Buffer[0] = (byte)command;
			int transferred = Execute(commandTransaction, 500);
			if (transferred == 0)
				throw new I2CException();
		}
		#endregion

		#region Command shortcuts
			public void Sleep() { SendCommand(Command.Sleep); }
			public void WakeUp() { SendCommand(Command.Wake); }
			public void StartCalibration() { SendCommand(Command.StartCalibration); }
			public void EndCalibration() { SendCommand(Command.EndCalibration); }
			public void SaveMode() { SendCommand(Command.SaveMode); }
			public void UpdateOffsets() { SendCommand(Command.UpdateOffsets); }
			private void CalculateHeading() { SendCommand(Command.CalculateHeading); }
		#endregion

		#region Extra sensor info
			public ushort Address
			{
				set
				{
					writeEEPROM(EEPROMLocation.SlaveAddress, (byte)(value<<1));
					this.Config = new I2CDevice.Configuration(value, ClockSpeed);
				}
				get { return this.Config.Address; }
			}
			public Vector MagnetometerOffsets
			{
				get
				{
					int x = readEEPROM(EEPROMLocation.MagnetometerXOffsetMSB) << 8 |
						readEEPROM(EEPROMLocation.MagnetometerXOffsetLSB);
					int y = readEEPROM(EEPROMLocation.MagnetometerXOffsetMSB) << 8 |
						readEEPROM(EEPROMLocation.MagnetometerXOffsetLSB);
					return new Vector(x, y);
				}
				set
				{
					int x = (int)value.X;
					int y = (int)value.Y;

					writeEEPROM(EEPROMLocation.MagnetometerXOffsetMSB, (byte)(x >> 8));
					writeEEPROM(EEPROMLocation.MagnetometerXOffsetLSB, (byte)x);
					writeEEPROM(EEPROMLocation.MagnetometerYOffsetMSB, (byte)(y >> 8));
					writeEEPROM(EEPROMLocation.MagnetometerYOffsetLSB, (byte)y);
				}
			}
			public byte TimeDelay
			{
				get { return readEEPROM(EEPROMLocation.TimeDelay); }
				set { writeEEPROM(EEPROMLocation.TimeDelay, value); }
			}
			public byte MeasurementCount
			{
				get { return readEEPROM(EEPROMLocation.MeasurementCount); }
				set { writeEEPROM(EEPROMLocation.MeasurementCount, value); }
			}
			public byte SoftwareVersion
			{
				get { return readEEPROM(EEPROMLocation.SoftwareVersion); }
			}
		#endregion

		#region Reading and writing from EEPROM and RAM
			private I2CTransaction[] readTransaction = new I2CTransaction[] {
				CreateWriteTransaction(new byte[2]),
				CreateReadTransaction(new byte[1])
			};
			private I2CTransaction[] writeTransaction = new I2CTransaction[] {
				CreateWriteTransaction(new byte[3])
			};

			protected byte readEEPROM(EEPROMLocation address)
			{
				readTransaction[0].Buffer[0] = (byte)Command.ReadEEPROM;
				readTransaction[0].Buffer[1] = (byte)address;

				if (Execute(readTransaction, 500) == 0)
					throw new I2CException();
				else
					return readTransaction[1].Buffer[0];
			}
			protected void writeEEPROM(EEPROMLocation address, byte value)
			{
				writeTransaction[0].Buffer[0] = (byte)Command.WriteEEPROM;
				writeTransaction[0].Buffer[1] = (byte)address;
				writeTransaction[0].Buffer[2] = (byte)value;

				if (Execute(writeTransaction, 500) == 0)
					throw new I2CException();
			}
			public byte readRAM(RAMLocation address)
			{
				readTransaction[0].Buffer[0] = (byte)Command.ReadRAM;
				readTransaction[0].Buffer[1] = (byte)address;

				if (Execute(readTransaction, 500) == 0)
					throw new I2CException();
				else
					return readTransaction[1].Buffer[0];
			}
			protected void writeRAM(RAMLocation address, byte value)
			{
				writeTransaction[0].Buffer[0] = (byte)Command.WriteRAM;
				writeTransaction[0].Buffer[1] = (byte)address;
				writeTransaction[0].Buffer[2] = (byte)value;

				if (Execute(writeTransaction, 500) == 0)
					throw new I2CException();
			}
		#endregion
	}
}
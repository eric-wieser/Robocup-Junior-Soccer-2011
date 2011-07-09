using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Native;
using Microsoft.SPOT.Hardware;
using Technobotts.Utilities;

namespace Technobotts.Robotics
{
	public class SensorPoller : IDisposable
	{
		public static readonly FEZ_Pin.Digital[] IRDetectorPins = new FEZ_Pin.Digital[] {
			FEZ_Pin.Digital.Di36, FEZ_Pin.Digital.Di37, FEZ_Pin.Digital.Di38, FEZ_Pin.Digital.Di39,
			FEZ_Pin.Digital.Di40, FEZ_Pin.Digital.Di41, FEZ_Pin.Digital.Di42, FEZ_Pin.Digital.Di43,
			FEZ_Pin.Digital.Di44, FEZ_Pin.Digital.Di45, FEZ_Pin.Digital.Di46, FEZ_Pin.Digital.Di47,
			FEZ_Pin.Digital.Di48, FEZ_Pin.Digital.Di49, FEZ_Pin.Digital.Di50, FEZ_Pin.Digital.Di51
		};

		public static readonly FEZ_Pin.Digital[] USDetectorPins = new FEZ_Pin.Digital[] {
			FEZ_Pin.Digital.Di20, FEZ_Pin.Digital.Di21, FEZ_Pin.Digital.Di22, FEZ_Pin.Digital.Di23
		};

		protected class NativeIRSensor : IIntensityDetector
		{
			public InputPort InnerPort { get; set; }

			public NativeIRSensor(FEZ_Pin.Digital pin)
			{
				InnerPort = new InputPort((Cpu.Pin)pin, false, Port.ResistorMode.PullUp);
			}

			public int Intensity { get; set; }

			public void Recalculate() { }
		}

		protected class NativeUSSensor : IRangeFinder
		{
			public TristatePort InnerPort { get; set; }

			public NativeUSSensor(FEZ_Pin.Digital pin)
			{
				InnerPort = new TristatePort((Cpu.Pin)pin, false, false, Port.ResistorMode.PullDown);
			}

			public int DistanceCM { get; set; }
		}

		public IRangeFinder[] US;
		public IIntensityDetector[] IR;

		private static RLP.Procedure _dataProcedure;
		private byte[] _dataBuffer = new byte[20];

		private void InitRLP()
		{
			RLP.Enable();
			RLP.Unlock(
				Resources.GetString(Resources.StringResources.UserID),
				Resources.GetBytes(Resources.BinaryResources.Key)
			);
		}

		private static RLP.Procedure GetDataProcedure()
		{
			Debug.GC(true);
			byte[] elf_file = Resources.GetBytes(Resources.BinaryResources.SensorPoller);
			RLP.LoadELF(elf_file);
			RLP.InitializeBSSRegion(elf_file);

			RLP.Procedure procedure = RLP.GetProcedure(elf_file, "acquire_data");
			elf_file = null;
			Debug.GC(true);

			return procedure;
		}


		public SensorPoller()
		{
			if (_dataProcedure == null)
			{
				InitRLP();
				_dataProcedure = GetDataProcedure();
			}

			US = new IRangeFinder[USDetectorPins.Length];
			for (int i = 0; i < US.Length; i++)
			{
				US[i] = new NativeUSSensor(USDetectorPins[i]);
			}

			IR = new IIntensityDetector[IRDetectorPins.Length];
			for (int i = 0; i < IR.Length; i++)
			{
				IR[i] = new NativeIRSensor(IRDetectorPins[i]);
			}
		}

		public void Poll()
		{
			int[] distances = new int[4];
			_dataProcedure.InvokeEx(_dataBuffer);
			for (int i = 0; i < IR.Length; i++)
				((NativeIRSensor) IR[i]).Intensity = _dataBuffer[i];
			for (int i = 0; i < US.Length; i++)
				((NativeUSSensor)US[i]).DistanceCM = _dataBuffer[i + IR.Length];

			USDistances = new Distances
			{
				Top = US[0].DistanceCM,
				Right = US[1].DistanceCM,
				Bottom = US[2].DistanceCM,
				Left = US[3].DistanceCM,
			};
		}

		public int BrokenIRSensorCount
		{
			get
			{
				int i = 0;
				foreach (IIntensityDetector ir in IR)
					if (ir.Intensity == 0) i++;
				return i;
			}
		}

        public string BrokenIRSensors
        {
            get
            {
				string s = "";
				string sep = "";
                for (int i = 0; i < IR.Length; i++)
					if (IR[i].Intensity == 0)
					{
						s += sep + i;
						sep = ",";
					}
                return s;
            }
        }

		public Distances USDistances { get; private set; }

		public void Dispose()
		{
			foreach (NativeIRSensor sensor in IR)
			{
				sensor.InnerPort.Dispose();
			}
			foreach (NativeUSSensor sensor in US)
			{
				sensor.InnerPort.Dispose();
			}

		}
	}
}

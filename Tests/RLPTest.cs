using System;
using Microsoft.SPOT;

using GHIElectronics.NETMF.Native;
using GHIElectronics.NETMF.FEZ;
using Microsoft.SPOT.Hardware;

namespace Technobotts.Tests
{
    public class RLPTest
    {
		static FEZ_Pin.Digital[] IRDetectorPins = new FEZ_Pin.Digital[] {
			FEZ_Pin.Digital.Di36, FEZ_Pin.Digital.Di37, FEZ_Pin.Digital.Di38, FEZ_Pin.Digital.Di39,
			FEZ_Pin.Digital.Di40, FEZ_Pin.Digital.Di41, FEZ_Pin.Digital.Di42, FEZ_Pin.Digital.Di43,
			FEZ_Pin.Digital.Di44, FEZ_Pin.Digital.Di45, FEZ_Pin.Digital.Di46, FEZ_Pin.Digital.Di47,
			FEZ_Pin.Digital.Di48, FEZ_Pin.Digital.Di49, FEZ_Pin.Digital.Di50, FEZ_Pin.Digital.Di51
		};

		static FEZ_Pin.Digital[] USDetectorPins = new FEZ_Pin.Digital[] {
			FEZ_Pin.Digital.Di20, FEZ_Pin.Digital.Di21, FEZ_Pin.Digital.Di22, FEZ_Pin.Digital.Di23
		};

		public static void InitRLP()
		{
			/*new byte[] { 0x50, 0xA4, 0x68, 0x74, 0x83, 0x75, 0xBA, 0x40, 0x2D, 0x84, 0xAF, 0x46, 0x78, 0xCC, 0x8F, 0x49, 0xDF, 0xBD, 0xCF, 0x52, 0xA3, 0x1D, 0x5A, 0xC5, 0xE4, 0x99, 0x19, 0x27, 0xC5, 0x36, 0xF2, 0xA5 }*/
			RLP.Enable();
			RLP.Unlock(
				Resources.GetString(Resources.StringResources.UserID),
				Resources.GetBytes(Resources.BinaryResources.Key)
			);
		}

		public static RLP.Procedure GetDataProcedure()
		{
			byte[] elf_file = Resources.GetBytes(Resources.BinaryResources.SensorPoller);
			RLP.LoadELF(elf_file);
			RLP.InitializeBSSRegion(elf_file);

			RLP.Procedure procedure = RLP.GetProcedure(elf_file, "acquire_data");
			elf_file = null;
			Debug.GC(true);

			return procedure;
		}

        public static void Main()
        {
			InitRLP();
			RLP.Procedure GetData = GetDataProcedure();

			InputPort[] ports = new InputPort[16];
			for (int i = 0; i < IRDetectorPins.Length; i++)
			{
				ports[i] = new InputPort((Cpu.Pin)IRDetectorPins[i], false, Port.ResistorMode.PullUp);
			}

			TristatePort[] tports = new TristatePort[4];
			for (int i = 0; i < tports.Length; i++)
			{
				tports[i] = new TristatePort((Cpu.Pin)USDetectorPins[i], false, false, Port.ResistorMode.PullDown);
			}

            byte [] myArray = new byte[20];

			while (true)
			{
				DateTime dtNow = DateTime.UtcNow;
				int time = GetData.InvokeEx(myArray);
				Debug.Print((DateTime.UtcNow - dtNow).ToString());
				Debug.Print("Expiry Time=" + time);
				for (int i = 0; i < 20; i++)
				{
					Debug.Print(myArray[i].ToString());
				}
			}
#if bogus
            // Test several types
            uint[] uintArray = new uint[] { 1, 123456789, 32 };
            byte[] byteArray = new byte[] { 0x12, 0xFF, 0x2D };

            int error = ArgumentTest.InvokeEx(uintArray, -5, 4.7f, "Hello!", byteArray);
            Debug.Print("Error:" + error.ToString());

            // This test passes float general array to FloatTest funtion and fill it up with floating numbers
            float[] farray = new float[10];
            FloatTest.InvokeEx(farray, farray.Length, 1.23f); // the formula is index/(1+1.23f)
            foreach (float x in farray)
            {
                Debug.Print(x.ToString());
            }
#endif
        }
    }
}

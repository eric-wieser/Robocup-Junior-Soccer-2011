using System;
using System.Threading;
using Microsoft.SPOT;
using GHIElectronics.NETMF;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.Hardware.LowLevel;

namespace Technobotts.Tests
{
	class RTCTest
	{
		public static Register tickCounter = new Register(0xE0024004);
		public static Register controlRegister = new Register(0xE0024008);
		public static Register PREINT = new Register(0xE0024080);
		public static Register PREFRAC = new Register(0xE0024084);

		public static string ArrayToString(int[] array)
		{
			string data = "[";
			string sep = "";
			foreach (int d in array)
			{
				data += sep + d;
				sep = ",\t";
			}
			data += "]";
			return data;
		}

		public static int SumArray(int[] array)
		{
			int sum = 0;
			foreach (int d in array)
			{
				sum += d;
			}
			return sum;
		}

		public static void Main()
		{

			PREINT.Write(732);
			PREFRAC.Write(13824);

			controlRegister.SetBits(1<<0);
			controlRegister.ClearBits(1 << 1 | 1 << 4);
			int[] times = new int[10];
			while (true)
			{
				uint lastClock = tickCounter.Read()/2;
				for (int i = 0; i < times.Length; i++)
				{
					Thread.Sleep(100);

					uint clock = tickCounter.Read()/2;
					if ((times[i] = (int)(clock - lastClock)) < 0) times[i] += 32768;
					lastClock = clock;
				}

				Debug.Print(""+ArrayToString(times));
			}
		}
	}
}

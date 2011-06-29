using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Threading;

namespace Technobotts.Hardware
{
	public class Button : IDisposable
	{
		public InputPort InnerPort;

		public Button(FEZ_Pin.Digital pin) {
			InnerPort = new InputPort((Cpu.Pin)pin, true, Port.ResistorMode.PullUp);
		}

		public bool IsPressed { get { return !InnerPort.Read(); } }

		public void WaitForPress(int pauseTime = 200)
		{
			while (!IsPressed) ;
			Thread.Sleep(pauseTime);
			while (IsPressed) ;
		}

		public void Dispose()
		{
			InnerPort.Dispose();
		}
	}
}

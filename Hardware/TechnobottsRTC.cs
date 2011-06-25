using System;
using GHIElectronics.NETMF;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.Hardware.LowLevel;

public class TechnobottsRTC
{
	public static Register tickCounter = new Register(0xE0024004);
	public static Register controlRegister = new Register(0xE0024008);
	public static Register PREINT = new Register(0xE0024080);
	public static Register PREFRAC = new Register(0xE0024084);

	const int clockFreq = 96000000 / 4;

	static TechnobottsRTC()
	{
		uint divider = clockFreq / 32768 - 1;
		uint remainder = clockFreq % 32768;

		PREINT.Write(divider);
		PREFRAC.Write(remainder);

		controlRegister.SetBits(1 << 0);
		controlRegister.ClearBits(1 << 1 | 1 << 4);
	}

	public static int Ticks
	{
		get { return (int) tickCounter.Read(); }
	}

	public static double TickDiffSeconds(int init)
	{
		double ret = (Ticks - init)/65536.0;
		return (ret < 0) ? ret+1.0 : ret;
	}

}

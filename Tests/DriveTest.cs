using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Technobotts.Geometry;
using Technobotts.Robotics;
using Technobotts.Robotics.Navigation;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;
#if false
namespace Technobotts.Tests
{
	class DriveTest
	{
		static IMotor motorA = new DCMotor(PWM.Pin.PWM1, FEZ_Pin.Digital.Di20, FEZ_Pin.Digital.Di21);
		static IMotor motorB = new DCMotor(PWM.Pin.PWM2, FEZ_Pin.Digital.Di22, FEZ_Pin.Digital.Di23);
		static IMotor motorC = new DCMotor(PWM.Pin.PWM3, FEZ_Pin.Digital.Di24, FEZ_Pin.Digital.Di25);

		static InputPort button = new InputPort((Cpu.Pin)FEZ_Pin.Digital.LDR, true, Port.ResistorMode.PullUp);

		/*
		static HolonomicDrive drive = new HolonomicDrive(new {
			new HolonomicDrive.Wheel(
				Vector.FromPolarCoords(95,0),
				Matrix.FromRotation(0),

		});*/

		public static void waitForButton(int time = 200)
		{
			while (button.Read()) ;
			Thread.Sleep(time);
			while (!button.Read()) ;
		}

		public static void Main()
		{
			while (true)
			{
				motorA.Speed = 0;
				//motorB.Speed = 0;
				//motorC.Speed = 0;
				waitForButton();
				motorA.Speed = 1;
				//motorB.Speed = -0.5;
				//motorC.Speed = 0.25;
				waitForButton();
			}
		}

	}
}
#endif
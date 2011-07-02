//#define P
#define PI
//#define PID

using System;
using System.Threading;
using Microsoft.SPOT;
using Technobotts.Utilities;
using Technobotts.Soccer;

namespace Technobotts.Tests
{
	class PIDTest
	{
		public static PIDController pid;
		public static void Main()
		{
			using (Robot r = new Robot())
			{
				//pid = new PIDController(0.75, 0.1, 0.005);
				//PIDController.CoefficientsFromZieglerNicholsMethod(1.05, 0.95)
				/*
				double[] pidK = PIDController.CoefficientsFromZieglerNicholsMethod(
					1.15, 0.87,
					PIDController.ZieglerNicholsType.SomeOvershoot
				);
				pid = new PIDController(0.5, 1.5, 0.075)
				{
					Input = new PIDController.InputFunction(
						() => r.Compass.Angle,
						Range.Angle
					),
					Output = new PIDController.OutputFunction(
						(value) => { Debug.Print(value+""); r.OldDrive.TurnVelocity = -value; },
						Range.SignedAngle * 2
					),
					Continuous = true,
					SetPoint = 0
				};
				r.Button.WaitForPress();
				*/
				//pid.Enabled = true;
				Thread.Sleep(500);
				Debug.Print(""+r.Compass.Angle);
				Thread.Sleep(500);

				r.Compass.RecordNorthDirection();
				r.Drive.ControlEnabled = true;
				r.Drive.TargetHeading = 0;
				Thread.Sleep(1000);
				r.Drive.TargetHeading = System.Math.PI / 2;
				Thread.Sleep(1000);
				r.Drive.TargetHeading = System.Math.PI;
				Thread.Sleep(1000);
				r.Drive.TargetHeading = -System.Math.PI / 2;
				Thread.Sleep(1000);
				r.Drive.ControlEnabled = false;
			}
		}
	}
}

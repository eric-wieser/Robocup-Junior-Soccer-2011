//#define P
#define PI
//#define PID

using System;
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

				pid = new PIDController(PIDController.CoefficientsFromZieglerNicholsMethod(1.15, 0.87))
				{
					Input = new PIDController.InputFunction(
						() => r.Compass.Angle,
						Range.Angle
					),
					Output = new PIDController.OutputFunction(
						(value) => r.Drive.TurnVelocity = -value,
						Range.SignedAngle * 2
					),
					Continuous = true,
					SetPoint = 0
				};


				r.Button.WaitForPress();

				pid.Enabled = true;
				r.Button.WaitForPress();
				pid.SetPoint = System.Math.PI/2;
				r.Button.WaitForPress();
				pid.SetPoint = System.Math.PI;
				r.Button.WaitForPress();
				pid.SetPoint = -System.Math.PI / 2;
				r.Button.WaitForPress();
				pid.Enabled = false;
			}
		}
	}
}

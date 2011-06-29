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
				pid = new PIDController(0.75, 0.1);
				pid.Input = new PIDController.InputFunction(
					() => r.Compass.Angle,
					Range.Angle
				);
				pid.Output = new PIDController.OutputFunction(
					(value) => r.Drive.TurnVelocity = -value,
					Range.SignedAngle * 2
				);
				pid.Continuous = true;
				pid.SetPoint = 0;

				r.Button.WaitForPress();

				pid.Enabled = true;

				r.Button.WaitForPress();

				pid.Enabled = false;
			}
		}
	}
}

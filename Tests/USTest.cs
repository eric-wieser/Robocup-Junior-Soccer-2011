using System;
using Microsoft.SPOT;
using Technobotts.Soccer;
using Technobotts.Robotics;

namespace Technobotts.Tests
{
	public class USTest
	{
		public static void Main()
		{
			Robot r = new Robot();
			IRangeFinder[] us = r.Sensors.US;
			while (true)
			{
				r.Sensors.Poll();
				int front = us[0].DistanceCM;
				int right = us[1].DistanceCM;
				int back = us[2].DistanceCM;
				int left = us[3].DistanceCM;

				if (r.Button.IsPressed)
					Debug.Print("Vertical: " + front + " + " + back + " = " + (front + back));
				else
					Debug.Print("Horizontal: " + left + " + " + right + " = " + (left + right));
			}
		}
	}
}

using System;
using Microsoft.SPOT;
using Technobotts.Soccer;
using Technobotts.Robotics;
using Technobotts.Utilities;

namespace Technobotts.Tests
{
	public class USTest
	{
		public static void Main()
		{
			Robot r = new Robot();
			IRangeFinder[] us = r.Sensors.US;
			Debug.Print(""+new Range().Contains(1));
			USSensorAggregator uss = new USSensorAggregator()
			{
				Front = us[0],
				Right = us[1],
				Back = us[2],
				Left = us[3],
				Compass = r.Compass
			};

			while (true)
			{
				r.Sensors.Poll();
				Debug.Print(""+uss.GetPosition());
			}
		}
	}
}

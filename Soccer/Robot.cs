using System;
using Microsoft.SPOT;
using Technobotts.Robotics.Navigation;
using Technobotts.Robotics;

namespace Technobotts.Soccer
{
	public class Robot /*: HolonomicDrive*/
	{
		AngleFinder Compass;
		IntensityDetectorArray BallDetector;
		Solenoid Kicker;
	}
}

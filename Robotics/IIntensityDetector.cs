using System;
using System.Text;

namespace Technobotts.Robotics
{
	public interface IIntensityDetector
	{
		int Intensity { get; }
		void Recalculate();
	}
}

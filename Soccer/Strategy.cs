using System;
using Microsoft.SPOT;

namespace Technobotts.Soccer
{
	public class Strategy
	{
		protected Robot robot = new Robot();

		public virtual void ActiveInit() {}
		public virtual void ActivePeriodic() { }
		public virtual void ActiveCleanUp() { robot.Drive.Stop(); }

		public virtual void DisabledInit() { }
		public virtual void DisabledPeriodic() { }
		public virtual void DisabledCleanUp() { }

		public bool IsActive = false;

		public void Execute()
		{
			try
			{
				while (true)
				{
					robot.LEDs.ModeIndicator.StartBlinking(1000, 0.75);
					DisabledInit();
					while (!robot.Button.IsPressed)
						DisabledPeriodic();
					DisabledCleanUp();

					robot.LEDs.ModeIndicator.State = true;
					ActiveInit();
					while (robot.Button.IsPressed)
						ActivePeriodic();
					ActiveCleanUp();
				}
			}
			catch
			{
				robot.LEDs.StartBlinking(250, 0.5);
			}
		}
	}
}

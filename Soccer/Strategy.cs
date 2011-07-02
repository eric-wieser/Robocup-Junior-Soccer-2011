using System;
using Microsoft.SPOT;

namespace Technobotts.Soccer
{
	public class Strategy
	{
		protected Robot robot = new Robot();

		public virtual void Activated() { }
		public virtual void ActivePeriodic() {  }

		public virtual void Disabled() { }
		public virtual void DisabledPeriodic() { }

		public bool IsActive = false;

		public void Execute()
		{

			try
			{
				while (true)
				{
					robot.LEDs.ModeIndicator.StartBlinking(1000, 0.75);
					Disabled();
					robot.Drive.Stop();
					while (!robot.Button.IsPressed)
					{
						robot.Sensors.Poll();
						Utilities.SystemTime.Update();
						robot.ShowDiagnostics();
						DisabledPeriodic();
					}

					robot.LEDs.ModeIndicator.State = true;
					Activated();
					while (robot.Button.IsPressed)
					{
						robot.Sensors.Poll();
						Utilities.SystemTime.Update();
						robot.ShowDiagnostics();
						ActivePeriodic();
					}

				}
			}
			catch
			{
				robot.Drive.Stop();
				robot.LEDs.StartBlinking(250, 0.5);
			}
		}
	}
}

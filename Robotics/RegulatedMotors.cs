using System;
using Microsoft.SPOT;
using System.Threading;
using System.Collections;
using Technobotts.Utilities;

namespace Technobotts.Robotics
{
	class RegulatedMotor : IMotor
	{
		private static Timer _controlLoop;
		private static double _period = 0.025;

		private static double step = 0.33;

		private static ArrayList motors = new ArrayList();

		private IMotor _innerMotor;

		public static void EnableRegulatedMotorAlgorithm(bool bEnable)
		{
			if (_controlLoop != null) _controlLoop.Dispose();

			if (bEnable)
			{
				_controlLoop = new Timer(new TimerCallback(updateMotors), null, 0, (int)(_period * 1000));
			}
			else
			{
				// already switched off.
			}
		}

		static RegulatedMotor()
		{	
		}

		public RegulatedMotor(IMotor motor)
		{
			_innerMotor = motor;
			lock (motors)
			{
				motors.Add(this);
			}
		}

		public double Speed { get; set; }
		public NeutralMode NeutralMode { get { return _innerMotor.NeutralMode; } set { _innerMotor.NeutralMode = value; } }

		private static void updateMotors(object o)
		{
			lock (motors)
			{
				foreach (RegulatedMotor m in motors)
				{
					if (m.Speed > m._innerMotor.Speed + step)
						m._innerMotor.Speed += step;
					else if (m.Speed < m._innerMotor.Speed - step)
						m._innerMotor.Speed -= step;
					else
						m._innerMotor.Speed = m.Speed;
				}
			}
		}
	}
}

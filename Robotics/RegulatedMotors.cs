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
		private static double _period = 0.05;

		public double MaxAcceleration = 16;

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
		public double ActualSpeed { get { return _innerMotor.Speed; } private set { _innerMotor.Speed = value; } }
		public NeutralMode NeutralMode { get { return _innerMotor.NeutralMode; } set { _innerMotor.NeutralMode = value; } }

		private void updateSpeed()
		{
			const double threshold = 0.1;

			//Going fast forward
			if (ActualSpeed > threshold)
			{
				if (Speed > threshold)
					ActualSpeed = Speed;
				else
					ActualSpeed = threshold;
			}

			//Going fast backward
			else if (ActualSpeed < -threshold)
			{
				if (Speed < -threshold)
					ActualSpeed = Speed;
				else
					ActualSpeed = -threshold;
			}

			else
			{
				Range allowableChange = new Range(MaxAcceleration * _period);
				ActualSpeed += allowableChange.Clip(Speed - ActualSpeed);
			}
		}

		private static void updateMotors(object o)
		{
			lock (motors)
			{
				foreach (RegulatedMotor m in motors)
				{
					m.updateSpeed();
				}
			}
		}
	}
}

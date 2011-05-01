using System;
using System.Threading;

using Microsoft.SPOT;

namespace Technobotts.Utilities
{

	public class PIDController : IDisposable
	{
		public const double DefaultPeriod = .05;

		public interface IInput
		{
			Range Range { get; }
			double Value { get; }
		}
		public interface IOutput
		{
			Range Range { get; }
			double Value { set; }
		}

		public double P { get; private set; }
		public double I { get; private set; }
		public double D { get; private set; }
		public double Period { get; private set; }

		private IOutput _output;
		public IOutput Output {
			get { return _output; }
			private set {
				lock (this)
				{
					_output = value;
					Enabled = Enabled;
				}
			}
		}
		private IInput _input;
		public IInput Input
		{
			get { return _input; }
			private set
			{
				lock (this)
				{
					_input = value;
					Enabled = Enabled;
				}
			}
		}

		private double _setPoint;
		public double SetPoint
		{
			get { return _setPoint; }
			set
			{
				if (Input != null)
				{
					_setPoint = Input.Range.Clip(value);
				}
				else
					_setPoint = value;
			}
		}

		public bool Continuous { get; set; }
		private Range _tolerance;
		public double Tolerance {
			get { return _tolerance.Max; }
			set { _tolerance = new Range(value / 100); }
		}

		private bool _enabled = false;
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				lock (this)
				{
					_enabled = value && Output != null && Input != null;
				}
			}
		}

		public double Error { get; private set; }
		public double PrevError { get; private set; }
		public double TotalError { get; private set; }
		Timer _controlLoop;

		///<summary>Allocate a PID object with the given constants for P, I, D</summary>
		///<param name="p">the proportional coefficient</param>
		///<param name="i"> the integral coefficient</param>
		///<param name="d"> the derivative coefficient</param>
		///<param name="period">the loop time for doing calculations. This particularly effects calculations of the
		///integral and differential terms. The default is 50ms</param>
		public PIDController(double p, double i = 0, double d = 0, double period = DefaultPeriod)
		{
			P = p;
			I = i;
			D = d;
			Period = period;

			_controlLoop = new Timer(new TimerCallback(calculatePID), this, 0, (int)(Period * 1000));

			Reset();
		}

		public void Dispose()
		{
			_controlLoop.Dispose();
		}

		private void calculate()
		{
			lock (this)
			{
				if (Input == null || Output == null)
				{
					Enabled = false;
					return;
				}
			}

			if (Enabled)
			{
				double result;
				lock (this)
				{
					//Handle continuity
					double error = SetPoint - Input.Value;
					if (Continuous)
					{
						double inputRange = Input.Range.Span;
						while (error > inputRange / 2)
							error -= inputRange;
						while (error < -inputRange / 2)
							error += inputRange;
					}
					Error = error;

					//Handle integral part overflow
					double newTotal = TotalError + Error;
					if (Output.Range.Contains(newTotal * I))
						TotalError = newTotal;

					//Calcate PID output
					result =
						P * Error +
						I * TotalError * Period +
						D * (Error - PrevError) / Period;

					//Normalize to wihthin max and min output
					result = Output.Range.Clip(result);

					PrevError = Error;
				}

				Output.Value = result;
			}
		}

		private static void calculatePID(Object @object)
		{
			PIDController pid = (PIDController)@object;
			pid.calculate();
		}

		public void SetPID(double p, double i, double d)
		{
			lock (this)
			{
				P = p;
				I = i;
				D = d;
			}
		}

		public bool OnTarget
		{
			get
			{
				double acceptableError = Tolerance * Input.Range.Span;

				lock (this)
				{
					return _tolerance.Contains(Error / Input.Range.Span);
				}
			}
		}

		///<summary>Reset the previous error and the integral term, and disables the controller</summary>
		public void Reset()
		{
			lock (this)
			{
				Enabled = false;
				Error = 0;
				PrevError = 0;
				TotalError = 0;
			}
		}
	}
}
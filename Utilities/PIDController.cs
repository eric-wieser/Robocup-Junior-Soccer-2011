using System;
using System.Threading;

using Microsoft.SPOT;
using System.Runtime.CompilerServices;

namespace Technobotts.Utilities
{

	public class PIDController : IDisposable
	{
		public const double DefaultPeriod = .05;

		public class InputFunction
		{
			public delegate double Getter();

			public InputFunction(Getter getter, Range range = null)
			{
				Range = range == null ? Range.Infinite : range;
				Get = getter;
			}

			public Getter Get { get; private set; }
			public Range Range { get; private set; }
		}
		public class OutputFunction
		{
			public delegate void Setter(double value);

			public OutputFunction(Setter setter, Range range = null)
			{
				Range = range == null ? Range.Infinite : range;
				Set = setter;
			}

			public Setter Set { get; private set; }
			public Range Range { get; private set; }
		}
		public double P { get; private set; }
		public double I { get; private set; }
		public double D { get; private set; }
		public double Period { get; private set; }

		private OutputFunction _output;
		public OutputFunction Output {
			get { return _output; }
			
			[MethodImpl(MethodImplOptions.Synchronized)]
			set {
				_output = value;
				Enabled = Enabled;
				SetPoint = SetPoint;
			}
		}
		private InputFunction _input;
		public InputFunction Input
		{
			get { return _input; }
			
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				_input = value;
				Enabled = Enabled;
				SetPoint = SetPoint;
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
			set { if (Output != null && Input != null) lock (Output) { _enabled = value; } }
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

		public static PIDController FromZieglerNicholsMethod(double criticalP, double oscillationPeriod, string type = "PID")
		{
			switch (type)
			{
				case "P":
					return new PIDController(0.5 * criticalP);
				case "PI":
					return new PIDController(
						0.45 * criticalP,
						1.2 * criticalP / oscillationPeriod);
				case "PID":
					return new PIDController(
						0.60 * criticalP,
						2 * criticalP / oscillationPeriod,
						criticalP * oscillationPeriod / 8);
				default:
					throw new ArgumentException(
						"Not a valid PID type: must be one of P, PI, or PID",
						"type");
			}
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
				lock (Output)
				{
					double result;
					lock (this)
					{
						//Handle continuity
						double error = SetPoint - Input.Get();
						if (DoubleEx.IsNaN(error)) return;
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

						//Normalize to within max and min output
						result = Output.Range.Clip(result);

						PrevError = Error;
					}

					Output.Set(result);
				}
			}
		}

		private static void calculatePID(Object @object)
		{
			PIDController pid = (PIDController)@object;
			pid.calculate();
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SetPID(double p, double i, double d)
		{
			P = p;
			I = i;
			D = d;
		}

		public bool OnTarget
		{
			get
			{
				return _tolerance.Contains(Error / Input.Range.Span);
			}
		}

		///<summary>Reset the previous error and the integral term, and disables the controller</summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Reset()
		{
			Enabled = false;
			Error = 0;
			PrevError = 0;
			TotalError = 0;
		}
	}
}

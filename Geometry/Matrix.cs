using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.System;

namespace Technobotts.Geometry
{
	public class Matrix
	{
		#region Common Matrix constants
		///<summary>The identity matrix</summary>
		public static readonly Matrix Identity = new Matrix(1, 0, 0, 1);
		///<summary>90 Degree counterclockwise rotation</summary>
		public static readonly Matrix Rotate90 = new Matrix(0, 1, -1, 0);
		///<summary>180 Degree counterclockwise rotation</summary>
		public static readonly Matrix Rotate180 = new Matrix(-1, 0, 0, -1);
		///<summary>270 Degree counterclockwise rotation</summary>
		public static readonly Matrix Rotate270 = new Matrix(0, -1, 1, 0);
		#endregion

		private double _a, _b, _c, _d;
		private Matrix _inverseMatrix = null; //Store for cheap future usage

		public static Matrix FromRotation(double angle)
		{
			double sinA = MathEx.Sin(angle);
			double cosA = MathEx.Cos(angle);
			return new Matrix(cosA, sinA, -sinA, cosA);
		}
		/**
		 * Construct a square 2x2 Matrix representing the transformation onto a new
		 * pair of coordinate axes
		 */
		public static Matrix FromCoordinateAxes(Vector yAxis, Vector xAxis)
		{
			return new Matrix(
				xAxis.X, yAxis.X,
				xAxis.Y, yAxis.Y);
		}
		public static Matrix FromPrincipleAxis(Vector axis)
		{
			return new Matrix(
				axis.Y, axis.X,
				-axis.X, axis.Y);
		}

		///<summary>Construct a square 2x2 Matrix of the form
		///
		///<code>
		///[ <paramref name="a"/> <paramref name="b"/> ]
		///[ <paramref name="c"/> <paramref name="d"/> ]
		///</code>
		///</summary>
		public Matrix(double a, double b, double c, double d)
		{
			this._a = a;
			this._b = b;
			this._c = c;
			this._d = d;
		}


		public Double Determinant
		{
			get { return _a * _d - _b * _c; }
		}

		public Matrix Inverse()
		{
			if (_inverseMatrix == null)
			{
				_inverseMatrix = new Matrix(_d, -_b, -_c, _a) / Determinant;
				_inverseMatrix._inverseMatrix = this;
			}
			return _inverseMatrix;
		}

		public static Matrix operator /(Matrix m, double k)
		{
			return new Matrix(m._a / k, m._b / k, m._c / k, m._d / k);
		}

		#region Multiplication operators
		public static Matrix operator *(Matrix m, double k)
		{
			return new Matrix(m._a * k, m._b * k, m._c * k, m._d * k);
		}
		public static Vector operator *(Matrix m, Vector v)
		{
			return new Vector(m._a * v.X + m._b * v.Y, m._c * v.X + m._d * v.Y);
		}
		public static Matrix operator *(Matrix m1, Matrix m2)
		{
			return new Matrix(
				m1._a * m2._a + m1._b * m2._c,
				m1._a * m2._b + m1._b * m2._d,
				m1._c * m2._a + m1._d * m2._c,
				m1._c * m2._b + m1._d * m2._d);
		}
		#endregion

		public static implicit operator Matrix(double k)
		{
			return new Matrix(
				k, 0,
				0, k);
		}
		
		public static explicit operator Matrix(Vector v)
		{
			return new Matrix(
				v.X, 0,
				0, v.Y);
		}

		public string toString()
		{
			return "Matrix: [[" + _a + "," + _b + "],[" + _c + "," + _d + "]]";
		}

		public override bool Equals(Object obj)
		{
			Matrix that = obj as Matrix;
			return that != null && _a == that._a && _b == that._b && _c == that._c && _d == that._d;
		}

		public override int GetHashCode()
		{
			return
				_a.GetHashCode() << 24 +
				_b.GetHashCode() << 16 +
				_c.GetHashCode() << 8 +
				_d.GetHashCode();
		}
	}
}

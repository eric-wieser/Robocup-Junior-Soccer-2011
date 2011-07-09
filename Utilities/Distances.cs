using System;
using Microsoft.SPOT;

namespace Technobotts.Utilities
{
	public struct Distances
	{
		public int Top;
		public int Right;
		public int Bottom;
		public int Left;

		public Distances(int[] array)
		{
			Top = array[0];
			Right = array[0];
			Bottom = array[0];
			Left = array[0];
		}
		public void rotate(int n)
		{
			n = n % 4;
			for (int i = 0; i < n; i++)
			{
				int temp;
				temp = Top;
				Top = Right;
				Right = Bottom;
				Bottom = Left;
				Left = temp;

			}
		}

		public int this[int index] {
			set
			{
				switch (index % 4)
				{
					case 0: Top = value; break;
					case 1: Right = value; break;
					case 2: Bottom = value; break;
					case 3: Left = value; break;
				}
			}
		}

		public static Distances operator -(Distances a, Distances b)
		{
			return new Distances
			{
				Top = a.Top - b.Top,
				Right = a.Right - b.Right,
				Bottom = a.Bottom - b.Bottom,
				Left = a.Left - b.Left
			};
		}
	}
}

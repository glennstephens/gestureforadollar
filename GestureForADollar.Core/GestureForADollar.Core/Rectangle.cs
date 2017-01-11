using System;

namespace GestureForADollar.Core
{
	public class Rectangle
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }

		public Rectangle(double x, double y, double width, double height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		public static Rectangle Empty
		{
			get
			{
				return new Rectangle(0, 0, 0, 0);
			}
		}
	}
}

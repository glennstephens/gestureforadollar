using System;
using System.Collections.Generic;

namespace GestureForADollar.Core
{
	public class Unistroke
	{
		public string Name { get; set; }
		public List<Point> Points { get; set; }
		public List<double> Vector { get; set; }

		public Unistroke(string name, List<Point> points)
		{
			Points = points;
			Name = name;
		}
	}
}

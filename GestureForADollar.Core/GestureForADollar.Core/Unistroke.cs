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
			this.Name = name;

			var resampled = PointsHelpers.Resample(points, PointsHelpers.NumPoints);
			var radians = PointsHelpers.IndicativeAngle(resampled);
			var rotated = PointsHelpers.RotateBy(resampled, -radians);
			var scaled = PointsHelpers.ScaleTo(rotated, PointsHelpers.SquareSize);
			Points = PointsHelpers.TranslateTo(scaled, PointsHelpers.Origin);
			Vector = PointsHelpers.Vectorize(Points); // for Protractor
		}
	}
}

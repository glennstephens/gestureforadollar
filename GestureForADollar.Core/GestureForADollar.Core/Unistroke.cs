using System;
using System.Collections.Generic;

namespace GestureForADollar.Core
{
	public class Unistroke
	{
		public string Name { get; set; }
		public List<Point> OriginalPoints { get; set; }
		public List<Point> Points { get; set; }
		public List<double> Vector { get; set; }

		protected void Initialize(string name, List<Point> points)
		{
			this.Name = name;
			this.OriginalPoints = points;
		}

		public Unistroke(string name, List<Point> points)
		{
			Initialize(name, points);

			Setup();
		}

		protected virtual void Setup()
		{
			var resampled = PointsHelpers.Resample(OriginalPoints, PointsHelpers.NumPoints);
			var radians = PointsHelpers.IndicativeAngle(resampled);
			var rotated = PointsHelpers.RotateBy(resampled, -radians);
			var scaled = PointsHelpers.ScaleTo(rotated, PointsHelpers.SquareSize);
			Points = PointsHelpers.TranslateTo(scaled, PointsHelpers.Origin);
			Vector = PointsHelpers.Vectorize(Points); // for Protractor
		}
	}
}

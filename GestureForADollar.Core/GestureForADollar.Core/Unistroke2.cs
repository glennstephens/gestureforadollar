using System;
using System.Collections.Generic;

namespace GestureForADollar.Core
{
	public class Unistroke2 : Unistroke
	{
		public bool UseBoundedRotationInvariance { get; set; }

		public Point StartUnitVector { get; set; }

		public Unistroke2(string name, bool useBoundedRotationInvariance, List<Point> points) : base(name, points)
		{
			UseBoundedRotationInvariance = useBoundedRotationInvariance;
		}

		protected override void Setup()
		{
			var resampled = PointsHelpers.Resample(OriginalPoints, PointsHelpers.NumPoints);
			var radians = PointsHelpers.IndicativeAngle(resampled);
			var rotated = PointsHelpers.RotateBy(resampled, -radians);

			var scaled = PointsHelpers.ScaleDimTo(rotated, PointsHelpers.SquareSize, PointsHelpers.OneDThreshold);

			if (UseBoundedRotationInvariance)
				scaled = PointsHelpers.RotateBy(scaled, -radians);

			this.StartUnitVector = PointsHelpers.CalcStartUnitVector(scaled, PointsHelpers.StartAngleIndex);
			this.Vector = PointsHelpers.Vectorize2(scaled, UseBoundedRotationInvariance);
		}
	}

	
}

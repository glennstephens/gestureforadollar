﻿using System;
using System.Collections.Generic;

namespace GestureForADollar.Core
{
	public class DollarRecognizer
	{
		public List<Unistroke> Unistrokes { get; set; } = new List<Unistroke>();

		public Result Recognize(List<Point> points, bool useProtractor = true)
		{
			if (points.Count < 4)
				return Result.TooFewPoints();

			Rectangle rect = PointsHelpers.BoundingBox(points);

			points = PointsHelpers.Resample(points, PointsHelpers.NumPoints);
			var radians = PointsHelpers.IndicativeAngle(points);
			points = PointsHelpers.RotateBy(points, -radians);
			points = PointsHelpers.ScaleTo(points, PointsHelpers.SquareSize);
			points = PointsHelpers.TranslateTo(points, PointsHelpers.Origin);
			var vector = PointsHelpers.Vectorize(points); // for Protractor

			var b = Double.PositiveInfinity;
			var u = -1;
			for (var i = 0; i < this.Unistrokes.Count; i++) // for each unistroke
			{
				double d;
				if (useProtractor) // for Protractor
					d = PointsHelpers.OptimalCosineDistance(this.Unistrokes[i].Vector, vector);
				else // Golden Section Search (original $1)
					d = PointsHelpers.DistanceAtBestAngle(points, this.Unistrokes[i], -PointsHelpers.AngleRange, +PointsHelpers.AngleRange, PointsHelpers.AnglePrecision);
				if (d < b)
				{
					b = d; // best (least) distance
					u = i; // unistroke
				}
			}
			return (u == -1) ? 
				new Result("No match.", 0.0, 0, Rectangle.Empty) : 
				new Result(Unistrokes[u].Name, useProtractor ? 1.0 / b : 1.0 - b / PointsHelpers.HalfDiagonal, radians, rect);
		}

		private void AddReverseOfStroke(Unistroke stroke)
		{
			var reversedPoints = new List<Point>();
			for (int i = stroke.OriginalPoints.Count - 1; i >= 0; i--)
				reversedPoints.Add(stroke.OriginalPoints[i]);

			Unistrokes.Add(new Unistroke(stroke.Name, reversedPoints));
		}

		public DollarRecognizer AddGesture(Unistroke stroke, bool addReverse = false)
		{
			Unistrokes.Add(stroke);

			if (addReverse)
				AddReverseOfStroke(stroke);

			return this;
		}

		public void Clear()
		{
			Unistrokes.Clear();
		}
	}
}

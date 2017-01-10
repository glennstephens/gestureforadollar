using System;
using System.Collections.Generic;

namespace GestureForADollar.Core
{
	public static class PointsHelpers
	{
		public static int NumUnistrokes = 16;
		public static int NumPoints = 64;
		public static double SquareSize = 250.0;
		public static Point Origin = new Point(0, 0);
		public static double Diagonal = Math.Sqrt(SquareSize * SquareSize + SquareSize * SquareSize);
		public static double HalfDiagonal = 0.5 * Diagonal;
		public static double AngleRange = PointsHelpers.DegreesToRadians(45.0);
		public static double AnglePrecision = PointsHelpers.DegreesToRadians(2.0);
		public static double Phi = 0.5 * (-1.0 + Math.Sqrt(5.0)); // Golden Ratio

		// Checked
		public static double DegreesToRadians(double angle)
		{
			return angle * Math.PI / 180;
		}

		// Checked
		public static double Distance(Point p1, Point p2)
		{
			var dx = p2.X - p1.X;
			var dy = p2.Y - p1.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		// Checked
		public static double PathLength(List<Point> points)
		{
			var d = 0.0;

			for (var i = 1; i < points.Count; i++)
				d += Distance(points[i - 1], points[i]);

			return d;
		}

		// Checked. May need to validate
		public static List<Point> Resample(List<Point> points, int n)
		{
			var I = PathLength(points) / ((double)n - 1); // interval length
			var D = 0.0;
			var newpoints = new List<Point>(new[] { points[0] });
			for (var i = 1; i < points.Count; i++)
			{
				var d = Distance(points[i - 1], points[i]);
				if ((D + d) >= I)
				{
					var qx = points[i - 1].X + ((I - D) / d) * (points[i].X - points[i - 1].X);
					var qy = points[i - 1].Y + ((I - D) / d) * (points[i].Y - points[i - 1].Y);
					var q = new Point(qx, qy);
					newpoints.Add(q); // append new point 'q'
					points.Insert(i, q);
					D = 0.0;
				}
				else D += d;
			}

			if (newpoints.Count == n - 1) // somtimes we fall a rounding-error short of adding the last point, so add it if so
				newpoints.Add(new Point(points[points.Count - 1].X, points[points.Count - 1].Y));

			return newpoints;
		}

		// Checked
		public static double IndicativeAngle(List<Point> points)
		{
			var c = Centroid(points);
			return Math.Atan2(c.Y - points[0].Y, c.X - points[0].X);
		}

		// Checked
		public static List<Point> RotateBy(List<Point> points, double radians) // rotates points around centroid
		{
			var c = Centroid(points);
			var cos = Math.Cos(radians);
			var sin = Math.Sin(radians);

			var newpoints = new List<Point>();
			for (var i = 0; i < points.Count; i++)
			{
				var qx = (points[i].X - c.X) * cos - (points[i].Y - c.Y) * sin + c.X;
				var qy = (points[i].X - c.X) * sin + (points[i].Y - c.Y) * cos + c.Y;
				newpoints.Add(new Point(qx, qy));
			}

			return newpoints;
		}

		public static List<Point> ScaleTo(List<Point> points, double size) // non-uniform scale; assumes 2D gestures (i.e., no lines)
		{
			var B = BoundingBox(points);
			var newpoints = new List<Point>();
			for (var i = 0; i < points.Count; i++)
			{
				var qx = points[i].X * (size / B.Width);
				var qy = points[i].Y * (size / B.Height);
				newpoints.Add(new Point(qx, qy));
			}

			return newpoints;
		}

		// Checked
		public static List<Point> TranslateTo(List<Point> points, Point pt) // translates points' centroid
		{
			var c = Centroid(points);
			var newpoints = new List<Point>();
			for (var i = 0; i < points.Count; i++)
			{
				var qx = points[i].X + pt.X - c.X;
				var qy = points[i].Y + pt.Y - c.Y;
				newpoints.Add(new Point(qx, qy));
			}
			return newpoints;
		}

		// Checked
		public static List<double> Vectorize(List<Point> points) // for Protractor
		{
			var sum = 0.0;
			var vector = new List<double>();
			for (var i = 0; i < points.Count; i++)
			{
				vector.Add(points[i].X);
				vector.Add(points[i].Y);
				sum += points[i].X * points[i].X + points[i].Y * points[i].Y;
			}

			var magnitude = Math.Sqrt(sum);
			for (var i = 0; i < vector.Count; i++)
				vector[i] /= magnitude;

			return vector;
		}

		// Checked
		public static double OptimalCosineDistance(List<double> v1, List<double> v2) // for Protractor
		{
			var a = 0.0;
			var b = 0.0;

			for (var i = 0; i < v1.Count; i += 2)
			{
				a += v1[i] * v2[i] + v1[i + 1] * v2[i + 1];
				b += v1[i] * v2[i + 1] - v1[i + 1] * v2[i];
			}

			var angle = Math.Atan(b / a);
			return Math.Acos(a * Math.Cos(angle) + b * Math.Sin(angle));
		}

		// Checked
		public static double DistanceAtBestAngle(List<Point> points, Unistroke stroke, double a, double b, double threshold)
		{
			var x1 = PointsHelpers.Phi * a + (1.0 - PointsHelpers.Phi) * b;
			var f1 = DistanceAtAngle(points, stroke, x1);
			var x2 = (1.0 - PointsHelpers.Phi) * a + PointsHelpers.Phi * b;
			var f2 = DistanceAtAngle(points, stroke, x2);
			while (Math.Abs(b - a) > threshold)
			{
				if (f1 < f2)
				{
					b = x2;
					x2 = x1;
					f2 = f1;
					x1 = Phi * a + (1.0 - Phi) * b;
					f1 = DistanceAtAngle(points, stroke, x1);
				}
				else {
					a = x1;
					x1 = x2;
					f1 = f2;
					x2 = (1.0 - Phi) * a + Phi * b;
					f2 = DistanceAtAngle(points, stroke, x2);
				}
			}
			return Math.Min(f1, f2);
		}

		// Checked
		public static double DistanceAtAngle(List<Point> points, Unistroke stroke, double radians)
		{
			var newpoints = RotateBy(points, radians);
			return PathDistance(newpoints, stroke.Points);
		}

		// Checked
		public static Point Centroid(List<Point> points)
		{
			var x = 0.0;
			var y = 0.0;

			for (var i = 0; i < points.Count; i++)
			{
				x += points[i].X;
				y += points[i].Y;
			}
			x /= (double)points.Count;
			y /= (double)points.Count;

			return new Point(x, y);
		}

		// Checked
		public static Rectangle BoundingBox(List<Point> points)
		{
			var minX = Double.PositiveInfinity;
			var maxX = Double.NegativeInfinity;
			var minY = Double.PositiveInfinity;
			var maxY = Double.NegativeInfinity;

			for (var i = 0; i < points.Count; i++)
			{
				minX = Math.Min(minX, points[i].X);
				minY = Math.Min(minY, points[i].Y);
				maxX = Math.Max(maxX, points[i].X);
				maxY = Math.Max(maxY, points[i].Y);
			}

			return new Rectangle(minX, minY, maxX - minX, maxY - minY);
		}

		public static double PathDistance(List<Point> pts1, List<Point> pts2)
		{
			var d = 0.0;
			for (var i = 0; i < pts1.Count; i++) // assumes pts1.length == pts2.length
				d += Distance(pts1[i], pts2[i]);
			return d / pts1.Count;
		}
	}
}

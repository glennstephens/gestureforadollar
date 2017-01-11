using System;

namespace GestureForADollar.Core
{
	public class Result
	{
		public double Score
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool HasMatch
		{
			get
			{
				return Score > 0;
			}
		}

		public double IndicativeAngle { get; set; }

		public double IndicativeAngleInDegrees
		{
			get
			{
				return PointsHelpers.RadiansToDegree(IndicativeAngle);
			}
		}

		public Rectangle BoundingRectangle
		{
			get;
			set;
		}

		public Result(string name, double score, double indicativeAngle, Rectangle bounds)
		{
			this.Name = name;
			this.Score = score;
			this.BoundingRectangle = bounds;
			this.IndicativeAngle = indicativeAngle;
		}

		public static Result TooFewPoints()
		{
			return new Result("Too few points", -1, 0, Rectangle.Empty);
		}
	}
}

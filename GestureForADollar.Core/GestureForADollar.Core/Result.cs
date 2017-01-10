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

		public Result(string name, double score)
		{
			this.Name = name;
			this.Score = score;
		}

		public static Result TooFewPoints()
		{
			return new Result("Too few points", -1);
		}
	}
}

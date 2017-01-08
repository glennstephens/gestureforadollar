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

		public Result(string name, double score)
		{
			this.Name = name;
			this.Score = score;
		}
	}
}

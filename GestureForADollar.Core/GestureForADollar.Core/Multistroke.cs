using System;
using System.Collections.Generic;

namespace GestureForADollar.Core
{
	public class Multistroke
	{
		public string Name { get; set; }
		public bool UseBoundedRotationInvariance { get; set; }
		public List<Unistroke2> Unistrokes { get; set; }
		int NumberOfStrokes { get; set; }

		//public Multistroke(string name, bool useBoundedRotationInvariance, List<Unistroke2> strokes)
		//{
		//	Name = name;
		//	UseBoundedRotationInvariance = useBoundedRotationInvariance;
		//	NumberOfStrokes = strokes.Count;

		//	var order = new List<int>();
		//	for (var i = 0; i < strokes.Count; i++)
		//		order.Add(i); // initialize
		//	var orders = new List<int>(); // array of integer arrays
		//	PointsHelpers.HeapPermute(strokes.Count, order, /*out*/ orders);

		//	var unistrokes = PointsHelpers.MakeUnistrokes(strokes, orders); // returns array of point arrays
		//	this.Unistrokes = new List<Unistroke2>(); // unistrokes for this multistroke
		//	for (var j = 0; j < unistrokes.Count; j++)
		//		this.Unistrokes.Add(new Unistroke2(name, useBoundedRotationInvariance, unistrokes[j]));
		//}
	}
}

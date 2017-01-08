using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using GestureForADollar.Core;
using UIKit;

namespace GestureForADollar.GestureMaker
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		DollarRecognizer recognizer = new DollarRecognizer();

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.Prompt = "Draw to recognize";

			recognizer
				.AddGesture(UnistrokeSamples.Star)
				.AddGesture(UnistrokeSamples.Circle)
				.AddGesture(UnistrokeSamples.Check)
				.AddGesture(UnistrokeSamples.Arrow)
				.AddGesture(UnistrokeSamples.Caret);

			this.View = new DrawableView(View.Frame, allPoints);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		List<Point> allPoints = new List<Point>();

		public class DrawableView : UIView
		{
			readonly List<Point> allPoints;

			public DrawableView(CGRect rect, List<Point> allPoints) : base(rect)
			{
				BackgroundColor = UIColor.White;
				this.allPoints = allPoints;
			}

			public override void Draw(CGRect rect)
			{
				base.Draw(rect);

				using (CGContext g = UIGraphics.GetCurrentContext())
				{
					g.SetLineWidth(4);
					UIColor.Blue.SetStroke();

					var path = new CGPath();

					var iOSPoints = allPoints.Select(p => new CGPoint(p.X, p.Y)).ToArray();
					path.AddLines(iOSPoints);

					g.AddPath(path);
					g.DrawPath(CGPathDrawingMode.Stroke);
				}
			}
		}

		void AddPoint(Foundation.NSSet touches)
		{
			var touch = touches.AnyObject as UITouch;
			var location = touch.LocationInView(this.View);
			allPoints.Add(new Point(location.X, location.Y));

			View.SetNeedsDisplay();
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			allPoints.Clear();

			AddPoint(touches);
		}

		public override void TouchesMoved(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			AddPoint(touches);
		}

		public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			AddPoint(touches);

			// Start the recognition
			var item = recognizer.Recognize(allPoints, true);
			NavigationItem.Prompt = item.Name + " " + item.Score.ToString();
		}
	}
}

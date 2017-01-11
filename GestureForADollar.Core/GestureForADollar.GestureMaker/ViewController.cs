using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
				.AddGesture(UnistrokeSamples.Circle, true)
				.AddGesture(UnistrokeSamples.Check)
				.AddGesture(UnistrokeSamples.Arrow)
				.AddGesture(UnistrokeSamples.Rectangle, true)
				.AddGesture(UnistrokeSamples.LeftSquareBracket)
				.AddGesture(UnistrokeSamples.RightSquareBracket)
				.AddGesture(UnistrokeSamples.LeftCurlyBrace)
				.AddGesture(UnistrokeSamples.RightCurlyBrace)
				.AddGesture(UnistrokeSamples.Delete)
				.AddGesture(UnistrokeSamples.Triangle)
				.AddGesture(UnistrokeSamples.Caret)
				.AddGesture(UnistrokeSamples.Pigtail)
				.AddGesture(UnistrokeSamples.V)
				.AddGesture(UnistrokeSamples.X)
				.AddGesture(UnistrokeSamples.ZigZag);

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

			if (touch.Type != UITouchType.Stylus)
				return;
			
			var location = touch.LocationInView(this.View);
			allPoints.Add(new Point(location.X, location.Y));

			//touch.Force;
			//touch.AltitudeAngle;
			//touch.MaximumPossibleForce;
			//touch.Type = UITouchType.Stylus;

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

		public async override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			AddPoint(touches);

			// Start the recognition
			var item = recognizer.Recognize(allPoints);
			string display = string.Format("{0} - {1}, {2}", item.Name, item.Score, item.IndicativeAngleInDegrees);
			NavigationItem.Prompt = display;

			UILabel lbl = new UILabel();
			lbl.Frame = new CGRect(item.BoundingRectangle.X, item.BoundingRectangle.Y, item.BoundingRectangle.Width, item.BoundingRectangle.Height);
			lbl.BackgroundColor = UIColor.FromRGBA(0, 0, 255, 100);
			lbl.Lines = 0;
			lbl.LineBreakMode = UILineBreakMode.CharacterWrap;
			lbl.Text = display;
			Add(lbl);

			await Task.Delay(4000);
			await UIView.AnimateAsync(0.4, () =>
			{
				lbl.Alpha = 0;
			});
			lbl.RemoveFromSuperview();
		}
	}
}

using System;
using System.ComponentModel;
using GLKit;
using OpenGLES;
using Foundation;
using CoreAnimation;
using RectangleF = CoreGraphics.CGRect;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using CoreGraphics;

[assembly: ExportRenderer(typeof(FVD.Native.NativeOpenGLView), typeof(FVDpp.iOS.NativeRenderer.NativeOpenGLViewRenderer))]

namespace FVDpp.iOS.NativeRenderer {
	public class NativeOpenGLViewRenderer : ViewRenderer<FVD.Native.NativeOpenGLView, GLKView>
	{
		UIPanGestureRecognizer panGestureRecognizer;
		UIPinchGestureRecognizer pinchGestureRecognizer;
		UIRotationGestureRecognizer rotationGestureRecognizer;
		UITapGestureRecognizer tapGestureRecognizer;
		UILongPressGestureRecognizer longPressGestureRecognizer;

		CADisplayLink _displayLink;

		public void Display(object sender, EventArgs eventArgs)
		{
			if (Element.HasRenderLoop)
				return;
			SetupRenderLoop(true);
		}

		protected override void Dispose(bool disposing)
		{
			if (_displayLink != null)
			{
				_displayLink.Invalidate();
				_displayLink.Dispose();
				_displayLink = null;

				if (Element != null)
					((IOpenGlViewController)Element).DisplayRequested -= Display;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<FVD.Native.NativeOpenGLView> e)
		{
			if (e.OldElement != null)
			{
				((IOpenGlViewController)e.OldElement).DisplayRequested -= Display;
			}

			if (e.NewElement != null)
			{
				var context = new EAGLContext(EAGLRenderingAPI.OpenGLES3);
				var glkView = new GLKView(RectangleF.Empty) { Context = context, DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24, Delegate = new Delegate(e.NewElement) };
				SetNativeControl(glkView);
				glkView.Alpha = 0.0f;

				((FVD.Native.NativeOpenGLView)e.NewElement).OnFadeIn = () => { 
					var minAlpha = (nfloat)0.0f;
					var maxAlpha = (nfloat)1.0f;
					Boolean isIn = true;

					glkView.Alpha = isIn ? minAlpha : maxAlpha;
					glkView.Transform = CGAffineTransform.MakeIdentity();
					UIView.Animate(1, 0, UIViewAnimationOptions.CurveEaseInOut,
						() =>
						{
							glkView.Alpha = isIn ? maxAlpha : minAlpha;
						}, null
					);
				};

				((IOpenGlViewController)e.NewElement).DisplayRequested += Display;

				SetupRenderLoop(false);

				pinchGestureRecognizer = new UIPinchGestureRecognizer((UIPinchGestureRecognizer obj) =>
				{
					FVD.Gestures.PitchGestureRecognizer gestureRecognizer = new FVD.Gestures.PitchGestureRecognizer();

					gestureRecognizer.scale = (float)obj.Scale;

					CoreGraphics.CGPoint point = obj.LocationInView(NativeView);
					gestureRecognizer.point.x = (float)point.X;
					gestureRecognizer.point.y = (float)point.Y;

					gestureRecognizer.numberOfTouches = (int)obj.NumberOfTouches;

					if (obj.State == UIGestureRecognizerState.Began)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Began;

					if (obj.State == UIGestureRecognizerState.Ended)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Cancelled)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Changed)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Moved;

					var OnPitchRecognized = e.NewElement.OnPitchRecognized;
					if (OnPitchRecognized != null)
						OnPitchRecognized(gestureRecognizer);
				});

				panGestureRecognizer = new UIPanGestureRecognizer((UIPanGestureRecognizer obj) =>
				{
					FVD.Gestures.PanGestureRecognizer gestureRecognizer = new FVD.Gestures.PanGestureRecognizer();

					CoreGraphics.CGPoint point = obj.LocationInView(NativeView);
					gestureRecognizer.point.x = (float)point.X;
					gestureRecognizer.point.y = (float)point.Y;

					CoreGraphics.CGPoint velocity = obj.VelocityInView(NativeView);
					gestureRecognizer.velocity.x = (float)velocity.X;
					gestureRecognizer.velocity.y = (float)velocity.Y;

					gestureRecognizer.numberOfTouches = (int)obj.NumberOfTouches;

					if (obj.State == UIGestureRecognizerState.Began)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Began;

					if (obj.State == UIGestureRecognizerState.Ended)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Cancelled)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Changed)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Moved;

					var OnPanRecognized = e.NewElement.OnPanRecognized;
					if (OnPanRecognized != null)
						OnPanRecognized(gestureRecognizer);
				});

				rotationGestureRecognizer = new UIRotationGestureRecognizer((UIRotationGestureRecognizer obj) =>
				{
					FVD.Gestures.RotationGestureRecognizer gestureRecognizer = new FVD.Gestures.RotationGestureRecognizer();

					CoreGraphics.CGPoint point = obj.LocationInView(NativeView);
					gestureRecognizer.point.x = (float)point.X;
					gestureRecognizer.point.y = (float)point.Y;

					gestureRecognizer.rotation = (float)obj.Rotation;

					gestureRecognizer.numberOfTouches = (int)obj.NumberOfTouches;

					if (obj.State == UIGestureRecognizerState.Began)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Began;

					if (obj.State == UIGestureRecognizerState.Ended)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Cancelled)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Changed)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Moved;

					var OnRotationRecognized = e.NewElement.OnRotationRecognized;
					if (OnRotationRecognized != null)
						OnRotationRecognized(gestureRecognizer);
				});

				tapGestureRecognizer = new UITapGestureRecognizer((UITapGestureRecognizer obj) =>
				{
					FVD.Gestures.TapGestureRecognizer gestureRecognizer = new FVD.Gestures.TapGestureRecognizer();

					CoreGraphics.CGPoint point = obj.LocationInView(NativeView);
					gestureRecognizer.point.x = (float)point.X;
					gestureRecognizer.point.y = (float)point.Y;

					gestureRecognizer.numberOfTouches = (int)obj.NumberOfTouches;

					if (obj.State == UIGestureRecognizerState.Began)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Began;

					if (obj.State == UIGestureRecognizerState.Ended)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Cancelled)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Changed)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Moved;

					var OnTapRecognized = e.NewElement.OnTapRecognized;
					if (OnTapRecognized != null)
						OnTapRecognized(gestureRecognizer);
				});


				longPressGestureRecognizer = new UILongPressGestureRecognizer((UILongPressGestureRecognizer obj) =>
				{
					FVD.Gestures.LongPressGestureRecognizer gestureRecognizer = new FVD.Gestures.LongPressGestureRecognizer();

					CoreGraphics.CGPoint point = obj.LocationInView(NativeView);
					gestureRecognizer.point.x = (float)point.X;
					gestureRecognizer.point.y = (float)point.Y;

					gestureRecognizer.numberOfTouches = (int)obj.NumberOfTouches;

					if (obj.State == UIGestureRecognizerState.Began)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Began;

					if (obj.State == UIGestureRecognizerState.Ended)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Cancelled)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Ended;

					if (obj.State == UIGestureRecognizerState.Changed)
						gestureRecognizer.state = FVD.Gestures.RecognizerState.Moved;

					var OnLongPressRecognized = e.NewElement.OnLongPressRecognized;
					if (OnLongPressRecognized != null)
						OnLongPressRecognized(gestureRecognizer);
				});

				glkView.AddGestureRecognizer(pinchGestureRecognizer);
				glkView.AddGestureRecognizer(panGestureRecognizer);
				glkView.AddGestureRecognizer(rotationGestureRecognizer);
				glkView.AddGestureRecognizer(tapGestureRecognizer);
				glkView.AddGestureRecognizer(longPressGestureRecognizer);
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == OpenGLView.HasRenderLoopProperty.PropertyName)
				SetupRenderLoop(false);
		}

		void SetupRenderLoop(bool oneShot)
		{
			if (_displayLink != null)
				return;
			if (!oneShot && !Element.HasRenderLoop)
				return;

			_displayLink = CADisplayLink.Create(() =>
			{
				var control = Control;
				var model = Element;
				if (control != null)
					control.Display();
				if (control == null || model == null || !model.HasRenderLoop)
				{
					_displayLink.Invalidate();
					_displayLink.Dispose();
					_displayLink = null;
				}
			});

			_displayLink.AddToRunLoop(NSRunLoop.Current, NSRunLoop.NSDefaultRunLoopMode);
		}

		class Delegate : GLKViewDelegate
		{
			readonly FVD.Native.NativeOpenGLView _model;
			System.Diagnostics.Stopwatch timeElapsed = new System.Diagnostics.Stopwatch();

			public Delegate(FVD.Native.NativeOpenGLView model)
			{
				_model = model;
			}

			public override void DrawInRect(GLKView view, RectangleF rect)
			{
				_model.renderTimeInSeconds = timeElapsed.ElapsedMilliseconds * 0.001f;
				timeElapsed.Restart();

				var onDisplay = _model.OnDisplay;
				if (onDisplay == null)
					return;
				onDisplay(rect.ToRectangle());
			}
		}
	}
}
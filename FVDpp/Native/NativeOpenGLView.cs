using System;
using Xamarin.Forms;

namespace FVD.Native
{
	public class NativeOpenGLView : View, IOpenGlViewController
	{
		// Actions
		public Action<Gestures.PanGestureRecognizer> OnPanRecognized { get; set; }
		public Action<Gestures.PitchGestureRecognizer> OnPitchRecognized { get; set; }
		public Action<Gestures.RotationGestureRecognizer> OnRotationRecognized { get; set; }
		public Action<Gestures.TapGestureRecognizer> OnTapRecognized { get; set; }
		public Action<Gestures.LongPressGestureRecognizer> OnLongPressRecognized { get; set; }

		public Action OnFadeIn { get; set; }

		public float renderTimeInSeconds = 0.0f;

		public static readonly BindableProperty HasRenderLoopProperty = BindableProperty.Create("HasRenderLoop", typeof(bool), typeof(OpenGLView), default(bool));

		public bool HasRenderLoop
		{
			get { return (bool)GetValue(HasRenderLoopProperty); }
			set { SetValue(HasRenderLoopProperty, value); }
		}

		public Action<Rectangle> OnDisplay { get; set; }

		event EventHandler IOpenGlViewController.DisplayRequested
		{
			add { DisplayRequested += value; }
			remove { DisplayRequested -= value; }
		}

		public void Display()
		{
			EventHandler handler = DisplayRequested;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public void FadeIn()
		{
			if (OnFadeIn != null)
				OnFadeIn();
		}

		event EventHandler DisplayRequested;
	}
}
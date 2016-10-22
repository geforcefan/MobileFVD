using System;
using System.ComponentModel;
using Android.Opengl;
using Object = Java.Lang.Object;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using EGLConfig = Javax.Microedition.Khronos.Egl.EGLConfig;
using Javax.Microedition.Khronos.Opengles;

[assembly: ExportRenderer(typeof(FVD.Native.NativeOpenGLView), typeof(FVDpp.Droid.Renderer.NativeOpenGLViewRenderer))]

namespace FVDpp.Droid.Renderer
{
	class NativeOpenGLViewRenderer : ViewRenderer<FVD.Native.NativeOpenGLView, GLSurfaceView>
	{
		bool _disposed;

		public NativeOpenGLViewRenderer()
		{
			AutoPackage = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				_disposed = true;

				if (Element != null)
					((IOpenGlViewController)Element).DisplayRequested -= Render;
			}
			base.Dispose(disposing);
		}

		protected GLSurfaceView CreateNativeControl()
		{
			return new GLSurfaceView(Context);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<FVD.Native.NativeOpenGLView> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
				((IOpenGlViewController)Element).DisplayRequested -= Render;

			if (e.NewElement != null)
			{
				GLSurfaceView surfaceView = Control;
				if (surfaceView == null)
				{
					surfaceView = CreateNativeControl();
					surfaceView.SetEGLContextClientVersion(3);
					SetNativeControl(surfaceView);
				}

				((IOpenGlViewController)Element).DisplayRequested += Render;
				surfaceView.SetRenderer(new Renderer(Element));
				SetRenderMode();

				((FVD.Native.NativeOpenGLView)e.NewElement).OnFadeIn = () =>
				{
					surfaceView.Alpha = 1.0f;
				};
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == OpenGLView.HasRenderLoopProperty.PropertyName)
				SetRenderMode();
		}

		void Render(object sender, EventArgs eventArgs)
		{
			if (Element.HasRenderLoop)
				return;
			Control.RequestRender();
		}

		void SetRenderMode()
		{
			Control.RenderMode = Element.HasRenderLoop ? Rendermode.Continuously : Rendermode.WhenDirty;
		}

		class Renderer : Object, GLSurfaceView.IRenderer
		{
			readonly FVD.Native.NativeOpenGLView _model;
			Rectangle _rect;

			public Renderer(FVD.Native.NativeOpenGLView model)
			{
				_model = model;
			}

			public void OnDrawFrame(IGL10 gl)
			{
				Action<Rectangle> onDisplay = _model.OnDisplay;
				if (onDisplay == null)
					return;
				onDisplay(_rect);
			}

			public void OnSurfaceChanged(IGL10 gl, int width, int height)
			{
				_rect = new Rectangle(0.0, 0.0, width, height);
			}

			public void OnSurfaceCreated(IGL10 gl, EGLConfig config)
			{
			}
		}
	}
}
using System;

using OpenTK.Graphics.ES30;

using Xamarin.Forms;
using FVD.Camera;
using System.Linq;

namespace FVD.TrackView
{
	public partial class MainView : ContentPage
	{
		private Model.Track currentTrack;

		private Native.NativeOpenGLView glView = null;
		private bool initIsDone = false;

		private Vec2 windowDimensions = new Vec2(0, 0);

		// Shaders
		Core.ShaderProgram floorShader;
		Core.ShaderProgram trackShader;

		// Renderer
		Renderer.Floor floorRenderer = null;
		Renderer.Track trackRenderer = null;
		Renderer.TrackSectionPicker trackSectionPicker = null;

		public MainView()
		{
			InitializeComponent();
		}

		private void createGL()
		{
			glView = new Native.NativeOpenGLView
			{
				HasRenderLoop = true
			};
			Console.WriteLine("createGL");

			glView.WidthRequest = windowDimensions.X;
			glView.HeightRequest = windowDimensions.Y;

			glView.OnDisplay = r =>
			{
				Console.WriteLine("OnDisplay");

				if (!initIsDone)
					initGL();
			};

			GLLayer.Children.Add(glView);
		}

		private void initGL()
		{
			Console.WriteLine("initGL");

			initIsDone = true;

			printContextInformation();

			Core.Texture.init();
			initCameras();
			initShaders();
			initRenderers();

			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			glView.OnDisplay = r =>
			{
				renderGL();
				glView.FadeIn();

				glView.OnDisplay = b =>
				{
					renderGL();
				};
			};
		}

		private void initShaders()
		{
			floorShader = new Core.ShaderProgram("FloorVertex", "FloorFragment");
			trackShader = new Core.ShaderProgram("TrackVertex", "TrackFragment");
		}

		private void initRenderers()
		{
			floorRenderer = new Renderer.Floor();

			initTrack(currentTrack);
		}

		private void renderGL()
		{
			GL.Viewport(0, 0, (int)cameraManager.currentCamera.viewPortWidth, (int)cameraManager.currentCamera.viewPortHeight);

			GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
			GL.Clear((ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit));

			FPS.Text = (int)(1000.0f / (glView.renderTimeInSeconds / 0.001f)) + " FPS";

			cameraManager.currentCamera.Update();

			floorRenderer.Render(floorShader, cameraManager.currentCamera);

			if (trackRenderer != null)
				trackRenderer.Render(trackShader, cameraManager.currentCamera);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			createGL();
		}

		protected override void OnSizeAllocated(double w, double h)
		{
			base.OnSizeAllocated(w, h);
			windowDimensions.X = w;
			windowDimensions.Y = h;

			if (initIsDone)
			{
				glView.HeightRequest = h;
				glView.WidthRequest = w;

				cameraManager.SetViewPort(windowDimensions.X, windowDimensions.Y);
			}
		}

		private Vec2 getWindowDimensions()
		{
			return windowDimensions;
		}

		private void printContextInformation()
		{
			Console.WriteLine("===============================================================");
			Console.WriteLine("Version: " + GL.GetString(StringName.Version));
			Console.WriteLine("Extensions: " + GL.GetString(StringName.Extensions));
			Console.WriteLine("Renderer: " + GL.GetString(StringName.Renderer));
			Console.WriteLine("ShadingLanguageVersion: " + GL.GetString(StringName.ShadingLanguageVersion));
			Console.WriteLine("Vendor: " + GL.GetString(StringName.Vendor));
			Console.WriteLine("Version: " + GL.GetString(StringName.Version));
			Console.WriteLine("===============================================================");
		}
	}
}
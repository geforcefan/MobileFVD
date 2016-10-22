using System;
using GlmNet;

namespace FVD.Camera
{
	public abstract class Camera
	{
		public float fov = 90.0f;
		public float near = 0.1f;
		public float far = 1000.0f;

		public mat4 ProjectionMatrix;
		public mat4 ModelMatrix;
		public mat4 ProjectionModelMatrix;

		public double viewPortHeight;
		public double viewPortWidth;

		public vec3 cameraPos = new vec3(5.0f, 5.0f, 7.0f);

		public bool cameraIsMoving = false;

		private CameraManager cameraManager;

		abstract public void Update();
		abstract public void StopMovement();

		public Action<Gestures.PanGestureRecognizer> OnPanRecognized { get; set; }
		public Action<Gestures.PitchGestureRecognizer> OnPitchRecognized { get; set; }
		public Action<Gestures.RotationGestureRecognizer> OnRotationRecognized { get; set; }
		public Action<Gestures.TapGestureRecognizer> OnTapRecognized { get; set; }
		public Action<Gestures.LongPressGestureRecognizer> OnLongPressRecognized { get; set; }

		public void setViewPort(double w, double h)
		{
			viewPortHeight = h;
			viewPortWidth = w;
		}

		public void setCameraManager(CameraManager _cameraManager)
		{
			cameraManager = _cameraManager;
		}

		public float getRenderTimeInSeconds()
		{
			return cameraManager.getRenderTimeInSeconds();
		}
	}
}

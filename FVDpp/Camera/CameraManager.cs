using System;
using System.Collections.Generic;

namespace FVD.Camera
{
	public class CameraManager
	{
		public class CameraInstance
		{
			public Camera cam;
			public String identifier;

			public CameraInstance(Camera _cam, String _identifier)
			{
				cam = _cam;
				identifier = _identifier;
			}
		}

		List<CameraInstance> cameras = new List<CameraInstance>();
		Native.NativeOpenGLView glView = null;

		public Camera currentCamera = null;
		private int currentIndex = 0;

		public CameraManager(Native.NativeOpenGLView _glView)
		{
			glView = _glView;
		}

		public String GetActiveCamera()
		{
			return cameras[currentIndex].identifier;
		}

		public void AddCamera(Camera cam, String identifier)
		{
			cameras.Add(new CameraInstance(cam, identifier));
		}

		private CameraInstance GetCameraInstance(String identifier)
		{
			for (int i = 0; i < cameras.Count; i++)
			{
				if (cameras[i].identifier == identifier)
				{
					return cameras[i];
				}
			}

			return null;
		}

		private int GetIndexOfCameraInstance(CameraInstance camInstance)
		{
			for (int i = 0; i < cameras.Count; i++)
			{
				if (cameras[i] == camInstance)
				{
					return i;
				}
			}

			return -1;
		}

		private int GetIndexOfCameraInstance(String identifier)
		{
			return GetIndexOfCameraInstance(GetCameraInstance(identifier));
		}

		public void ReplaceCamera(String identifier, Camera cam)
		{
			int index = GetIndexOfCameraInstance(identifier);

			cam.setViewPort(cameras[index].cam.viewPortWidth, cameras[index].cam.viewPortHeight);

			if (currentCamera == cameras[index].cam)
			{
				currentIndex = index;
				currentCamera = cam;
			}

			cameras[index].cam = cam;
		}

		private void RegisterEvents(Camera cam)
		{
			Console.WriteLine("GlView instance is: " + glView);
			Console.WriteLine("Camera instance is: " + cam);

			glView.OnPanRecognized += cam.OnPanRecognized;
			glView.OnPitchRecognized += cam.OnPitchRecognized;
			glView.OnRotationRecognized += cam.OnRotationRecognized;
			glView.OnTapRecognized += cam.OnTapRecognized;
			glView.OnLongPressRecognized += cam.OnLongPressRecognized;

			cam.setCameraManager(this);
		}

		private void RemoveEvents(Camera cam)
		{
			glView.OnPanRecognized -= cam.OnPanRecognized;
			glView.OnPitchRecognized -= cam.OnPitchRecognized;
			glView.OnRotationRecognized -= cam.OnRotationRecognized;
			glView.OnTapRecognized -= cam.OnTapRecognized;
			glView.OnLongPressRecognized -= cam.OnLongPressRecognized;
		}

		public String AcivateNext()
		{
			currentIndex++;
			if (currentIndex >= cameras.Count)
				currentIndex = 0;

			Activate(cameras[currentIndex].identifier);

			return cameras[currentIndex].identifier;
		}

		public void Activate(String identifier)
		{
			Console.WriteLine("Activate Camera: " + identifier);

			CameraInstance camInstance = GetCameraInstance(identifier);

			Console.WriteLine("Found Camera Instance: " + camInstance);

			if (camInstance != null)
			{
				if (currentCamera != null)
				{
					Console.WriteLine("Remove Events from currentCamera: " + currentCamera);

					RemoveEvents(currentCamera);
				}

				Console.WriteLine("Stop movement on Camera: " + camInstance.cam);

				camInstance.cam.StopMovement();

				Console.WriteLine("Set current Camera: " + camInstance.cam);
				currentCamera = camInstance.cam;

				Console.WriteLine("Gonna Set current Index to: " + camInstance);
				currentIndex = GetIndexOfCameraInstance(camInstance);

				Console.WriteLine("current Index is now: " + currentIndex);
				Console.WriteLine("Register Events to currentCamera: " + currentCamera);

				RegisterEvents(currentCamera);
			}
		}

		public void SetViewPort(double w, double h)
		{
			for (int i = 0; i < cameras.Count; i++)
			{ 
				cameras[i].cam.setViewPort(w * 2.0f, h * 2.0f);
			}	
		}

		public float getRenderTimeInSeconds()
		{
			return glView.renderTimeInSeconds;
		}
	}
}

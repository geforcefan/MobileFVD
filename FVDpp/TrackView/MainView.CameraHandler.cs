using System;
using FVD.Camera;

namespace FVD.TrackView
{
	public partial class MainView
	{
		CameraManager cameraManager;
		Model.Section activeSctionBeforePOV;

		private void initCameras()
		{
			cameraManager = new CameraManager(glView);
			cameraManager.AddCamera(new FreeCamera(), "FreeCamera");
			cameraManager.AddCamera(new POVCamera(currentTrack), "POV");

			cameraManager.SetViewPort(windowDimensions.X, windowDimensions.Y);
			cameraManager.Activate("FreeCamera");
		}

		void OnChangeCamera(object sender, System.EventArgs e)
		{
			String whichCamera = cameraManager.AcivateNext();

			// On POV mode, we want to deactivate any selection, but we will restore it by changing the view back
			/*if (whichCamera.Equals("POV"))
			{
				activeSctionBeforePOV = currentTrack.activeSection;
				currentTrack.activateSection(null);
			}
			else {
				currentTrack.activateSection(activeSctionBeforePOV);
			}*/
		}
	}
}

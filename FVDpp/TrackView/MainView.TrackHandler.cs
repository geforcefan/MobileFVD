using System;
using GlmNet;
using System.Linq;
using OpenTK.Graphics.ES30;
using System.Threading.Tasks;

namespace FVD.TrackView
{
	public partial class MainView
	{
		public void initTrack(Model.Track track)
		{
			currentTrack = track;
			currentTrack.DrawHeartline = true;

			Title = track.Name;

			if (glView != null)
			{
				// This is only a workaround. glView is hold globally to prevent creating an instance over and over again.
				// But we have to unset the content first and append glView again, thus the ContentPage
				// can trigger events to display the glView. Otherwise the view stays white
				//Content = null;
				//Content = InsideLayout;
				Console.WriteLine("Initing new track");

				trackRenderer = new Renderer.Track(currentTrack);
				trackSectionPicker = new Renderer.TrackSectionPicker();

				currentTrack.OnSectionActivated -= OnSectionActivated;
				currentTrack.OnSectionActivated += OnSectionActivated;

				currentTrack.startPos = new vec3(0.0f, 30.0f, 0.0f);

				currentTrack.insertSection(Model.SectionType.Forced, -1);

				((Model.SectionForce)currentTrack.sections.First()).normForce.subFunctions[0].changeLength(1.0f);
				((Model.SectionForce)currentTrack.sections.First()).normForce.subFunctions[0].symArg = -1.0f;

				((Model.SectionForce)currentTrack.sections.First()).normForce.appendSubFunction(1.0f, 0);
				((Model.SectionForce)currentTrack.sections.First()).normForce.subFunctions[1].symArg = 0;

				((Model.SectionForce)currentTrack.sections.First()).normForce.appendSubFunction(1.0f, 1);
				((Model.SectionForce)currentTrack.sections.First()).normForce.subFunctions[2].symArg = 3.6f;

				((Model.SectionForce)currentTrack.sections.First()).normForce.appendSubFunction(0.8f, 2);
				((Model.SectionForce)currentTrack.sections.First()).normForce.subFunctions[3].symArg = 0.0f;

				((Model.SectionForce)currentTrack.sections.First()).normForce.appendSubFunction(1.0f, 3);
				((Model.SectionForce)currentTrack.sections.First()).normForce.subFunctions[4].symArg = -4.0f;

				((Model.SectionForce)currentTrack.sections.First()).normForce.appendSubFunction(1.2f, 4);
				((Model.SectionForce)currentTrack.sections.First()).normForce.subFunctions[5].symArg = 0.0f;

				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[0].changeLength(1.9f);
				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[0].symArg = 0.0f;

				((Model.SectionForce)currentTrack.sections.First()).rollFunc.appendSubFunction(1.0f, 0);
				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[1].changeDegree(Model.FunctionDegree.Cubic);
				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[1].symArg = -49.0f;

				((Model.SectionForce)currentTrack.sections.First()).rollFunc.appendSubFunction(1.0f, 1);
				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[2].changeDegree(Model.FunctionDegree.Cubic);
				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[2].symArg = 100.0f;

				((Model.SectionForce)currentTrack.sections.First()).rollFunc.appendSubFunction(0.6f, 2);
				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[3].changeDegree(Model.FunctionDegree.Cubic);
				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[3].symArg = -51.0f;

				((Model.SectionForce)currentTrack.sections.First()).rollFunc.appendSubFunction(1.5f, 3);
				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[4].changeDegree(Model.FunctionDegree.Cubic);
				((Model.SectionForce)currentTrack.sections.First()).rollFunc.subFunctions[4].symArg = 0.0f;

				((Model.SectionForce)currentTrack.sections.First()).latForce.lockFunction(0);

				/*currentTrack.insertSection(Model.SectionType.Straight, -1);
				((Model.SectionStraight)currentTrack.sections[1]).rollFunc.subFunctions.First().maxArgument = 30.0f;
				((Model.SectionStraight)currentTrack.sections[1]).rollFunc.subFunctions.First().symArg = 180.0f;*/

				currentTrack.UpdateTrack(0, 0);

				// We have to bind some events, so the track can manipuated for certain touch gestures
				glView.OnTapRecognized -= SectionOnScreenTapped; // First remove the tap callback
				glView.OnTapRecognized += SectionOnScreenTapped;

				glView.OnLongPressRecognized -= SectionOnLongPressRecognized;
				glView.OnLongPressRecognized += SectionOnLongPressRecognized;

				// Reset the POV Camera
				cameraManager.ReplaceCamera("POV", new Camera.POVCamera(currentTrack));
			}
		}
	}
}

using System;
namespace FVD.TrackView
{
	public partial class MainView {
		private bool longPressTurendOn = false;

		async public System.Threading.Tasks.Task ShowNewSectionDialog(Model.Section sectionToActivateOnCancel)
		{
			var answer = await DisplayActionSheet("New Section", "Cancel", null, "Straight Section", "Forced Section");
			switch (answer)
			{
				case "Straight Section":
					currentTrack.insertSection(Model.SectionType.Straight, -1);
					break;
				case "Forced Section":
					currentTrack.insertSection(Model.SectionType.Forced, -1);
					break;
				case "Cancel":
					currentTrack.activateSection(sectionToActivateOnCancel);
					break;
			}
		}

		async public System.Threading.Tasks.Task ShowDeleteSectionDialog(Model.Section section, Model.Section sectionToActivateOnCancel)
		{
			var answer = await DisplayActionSheet("Delete Section?", "Cancel", "Delete");
			switch (answer)
			{
				case "Delete":
					currentTrack.removeSection(section);
					break;
				case "Cancel":
					currentTrack.activateSection(sectionToActivateOnCancel);
					break;
			}
		}

		public void OnSectionActivated(Model.Section section)
		{
			Console.WriteLine("OnSectionActivated in MainView Handler: " + section + " - longPressTurendOn: " + longPressTurendOn);

			// prevent doing anything when the activision came from a long press:
			// We have the menu for deleting or adding a section by long pressing, thats
			// why we dont want to open the properties of the section
			if (longPressTurendOn)
			{
				return;
			}

			if (section == null)
			{
				SectionProperties.IsVisible = false;
			}
			else {
				Console.WriteLine("SectionProperties Reference: " + SectionProperties);

				SectionProperties.Children.Clear();
				SectionProperties.Children.Add(new SectionParameter.SectionParameterView(section));
				SectionProperties.IsVisible = true;
			}
		}

		public void SectionOnScreenTapped(Gestures.TapGestureRecognizer gestureRecognizer)
		{
			if (cameraManager.currentCamera.cameraIsMoving)
				return;
			
			if (gestureRecognizer.numberOfTouches == 1 && gestureRecognizer.state == Gestures.RecognizerState.Ended)
			{
				// we have to prevent a tap when we have long pressed already
				if (longPressTurendOn)
				{
					return;
				}

				if (trackSectionPicker != null)
				{
					Model.Section selectedSection = trackSectionPicker.pickSection(currentTrack, gestureRecognizer.point, cameraManager.currentCamera);

					currentTrack.activateSection(selectedSection);
				}
			}
		}

		async public void SectionOnLongPressRecognized(Gestures.LongPressGestureRecognizer gestureRecognizer)
		{
			cameraManager.currentCamera.StopMovement();

			if (gestureRecognizer.state == Gestures.RecognizerState.Began)
			{
				longPressTurendOn = true;
			}

			if (gestureRecognizer.state == Gestures.RecognizerState.Ended)
			{
				longPressTurendOn = false;
			}

			if (gestureRecognizer.numberOfTouches == 1 && gestureRecognizer.state == Gestures.RecognizerState.Began)
			{
				if (trackSectionPicker != null)
				{
					Model.Section lastSection = currentTrack.activeSection;

					Model.Section selectedSection = trackSectionPicker.pickSection(currentTrack, gestureRecognizer.point, cameraManager.currentCamera);
					currentTrack.activateSection(selectedSection);

					if (selectedSection != null)
					{
						await ShowDeleteSectionDialog(selectedSection, lastSection);
					}
					else {
						await ShowNewSectionDialog(lastSection);
					}
				}
			}
		}
	}
}
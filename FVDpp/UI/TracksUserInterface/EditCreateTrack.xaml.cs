using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;

namespace FVD.UI
{
	public partial class EditCreateTrack : ContentPage
	{
		private Model.Track track = null;
		private Model.Project project = null;
		private ProjectOverview projectPage = null;

		public EditCreateTrack(Model.Project _project, Model.Track _track, ProjectOverview _projectPage)
		{
			BindingContext = track = _track;
			projectPage = _projectPage;
			project = _project;

			track.ProjectID = project.ProjectID;

			InitializeComponent();

			if (track.TrackID > 0)
			{
				Title = track.Name;
				ButtonSave.Text = "Save";
			}
			else {
				ButtonSave.Text = "Create";
			}

			InputName.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence | KeyboardFlags.Spellcheck);
			InputDescription.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence | KeyboardFlags.Spellcheck);
		}

		async public System.Threading.Tasks.Task<Boolean> checkFormValidation()
		{
			if (string.IsNullOrWhiteSpace(InputName.Text))
			{
				await DisplayAlert("Validation Error", "Please provide a Track Name", "OK");
				return false;
			}
			else {
				return true;
			}
		}

		async public void OnTrackCancel(object sender, System.EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		async public void OnTrackSave(object sender, System.EventArgs e)
		{
			if (track.TrackID > 0)
			{
				if (await checkFormValidation())
				{
					track.Name = InputName.Text;
					track.Description = InputDescription.Text;
					track.DrawTrack = DrawTrack.On;
					track.DrawHeartline = DrawHeartline.On;

					Services.TrackService.updateTrack(track);
					project.SortTrackList();

					await Navigation.PopModalAsync();
				}
			}
			else {
				if (await checkFormValidation())
				{
					track.Name = InputName.Text;
					track.Description = InputDescription.Text;
					track.DrawTrack = DrawTrack.On;
					track.DrawHeartline = DrawHeartline.On;

					try
					{
						Services.TrackService.createTrack(track);

						project.TrackList.Add(track);
						project.SortTrackList();

						await projectPage.Navigation.PopModalAsync();
						//await projectPage.Navigation.PushAsync(new UI.ProjectOverview(project));
					}
					catch (SQLite.SQLiteException ex)
					{
						await DisplayAlert("Track Exists", ex.ToString() + "Please chose a different track name", "OK");
					}
				}
			}
		}
	}
}


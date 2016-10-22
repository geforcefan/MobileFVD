using System;
using System.Collections.Generic;

using System.Linq;
using Xamarin.Forms;

namespace FVD.UI
{
	public partial class ProjectOverview : ContentPage
	{
		private Model.Project project;

		public ProjectOverview(Model.Project _project)
		{
			project = _project;
			InitializeComponent();

			var tracks = Services.TrackService.getAllTracks(project.ProjectID);

			project.TrackList.Clear();

			foreach (var track in tracks)
				project.TrackList.Add(track);

			project.SortTrackList();

			TrackView.HasUnevenRows = true;
			TrackView.ItemsSource = project.TrackList;

			Title = project.Name;
		}

		async public void OnTrackNew(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new EditCreateTrack(project, new Model.Track(), this)));
		}

		public void OnTrackDelete(object sender, System.EventArgs e)
		{
			var mi = ((MenuItem)sender);

			Services.TrackService.deleteTrack((Model.Track)mi.CommandParameter);
			Core.Global.Projects.getProjectByID(project.ProjectID).TrackList.Remove((Model.Track)mi.CommandParameter);
		}

		async public void OnTrackSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return;
			}

			Model.Track selectedTrack = ((Model.Track)(((ListView)sender).SelectedItem));

			await Navigation.PushAsync(Core.Global.getMainViewForTrack(selectedTrack));
			((ListView)sender).SelectedItem = null;
		}

		async public void OnTrackEdit(object sender, System.EventArgs e)
		{
			var trackEditMenuItem = ((MenuItem)sender);
			await Navigation.PushModalAsync(new NavigationPage(new EditCreateTrack(project, (Model.Track)trackEditMenuItem.CommandParameter, this)));
		}

		void SearchBarOnChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			TrackView.BeginRefresh();

			if (string.IsNullOrWhiteSpace(e.NewTextValue))
				TrackView.ItemsSource = project.TrackList;
			else
				TrackView.ItemsSource = project.TrackList.Where(i => i.Name.ToLower().Contains(e.NewTextValue.ToLower()));

			TrackView.EndRefresh();
		}
	}
}


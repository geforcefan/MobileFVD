using System;
using System.Collections.Generic;
using System.Linq;

namespace FVD.Core
{
	class ProjectMainView
	{
		public int ProjectID { set; get; }
		public TrackView.MainView View { set; get; }
	}

	static class Global
	{
		static List<ProjectMainView> projectMainViews = new List<ProjectMainView>();

		static public Model.Projects Projects = new FVD.Model.Projects();

		static public TrackView.MainView getMainViewForTrack(Model.Track track)
		{
			TrackView.MainView view = null;

			try
			{
				view = Global.projectMainViews.Single(v => v.ProjectID.Equals(track.ProjectID)).View;
			}
			catch (Exception)
			{
				view = new TrackView.MainView();
				
				Global.projectMainViews.Add(new ProjectMainView
				{
					ProjectID = track.ProjectID,
					View = view
				});
			}

				//view = new UI.ProjectView();

			view.initTrack(track);
			return view;
		}
	}
}


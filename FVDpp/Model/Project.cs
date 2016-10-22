using System;
using System.Linq;
using System.Collections.ObjectModel;
using SQLite;

namespace FVD.Model
{
	[Table("Projects")]
	public class Project
	{
		[PrimaryKey, AutoIncrement]
		public int ProjectID { get; set; }

		[MaxLength(64), Unique]
		public String Name { get; set; }

		[MaxLength(64)]
		public String Description { get; set; }

		[Ignore]
		public ObservableCollection<Track> TrackList { get; set; }

		public Project()
		{
			Name = "";
			Description = "";
			TrackList = new ObservableCollection<Track>();
		}

		public void SortTrackList()
		{
			ObservableCollection<Track> TrackListSort = new ObservableCollection<Track>(TrackList.OrderBy(TrackList => TrackList.Name));
			TrackList.Clear();
			foreach (var project in TrackListSort)
			{
				TrackList.Add(project);
			}
		}
	}
}


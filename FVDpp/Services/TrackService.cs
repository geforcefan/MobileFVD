using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;

namespace FVD.Services
{
	static class TrackService
	{
		static public TableQuery<Model.Track> getAllTracks(int projectID)
		{
			var db = Core.Database.getDB();
			db.CreateTable<Model.Track>();

			var Tracks = db.Table<Model.Track>().Where(v => v.ProjectID.Equals(projectID));
			return Tracks;
		}

		static public int createTrack(Model.Track track)
		{
			var db = Core.Database.getDB();
			return db.Insert(track);
		}

		static public void deleteTrack(Model.Track track)
		{
			var db = Core.Database.getDB();
			db.Delete(track);
		}

		static public void updateTrack(Model.Track track)
		{
			var db = Core.Database.getDB();
			db.Update(track);
		}
	}
}


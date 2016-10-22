using System;
using System.Collections.Generic;
using SQLite;
using System.Diagnostics;

namespace FVD.Services
{
	static class ProjectService
	{
		static public TableQuery<Model.Project> getAllProjects()
		{
			var db = Core.Database.getDB();
			db.CreateTable<Model.Project>();

			var ProjectTable = db.Table<Model.Project>();
			return ProjectTable;
		}

		static public int createProject(Model.Project project)
		{
			var db = Core.Database.getDB();
			return db.Insert(project);
		}

		static public void deleteProject(Model.Project project)
		{
			var db = Core.Database.getDB();
			db.Delete(project);
		}

		static public void updateProject(Model.Project project)
		{
			var db = Core.Database.getDB();
			db.Update(project);
		}
	}
}


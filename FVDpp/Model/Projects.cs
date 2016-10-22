using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FVD.Model
{
	public class Projects
	{
		public ObservableCollection<Project> ProjectsList { get; set; }

		public Projects()
		{
			ProjectsList = new ObservableCollection<Project>();
		}

		public Project getProjectByID(int projectID)
		{
			return ProjectsList.Single(v => v.ProjectID.Equals(projectID));
		}

		public Project getProjectByIndex(int index)
		{
			return ProjectsList[index];
		}

		public void SortProjectsList()
		{
			ObservableCollection<Project> ProjectsListSort = new ObservableCollection<Project>(ProjectsList.OrderBy(ProjectsList => ProjectsList.Name));
			ProjectsList.Clear();
			foreach (var project in ProjectsListSort)
			{
				ProjectsList.Add(project);
			}
		}
	}
}


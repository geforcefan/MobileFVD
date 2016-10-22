using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;

namespace FVD.UI
{
	public partial class ProjectsOverview : ContentPage
	{
		public ProjectsOverview()
		{
			InitializeComponent();

			var projects = Services.ProjectService.getAllProjects();

			Core.Global.Projects.ProjectsList.Clear();

			foreach (var project in projects)
				Core.Global.Projects.ProjectsList.Add(project);

			Core.Global.Projects.SortProjectsList();

			ProjectsView.HasUnevenRows = true;
			ProjectsView.ItemsSource = Core.Global.Projects.ProjectsList;
		}

		public void OnProjectDelete(object sender, System.EventArgs e)
		{
			var mi = ((MenuItem)sender);

			Services.ProjectService.deleteProject((Model.Project)mi.CommandParameter);
			Core.Global.Projects.ProjectsList.Remove((Model.Project)mi.CommandParameter);
		}

		async public void OnProjectSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return;
			}

			await Navigation.PushAsync(new UI.ProjectOverview((Model.Project)(((ListView)sender).SelectedItem)));
			((ListView)sender).SelectedItem = null;
		}

		async public void OnProjectNew(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new EditCreateProject(new Model.Project(), this)));
		}

		async public void OnProjectEdit(object sender, System.EventArgs e)
		{
			var projectEditMenuItem = ((MenuItem)sender);
			await Navigation.PushModalAsync(new NavigationPage(new EditCreateProject((Model.Project)projectEditMenuItem.CommandParameter, this)));
		}

		void SearchBarOnChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			ProjectsView.BeginRefresh();

			if (string.IsNullOrWhiteSpace(e.NewTextValue))
				ProjectsView.ItemsSource = Core.Global.Projects.ProjectsList;
			else
				ProjectsView.ItemsSource = Core.Global.Projects.ProjectsList.Where(i => i.Name.Contains(e.NewTextValue));

			ProjectsView.EndRefresh();
		}
	}
}


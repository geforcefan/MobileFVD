using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Linq;

using Xamarin.Forms;

namespace FVD.UI
{
	public partial class EditCreateProject : ContentPage
	{
		private Model.Project project = null;
		private ProjectsOverview projectPage = null;

		public EditCreateProject(Model.Project _project, ProjectsOverview _projectPage)
		{
			BindingContext = project = _project;
			projectPage = _projectPage;

			InitializeComponent();

			if (project.ProjectID > 0)
			{
				Title = project.Name;
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
				await DisplayAlert("Validation Error", "Please provide a Project Name", "OK");
				return false;
			}
			else {
				return true;
			}
		}

		async public void OnProjectCancel(object sender, System.EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		async public void OnProjectSave(object sender, System.EventArgs e)
		{
			if (project.ProjectID > 0)
			{
				if (await checkFormValidation())
				{
					project.Name = InputName.Text;
					project.Description = InputDescription.Text;

					Services.ProjectService.updateProject(project);
					Core.Global.Projects.SortProjectsList();

					await Navigation.PopModalAsync();
				}
			}
			else {
				if (await checkFormValidation())
				{
					project.Name = InputName.Text;
					project.Description = InputDescription.Text;

					try
					{
						Services.ProjectService.createProject(project);
						Core.Global.Projects.ProjectsList.Add(project);
						Core.Global.Projects.SortProjectsList();

						await projectPage.Navigation.PopModalAsync();
						await projectPage.Navigation.PushAsync(new UI.ProjectOverview(project));
					}
					catch (SQLite.SQLiteException)
					{
						await DisplayAlert("Project Exists", "Please chose a different project name", "OK");
					}
				}
			}
		}
	}
}


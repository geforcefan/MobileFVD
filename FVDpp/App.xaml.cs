using Xamarin.Forms;
using GlmNet;
using System.Linq;

namespace FVD
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			//MainPage = new NavigationPage(new FVD.UI.ProjectsOverview());

			Model.Track track = new Model.Track();
			track.Name = "Test Coaster";
			track.ProjectID = 1;

			MainPage = new NavigationPage(Core.Global.getMainViewForTrack(track));
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}


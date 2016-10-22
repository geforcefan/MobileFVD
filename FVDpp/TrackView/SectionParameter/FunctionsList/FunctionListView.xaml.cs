using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace FVD.TrackView.SectionParameter
{
	public partial class FunctionsListView : ContentView
	{
		public FunctionsListView()
		{
			InitializeComponent();
		}

		public void AddFunction(Model.Function function, Color color)
		{
			Functions.Children.Add(new FunctionListView(function, color));
		}
	}
}

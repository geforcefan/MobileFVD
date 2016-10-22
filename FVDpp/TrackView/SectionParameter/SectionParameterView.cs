using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FVD.TrackView.SectionParameter
{
	public class SectionParameterView : ContentView
	{
		Model.Section section;

		public SectionParameterView(Model.Section _section)
		{
			section = _section;

			if (section.type == Model.SectionType.Forced)
			{
				FunctionsListView fl = new FunctionsListView();
				fl.AddFunction(((Model.SectionForce)section).normForce, Color.Blue);
				fl.AddFunction(((Model.SectionForce)section).rollFunc, Color.Red);
				fl.AddFunction(((Model.SectionForce)section).latForce, Color.Green);
				Content = fl;
			}

			if (section.type == Model.SectionType.Straight)
				Content = new SectionParameterStraightView(section);
		}
	}
}

using System;

using Xamarin.Forms;

namespace FVD.TrackView.SectionParameter
{
	public class FunctionListView : ContentView
	{
		Model.Function function;
		StackLayout subfunctionsLayout;
		Color color;

		public FunctionListView(Model.Function _function, Color _color)
		{
			function = _function;
			color = _color;

			subfunctionsLayout = new StackLayout { HeightRequest= 30 , Spacing = 0, Orientation= StackOrientation.Horizontal };
			Content = subfunctionsLayout;

			for (int i = 0; i < _function.subFunctions.Count; i++)
			{
				Console.WriteLine("Adding SubFunction " + i);
				AddSubFunction(_function.subFunctions[i]);
			}
		}

		public void OnTapGestureRecognizerTapped(object sender, EventArgs args) {
		}

		public void AddSubFunction(Model.SubFunction subFunction)
		{
			Color borderColor = new Color(color.R - 0.4, color.G - 0.4 , color.B - 0.4);
			StackLayout border = new StackLayout { 
				BackgroundColor = borderColor, 
				Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 }, 
				VerticalOptions = LayoutOptions.FillAndExpand, 
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			//border.GestureRecognizers.Add(new TapGestureRecognizer(OnTapGestureRecognizerTapped));

			StackLayout inner = new StackLayout { BackgroundColor = color, Margin = new Thickness { Bottom = 0, Left = 0, Right = 1, Top = 0 }, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };

			Label valueLabel = new Label { TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand };
			inner.Children.Add(valueLabel);

			border.Children.Add(inner);
			subfunctionsLayout.Children.Add(border);

			RenderValueForSubFunction(subfunctionsLayout.Children.Count - 1);
			RenderDeactivateSubFunction(subfunctionsLayout.Children.Count - 1);
		}

		private void RenderActivateSubFunction(int index)
		{
			((StackLayout)subfunctionsLayout.Children[index]).Opacity = 1.0f;
		}

		private void RenderDeactivateSubFunction(int index)
		{
			((StackLayout)subfunctionsLayout.Children[index]).Opacity = 0.5f;
		}

		private void RenderValueForSubFunction(int index)
		{
			Model.SubFunction sf = function.subFunctions[index];
			float val = sf.maxArgument - sf.minArgument;

			subfunctionsLayout.Children[index].WidthRequest = valToPixel(val);

			((Label)((StackLayout)((StackLayout)subfunctionsLayout.Children[index]).Children[0]).Children[0]).Text = String.Format("{0:0.00} s", val);
		}

		private int valToPixel(float val)
		{
			return (int)(val * 100);
		}

	}
}


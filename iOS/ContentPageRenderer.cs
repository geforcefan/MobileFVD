﻿using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ContentPageRenderer))]
namespace FVDpp.iOS
    {
        public class ContentPageRenderer : PageRenderer
	{
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			var itemsInfo = (this.Element as ContentPage).ToolbarItems;

			var navigationItem = this.NavigationController.TopViewController.NavigationItem;
			var leftNativeButtons = (navigationItem.LeftBarButtonItems ?? new UIBarButtonItem[] { }).ToList();
			var rightNativeButtons = (navigationItem.RightBarButtonItems ?? new UIBarButtonItem[] { }).ToList();

			rightNativeButtons.ForEach(nativeItem =>
			{
				var info = GetButtonInfo(itemsInfo, nativeItem.Title);

				if (info == null || info.Priority != 0)
				{
					if (info.Priority == 1)
						nativeItem.Style = UIBarButtonItemStyle.Done;

					return;
				}

				rightNativeButtons.Remove(nativeItem);
				leftNativeButtons.Add(nativeItem);
			});

			navigationItem.RightBarButtonItems = rightNativeButtons.ToArray();
			navigationItem.LeftBarButtonItems = leftNativeButtons.ToArray();
		}

		private ToolbarItem GetButtonInfo(IList<ToolbarItem> items, string name)
		{
			if (string.IsNullOrEmpty(name) || items == null)
				return null;

			return items.ToList().Where(itemData => name.Equals(itemData.Name)).FirstOrDefault();
		}
	}
	}
// This file has been autogenerated from parsing an Objective-C header file added in Xcode.
using System;
using CoreGraphics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using OneTradeCentral.DTOs;

namespace OneTradeCentral.iOS
{
	public partial class MainViewController : UIViewController
	{
		static UIPopoverController menuPopoverController;
		static UIBarButtonItem navbarButton;

		public MainViewController (IntPtr handle) : base (handle)
		{
			Title = NSBundle.MainBundle.LocalizedString (AppInfo.MAIN_TITLE, AppInfo.MAIN_TITLE);
			// Deprecated in iOS 7
//			ContentSizeForViewInPopover = new SizeF (320f, 600f);
		}
			

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier != "AppInfoSegue" && shouldShowNavigationItem ()) {
				var destinationNavController = segue.DestinationViewController as UINavigationController;
				var navigationItem = destinationNavController.TopViewController.NavigationItem;

				if (navbarButton != null && menuPopoverController != null) {

					navbarButton.Title = NSBundle.MainBundle.LocalizedString (AppInfo.MAIN_TITLE, AppInfo.MAIN_TITLE);
					navigationItem.LeftItemsSupplementBackButton = true;
					navigationItem.SetLeftBarButtonItem (navbarButton, true);

					if (menuPopoverController.PopoverVisible)
						menuPopoverController.Dismiss (true);
				}
			}
		}

		public override bool ShouldPerformSegue (string segueIdentifier, NSObject sender)
		{

			UISplitViewController splitViewController = SplitViewController;
			if (splitViewController.ViewControllers.Length == 2 && 
				splitViewController.ViewControllers [1] is UINavigationController) {

				UINavigationController uiNavController = (UINavigationController) splitViewController.ViewControllers [1];


				if ((uiNavController.TopViewController is OrderViewController ||
					uiNavController.TopViewController is OrderConfirmationController) 
					&& segueIdentifier != null) {

					bool confirmCancel = false;

					if (uiNavController.TopViewController is OrderConfirmationController)
						confirmCancel = true;

					if (uiNavController.TopViewController is OrderViewController &&
					    ((OrderViewController)
							((UINavigationController)splitViewController.ViewControllers [1]).TopViewController).IsEdited)
						confirmCancel = true;

					if (confirmCancel) {
						confirmUnsavedChanges (segueIdentifier, sender);
						return false;
					} 
				}
			}

			return true;
		}

		private void confirmUnsavedChanges (string segueIdentifier, NSObject sender)
		{
			InvokeOnMainThread (new Action (() => {
				UIAlertView alertView = new UIAlertView ("Unsaved Changes", "If you continue the order will be lost. Continue?", null, 
					                        NSBundle.MainBundle.LocalizedString ("Cancel", "cancel"),
					                        NSBundle.MainBundle.LocalizedString ("Yes", "yes"));

				alertView.Clicked += (object clickSender, UIButtonEventArgs e) => {
					Console.WriteLine ("Chicken!");

					if (e.ButtonIndex == 1) {
						PerformSegue (segueIdentifier, sender);
					}
				};

				alertView.Show ();
			}));
		}

		bool shouldShowNavigationItem ()
		{
			return (InterfaceOrientation == UIInterfaceOrientation.Portrait || InterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown);
		}

		public class SplitViewDelegate: UISplitViewControllerDelegate
		{

			public override bool ShouldHideViewController (UISplitViewController svc, UIViewController viewController, UIInterfaceOrientation inOrientation)
			{
				return ((inOrientation == UIInterfaceOrientation.Portrait) || (inOrientation == UIInterfaceOrientation.PortraitUpsideDown));
			}

			public override void WillHideViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem barButtonItem, UIPopoverController pc)
			{
				UINavigationController detailNavigationController = svc.ViewControllers [1] as UINavigationController;
				var navigationItem = detailNavigationController.TopViewController.NavigationItem;
				barButtonItem.Title = NSBundle.MainBundle.LocalizedString (AppInfo.MAIN_TITLE, AppInfo.MAIN_TITLE);
				navigationItem.LeftItemsSupplementBackButton = true;
				navigationItem.SetLeftBarButtonItem (barButtonItem, true);
				navbarButton = barButtonItem;
				menuPopoverController = pc;
				// disable right swipe that displays the main view controller.  iOS Bug ID# 10170209
				svc.PresentsWithGesture = false;
			}

			public override void WillShowViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem button)
			{
				UINavigationController detailNavigationController = svc.ViewControllers [1] as UINavigationController;

				// Absence of the code below will crash the app when selecting a menu in Landscape mode.
				navbarButton = button;

				var navigationItem = detailNavigationController.TopViewController.NavigationItem;
				navigationItem.SetLeftBarButtonItem (null, true);
			}

		}
	}
}

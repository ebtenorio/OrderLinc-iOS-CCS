using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;

namespace OneTradeCentral.iOS
{
	public class ActionSheetDatePicker {
		#region -= declarations =-
		
		UIActionSheet actionSheet;
		UIButton doneButton = UIButton.FromType (UIButtonType.RoundedRect);
		UIView owner;
		UILabel titleLabel = new UILabel ();
		
		#endregion
		
		#region -= properties =-
		
		/// <summary>
		/// Set any datepicker properties here
		/// </summary>
		public UIDatePicker DatePicker
		{
			get { return datePicker; }
			set { datePicker = value; }
		}
		UIDatePicker datePicker = new UIDatePicker(CGRect.Empty);
		
		/// <summary>
		/// The title that shows up for the date picker
		/// </summary>
		public string Title
		{
			get { return titleLabel.Text; }
			set { titleLabel.Text = value; }
		}
		
		#endregion
		
		#region -= constructor =-
		
		/// <summary>
		/// 
		/// </summary>
		public ActionSheetDatePicker (UIView owner)
		{
			// save our uiview owner
			this.owner = owner;
			
			// configure the title label
			titleLabel.BackgroundColor = UIColor.Clear;
			titleLabel.TextColor = UIColor.LightTextColor;
			titleLabel.Font = UIFont.BoldSystemFontOfSize (18);
			
			// configure the done button
			doneButton.SetTitle ("done", UIControlState.Normal);
			doneButton.TouchUpInside += (s, e) => { actionSheet.DismissWithClickedButtonIndex (0, true); };
			
			// create + configure the action sheet
			actionSheet = new UIActionSheet () { Style = UIActionSheetStyle.BlackTranslucent };
			actionSheet.Clicked += (s, e) => { Logger.log (String.Format("Clicked on item {0}", e.ButtonIndex)); };
			
			// add our controls to the action sheet
			actionSheet.AddSubview (datePicker);
			actionSheet.AddSubview (titleLabel);
			actionSheet.AddSubview (doneButton);
		}
		
		#endregion
		
		#region -= public methods =-
		
		/// <summary>
		/// Shows the action sheet picker from the view that was set as the owner.
		/// </summary>
		public void Show ()
		{
			// declare vars
			float titleBarHeight = 40;
			CGSize doneButtonSize = new CGSize (71, 30);
			CGSize actionSheetSize = new CGSize (owner.Frame.Width, datePicker.Frame.Height + titleBarHeight);
			CGRect actionSheetFrame = new CGRect (0, owner.Frame.Height - actionSheetSize.Height
			                                              , actionSheetSize.Width, actionSheetSize.Height);
			
			// show the action sheet and add the controls to it
			actionSheet.ShowInView (owner);
			
			// resize the action sheet to fit our other stuff
			actionSheet.Frame = actionSheetFrame;
			
			// move our picker to be at the bottom of the actionsheet (view coords are relative to the action sheet)
			datePicker.Frame = new CGRect 
				(datePicker.Frame.X, titleBarHeight, datePicker.Frame.Width, datePicker.Frame.Height);
			
			// move our label to the top of the action sheet
			titleLabel.Frame = new CGRect (10, 4, owner.Frame.Width - 100, 35);
			
			// move our button
			doneButton.Frame = new CGRect (actionSheetSize.Width - doneButtonSize.Width - 10, 7, doneButtonSize.Width, doneButtonSize.Height);
		}
		
		/// <summary>
		/// Dismisses the action sheet date picker
		/// </summary>
		public void Hide (bool animated)
		{
			actionSheet.DismissWithClickedButtonIndex (0, animated);
		}
		
		#endregion		
	}
}


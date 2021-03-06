// This file has been autogenerated from parsing an Objective-C header file added in Xcode.

using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using OneTradeCentral.DTOs;

namespace OneTradeCentral.iOS
{
	public partial class OrderInfoViewController : UIViewController
	{

		public Order Order { get; set; }
		static OrderLine SelectedOrderLine;

		NSDateFormatter dateFormatter = new NSDateFormatter(){
			DateFormat = "dd/MM/yyyy"

		};

		public OrderInfoViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			DALFacade dal = new DALFacade ();


			if (DateTime.Parse (Order.OrderDate.ToString ("d")) < DateTime.Parse (Identity.LastVersionReleaseDate ()) && dal.hasCustomerOldID (Order)) {

				CustomerNameField.Text = Order.CustomerName + " " + Order.Customer.StateCode;// dal.getCustomerByOldID (Order.Customer.ID).Name; // Order.Customer.Name;

				if (Order.ProviderID > 0) {
					ProviderLabel.Text = dal.getProviderByOldID (Order.ProviderID).Name;
				} else {
					ProviderLabel.Text = "";
				}

				if (Order.ProviderWarehouseID > 0) {

					WarehouseLabel.Text = dal.getWarehouseByOldID (Order.ProviderWarehouseID).Name;
				} else {
					WarehouseLabel.Text = "";
				}


			} else {
				CustomerNameField.Text = Order.CustomerName + " " + Order.Customer.StateCode;
				ProviderLabel.Text = dal.getProviderByID (Order.ProviderID).Name; // Provider Label
				WarehouseLabel.Text = dal.getWarehouseByID (Order.WarehouseID).Name;
			}




			if (Order.UploadStatus <= (int)Order.STATUS.Pending)
				OrderNumberLabel.Text = "Pending";
			else
				OrderNumberLabel.Text = Order.OrderNumber;

			if (dateFormatter.ToString ((NSDate)Order.OrderDate.ToLocalTime ()) == dateFormatter.ToString ((NSDate)DateTime.Now)) {
				OrderDateLabel.Text = "Today";
			} else {
				OrderDateLabel.Text = dateFormatter.ToString ((NSDate)Order.OrderDate.ToLocalTime());
			}


			if (Order.HoldDate != DateTime.MinValue)
				HoldDateLabel.Text = Order.HoldDate.ToLocalTime ().ToShortDateString ();
			else
				HoldDateLabel.Text = "N/A";


			// OldIDs code for Provider and warehousebelow
			// see ticket #132, store id is entered by sales reps
			StoreIDLabel.Text = "(" + Order.StoreID.ToString () + ")"; // Placed open/close paretheses on the storeID

//			StateLabel.Text = Order.Customer.StateCode;
			StoreMgrFullNameLabel.Text = Order.StoreMgrFirstName + " " + Order.StoreMgrLastName;
			StoreMgrEmailLabel.Text = Order.StoreMgrEmail;
			StoreMgrPhoneLabel.Text = Order.StoreMgrPhone;

			if (Order.IsRegularOrder == null) {

				if (dateFormatter.ToString (NSDate.Now) == dateFormatter.ToString ((NSDate)Order.RequestedReleaseDate.ToLocalTime())) {
					this.StateLabel.Text = "Today";
					this.ReleaseDateLabel.Text = "";
				} else {
					this.StateLabel.Text = ""; // dateFormatter.ToString ((NSDate)Order.RequestedReleaseDate.ToLocalTime());
					this.ReleaseDateLabel.Text = "";
				}
			}
			else if ((bool)Order.IsRegularOrder) {
				this.ReleaseDateLabel.Text = "Release Date:";

				if (dateFormatter.ToString (NSDate.Now) == dateFormatter.ToString ((NSDate)Order.RequestedReleaseDate.ToLocalTime())) {
					this.StateLabel.Text = "Today";
				} else {
					this.StateLabel.Text = dateFormatter.ToString ((NSDate)Order.RequestedReleaseDate.ToLocalTime());
				}
			} else {
				this.ReleaseDateLabel.Text = "";
				this.StateLabel.Text = "Pre-sell";
			}

			var sigImageFilePath = Constants.GetImageFilePath (Order.SignatureFilename);
			UIImage signatureImage = UIImage.FromFile (sigImageFilePath);
			if (signatureImage != null)
				SignatureImageView.Image = signatureImage;

//			ShipToAddressLabel.Text = Order.Customer.getShipToAddress();
			// Fix for #60.  Remove Required Date and Payment Terms, not needed by Wrigley's
//			if (Order.RequiredDate > DateTime.MinValue)
//				RequiredDateLabel.Text = Order.RequiredDate.ToString ("ddd, dd-MMM-yyyy");
//			if (Order.PaymentTermsDescription != null)
//				TermsLabel.Text = Order.PaymentTermsDescription;
			if (Order.OrderLineList != null && Order.OrderLineList.Count > 0)
				this.TableView.Source = new OrderLinesInfoSource(Order.OrderLineList); 
			else
				this.TableView.Source = new OrderLinesInfoSource(dal.getOrderLineListByOrderPK (Order.PK));
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == "OrderLineInfoSegue") {
				var orderLineInfoController = segue.DestinationViewController as OrderLineInfoController;
				orderLineInfoController.OrderLineItem = SelectedOrderLine;
			}
		}

		partial void dismiss (Foundation.NSObject sender) {
			DismissViewController(true, null);
		}

		public class OrderLinesInfoSource : UITableViewSource {

			IList<OrderLine> _orderLineList;
			static readonly NSString CellIdentifier = new NSString("OrderLineInfoCell");
			DALFacade dal = new DALFacade ();

			public OrderLinesInfoSource (IList<OrderLine> orderLineList) {
				this._orderLineList = orderLineList;
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				if (_orderLineList == null)
					return 0;
				else
					return _orderLineList.Count;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				SelectedOrderLine = _orderLineList [indexPath.Row];
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{

				// OLD IDs for Products should be placed here
				var cell = (UITableViewCell)tableView.DequeueReusableCell (CellIdentifier, indexPath);
				var orderLine = _orderLineList [indexPath.Row];

				if (orderLine.ProductName != null) {
					// display product info from orderlines if it exists there
					cell.TextLabel.Text = orderLine.ProductName;
					cell.DetailTextLabel.Text = String.Format ("{1}", orderLine.ProductCode, orderLine.Quantity);
				} else if (orderLine.Product != null) {
					// use details from product if it orderlines does not containproduct info
					var product = orderLine.Product;
					cell.TextLabel.Text = product.Name;
					// pricing info not required by Wrigley's
					//				var amount = (orderLine.Product.UnitPrice * orderLine.Quantity);
					//				if (orderLine.DiscountRate > 0)
					//					amount *= (1 - orderLine.DiscountRate);
					cell.DetailTextLabel.Text = String.Format ("{1}", product.Code, orderLine.Quantity);
				} else if (orderLine.ProductID != 0) {
					// OLD ID PROCESS FOR THE PRODUCTS
					var product = dal.getProductListByOldID (orderLine.Product.ID);
					cell.TextLabel.Text = product[0].Name;
					cell.DetailTextLabel.Text = String.Format ("{1}", product[0].Code, orderLine.Quantity);

				}

				else {
					// this could happen if the re-synchronized product list is missing the original products
					cell.TextLabel.Text = "<product info not found>"; 
					cell.DetailTextLabel.Text = "";
				}
//				cell.DetailTextLabel.Text = "Qty: " + orderLine.Quantity.ToString() + ". Amt: $ " + amount.ToString();
				return cell;
			}

			public override string TitleForFooter (UITableView tableView, nint section)
			{
				// pricing info not required by Wrigley's
//				decimal total = 0m;
//				if (_orderLineList != null) {
//					foreach (var ol in _orderLineList) {
//						total += (ol.Quantity * ol.Product.UnitPrice * (1 - ol.DiscountRate));
//					}
//				} 
				return String.Format ("Line items: {0}", _orderLineList.Count);
//				return "Total: $ " + total.ToString();
			}

		}
	}
}

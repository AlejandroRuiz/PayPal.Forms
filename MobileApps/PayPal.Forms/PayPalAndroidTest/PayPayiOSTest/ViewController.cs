using System;

using UIKit;

namespace PayPayiOSTest
{
	public partial class ViewController : UIViewController
	{
		void FuturePayments_TouchUpInside(object sender, EventArgs e)
		{
			MainManager.FuturePayment();
		}

		PayPalManager MainManager;

		public ViewController (IntPtr handle) : base (handle)
		{
			MainManager = new PayPalManager (
				"YOUR APP ID STRING",
				"YOUR APP ID STRING"
			);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			btnTest.TouchUpInside += BtnTest_TouchUpInside;
			futurePayments.TouchUpInside += FuturePayments_TouchUpInside;
		}

		void BtnTest_TouchUpInside (object sender, EventArgs e)
		{
			MainManager.BuySomething ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


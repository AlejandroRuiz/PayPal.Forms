using System;

using UIKit;

namespace PayPayiOSTest
{
	public partial class ViewController : UIViewController
	{

		PayPalManager MainManager;

		public ViewController (IntPtr handle) : base (handle)
		{
			MainManager = new PayPalManager (
				"AZ1efDFvrgcY-PqxaoVnZDg53n2_AMBVN3_qf3Q3Hm_z5jEuk4NXd_Htv8fmis1vrM0uwidTmXIc5tS5",
				"ATdpUY5hE7rhrNmDnKjJLTD2NkyNRjEO7oq62DJdthmFjENBKKottH1AtXqr4Yatkcaj9GGdrJcZOYtL"
			);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			btnTest.TouchUpInside += BtnTest_TouchUpInside;
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


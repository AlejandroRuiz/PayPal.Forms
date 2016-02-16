using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using PayPal.Forms.Abstractions;

namespace PayPal.Forms.Test.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			CrossPaypalManager.Init (
				new PayPalConfiguration (
					PayPal.Forms.Abstractions.Enum.Environment.NoNetwork,
					"ATdpUY5hE7rhrNmDnKjJLTD2NkyNRjEO7oq62DJdthmFjENBKKottH1AtXqr4Yatkcaj9GGdrJcZOYtL"
				){
					AcceptCreditCards = true,
					MerchantName = "Test Store",
					MerchantPrivacyPolicyUri = "https://www.example.com/privacy",
					MerchantUserAgreementUri = "https://www.example.com/legal"
				}
			);

			LoadApplication (new App ());

			return base.FinishedLaunching (app, options);
		}
	}
}


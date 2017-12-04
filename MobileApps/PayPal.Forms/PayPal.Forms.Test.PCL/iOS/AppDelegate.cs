using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using PayPal.Forms.Abstractions;

namespace PayPal.Forms.Test.PCL.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();
            Forms.CrossPayPalManager.Init(
                new PayPalConfiguration(PayPalEnvironment.NoNetwork, "YOUR ID STRING")
                {
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


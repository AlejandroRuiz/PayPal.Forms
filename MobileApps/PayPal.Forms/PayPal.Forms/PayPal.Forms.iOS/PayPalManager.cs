using System;
using Xamarin.PayPal.iOS;
using Foundation;
using System.Diagnostics;
using System.Collections.Generic;
using UIKit;
using System.Linq;

namespace PayPal.Forms
{
	public class PayPalManager : PayPalPaymentDelegate
	{
		#region PayPalProfileSharingDelegate

		public void UserDidCancelPayPalProfileSharingViewController(PayPalProfileSharingViewController profileSharingViewController)
		{
			Debug.WriteLine("PayPal Profile Sharing Authorization Canceled");
			profileSharingViewController?.DismissViewController(true, null);
			OnCancelled?.Invoke();
			OnCancelled = null;
		}

		public void PayPalProfileSharingViewController(PayPalProfileSharingViewController profileSharingViewController, NSDictionary profileSharingAuthorization)
		{
			Debug.WriteLine("PayPal Profile Sharing Authorization Success!");
			profileSharingViewController.DismissViewController (true, () => {
				NSError err = null;
				NSData jsonData = NSJsonSerialization.Serialize(profileSharingAuthorization, NSJsonWritingOptions.PrettyPrinted, out err);
				NSString first = new NSString("");
				if(err == null){
					first = new NSString(jsonData, NSStringEncoding.UTF8);
				}else{
					Debug.WriteLine(err.LocalizedDescription);
				}
				OnSuccess?.Invoke (first.ToString());
				OnSuccess =  null;
			});
		}

		#endregion

		#region PayPalFuturePaymentDelegate

		public void PayPalFuturePaymentDidCancel(PayPalFuturePaymentViewController futurePaymentViewController)
		{
			Debug.WriteLine("PayPal Future Payment Authorization Canceled");
			futurePaymentViewController?.DismissViewController(true, null);
			OnCancelled?.Invoke ();
			OnCancelled = null;
		}

		void PayPalFuturePaymentViewController(PayPalFuturePaymentViewController futurePaymentViewController, NSDictionary futurePaymentAuthorization)
		{
			Debug.WriteLine("PayPal Future Payment Authorization Success!");
			futurePaymentViewController?.DismissViewController(true, () =>
			{
				NSError err = null;
				NSData jsonData = NSJsonSerialization.Serialize(futurePaymentAuthorization, NSJsonWritingOptions.PrettyPrinted, out err);
				NSString first = new NSString("");
				if(err == null){
					first = new NSString(jsonData, NSStringEncoding.UTF8);
				}else{
					Debug.WriteLine(err.LocalizedDescription);
				}

				OnSuccess?.Invoke (first.ToString());
				OnSuccess =  null;
			});
		}

		#endregion

		#region ProvideCreditCard

		public void UserDidCancelPaymentViewController(CardIOPaymentViewController paymentViewController)
		{
			paymentViewController.DismissViewController(true, null);
			RetrieveCardCancelled?.Invoke();
		}

		public void UserDidProvideCreditCardInfo(CardIOCreditCardInfo cardInfo, CardIOPaymentViewController paymentViewController)
		{
			paymentViewController.DismissViewController(true, null);
			RetrieveCardSuccess?.Invoke(cardInfo);
		}

		#endregion

		PayPalConfiguration _payPalConfig;

		bool _acceptCreditCards;
		public bool AcceptCreditCards {
			get {
				return _acceptCreditCards;
			}
			set {
				_acceptCreditCards = value;
				_payPalConfig.AcceptCreditCards = _acceptCreditCards;
			}
		}

		string _environment;
		public string Environment {
			get {
				return _environment;
			}
			set {
				if (value != _environment) {
					PayPalMobile.PreconnectWithEnvironment (value);
				}
				_environment = value;
			}
		}

		public PayPalManager(PayPal.Forms.Abstractions.PayPalConfiguration xfconfig)
		{
			NSString key = null;
			NSString value = new NSString (xfconfig.PayPalKey);
			string env = string.Empty;
			switch (xfconfig.Environment) {
			case PayPal.Forms.Abstractions.Enum.PayPalEnvironment.NoNetwork:
				key = Constants.PayPalEnvironmentNoNetwork;
				env = Constants.PayPalEnvironmentNoNetwork.ToString ();
				break;
				case PayPal.Forms.Abstractions.Enum.PayPalEnvironment.Production:
				key = Constants.PayPalEnvironmentProduction;
				env = Constants.PayPalEnvironmentProduction.ToString ();
				break;
				case PayPal.Forms.Abstractions.Enum.PayPalEnvironment.Sandbox:
				key = Constants.PayPalEnvironmentSandbox;
				env = Constants.PayPalEnvironmentSandbox.ToString ();
				break;
			}

			PayPalMobile.InitializeWithClientIdsForEnvironments (NSDictionary.FromObjectsAndKeys (
				new NSObject[] {
					value,
					value,
					value
				}, new NSObject[] {
					key,
					Constants.PayPalEnvironmentProduction,
					Constants.PayPalEnvironmentSandbox
				}
			));

			Environment = env;

			_payPalConfig = new PayPalConfiguration ();
			AcceptCreditCards = xfconfig.AcceptCreditCards;

			_payPalConfig.MerchantName = xfconfig.MerchantName;
			_payPalConfig.MerchantPrivacyPolicyURL = new NSUrl (xfconfig.MerchantPrivacyPolicyUri);
			_payPalConfig.MerchantUserAgreementURL = new NSUrl (xfconfig.MerchantUserAgreementUri);
			_payPalConfig.LanguageOrLocale = NSLocale.PreferredLanguages [0];
			_payPalConfig.PayPalShippingAddressOption = PayPalShippingAddressOption.Both;

			Debug.WriteLine ("PayPal iOS SDK Version: " + PayPalMobile.LibraryVersion);
		}

		Action OnCancelled;

		Action<string> OnSuccess;

		Action<string> OnError;

		public void BuyItems(
			PayPal.Forms.Abstractions.PayPalItem[] items,
			Decimal xfshipping,
			Decimal xftax,
			Action onCancelled,
			Action<string> onSuccess,
			Action<string> onError,
			PayPal.Forms.Abstractions.ShippingAddress address
		) {

			OnCancelled = onCancelled;
			OnSuccess = onSuccess;
			OnError = onError;

			List<PayPalItem> nativeItems = new List<PayPalItem> ();
			foreach (var product in items) {
				nativeItems.Add ( PayPalItem.ItemWithName (
					product.Name,
					(nuint)product.Quantity,
					new NSDecimalNumber(RoundNumber((double)product.Price)),
					product.Currency,
					product.SKU)
				);
			}

			var subtotal = PayPalItem.TotalPriceForItems (nativeItems.ToArray ());

			var shipping = new NSDecimalNumber(RoundNumber((double)xfshipping));
			var tax = new NSDecimalNumber (RoundNumber ((double)xftax));
			var paymentDetails = PayPalPaymentDetails.PaymentDetailsWithSubtotal (subtotal, shipping, tax);

			var total = subtotal.Add (shipping).Add (tax);

			var payment = PayPalPayment.PaymentWithAmount (total, nativeItems.FirstOrDefault().Currency, "Multiple items", PayPalPaymentIntent.Sale);
			payment.Items = nativeItems.ToArray ();
			payment.PaymentDetails = paymentDetails;

			if (address != null)
			{
				payment.ShippingAddress = PayPalShippingAddress.ShippingAddressWithRecipientName(
					address.RecipientName,
					address.Line1,
					address.Line2,
					address.City,
					address.State,
					address.PostalCode,
					address.CountryCode
				);
			}

			if (payment.Processable) {
				var paymentViewController = new PayPalPaymentViewController(payment, _payPalConfig, this);
				var top = GetTopViewController (UIApplication.SharedApplication.KeyWindow);
				top.PresentViewController (paymentViewController, true, null);
			}else {
				OnError?.Invoke ("This particular payment will always be processable. If, for example, the amount was negative or the shortDescription was empty, this payment wouldn't be processable, and you'd want to handle that here.");
				Debug.WriteLine("Payment not processalbe:"+payment.Items);
			}

		}

		string RoundNumber(double myNumber)
		{
			var s = string.Format("{0:0.00}", myNumber);

			if (s.EndsWith("00"))
			{
				return ((int)myNumber).ToString();
			}
			else
			{
				return s;
			}
		}

		public void BuyItem(
			PayPal.Forms.Abstractions.PayPalItem item,
			Decimal xftax,
			Action onCancelled,
			Action<string> onSuccess,
			Action<string> onError,
			PayPal.Forms.Abstractions.ShippingAddress address
		){

			OnCancelled = onCancelled;
			OnSuccess = onSuccess;
			OnError = onError;


			var subTotal = new NSDecimalNumber(RoundNumber((double)item.Price));
			NSDecimalNumber amount = subTotal.Add (new NSDecimalNumber (RoundNumber ((double)xftax)));

			var paymentDetails = PayPalPaymentDetails.PaymentDetailsWithSubtotal (subTotal, new NSDecimalNumber(0), new NSDecimalNumber(RoundNumber((double)xftax)));

			var payment = PayPalPayment.PaymentWithAmount (amount, item.Currency, item.Name, PayPalPaymentIntent.Sale);
			payment.PaymentDetails = paymentDetails;
			payment.Items = new NSObject[]{
				PayPalItem.ItemWithName (
					item.Name,
					1,
					new  NSDecimalNumber (RoundNumber((double)item.Price)),
					item.Currency,
					item.SKU
				)
			};

			if (address != null) {
				payment.ShippingAddress = PayPalShippingAddress.ShippingAddressWithRecipientName(
					address.RecipientName,
					address.Line1,
					address.Line2,
					address.City,
					address.State,
					address.PostalCode,
					address.CountryCode
				);
			}


			if (payment.Processable) {
				var paymentViewController = new PayPalPaymentViewController(payment, _payPalConfig, this);
				var top = GetTopViewController (UIApplication.SharedApplication.KeyWindow);
				top.PresentViewController (paymentViewController, true, null);
			}else {
				OnError?.Invoke ("This particular payment will always be processable. If, for example, the amount was negative or the shortDescription was empty, this payment wouldn't be processable, and you'd want to handle that here.");
				OnError = null;
				Debug.WriteLine("Payment not processalbe:"+payment.Items);
			}
		}

		public override void PayPalPaymentDidCancel (PayPalPaymentViewController paymentViewController)
		{
			Debug.WriteLine ("PayPal Payment Cancelled");
			paymentViewController?.DismissViewController(true, null);
			OnCancelled?.Invoke ();
			OnCancelled = null;
		}

		public override void PayPalPaymentViewController (PayPalPaymentViewController paymentViewController, PayPalPayment completedPayment)
		{
			Debug.WriteLine("PayPal Payment Success !");
			paymentViewController.DismissViewController (true, () => {
				Debug.WriteLine ("Here is your proof of payment:" + completedPayment.Confirmation + "Send this to your server for confirmation and fulfillment.");

				NSError err = null;
				NSData jsonData = NSJsonSerialization.Serialize(completedPayment.Confirmation, NSJsonWritingOptions.PrettyPrinted, out err);
				NSString first = new NSString("");
				if(err == null){
					first = new NSString(jsonData, NSStringEncoding.UTF8);
				}else{
					Debug.WriteLine(err.LocalizedDescription);
				}

				OnSuccess?.Invoke (first.ToString());
				OnSuccess =  null;
			});
		}

		public void FuturePayment(Action onCancelled, Action<string> onSuccess)
		{
			OnCancelled = onCancelled;
			OnSuccess = onSuccess;
			var futurePaymentViewController = new PayPalFuturePaymentViewController(_payPalConfig, new CustomPayPalFuturePaymentDelegate(this));
			var top = GetTopViewController (UIApplication.SharedApplication.KeyWindow);
			top.PresentViewController(futurePaymentViewController, true, null);
		}

		public void AuthorizeProfileSharing(Action onCancelled, Action<string> onSuccess)
		{
			OnCancelled = onCancelled;
			OnSuccess = onSuccess;
			var infoSet = new NSSet(
				Constants.kPayPalOAuth2ScopeOpenId.ToString(),
				Constants.kPayPalOAuth2ScopeEmail.ToString(),
				Constants.kPayPalOAuth2ScopeAddress.ToString(),
				Constants.kPayPalOAuth2ScopePhone.ToString()
			);
			var profileSharingViewController = new PayPalProfileSharingViewController(infoSet, _payPalConfig, new CustomAuthorizeProfileSharingDelegate(this));
			var top = GetTopViewController(UIApplication.SharedApplication.KeyWindow);
			top.PresentViewController(profileSharingViewController, true, null);
		}

		Action RetrieveCardCancelled;

		Action<CardIOCreditCardInfo> RetrieveCardSuccess;

		public void RequestCardData(Action onCancelled, Action<CardIOCreditCardInfo> onSuccess, PayPal.Forms.Abstractions.Enum.CardIOLogo scannerLogo)
		{
			RetrieveCardCancelled = onCancelled;
			RetrieveCardSuccess = onSuccess;
			var scanViewController = new CardIOPaymentViewController(new CustomCardIOPaymentViewControllerDelegate(this));

			switch (scannerLogo) {
				case Abstractions.Enum.CardIOLogo.CardIO:
					scanViewController.HideCardIOLogo = false;
					scanViewController.UseCardIOLogo = true;
				break;
				case Abstractions.Enum.CardIOLogo.None:
					scanViewController.HideCardIOLogo = true;
					scanViewController.UseCardIOLogo = false;
				break;
			}
			var top = GetTopViewController(UIApplication.SharedApplication.KeyWindow);
			top.PresentViewController(scanViewController, true, null);
		}

		UIViewController GetTopViewController(UIWindow window) {
			var vc = window.RootViewController;

			while (vc.PresentedViewController != null) {
				vc = vc.PresentedViewController;
			}

			return vc;
		}


		public class CustomCardIOPaymentViewControllerDelegate : CardIOPaymentViewControllerDelegate
		{
			PayPalManager PayPalManager;

			public CustomCardIOPaymentViewControllerDelegate(PayPalManager manager)
			{
				PayPalManager = manager;
			}

			public override void UserDidCancelPaymentViewController(CardIOPaymentViewController paymentViewController)
			{
				PayPalManager.UserDidCancelPaymentViewController(paymentViewController);
			}

			public override void UserDidProvideCreditCardInfo(CardIOCreditCardInfo cardInfo, CardIOPaymentViewController paymentViewController)
			{
				PayPalManager.UserDidProvideCreditCardInfo(cardInfo, paymentViewController);
			}
		}

		public class CustomPayPalFuturePaymentDelegate : PayPalFuturePaymentDelegate
		{
			PayPalManager PayPalManager;

			public CustomPayPalFuturePaymentDelegate(PayPalManager manager)
			{
				PayPalManager = manager;	
			}

			public override void PayPalFuturePaymentDidCancel(Xamarin.PayPal.iOS.PayPalFuturePaymentViewController futurePaymentViewController)
			{
				PayPalManager.PayPalFuturePaymentDidCancel(futurePaymentViewController);
			}

			public override void PayPalFuturePaymentViewController(Xamarin.PayPal.iOS.PayPalFuturePaymentViewController futurePaymentViewController, NSDictionary futurePaymentAuthorization)
			{
				PayPalManager.PayPalFuturePaymentViewController(futurePaymentViewController, futurePaymentAuthorization);
			}
		}

		public class CustomAuthorizeProfileSharingDelegate : PayPalProfileSharingDelegate
		{
			PayPalManager PayPalManager;

			public CustomAuthorizeProfileSharingDelegate(PayPalManager manager)
			{
				PayPalManager = manager;
			}

			public override void UserDidCancelPayPalProfileSharingViewController(Xamarin.PayPal.iOS.PayPalProfileSharingViewController profileSharingViewController)
			{
				PayPalManager.UserDidCancelPayPalProfileSharingViewController(profileSharingViewController);
			}

			public override void PayPalProfileSharingViewController(Xamarin.PayPal.iOS.PayPalProfileSharingViewController profileSharingViewController, NSDictionary profileSharingAuthorization)
			{
				PayPalManager.PayPalProfileSharingViewController(profileSharingViewController, profileSharingAuthorization);
			}
		}
	}
}


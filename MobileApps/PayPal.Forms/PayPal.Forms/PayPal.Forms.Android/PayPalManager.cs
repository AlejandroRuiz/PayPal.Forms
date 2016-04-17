using System;
using Android.Content;
using Xamarin.PayPal.Android;
using Java.Math;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Widget;
using Org.Json;

namespace PayPal.Forms
{
	public class PayPalManager
	{
		Context Context;

		private static string CONFIG_ENVIRONMENT;

		private static string CONFIG_CLIENT_ID = "credential from developer.paypal.com";

		public static int REQUEST_CODE_PAYMENT = 1;
		public static int REQUEST_CODE_FUTURE_PAYMENT = 2;
		public static int REQUEST_CODE_PROFILE_SHARING = 3;

		private PayPalConfiguration config;

		public PayPalManager (Context context, PayPal.Forms.Abstractions.PayPalConfiguration xfconfig)
		{
			Context = context;

			switch (xfconfig.Environment) {
			case PayPal.Forms.Abstractions.Enum.PayPalEnvironment.NoNetwork:
				CONFIG_ENVIRONMENT = PayPalConfiguration.EnvironmentNoNetwork;
				break;
			case PayPal.Forms.Abstractions.Enum.PayPalEnvironment.Production:
				CONFIG_ENVIRONMENT = PayPalConfiguration.EnvironmentProduction;
				break;
			case PayPal.Forms.Abstractions.Enum.PayPalEnvironment.Sandbox:
				CONFIG_ENVIRONMENT = PayPalConfiguration.EnvironmentSandbox;
				break;
			}

			CONFIG_CLIENT_ID = xfconfig.PayPalKey;

			config = new PayPalConfiguration()
				.Environment(CONFIG_ENVIRONMENT)
				.ClientId(CONFIG_CLIENT_ID)
				.AcceptCreditCards(xfconfig.AcceptCreditCards)
				.MerchantName(xfconfig.MerchantName)
				.MerchantPrivacyPolicyUri(global::Android.Net.Uri.Parse(xfconfig.MerchantPrivacyPolicyUri))
				.MerchantUserAgreementUri(global::Android.Net.Uri.Parse(xfconfig.MerchantUserAgreementUri));

			Intent intent = new Intent (Context, typeof(PayPalService));
			intent.PutExtra (PayPalService.ExtraPaypalConfiguration, config);
			Context.StartService (intent);
		}

		private PayPalPayment getThingToBuy(string paymentIntent) {
			return new PayPalPayment (new BigDecimal ("1.75"), "USD", "sample item",
				paymentIntent);
		}

		private void addAppProvidedShippingAddress(PayPalPayment paypalPayment) {
			ShippingAddress shippingAddress =
				new ShippingAddress ().RecipientName ("Mom Parker").Line1 ("52 North Main St.")
					.City ("Austin").State ("TX").PostalCode ("78729").CountryCode ("US");
			paypalPayment.InvokeProvidedShippingAddress (shippingAddress);
		}

		private PayPalOAuthScopes getOauthScopes() {
			HashSet<string> scopes = new HashSet<string> ();
			scopes.Add (PayPalOAuthScopes.PaypalScopeOpenid);
			scopes.Add (PayPalOAuthScopes.PaypalScopeEmail);
			scopes.Add (PayPalOAuthScopes.PaypalScopeAddress);
			scopes.Add (PayPalOAuthScopes.PaypalScopePhone);
			return new PayPalOAuthScopes (scopes.ToList ());
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
				nativeItems.Add (new PayPalItem (
					product.Name,
					new Java.Lang.Integer ((int)product.Quantity),
					new BigDecimal (product.Price.ToString ()),
					product.Currency,
					product.SKU)
				);
			}

			BigDecimal subtotal = PayPalItem.GetItemTotal (nativeItems.ToArray ());
			BigDecimal shipping = new BigDecimal (xfshipping.ToString ());
			BigDecimal tax = new BigDecimal (xftax.ToString ());
			PayPalPaymentDetails paymentDetails = new PayPalPaymentDetails (shipping, subtotal, tax);
			BigDecimal amount = subtotal.Add (shipping).Add (tax);

			PayPalPayment payment = new PayPalPayment (amount, nativeItems.FirstOrDefault().Currency, "Multiple items", PayPalPayment.PaymentIntentSale);
			payment = payment.Items (nativeItems.ToArray ()).PaymentDetails (paymentDetails);

			if (address != null)
			{
				ShippingAddress shippingAddress = new ShippingAddress()
					.RecipientName(address.RecipientName)
					.Line1(address.Line1)
					.Line2(address.Line2)
					.City(address.City)
					.State(address.State)
					.PostalCode(address.PostalCode)
					.CountryCode(address.CountryCode);
				payment = payment.InvokeProvidedShippingAddress(shippingAddress);
			}

			payment = payment.EnablePayPalShippingAddressesRetrieval(true);

			Intent intent = new Intent (Context, typeof(PaymentActivity));

			intent.PutExtra (PayPalService.ExtraPaypalConfiguration, config);

			intent.PutExtra (PaymentActivity.ExtraPayment, payment);

			(Context as Activity).StartActivityForResult (intent, REQUEST_CODE_PAYMENT);
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
			BigDecimal amount = new BigDecimal (item.Price.ToString ()).Add (new BigDecimal (xftax.ToString ()));

			PayPalPayment payment = new PayPalPayment (amount, item.Currency, item.Name, PayPalPayment.PaymentIntentSale);

			if (address != null)
			{
				ShippingAddress shippingAddress = new ShippingAddress()
					.RecipientName(address.RecipientName)
					.Line1(address.Line1)
					.Line2(address.Line2)
					.City(address.City)
					.State(address.State)
					.PostalCode(address.PostalCode)
					.CountryCode(address.CountryCode);
				payment = payment.InvokeProvidedShippingAddress(shippingAddress);
			}

			payment = payment.EnablePayPalShippingAddressesRetrieval(true);

			Intent intent = new Intent (Context, typeof(PaymentActivity));

			intent.PutExtra (PayPalService.ExtraPaypalConfiguration, config);

			intent.PutExtra (PaymentActivity.ExtraPayment, payment);

			(Context as Activity).StartActivityForResult (intent, REQUEST_CODE_PAYMENT);
		}

		public string GetClientMetadataId() {
			string metadataId = PayPalConfiguration.GetClientMetadataId(Context);
			return metadataId;
		}

		public void FuturePayment(Action onCancelled, Action<string> onSuccess, Action<string> onError) {

			OnCancelled = onCancelled;
			OnSuccess = onSuccess;
			OnError = onError;

			Intent intent = new Intent (Context, typeof(PayPalFuturePaymentActivity));

			intent.PutExtra(PayPalService.ExtraPaypalConfiguration, config);

			(Context as Activity).StartActivityForResult(intent, REQUEST_CODE_FUTURE_PAYMENT);
		}

		public void AuthorizeProfileSharing(Action onCancelled, Action<string> onSuccess, Action<string> onError) {

			OnCancelled = onCancelled;
			OnSuccess = onSuccess;
			OnError = onError;

			Intent intent = new Intent (Context, typeof(PayPalProfileSharingActivity));

			intent.PutExtra(PayPalService.ExtraPaypalConfiguration, config);

			intent.PutExtra (PayPalProfileSharingActivity.ExtraRequestedScopes, getOauthScopes ());

			(Context as Activity).StartActivityForResult (intent, REQUEST_CODE_PROFILE_SHARING);
		}

		public void Destroy()
		{
			Context.StopService (new Intent (Context, typeof(PayPalService)));
		}

		public void OnActivityResult(int requestCode, Result resultCode, global::Android.Content.Intent data)
		{
			if (requestCode == PayPalManager.REQUEST_CODE_PAYMENT) {
				if (resultCode == Result.Ok) {
					PaymentConfirmation confirm =
						(PaymentConfirmation)data.GetParcelableExtra (PaymentActivity.ExtraResultConfirmation);
					if (confirm != null) {
						try {
							System.Diagnostics.Debug.WriteLine (confirm.ToJSONObject ().ToString (4));

							OnSuccess?.Invoke (confirm.ToJSONObject ().ToString ());
							OnSuccess = null;

						} catch (JSONException e) {
							OnError?.Invoke ("an extremely unlikely failure occurred: " + e.Message);
							OnError = null;
							System.Diagnostics.Debug.WriteLine ("an extremely unlikely failure occurred: " + e.Message);
						}
					}
					OnError?.Invoke ("Unknown Error");
					OnError = null;
				} else if (resultCode == Result.Canceled) {
					OnCancelled?.Invoke ();
					OnCancelled = null;
					System.Diagnostics.Debug.WriteLine ("The user canceled.");
				} else if ((int)resultCode == PaymentActivity.ResultExtrasInvalid) {
					OnError?.Invoke ("An invalid Payment or PayPalConfiguration was submitted. Please see the docs.");
					OnError = null;
					System.Diagnostics.Debug.WriteLine (
						"An invalid Payment or PayPalConfiguration was submitted. Please see the docs.");
				}
			}else if (requestCode == REQUEST_CODE_FUTURE_PAYMENT) {
				if (resultCode == Result.Ok) {
					PayPalAuthorization auth =
						(PayPalAuthorization)data.GetParcelableExtra (PayPalFuturePaymentActivity.ExtraResultAuthorization);
					if (auth != null) {
						try {
							System.Diagnostics.Debug.WriteLine (auth.ToJSONObject ().ToString (4));
							OnSuccess?.Invoke (auth.ToJSONObject ().ToString ());
							OnSuccess = null;
						} catch (JSONException e) {
							System.Diagnostics.Debug.WriteLine ("an extremely unlikely failure occurred: " + e.Message);
						}
					}
					OnError?.Invoke ("Unknown Error");
					OnError = null;
				} else if (resultCode == Result.Canceled) {
					OnCancelled?.Invoke ();
					OnCancelled = null;
					System.Diagnostics.Debug.WriteLine ("The user canceled.");
				} else if ((int)resultCode == PayPalFuturePaymentActivity.ResultExtrasInvalid) {
					OnError?.Invoke ("Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
					OnError = null;
					System.Diagnostics.Debug.WriteLine (
						"Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
				} 
			} else if (requestCode == REQUEST_CODE_PROFILE_SHARING) {
				if (resultCode == Result.Ok) {
					PayPalAuthorization auth =
						(PayPalAuthorization)data.GetParcelableExtra (PayPalProfileSharingActivity.ExtraResultAuthorization);
					if (auth != null) {
						try {
							System.Diagnostics.Debug.WriteLine (auth.ToJSONObject ().ToString (4));
							OnSuccess?.Invoke (auth.ToJSONObject ().ToString ());
							OnSuccess = null;
						} catch (JSONException e) {
							System.Diagnostics.Debug.WriteLine ("an extremely unlikely failure occurred: " + e.Message);
						}
					}
					OnError?.Invoke ("Unknown Error");
					OnError = null;
				} else if (resultCode == Result.Canceled) {
					OnCancelled?.Invoke ();
					OnCancelled = null;
					System.Diagnostics.Debug.WriteLine ("The user canceled.");
				} else if ((int)resultCode == PayPalFuturePaymentActivity.ResultExtrasInvalid) {
					OnError?.Invoke ("Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
					OnError = null;
					System.Diagnostics.Debug.WriteLine(
						"Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
				}
			}
		}
	}
}


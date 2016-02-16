using System;
using Android.Content;
using Xamarin.PayPal.Android;
using Java.Math;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Widget;
using Org.Json;

namespace PayPal.Forms.Android
{
	public class PayPalManager
	{
		Context Context;

		/**
     * - Set to PayPalConfiguration.ENVIRONMENT_PRODUCTION to move real money.
     * 
     * - Set to PayPalConfiguration.ENVIRONMENT_SANDBOX to use your test credentials
     * from https://developer.paypal.com
     * 
     * - Set to PayPalConfiguration.ENVIRONMENT_NO_NETWORK to kick the tires
     * without communicating to PayPal's servers.
     */
		private static string CONFIG_ENVIRONMENT;

		// note that these credentials will differ between live & sandbox environments.
		private static string CONFIG_CLIENT_ID = "credential from developer.paypal.com";

		public static int REQUEST_CODE_PAYMENT = 1;
		public static int REQUEST_CODE_FUTURE_PAYMENT = 2;
		public static int REQUEST_CODE_PROFILE_SHARING = 3;

		private PayPalConfiguration config;

		public PayPalManager (Context context, PayPal.Forms.Abstractions.PayPalConfiguration xfconfig)
		{
			Context = context;

			switch (xfconfig.Environment) {
			case PayPal.Forms.Abstractions.Enum.Environment.NoNetwork:
				CONFIG_ENVIRONMENT = PayPalConfiguration.EnvironmentNoNetwork;
				break;
			case PayPal.Forms.Abstractions.Enum.Environment.Production:
				CONFIG_ENVIRONMENT = PayPalConfiguration.EnvironmentProduction;
				break;
			case PayPal.Forms.Abstractions.Enum.Environment.Sandbox:
				CONFIG_ENVIRONMENT = PayPalConfiguration.EnvironmentSandbox;
				break;
			}

			CONFIG_CLIENT_ID = xfconfig.PayPalKey;

			config = new PayPalConfiguration ()
				.Environment (CONFIG_ENVIRONMENT)
				.ClientId (CONFIG_CLIENT_ID)
				.AcceptCreditCards (xfconfig.AcceptCreditCards)
			// The following are only used in PayPalFuturePaymentActivity.
				.MerchantName (xfconfig.MerchantName)
				.MerchantPrivacyPolicyUri (global::Android.Net.Uri.Parse (xfconfig.MerchantPrivacyPolicyUri))
				.MerchantUserAgreementUri (global::Android.Net.Uri.Parse (xfconfig.MerchantUserAgreementUri));

			Intent intent = new Intent (Context, typeof(PayPalService));
			intent.PutExtra (PayPalService.ExtraPaypalConfiguration, config);
			Context.StartService (intent);
		}

		private PayPalPayment getThingToBuy(string paymentIntent) {
			return new PayPalPayment (new BigDecimal ("1.75"), "USD", "sample item",
				paymentIntent);
		}

		/*
		* Add app-provided shipping address to payment
		*/
		private void addAppProvidedShippingAddress(PayPalPayment paypalPayment) {
			ShippingAddress shippingAddress =
				new ShippingAddress ().RecipientName ("Mom Parker").Line1 ("52 North Main St.")
					.City ("Austin").State ("TX").PostalCode ("78729").CountryCode ("US");
			paypalPayment.InvokeProvidedShippingAddress (shippingAddress);
		}

		/*
     	* Enable retrieval of shipping addresses from buyer's PayPal account
     	*/
		private void enableShippingAddressRetrieval(PayPalPayment paypalPayment, bool enable) {
			paypalPayment.EnablePayPalShippingAddressesRetrieval (enable);
		}

		private PayPalOAuthScopes getOauthScopes() {
			/* create the set of required scopes
         * Note: see https://developer.paypal.com/docs/integration/direct/identity/attributes/ for mapping between the
         * attributes you select for this app in the PayPal developer portal and the scopes required here.
         */
			HashSet<string> scopes = new HashSet<string> ();
			scopes.Add (PayPalOAuthScopes.PaypalScopeEmail);
			scopes.Add (PayPalOAuthScopes.PaypalScopeAddress);
			return new PayPalOAuthScopes (scopes.ToList ());
		}

		private void sendAuthorizationToServer(PayPalAuthorization authorization) {

			/**
         * TODO: Send the authorization response to your server, where it can
         * exchange the authorization code for OAuth access and refresh tokens.
         * 
         * Your server must then store these tokens, so that your server code
         * can execute payments for this user in the future.
         * 
         * A more complete example that includes the required app-server to
         * PayPal-server integration is available from
         * https://github.com/paypal/rest-api-sdk-python/tree/master/samples/mobile_backend
         */

		}

		Action OnCancelled;

		Action<string> OnSuccess;

		Action<string> OnError;

		public void BuyItems(
			PayPal.Forms.Abstractions.PayPalItem[] items,
			Deveel.Math.BigDecimal xfshipping,
			Deveel.Math.BigDecimal xftax,
			Action onCancelled,
			Action<string> onSuccess,
			Action<string> onError
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

			Intent intent = new Intent (Context, typeof(PaymentActivity));

			intent.PutExtra (PayPalService.ExtraPaypalConfiguration, config);

			intent.PutExtra (PaymentActivity.ExtraPayment, payment);

			(Context as Activity).StartActivityForResult (intent, REQUEST_CODE_PAYMENT);
		}

		public void BuyItem(
			PayPal.Forms.Abstractions.PayPalItem item,
			Deveel.Math.BigDecimal xftax,
			Action onCancelled,
			Action<string> onSuccess,
			Action<string> onError
		){

			OnCancelled = onCancelled;
			OnSuccess = onSuccess;
			OnError = onError;
			BigDecimal amount = new BigDecimal (item.Price.ToString ()).Add (new BigDecimal (xftax.ToString ()));

			PayPalPayment payment = new PayPalPayment (amount, item.Currency, item.Name, PayPalPayment.PaymentIntentSale);

			Intent intent = new Intent (Context, typeof(PaymentActivity));

			intent.PutExtra (PayPalService.ExtraPaypalConfiguration, config);

			intent.PutExtra (PaymentActivity.ExtraPayment, payment);

			(Context as Activity).StartActivityForResult (intent, REQUEST_CODE_PAYMENT);
		}

		public void FuturePaymentPurchase() {
			// Get the Client Metadata ID from the SDK
			String metadataId = PayPalConfiguration.GetClientMetadataId(Context);

			System.Diagnostics.Debug.WriteLine ("Client Metadata ID: " + metadataId);

			// TODO: Send metadataId and transaction details to your server for processing with
			// PayPal...
			Toast.MakeText (
				Context.ApplicationContext, "Client Metadata Id received from SDK", ToastLength.Long)
				.Show ();
		}

		public void FuturePayment() {
			Intent intent = new Intent (Context, typeof(PayPalFuturePaymentActivity));

			// send the same configuration for restart resiliency
			intent.PutExtra(PayPalService.ExtraPaypalConfiguration, config);

			(Context as Activity).StartActivityForResult(intent, REQUEST_CODE_FUTURE_PAYMENT);
		}

		public void ProfileSharing() {
			Intent intent = new Intent (Context, typeof(PayPalProfileSharingActivity));

			// send the same configuration for restart resiliency
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
							System.Diagnostics.Debug.WriteLine (confirm.ToJSONObject ().ToString(4));

							OnSuccess?.Invoke(confirm.ToJSONObject ().ToString());
							OnSuccess = null;

						} catch (JSONException e) {
							System.Diagnostics.Debug.WriteLine ("an extremely unlikely failure occurred: " + e.Message);
						}
					}
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
							System.Diagnostics.Debug.WriteLine(auth.ToJSONObject().ToString(4));

							String authorization_code = auth.AuthorizationCode;
							System.Diagnostics.Debug.WriteLine(authorization_code);

							sendAuthorizationToServer(auth);
							Toast.MakeText(
								Context.ApplicationContext,
								"Future Payment code received from PayPal", ToastLength.Long)
								.Show();

						} catch (JSONException e) {
							System.Diagnostics.Debug.WriteLine ("an extremely unlikely failure occurred: " + e.Message);
						}
					}
				} else if (resultCode == Result.Ok) {
					System.Diagnostics.Debug.WriteLine ("The user canceled.");
				} else if ((int)resultCode == PayPalFuturePaymentActivity.ResultExtrasInvalid) {
					System.Diagnostics.Debug.WriteLine (
						"Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
				} 
			} else if (requestCode == REQUEST_CODE_PROFILE_SHARING) {
				if (resultCode == Result.Ok) {
					PayPalAuthorization auth =
						(PayPalAuthorization)data.GetParcelableExtra (PayPalProfileSharingActivity.ExtraResultAuthorization);
					if (auth != null) {
						try {
							System.Diagnostics.Debug.WriteLine(auth.ToJSONObject().ToString(4));

							String authorization_code = auth.AuthorizationCode;
							System.Diagnostics.Debug.WriteLine(authorization_code);

							sendAuthorizationToServer(auth);
							Toast.MakeText(
								Context.ApplicationContext,
								"Profile Sharing code received from PayPal", ToastLength.Short)
								.Show();

						} catch (JSONException e) {
							System.Diagnostics.Debug.WriteLine ("an extremely unlikely failure occurred: " + e.Message);
						}
					}
				} else if (resultCode == Result.Canceled) {
					System.Diagnostics.Debug.WriteLine ("The user canceled.");
				} else if ((int)resultCode == PayPalFuturePaymentActivity.ResultExtrasInvalid) {
					System.Diagnostics.Debug.WriteLine(
						"Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
				}
			}
		}
	}
}


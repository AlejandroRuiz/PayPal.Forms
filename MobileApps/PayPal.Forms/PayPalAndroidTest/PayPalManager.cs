using System;
using Xamarin.PayPal.Android;
using Android.Content;
using Java.Math;
using Android.App;
using System.Collections.Generic;
using System.Linq;
using Android.Widget;
using Org.Json;

namespace PayPalAndroidTest
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
		private static String CONFIG_ENVIRONMENT = PayPalConfiguration.EnvironmentNoNetwork;

		// note that these credentials will differ between live & sandbox environments.
		private static String CONFIG_CLIENT_ID = "credential from developer.paypal.com";

		public static int REQUEST_CODE_PAYMENT = 1;
		public static int REQUEST_CODE_FUTURE_PAYMENT = 2;
		public static int REQUEST_CODE_PROFILE_SHARING = 3;

		private PayPalConfiguration config;

		public PayPalManager (Context context)
		{
			Context = context;
			config = new PayPalConfiguration ()
				.Environment (CONFIG_ENVIRONMENT)
				.ClientId (CONFIG_CLIENT_ID)
			// The following are only used in PayPalFuturePaymentActivity.
				.MerchantName ("Example Merchant")
				.MerchantPrivacyPolicyUri (Android.Net.Uri.Parse ("https://www.example.com/privacy"))
				.MerchantUserAgreementUri (Android.Net.Uri.Parse ("https://www.example.com/legal"));

			Intent intent = new Intent (Context, typeof(PayPalService));
			intent.PutExtra (PayPalService.ExtraPaypalConfiguration, config);
			Context.StartService (intent);
		}

		private PayPalPayment getThingToBuy(string paymentIntent) {
			return new PayPalPayment (new BigDecimal ("1.75"), "USD", "sample item",
				paymentIntent);
		}

		/* 
    	 * This method shows use of optional payment details and item list.
     	*/
		private PayPalPayment getStuffToBuy(string paymentIntent) {
			//--- include an item list, payment amount details
			PayPalItem[] items =
			{
				new PayPalItem("sample item #1", new Java.Lang.Integer(2), new BigDecimal("87.50"), "USD",
					"sku-12345678"),
				new PayPalItem("free sample item #2", new Java.Lang.Integer(1), new BigDecimal("0.00"),
					"USD", "sku-zero-price"),
				new PayPalItem("sample item #3 with a longer name", new Java.Lang.Integer(6), new BigDecimal("37.99"),
					"USD", "sku-33333") 
			};
			BigDecimal subtotal = PayPalItem.GetItemTotal(items);
			BigDecimal shipping = new BigDecimal("7.21");
			BigDecimal tax = new BigDecimal("4.67");
			PayPalPaymentDetails paymentDetails = new PayPalPaymentDetails(shipping, subtotal, tax);
			BigDecimal amount = subtotal.Add(shipping).Add(tax);
			PayPalPayment payment = new PayPalPayment(amount, "USD", "sample item", paymentIntent);
			payment.Items(items).PaymentDetails(paymentDetails);

			//--- set other optional fields like invoice_number, custom field, and soft_descriptor
			payment.Custom("This is text that will be associated with the payment that the app can use.");

			return payment;
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

		public void BuySomething() {
			/* 
         * PAYMENT_INTENT_SALE will cause the payment to complete immediately.
         * Change PAYMENT_INTENT_SALE to 
         *   - PAYMENT_INTENT_AUTHORIZE to only authorize payment and capture funds later.
         *   - PAYMENT_INTENT_ORDER to create a payment for authorization and capture
         *     later via calls from your server.
         * 
         * Also, to include additional payment details and an item list, see getStuffToBuy() below.
         */
			PayPalPayment thingToBuy = getThingToBuy (PayPalPayment.PaymentIntentSale);

			/*
         * See getStuffToBuy(..) for examples of some available payment options.
         */

			Intent intent = new Intent (Context, typeof(PaymentActivity));

			// send the same configuration for restart resiliency
			intent.PutExtra (PayPalService.ExtraPaypalConfiguration, config);

			intent.PutExtra (PaymentActivity.ExtraPayment, thingToBuy);

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

		public void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
		{
			if (requestCode == PayPalManager.REQUEST_CODE_PAYMENT) {
				if (resultCode == Result.Ok) {
					PaymentConfirmation confirm =
						(PaymentConfirmation)data.GetParcelableExtra (PaymentActivity.ExtraResultConfirmation);
					if (confirm != null) {
						try {
							System.Diagnostics.Debug.WriteLine (confirm.ToJSONObject ().ToString(4));
							System.Diagnostics.Debug.WriteLine(confirm.Payment .ToJSONObject ().ToString (4));
							/**
                         	*  TODO: send 'confirm' (and possibly confirm.getPayment() to your server for verification
                         	* or consent completion.
                         	* See https://developer.paypal.com/webapps/developer/docs/integration/mobile/verify-mobile-payment/
                         	* for more details.
                         	*
                         	* For sample mobile backend interactions, see
                         	* https://github.com/paypal/rest-api-sdk-python/tree/master/samples/mobile_backend
                         	*/
							Toast.MakeText (
								Context.ApplicationContext,
								"PaymentConfirmation info received from PayPal", ToastLength.Short)
								.Show ();

						} catch (JSONException e) {
							System.Diagnostics.Debug.WriteLine ("an extremely unlikely failure occurred: " + e.Message);
						}
					}
				} else if (resultCode == Result.Canceled) {
					System.Diagnostics.Debug.WriteLine ("The user canceled.");
				} else if ((int)resultCode == PaymentActivity.ResultExtrasInvalid) {
					System.Diagnostics.Debug.WriteLine (
						"An invalid Payment or PayPalConfiguration was submitted. Please see the docs.");
				}
			}else if (requestCode == REQUEST_CODE_FUTURE_PAYMENT) {
				if (resultCode == Result.Ok) {
					PayPalAuthorization auth = (Xamarin.PayPal.Android.PayPalAuthorization)data.GetParcelableExtra(PayPalFuturePaymentActivity.ExtraResultAuthorization);
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
					PayPalAuthorization auth = (Xamarin.PayPal.Android.PayPalAuthorization)data.GetParcelableExtra(PayPalProfileSharingActivity.ExtraResultAuthorization);
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


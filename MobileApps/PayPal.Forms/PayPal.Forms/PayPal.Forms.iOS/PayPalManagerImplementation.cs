using System;
using PayPal.Forms.Abstractions;
using System.Threading.Tasks;
using PayPal.Forms.Abstractions.Enum;

namespace PayPal.Forms
{
	public class PayPalManagerImplementation:IPayPalManager
	{
		TaskCompletionSource<PaymentResult> buyTcs;

		TaskCompletionSource<FuturePaymentsResult> rfpTcs;

		TaskCompletionSource<ProfileSharingResult> apsTcs;

		#region IPayPalManager implementation

		public Task<PaymentResult> Buy (PayPalItem[] items, Decimal shipping, Decimal tax, ShippingAddress address = null)
		{
			if (buyTcs != null) {
				buyTcs.TrySetCanceled ();
				buyTcs.TrySetResult (null);
			}
			buyTcs = new TaskCompletionSource<PaymentResult> ();
			Manager.BuyItems (items, shipping, tax, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError, address);
			return buyTcs.Task;
		}

		public Task<PaymentResult> Buy (PayPalItem item, Decimal tax, ShippingAddress address = null)
		{
			if (buyTcs != null) {
				buyTcs.TrySetCanceled ();
				buyTcs.TrySetResult (null);
			}
			buyTcs = new TaskCompletionSource<PaymentResult> ();
			Manager.BuyItem (item, tax, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError, address);
			return buyTcs.Task;
		}

		public Task<FuturePaymentsResult> RequestFuturePayments()
		{
			if (rfpTcs != null) {
				rfpTcs.TrySetCanceled ();
				rfpTcs.TrySetResult (null);
			}
			rfpTcs = new TaskCompletionSource<FuturePaymentsResult> ();
			Manager.FuturePayment(SendOnPayPalPaymentDidCancel, SendOnPayPalFuturePaymentsCompleted);
			return rfpTcs.Task;
		}

		public string ClientMetadataId {
			get {
				return Xamarin.PayPal.iOS.PayPalMobile.ClientMetadataID;
			}
		}

		public Task<ProfileSharingResult> AuthorizeProfileSharing()
		{
			if (apsTcs != null)
			{
				apsTcs.TrySetCanceled();
				apsTcs.TrySetResult(null);
			}
			apsTcs = new TaskCompletionSource<ProfileSharingResult>();
			Manager.AuthorizeProfileSharing(SendOnAuthorizeProfileSharingDidCancel, SendAuthorizeProfileSharingCompleted);
			return apsTcs.Task;
		}

		#endregion

		internal void SendOnAuthorizeProfileSharingDidCancel()
		{
			if (apsTcs != null)
			{
				apsTcs.TrySetResult(new ProfileSharingResult (PayPalStatus.Cancelled));
			}
		}

		internal void SendAuthorizeProfileSharingCompleted(string confirmationJSON)
		{
			if (apsTcs != null)
			{
				var serverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPal.Forms.Abstractions.ProfileSharingResult.PayPalProfileSharingResponse> (confirmationJSON);
				apsTcs.TrySetResult (new ProfileSharingResult (PayPalStatus.Successful, string.Empty, serverResponse));
			}
		}

		internal void SendOnPayPalPaymentDidCancel()
		{
			if (buyTcs != null) {
				buyTcs.TrySetResult (new PaymentResult (PayPalStatus.Cancelled));
			}
			if (rfpTcs != null) {
				rfpTcs.TrySetResult(new FuturePaymentsResult(PayPalStatus.Cancelled));
			}
		}

		internal void SendOnPayPalPaymentCompleted(string confirmationJSON)
		{
			if (buyTcs != null) {
				var serverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPal.Forms.Abstractions.PaymentResult.PayPalPaymentResponse> (confirmationJSON);
				buyTcs.TrySetResult (new PaymentResult (PayPalStatus.Successful, string.Empty, serverResponse));
			}
		}

		internal void SendOnPayPalFuturePaymentsCompleted(string confirmationJSON)
		{
			if (rfpTcs != null) {
				var serverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPal.Forms.Abstractions.FuturePaymentsResult.PayPalFuturePaymentsResponse> (confirmationJSON);
				rfpTcs.TrySetResult (new FuturePaymentsResult (PayPalStatus.Successful, string.Empty, serverResponse));
			}
		}

		internal void SendOnPayPalPaymentError(string errorMessage)
		{
			if (buyTcs != null) {
				buyTcs.TrySetResult (new PaymentResult (PayPalStatus.Cancelled, errorMessage));
			}
		}

		public static PayPalManager Manager { get; private set; }

		public PayPalManagerImplementation (PayPalConfiguration config)
		{
			Manager = new PayPalManager (config);
		}
	}
}


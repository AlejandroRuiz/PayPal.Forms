using System;
using PayPal.Forms.Abstractions;
using System.Threading.Tasks;
using PayPal.Forms.Android;
using PayPal.Forms.Abstractions.Enum;

namespace PayPal.Forms
{
	public class PayPalManagerImplementation : IPayPalManager
	{
		TaskCompletionSource<PaymentResult> buyTcs;

		TaskCompletionSource<FuturePaymentsResult> rfpTcs;

		#region IPayPalManager implementation

		public Task<PaymentResult> Buy(PayPalItem[] items, Deveel.Math.BigDecimal shipping, Deveel.Math.BigDecimal tax)
		{
			if (buyTcs != null)
			{
				buyTcs.TrySetCanceled();
				buyTcs.TrySetResult(null);
			}
			buyTcs = new TaskCompletionSource<PaymentResult>();
			Manager.BuyItems(items, shipping, tax, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError);
			return buyTcs.Task;
		}

		public Task<PaymentResult> Buy(PayPalItem item, Deveel.Math.BigDecimal tax)
		{
			if (buyTcs != null)
			{
				buyTcs.TrySetCanceled();
				buyTcs.TrySetResult(null);
			}
			buyTcs = new TaskCompletionSource<PaymentResult>();
			Manager.BuyItem(item, tax, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError);
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
				return Manager.GetClientMetadataId();
			}
		}

		#endregion

		internal void SendOnPayPalPaymentDidCancel()
		{
			if (buyTcs != null)
			{
				buyTcs.TrySetResult(new PaymentResult(PaymentResultStatus.Cancelled));
			}
			if (rfpTcs != null) {
				rfpTcs.TrySetResult(new FuturePaymentsResult(PaymentResultStatus.Cancelled));
			}
		}

		internal void SendOnPayPalPaymentCompleted(string confirmationJSON)
		{
			if (buyTcs != null)
			{
				var serverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPal.Forms.Abstractions.PaymentResult.PayPalPaymentResponse>(confirmationJSON);
				buyTcs.TrySetResult(new PaymentResult(PaymentResultStatus.Successful, string.Empty, serverResponse));
			}
		}

		internal void SendOnPayPalFuturePaymentsCompleted(string confirmationJSON)
		{
			if (rfpTcs != null) {
				var serverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPal.Forms.Abstractions.FuturePaymentsResult.PayPalFuturePaymentsResponse> (confirmationJSON);
				rfpTcs.TrySetResult (new FuturePaymentsResult (PaymentResultStatus.Successful, string.Empty, serverResponse));
			}
		}

		internal void SendOnPayPalPaymentError(string errorMessage)
		{
			if (buyTcs != null)
			{
				buyTcs.TrySetResult(new PaymentResult(PaymentResultStatus.Cancelled, errorMessage));
			}
		}

		public static PayPalManager Manager { get; private set; }

		public PayPalManagerImplementation(PayPalConfiguration config)
		{
			Manager = new PayPalManager(Xamarin.Forms.Forms.Context, config);
		}
	}
}


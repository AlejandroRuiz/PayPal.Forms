using System;
using PayPal.Forms.Abstractions;
using Deveel.Math;
using System.Threading.Tasks;
using PayPal.Forms.iOS;
using PayPal.Forms.Abstractions.Enum;

namespace PayPal.Forms
{
	public class PayPalManagerImplementation:IPayPalManager
	{
		TaskCompletionSource<PaymentResult> buyTcs;

		#region IPayPalManager implementation

		public Task<PaymentResult> Buy (PayPalItem[] items, Deveel.Math.BigDecimal shipping, Deveel.Math.BigDecimal tax)
		{
			if (buyTcs != null) {
				buyTcs.SetCanceled ();
				buyTcs.SetResult (null);
			}
			buyTcs = new TaskCompletionSource<PaymentResult> ();
			Manager.BuyItems (items, shipping, tax, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError);
			return buyTcs.Task;
		}

		public Task<PaymentResult> Buy (PayPalItem item, Deveel.Math.BigDecimal tax)
		{
			if (buyTcs != null) {
				buyTcs.SetCanceled ();
				buyTcs.SetResult (null);
			}
			buyTcs = new TaskCompletionSource<PaymentResult> ();
			Manager.BuyItem (item, tax, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError);
			return buyTcs.Task;
		}

		#endregion

		internal void SendOnPayPalPaymentDidCancel()
		{
			if (buyTcs != null) {
				buyTcs.TrySetResult (new PaymentResult (PaymentResultStatus.Cancelled));
			}
		}

		internal void SendOnPayPalPaymentCompleted(string confirmationJSON)
		{
			if (buyTcs != null) {
				var serverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPal.Forms.Abstractions.PaymentResult.PayPalResponse> (confirmationJSON);
				buyTcs.TrySetResult (new PaymentResult (PaymentResultStatus.Successful, string.Empty, serverResponse));
			}
		}

		internal void SendOnPayPalPaymentError(string errorMessage)
		{
			if (buyTcs != null) {
				buyTcs.TrySetResult (new PaymentResult (PaymentResultStatus.Cancelled, errorMessage));
			}
		}

		public static PayPalManager Manager { get; private set; }

		public PayPalManagerImplementation (PayPalConfiguration config)
		{
			Manager = new PayPalManager (config);
		}
	}
}


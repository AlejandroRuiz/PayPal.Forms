using System;

namespace PayPal.Forms.Abstractions
{
	public class OnPayPalPaymentErrorEventArgs:EventArgs
	{
		public string ErrorMessage { get; private set; }

		public OnPayPalPaymentErrorEventArgs (string errorMessage)
		{
			ErrorMessage = errorMessage;
		}
	}
}


using System;

namespace PayPal.Forms.Abstractions
{
    public class OnPayPalPaymentErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; }

        public OnPayPalPaymentErrorEventArgs(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }
    }
}
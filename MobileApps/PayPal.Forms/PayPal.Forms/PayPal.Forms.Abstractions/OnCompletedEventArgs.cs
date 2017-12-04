using System;

namespace PayPal.Forms.Abstractions
{
    public class OnCompletedEventArgs : EventArgs
    {
        public string ConfirmationJSON { get; }

        public OnCompletedEventArgs(string confirmationJSON)
        {
            this.ConfirmationJSON = confirmationJSON;
        }
    }
}
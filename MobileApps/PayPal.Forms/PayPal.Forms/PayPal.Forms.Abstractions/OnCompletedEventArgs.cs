using System;

namespace PayPal.Forms.Abstractions
{
	public class OnCompletedEventArgs:EventArgs
	{
		public string ConfirmationJSON { get; private set; }

		public OnCompletedEventArgs(string confirmationJSON)
		{
			ConfirmationJSON = confirmationJSON;
		}
	}
}


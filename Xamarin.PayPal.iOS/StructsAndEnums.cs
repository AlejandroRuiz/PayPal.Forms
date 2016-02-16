using System;
using ObjCRuntime;

namespace Xamarin.PayPal.iOS
{
	[Native]
	public enum PayPalShippingAddressOption : long
	{
		None = 0,
		Provided = 1,
		PayPal = 2,
		Both = 3
	}

	[Native]
	public enum PayPalPaymentIntent : long
	{
		Sale = 0,
		Authorize = 1,
		Order = 2
	}

	[Native]
	public enum PayPalPaymentViewControllerState : long
	{
		Unsent = 0,
		InProgress = 1
	}

	[Native]
	public enum CardIOCreditCardType : long
	{
		Unrecognized = 0,
		Ambiguous = 1,
		Amex = 51,
		Jcb = 74,
		Visa = 52,
		Mastercard = 53,
		Discover = 54
	}

	[Native]
	public enum CardIODetectionMode : long
	{
		CardImageAndNumber = 0,
		CardImageOnly,
		Automatic
	}
}




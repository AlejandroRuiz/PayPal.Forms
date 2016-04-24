using System;
using PayPal.Forms.Abstractions.Enum;

namespace PayPal.Forms.Abstractions
{
	public class ScanCardResult
	{
		public PayPalStatus Status { get; private set; }

		public Card Card { get; private set; }

		public ScanCardResult(PayPalStatus status, Card card = null)
		{
			Status = status;
			Card = card;
		}
	}
}


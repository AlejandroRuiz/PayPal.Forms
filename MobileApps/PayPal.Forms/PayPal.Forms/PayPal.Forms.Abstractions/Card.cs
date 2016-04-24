using System;
using Xamarin.Forms;

namespace PayPal.Forms.Abstractions
{
	public sealed class Card
	{
		public bool Scaned { get; private set; }

		public string RedactedCardNumber { get; private set; }

		public string PostalCode { get; private set; }

		public int ExpiryYear { get; private set; }

		public int ExpiryMonth { get; private set; }

		public string Cvv { get; private set; }

		public CreditCardType CardType { get; private set; }

		public string CardNumber { get; private set; }

		public ImageSource CardImage { get; private set; }

		public Card(bool scanned, string redactedCardNumber, string postalCode, int expiryYear, int expiryMonth, string cvv, CreditCardType cardType, string cardNumber, ImageSource cardImage)
		{
			this.Scaned = scanned;
			this.RedactedCardNumber = redactedCardNumber;
			this.PostalCode = postalCode;
			this.ExpiryYear = expiryYear;
			this.ExpiryMonth = expiryMonth;
			this.Cvv = cvv;
			this.CardType = cardType;
			this.CardNumber = cardNumber;
			this.CardImage = cardImage;
		}
	}
}


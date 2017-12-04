using System;
using Xamarin.Forms;

namespace PayPal.Forms.Abstractions
{
    public sealed class Card
    {
        public bool Scaned { get; }

        public string RedactedCardNumber { get; }

        public string PostalCode { get; }

        public int ExpiryYear { get; }

        public int ExpiryMonth { get; }

        public string Cvv { get; }

        public CreditCardType CardType { get; }

        public string CardNumber { get; }

        public ImageSource CardImage { get; }

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
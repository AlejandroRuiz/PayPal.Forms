using System;
namespace PayPal.Forms.Abstractions
{
    public class ShippingAddress
    {
        public string RecipientName { get; }

        public string Line1 { get; }

        public string Line2 { get; }

        public string City { get; }

        public string State { get; }

        public string PostalCode { get; }

        public string CountryCode { get; }

        public ShippingAddress(string recipientName, string line1, string line2, string city, string state, string postalCode, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(recipientName))
            {
                throw new ArgumentNullException(nameof(recipientName));
            }
            if (string.IsNullOrWhiteSpace(line1))
            {
                throw new ArgumentNullException(nameof(line1));
            }
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentNullException(nameof(city));
            }
            if (string.IsNullOrWhiteSpace(state))
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (string.IsNullOrWhiteSpace(postalCode))
            {
                throw new ArgumentNullException(nameof(postalCode));
            }
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                throw new ArgumentNullException(nameof(countryCode));
            }
            this.RecipientName = recipientName;
            this.Line1 = line1;
            this.Line2 = line2;
            this.City = city;
            this.State = state;
            this.PostalCode = postalCode;
            this.CountryCode = countryCode;
        }
    }
}
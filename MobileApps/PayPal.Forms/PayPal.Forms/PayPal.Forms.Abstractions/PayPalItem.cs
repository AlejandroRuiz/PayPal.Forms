using System;

namespace PayPal.Forms.Abstractions
{
    public class PayPalItem
    {
        public string Name { get; }

        public uint Quantity { get; }

        public Decimal Price { get; }

        public string Currency { get; }

        public string SKU { get; }

        public PayPalItem(string name, uint quantity, Decimal price, string currency, string sku)
        {
            this.Name = name;
            this.Quantity = quantity;
            this.Price = price;
            this.Currency = currency;
            this.SKU = sku;
        }

        public PayPalItem(string name, Decimal price, string currency)
        {
            this.Name = name;
            this.Price = price;
            this.Currency = currency;
        }
    }
}
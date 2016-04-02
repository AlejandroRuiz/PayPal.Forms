using System;

namespace PayPal.Forms.Abstractions
{
	public class PayPalItem
	{
		public string Name { get; private set; }

		public uint Quantity { get; private set; } 

		public Decimal Price { get; private set; }

		public string Currency { get; private set; }

		public string SKU { get; private set; }

		public PayPalItem (string name, uint quantity, Decimal price, string currency, string sku)
		{
			Name = name;
			Quantity = quantity;
			Price = price;
			Currency = currency;
			SKU = sku;
		}

		public PayPalItem (string name, Decimal price, string currency)
		{
			Name = name;
			Price = price;
			Currency = currency;
		}
	}
}


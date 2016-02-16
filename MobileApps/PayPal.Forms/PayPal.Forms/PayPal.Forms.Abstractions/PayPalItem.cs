using System;
using Deveel.Math;

namespace PayPal.Forms.Abstractions
{
	public class PayPalItem
	{
		public string Name { get; private set; }

		public uint Quantity { get; private set; } 

		public BigDecimal Price { get; private set; }

		public string Currency { get; private set; }

		public string SKU { get; private set; }

		public PayPalItem (string name, uint quantity, BigDecimal price, string currency, string sku)
		{
			Name = name;
			Quantity = quantity;
			Price = price;
			Currency = currency;
			SKU = sku;
		}

		public PayPalItem (string name, BigDecimal price, string currency)
		{
			Name = name;
			//Quantity = quantity;
			Price = price;
			Currency = currency;
			//SKU = sku;
		}
	}
}


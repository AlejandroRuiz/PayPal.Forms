using System;
using System.Threading.Tasks;

namespace PayPal.Forms.Abstractions
{
	public interface IPayPalManager
	{
		Task<PaymentResult> Buy (PayPalItem[] items, Decimal shipping, Decimal tax);

		Task<PaymentResult> Buy (PayPalItem item, Decimal tax);

		Task<FuturePaymentsResult> RequestFuturePayments();

		string ClientMetadataId { get; }
	}
}


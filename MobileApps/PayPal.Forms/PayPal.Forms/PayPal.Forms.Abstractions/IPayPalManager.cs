using System;
using System.Threading.Tasks;
using Deveel.Math;

namespace PayPal.Forms.Abstractions
{
	public interface IPayPalManager
	{
		Task<PaymentResult> Buy (PayPalItem[] items, BigDecimal shipping, BigDecimal tax);

		Task<PaymentResult> Buy (PayPalItem item, BigDecimal tax);

		Task<FuturePaymentsResult> RequestFuturePayments();

		string ClientMetadataId { get; }
	}
}


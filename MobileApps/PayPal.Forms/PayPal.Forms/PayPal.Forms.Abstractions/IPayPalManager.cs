using System;
using System.Threading.Tasks;

namespace PayPal.Forms.Abstractions
{
	public interface IPayPalManager
	{
		Task<PaymentResult> Buy (PayPalItem[] items, Decimal shipping, Decimal tax, ShippingAddress address = null);

		Task<PaymentResult> Buy(PayPalItem item, Decimal tax, ShippingAddress address = null);

		Task<FuturePaymentsResult> RequestFuturePayments();

		Task<ProfileSharingResult> AuthorizeProfileSharing();

		string ClientMetadataId { get; }
	}
}


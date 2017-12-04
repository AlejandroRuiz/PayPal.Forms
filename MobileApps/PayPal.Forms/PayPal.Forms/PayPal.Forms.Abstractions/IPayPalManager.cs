using System;
using System.Threading.Tasks;

namespace PayPal.Forms.Abstractions
{
    public interface IPayPalManager
    {
        Task<PaymentResult> Buy(PayPalItem[] items, Decimal shipping, Decimal tax, ShippingAddress address = null, PaymentIntent intent = PaymentIntent.Sale);

        Task<PaymentResult> Buy(PayPalItem item, Decimal tax, ShippingAddress address = null, PaymentIntent intent = PaymentIntent.Sale);

        Task<FuturePaymentsResult> RequestFuturePayments();

        Task<ProfileSharingResult> AuthorizeProfileSharing();

        Task<ScanCardResult> ScanCard(CardIOLogo scannerLogo = CardIOLogo.PayPal);

        string ClientMetadataId { get; }

        void SetConfig(PayPalConfiguration newConfig);

        void ClearUserData();
    }
}
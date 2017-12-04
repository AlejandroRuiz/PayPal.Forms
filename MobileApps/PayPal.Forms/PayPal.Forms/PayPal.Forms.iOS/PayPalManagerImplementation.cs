using System;
using PayPal.Forms.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PayPal.Forms
{
    public class PayPalManagerImplementation : IPayPalManager
    {
        TaskCompletionSource<PaymentResult> _buyTcs;
        TaskCompletionSource<FuturePaymentsResult> _rfpTcs;
        TaskCompletionSource<ProfileSharingResult> _apsTcs;
        TaskCompletionSource<ScanCardResult> _gcardTcs;

        public static PayPalManager Manager { get; private set; }

        public PayPalManagerImplementation(PayPalConfiguration config)
        {
            Manager = new PayPalManager(config);
        }

        #region IPayPalManager implementation

        public Task<PaymentResult> Buy(PayPalItem[] items, Decimal shipping, Decimal tax, ShippingAddress address = null, PaymentIntent intent = PaymentIntent.Sale)
        {
            if (_buyTcs != null)
            {
                _buyTcs.TrySetCanceled();
                _buyTcs.TrySetResult(null);
            }
            _buyTcs = new TaskCompletionSource<PaymentResult>();
            Manager.BuyItems(items, shipping, tax, intent, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError, address);
            return _buyTcs.Task;
        }

        public Task<PaymentResult> Buy(PayPalItem item, Decimal tax, ShippingAddress address = null, PaymentIntent intent = PaymentIntent.Sale)
        {
            if (_buyTcs != null)
            {
                _buyTcs.TrySetCanceled();
                _buyTcs.TrySetResult(null);
            }
            _buyTcs = new TaskCompletionSource<PaymentResult>();
            Manager.BuyItem(item, tax, intent, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError, address);
            return _buyTcs.Task;
        }

        public Task<FuturePaymentsResult> RequestFuturePayments()
        {
            if (_rfpTcs != null)
            {
                _rfpTcs.TrySetCanceled();
                _rfpTcs.TrySetResult(null);
            }
            _rfpTcs = new TaskCompletionSource<FuturePaymentsResult>();
            Manager.FuturePayment(SendOnPayPalPaymentDidCancel, SendOnPayPalFuturePaymentsCompleted);
            return _rfpTcs.Task;
        }

        public string ClientMetadataId
        {
            get
            {
                return Xamarin.PayPal.iOS.PayPalMobile.ClientMetadataID;
            }
        }

        public Task<ProfileSharingResult> AuthorizeProfileSharing()
        {
            if (_apsTcs != null)
            {
                _apsTcs.TrySetCanceled();
                _apsTcs.TrySetResult(null);
            }
            _apsTcs = new TaskCompletionSource<ProfileSharingResult>();
            Manager.AuthorizeProfileSharing(SendOnAuthorizeProfileSharingDidCancel, SendAuthorizeProfileSharingCompleted);
            return _apsTcs.Task;
        }

        public Task<ScanCardResult> ScanCard(CardIOLogo scannerLogo = CardIOLogo.PayPal)
        {
            if (_gcardTcs != null)
            {
                _gcardTcs.TrySetCanceled();
                _gcardTcs.TrySetResult(null);
            }
            _gcardTcs = new TaskCompletionSource<ScanCardResult>();
            Manager.RequestCardData(SendScanCardDidCancel, SendScanCardCompleted, scannerLogo);
            return _gcardTcs.Task;
        }

        public void SetConfig(PayPalConfiguration newConfig) => Manager.UpdateConfig(newConfig);

        public void ClearUserData() => Manager.ClearUserData();

        #endregion

        internal void SendOnAuthorizeProfileSharingDidCancel()
        {
            if (_apsTcs != null)
            {
                _apsTcs.TrySetResult(new ProfileSharingResult(PayPalStatus.Cancelled));
            }
        }

        internal void SendAuthorizeProfileSharingCompleted(string confirmationJSON)
        {
            if (_apsTcs != null)
            {
                var serverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPal.Forms.Abstractions.ProfileSharingResult.PayPalProfileSharingResponse>(confirmationJSON);
                _apsTcs.TrySetResult(new ProfileSharingResult(PayPalStatus.Successful, string.Empty, serverResponse));
            }
        }

        internal void SendOnPayPalPaymentDidCancel()
        {
            if (_buyTcs != null)
            {
                _buyTcs.TrySetResult(new PaymentResult(PayPalStatus.Cancelled));
            }
            if (_rfpTcs != null)
            {
                _rfpTcs.TrySetResult(new FuturePaymentsResult(PayPalStatus.Cancelled));
            }
        }

        internal void SendOnPayPalPaymentCompleted(string confirmationJSON)
        {
            if (_buyTcs != null)
            {
                var serverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPal.Forms.Abstractions.PaymentResult.PayPalPaymentResponse>(confirmationJSON);
                _buyTcs.TrySetResult(new PaymentResult(PayPalStatus.Successful, string.Empty, serverResponse));
            }
        }

        internal void SendOnPayPalFuturePaymentsCompleted(string confirmationJSON)
        {
            if (_rfpTcs != null)
            {
                var serverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPal.Forms.Abstractions.FuturePaymentsResult.PayPalFuturePaymentsResponse>(confirmationJSON);
                _rfpTcs.TrySetResult(new FuturePaymentsResult(PayPalStatus.Successful, string.Empty, serverResponse));
            }
        }

        internal void SendOnPayPalPaymentError(string errorMessage)
        {
            if (_buyTcs != null)
            {
                _buyTcs.TrySetResult(new PaymentResult(PayPalStatus.Cancelled, errorMessage));
            }
        }

        internal void SendScanCardDidCancel()
        {
            _gcardTcs.SetResult(new ScanCardResult(PayPalStatus.Cancelled));
        }

        internal void SendScanCardCompleted(Xamarin.PayPal.iOS.CardIOCreditCardInfo cardInfo)
        {
            var card = new Card(
                cardInfo.Scanned,
                cardInfo.RedactedCardNumber,
                cardInfo.PostalCode,
                (int)cardInfo.ExpiryYear,
                (int)cardInfo.ExpiryMonth,
                cardInfo.Cvv,
                ((CreditCardType)(uint)cardInfo.CardType),
                cardInfo.CardNumber,
                (cardInfo.CardImage == null) ? null : ImageSource.FromStream(() => cardInfo.CardImage.AsPNG().AsStream())
            );
            _gcardTcs.SetResult(new ScanCardResult(PayPalStatus.Successful, card));
        }
    }
}
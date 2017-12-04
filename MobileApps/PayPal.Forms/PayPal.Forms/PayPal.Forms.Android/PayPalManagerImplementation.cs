using System;
using PayPal.Forms.Abstractions;
using System.Threading.Tasks;
using PayPal.Forms.Android;
using System.IO;
using Android.Graphics;
using Xamarin.Forms;
using Android.Content;
using Newtonsoft.Json;

namespace PayPal.Forms
{
    public class PayPalManagerImplementation : IPayPalManager
    {
        public static PayPalManager Manager { get; private set; }

        TaskCompletionSource<PaymentResult> buyTcs;
        TaskCompletionSource<FuturePaymentsResult> rfpTcs;
        TaskCompletionSource<ProfileSharingResult> apsTcs;
        TaskCompletionSource<ScanCardResult> gcardTcs;

        public PayPalManagerImplementation(PayPalConfiguration config, Context context)
        {
            Manager = new PayPalManager(config, Context = context);
        }

        public Context Context { get; }

        #region IPayPalManager implementation

        public Task<PaymentResult> Buy(PayPalItem[] items, Decimal shipping, Decimal tax, ShippingAddress address = null, PaymentIntent intent = PaymentIntent.Sale)
        {
            if (buyTcs != null)
            {
                buyTcs.TrySetCanceled();
                buyTcs.TrySetResult(null);
            }
            buyTcs = new TaskCompletionSource<PaymentResult>();
            Manager.BuyItems(items, shipping, tax, intent, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError, address);
            return buyTcs.Task;
        }

        public Task<PaymentResult> Buy(PayPalItem item, Decimal tax, ShippingAddress address = null, PaymentIntent intent = PaymentIntent.Sale)
        {
            if (buyTcs != null)
            {
                buyTcs.TrySetCanceled();
                buyTcs.TrySetResult(null);
            }
            buyTcs = new TaskCompletionSource<PaymentResult>();
            Manager.BuyItem(item, tax, intent, SendOnPayPalPaymentDidCancel, SendOnPayPalPaymentCompleted, SendOnPayPalPaymentError, address);
            return buyTcs.Task;
        }

        public Task<FuturePaymentsResult> RequestFuturePayments()
        {
            if (rfpTcs != null)
            {
                rfpTcs.TrySetCanceled();
                rfpTcs.TrySetResult(null);
            }
            rfpTcs = new TaskCompletionSource<FuturePaymentsResult>();
            Manager.FuturePayment(SendOnPayPalPaymentDidCancel, SendOnPayPalFuturePaymentsCompleted, SendOnPayPalPaymentError);
            return rfpTcs.Task;
        }

        public string ClientMetadataId => Manager.GetClientMetadataId();

        public Task<ProfileSharingResult> AuthorizeProfileSharing()
        {
            if (apsTcs != null)
            {
                apsTcs.TrySetCanceled();
                apsTcs.TrySetResult(null);
            }
            apsTcs = new TaskCompletionSource<ProfileSharingResult>();
            Manager.AuthorizeProfileSharing(SendOnAuthorizeProfileSharingDidCancel, SendAuthorizeProfileSharingCompleted, SendOnPayPalPaymentError);
            return apsTcs.Task;
        }

        public Task<ScanCardResult> ScanCard(CardIOLogo scannerLogo = CardIOLogo.PayPal)
        {
            if (gcardTcs != null)
            {
                gcardTcs.TrySetCanceled();
                gcardTcs.TrySetResult(null);
            }
            gcardTcs = new TaskCompletionSource<ScanCardResult>();
            Manager.RequestCardData(SendScanCardDidCancel, SendScanCardCompleted, scannerLogo);
            return gcardTcs.Task;
        }

        public void SetConfig(PayPalConfiguration newConfig) => Manager.UpdateConfig(newConfig);

        public void ClearUserData() => Manager.ClearUserData();

        #endregion

        internal void SendOnAuthorizeProfileSharingDidCancel() => apsTcs?.TrySetResult(new ProfileSharingResult(PayPalStatus.Cancelled));

        internal void SendAuthorizeProfileSharingCompleted(string confirmationJSON)
        {
            if (apsTcs != null)
            {
                var serverResponse = JsonConvert.DeserializeObject<ProfileSharingResult.PayPalProfileSharingResponse>(confirmationJSON);
                apsTcs.TrySetResult(new ProfileSharingResult(PayPalStatus.Successful, string.Empty, serverResponse));
            }
        }

        internal void SendOnPayPalPaymentDidCancel()
        {
            if (buyTcs != null)
            {
                buyTcs.TrySetResult(new PaymentResult(PayPalStatus.Cancelled));
            }
            if (rfpTcs != null)
            {
                rfpTcs.TrySetResult(new FuturePaymentsResult(PayPalStatus.Cancelled));
            }
        }

        internal void SendOnPayPalPaymentCompleted(string confirmationJSON)
        {
            if (buyTcs != null)
            {
                var serverResponse = JsonConvert.DeserializeObject<PaymentResult.PayPalPaymentResponse>(confirmationJSON);
                buyTcs.TrySetResult(new PaymentResult(PayPalStatus.Successful, string.Empty, serverResponse));
            }
        }

        internal void SendOnPayPalPaymentError(string errorMessage)
        {
            if (buyTcs != null)
            {
                buyTcs.TrySetResult(new PaymentResult(PayPalStatus.Error, errorMessage));
            }
            if (rfpTcs != null)
            {
                rfpTcs.TrySetResult(new FuturePaymentsResult(PayPalStatus.Error, errorMessage));
            }
            if (apsTcs != null)
            {
                apsTcs.TrySetResult(new ProfileSharingResult(PayPalStatus.Error, errorMessage));
            }
        }

        internal void SendOnPayPalFuturePaymentsCompleted(string confirmationJSON)
        {
            if (rfpTcs != null)
            {
                var serverResponse = JsonConvert.DeserializeObject<FuturePaymentsResult.PayPalFuturePaymentsResponse>(confirmationJSON);
                rfpTcs.TrySetResult(new FuturePaymentsResult(PayPalStatus.Successful, string.Empty, serverResponse));
            }
        }

        internal void SendScanCardDidCancel() => gcardTcs?.SetResult(new ScanCardResult(PayPalStatus.Cancelled));

        internal void SendScanCardCompleted(Xamarin.PayPal.Android.CardIO.Payment.CreditCard cardInfo, Bitmap image)
        {
            var card = new Card(
                (image != null),//cardInfo.scanned,
                cardInfo.RedactedCardNumber,
                cardInfo.PostalCode,
                (int)cardInfo.ExpiryYear,
                (int)cardInfo.ExpiryMonth,
                cardInfo.Cvv,
                (CreditCardType)Enum.Parse(typeof(CreditCardType), cardInfo.CardType.Name, true),
                cardInfo.CardNumber,
                (image == null) ? null : ImageSource.FromStream(() =>
                {
                    MemoryStream ms = new MemoryStream();
                    image.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                    ms.Seek(0L, SeekOrigin.Begin);
                    return ms;
                })
            );
            gcardTcs.SetResult(new ScanCardResult(PayPalStatus.Successful, card));
        }
    }
}
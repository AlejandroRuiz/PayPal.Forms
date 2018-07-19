using System;
using Android.Content;
using Xamarin.PayPal.Android;
using Java.Math;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Widget;
using Org.Json;
using Xamarin.PayPal.Android.CardIO.Payment;
using Android.Graphics;
using System.Globalization;
using PayPalForms = PayPal.Forms.Abstractions;

namespace PayPal.Forms
{
    public class PayPalManager
    {
        public const int REQUEST_CODE_PAYMENT = 1;
        public const int REQUEST_CODE_FUTURE_PAYMENT = 2;
        public const int REQUEST_CODE_PROFILE_SHARING = 3;
        public const int REQUEST_CODE_CARD_SCAN = 4;

        string _configEnvironment;
        string _configClientId = "credential from developer.paypal.com";
        PayPalConfiguration _nativeConfig;
        PayPalForms.PayPalConfiguration _formsConfig;
        Action _onCancelled;
        Action<string> _onSuccess;
        Action<string> _onError;
        Action _retrieveCardCancelled;
        Action<Xamarin.PayPal.Android.CardIO.Payment.CreditCard, Bitmap> _retrieveCardSuccess;

        public PayPalManager(PayPalForms.PayPalConfiguration xfconfig, Context context)
        {
            Context = context;
            UpdateConfig(xfconfig);
        }

        public void BuyItems(PayPalForms.PayPalItem[] items, Decimal xfshipping, Decimal xftax, PayPalForms.PaymentIntent xfintent, Action onCancelled, Action<string> onSuccess, Action<string> onError, PayPalForms.ShippingAddress address)
        {
            _onCancelled = onCancelled;
            _onSuccess = onSuccess;
            _onError = onError;

            List<PayPalItem> nativeItems = new List<PayPalItem>();
            foreach (var product in items)
            {
                nativeItems.Add(new PayPalItem(
                    product.Name,
                    new Java.Lang.Integer((int)product.Quantity),
                    new BigDecimal(RoundNumber((double)product.Price)),
                    product.Currency,
                    product.SKU)
                );
            }
            BigDecimal subtotal = PayPalItem.GetItemTotal(nativeItems.ToArray());
            BigDecimal shipping = new BigDecimal(RoundNumber((double)xfshipping));
            BigDecimal tax = new BigDecimal(RoundNumber((double)xftax));
            PayPalPaymentDetails paymentDetails = new PayPalPaymentDetails(shipping, subtotal, tax);
            BigDecimal amount = subtotal.Add(shipping).Add(tax);
            string paymentIntent;
            switch (xfintent)
            {
                case PayPalForms.PaymentIntent.Authorize:
                    paymentIntent = PayPalPayment.PaymentIntentAuthorize;
                    break;
                case PayPalForms.PaymentIntent.Order:
                    paymentIntent = PayPalPayment.PaymentIntentOrder;
                    break;
                default:
                    paymentIntent = PayPalPayment.PaymentIntentSale;
                    break;
            }
            PayPalPayment payment = new PayPalPayment(amount, nativeItems.FirstOrDefault().Currency, "Multiple items", paymentIntent);
            payment = payment.Items(nativeItems.ToArray()).PaymentDetails(paymentDetails);
            if (address != null)
            {
                ShippingAddress shippingAddress = new ShippingAddress()
                    .RecipientName(address.RecipientName)
                    .Line1(address.Line1)
                    .Line2(address.Line2)
                    .City(address.City)
                    .State(address.State)
                    .PostalCode(address.PostalCode)
                    .CountryCode(address.CountryCode);
                payment = payment.InvokeProvidedShippingAddress(shippingAddress);
            }
            switch (_formsConfig.ShippingAddressOption)
            {
                case Abstractions.ShippingAddressOption.Both:
                case Abstractions.ShippingAddressOption.PayPal:
                    payment = payment.EnablePayPalShippingAddressesRetrieval(true);
                    break;
                default:
                    payment = payment.EnablePayPalShippingAddressesRetrieval(false);
                    break;
            }
            Intent intent = new Intent(Context, typeof(PaymentActivity));
            intent.PutExtra(PayPalService.ExtraPaypalConfiguration, _nativeConfig);
            intent.PutExtra(PaymentActivity.ExtraPayment, payment);
            (Context as Activity).StartActivityForResult(intent, REQUEST_CODE_PAYMENT);
        }

        public void BuyItem(PayPalForms.PayPalItem item, Decimal xftax, PayPalForms.PaymentIntent xfintent, Action onCancelled, Action<string> onSuccess, Action<string> onError, PayPalForms.ShippingAddress address)
        {
            _onCancelled = onCancelled;
            _onSuccess = onSuccess;
            _onError = onError;

            BigDecimal amount = new BigDecimal(RoundNumber((double)item.Price)).Add(new BigDecimal(RoundNumber((double)xftax)));
            string paymentIntent;
            switch (xfintent)
            {
                case PayPalForms.PaymentIntent.Authorize:
                    paymentIntent = PayPalPayment.PaymentIntentAuthorize;
                    break;

                case PayPalForms.PaymentIntent.Order:
                    paymentIntent = PayPalPayment.PaymentIntentOrder;
                    break;

                default:
                case PayPalForms.PaymentIntent.Sale:
                    paymentIntent = PayPalPayment.PaymentIntentSale;
                    break;
            }
            PayPalPayment payment = new PayPalPayment(amount, item.Currency, item.Name, paymentIntent);
            if (address != null)
            {
                ShippingAddress shippingAddress = new ShippingAddress()
                    .RecipientName(address.RecipientName)
                    .Line1(address.Line1)
                    .Line2(address.Line2)
                    .City(address.City)
                    .State(address.State)
                    .PostalCode(address.PostalCode)
                    .CountryCode(address.CountryCode);
                payment = payment.InvokeProvidedShippingAddress(shippingAddress);
            }
            switch (_formsConfig.ShippingAddressOption)
            {
                case Abstractions.ShippingAddressOption.Both:
                case Abstractions.ShippingAddressOption.PayPal:
                    payment = payment.EnablePayPalShippingAddressesRetrieval(true);
                    break;
                default:
                    payment = payment.EnablePayPalShippingAddressesRetrieval(false);
                    break;
            }
            Intent intent = new Intent(Context, typeof(PaymentActivity));
            intent.PutExtra(PayPalService.ExtraPaypalConfiguration, _nativeConfig);
            intent.PutExtra(PaymentActivity.ExtraPayment, payment);
            (Context as Activity).StartActivityForResult(intent, REQUEST_CODE_PAYMENT);
        }

        public string GetClientMetadataId() => PayPalConfiguration.GetClientMetadataId(Context);

        public void FuturePayment(Action onCancelled, Action<string> onSuccess, Action<string> onError)
        {
            _onCancelled = onCancelled;
            _onSuccess = onSuccess;
            _onError = onError;

            Intent intent = new Intent(Context, typeof(PayPalFuturePaymentActivity));
            intent.PutExtra(PayPalService.ExtraPaypalConfiguration, _nativeConfig);
            (Context as Activity).StartActivityForResult(intent, REQUEST_CODE_FUTURE_PAYMENT);
        }

        public void AuthorizeProfileSharing(Action onCancelled, Action<string> onSuccess, Action<string> onError)
        {
            _onCancelled = onCancelled;
            _onSuccess = onSuccess;
            _onError = onError;

            Intent intent = new Intent(Context, typeof(PayPalProfileSharingActivity));
            intent.PutExtra(PayPalService.ExtraPaypalConfiguration, _nativeConfig);
            intent.PutExtra(PayPalProfileSharingActivity.ExtraRequestedScopes, GetOauthScopes());
            (Context as Activity).StartActivityForResult(intent, REQUEST_CODE_PROFILE_SHARING);
        }

        public void RequestCardData(Action onCancelled, Action<CreditCard, Bitmap> onSuccess, PayPalForms.CardIOLogo scannerLogo)
        {
            _retrieveCardCancelled = onCancelled;
            _retrieveCardSuccess = onSuccess;

            var requireExpiry = _formsConfig == null || _formsConfig.ScanRequiresExpiry;
            var requireCvv = _formsConfig == null || _formsConfig.ScanRequiresCvv;
            var scanExpiry = _formsConfig == null || _formsConfig.ScanExpiry;
            var disableManualEntry = _formsConfig == null || _formsConfig.ScanDisableManualEntry;

            Intent intent = new Intent(Context, typeof(CardIOActivity));
            switch (scannerLogo)
            {
                case PayPalForms.CardIOLogo.CardIO:
                    intent.PutExtra(CardIOActivity.ExtraHideCardioLogo, false);
                    intent.PutExtra(CardIOActivity.ExtraUseCardioLogo, true);
                    break;
                case PayPalForms.CardIOLogo.None:
                    intent.PutExtra(CardIOActivity.ExtraHideCardioLogo, true);
                    intent.PutExtra(CardIOActivity.ExtraUseCardioLogo, false);
                    break;
            }
            intent.PutExtra(CardIOActivity.ExtraReturnCardImage, true);
            intent.PutExtra(CardIOActivity.ExtraRequireExpiry, requireExpiry);
            intent.PutExtra(CardIOActivity.ExtraRequireCvv, requireCvv);
            intent.PutExtra(CardIOActivity.ExtraScanExpiry, scanExpiry);
            intent.PutExtra(CardIOActivity.ExtraSuppressManualEntry, disableManualEntry);

            (Context as Activity).StartActivityForResult(intent, REQUEST_CODE_CARD_SCAN);
        }

        public void Destroy()
        {
            try
            {
                Context.StopService(new Intent(Context, typeof(PayPalService)));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"[Exception] while stopping service {e.Message}");
            }
        }

        public void UpdateConfig(PayPalForms.PayPalConfiguration _newConfig)
        {
            _formsConfig = _newConfig;
            InitConfig();
        }

        public void ClearUserData()
        {
            Destroy();
            _nativeConfig = _nativeConfig.RememberUser(false);
            StartService();
        }

        public void OnActivityResult(int requestCode, Result resultCode, global::Android.Content.Intent data)
        {
            if (requestCode == REQUEST_CODE_PAYMENT)
            {
                if (resultCode == Result.Ok)
                {
                    PaymentConfirmation confirm = (PaymentConfirmation)data.GetParcelableExtra(PaymentActivity.ExtraResultConfirmation);
                    if (confirm != null)
                    {
                        try
                        {
                            _onSuccess?.Invoke(confirm.ToJSONObject().ToString());
                            _onSuccess = null;

                        }
                        catch (JSONException e)
                        {
                            _onError?.Invoke("an extremely unlikely failure occurred: " + e.Message);
                            _onError = null;
                            System.Diagnostics.Debug.WriteLine("an extremely unlikely failure occurred: " + e.Message);
                        }
                    }
                    _onError?.Invoke("Unknown Error");
                    _onError = null;
                }
                else if (resultCode == Result.Canceled)
                {
                    _onCancelled?.Invoke();
                    _onCancelled = null;
                    System.Diagnostics.Debug.WriteLine("The user canceled.");
                }
                else if ((int)resultCode == PaymentActivity.ResultExtrasInvalid)
                {
                    _onError?.Invoke("An invalid Payment or PayPalConfiguration was submitted. Please see the docs.");
                    _onError = null;
                    System.Diagnostics.Debug.WriteLine("An invalid Payment or PayPalConfiguration was submitted. Please see the docs.");
                }
            }
            else if (requestCode == REQUEST_CODE_FUTURE_PAYMENT)
            {
                if (resultCode == Result.Ok)
                {
                    PayPalAuthorization auth = (PayPalAuthorization)data.GetParcelableExtra(PayPalFuturePaymentActivity.ExtraResultAuthorization);
                    if (auth != null)
                    {
                        try
                        {
                            _onSuccess?.Invoke(auth.ToJSONObject().ToString());
                            _onSuccess = null;
                        }
                        catch (JSONException e)
                        {
                            System.Diagnostics.Debug.WriteLine("an extremely unlikely failure occurred: " + e.Message);
                        }
                    }
                    _onError?.Invoke("Unknown Error");
                    _onError = null;
                }
                else if (resultCode == Result.Canceled)
                {
                    _onCancelled?.Invoke();
                    _onCancelled = null;
                    System.Diagnostics.Debug.WriteLine("The user canceled.");
                }
                else if ((int)resultCode == PayPalFuturePaymentActivity.ResultExtrasInvalid)
                {
                    _onError?.Invoke("Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
                    _onError = null;
                    System.Diagnostics.Debug.WriteLine("Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
                }
            }
            else if (requestCode == REQUEST_CODE_PROFILE_SHARING)
            {
                if (resultCode == Result.Ok)
                {
                    PayPalAuthorization auth = (PayPalAuthorization)data.GetParcelableExtra(PayPalProfileSharingActivity.ExtraResultAuthorization);
                    if (auth != null)
                    {
                        try
                        {
                            _onSuccess?.Invoke(auth.ToJSONObject().ToString());
                            _onSuccess = null;
                        }
                        catch (JSONException e)
                        {
                            System.Diagnostics.Debug.WriteLine("an extremely unlikely failure occurred: " + e.Message);
                        }
                    }
                    _onError?.Invoke("Unknown Error");
                    _onError = null;
                }
                else if (resultCode == Result.Canceled)
                {
                    _onCancelled?.Invoke();
                    _onCancelled = null;
                    System.Diagnostics.Debug.WriteLine("The user canceled.");
                }
                else if ((int)resultCode == PayPalFuturePaymentActivity.ResultExtrasInvalid)
                {
                    _onError?.Invoke("Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
                    _onError = null;
                    System.Diagnostics.Debug.WriteLine("Probably the attempt to previously start the PayPalService had an invalid PayPalConfiguration. Please see the docs.");
                }
            }
            else if (requestCode == REQUEST_CODE_CARD_SCAN)
            {
                if (data == null)
                {
                    _retrieveCardCancelled?.Invoke();
                    _retrieveCardCancelled = null;
                    System.Diagnostics.Debug.WriteLine("The user canceled.");
                }
                else
                {
                    var card = (CreditCard)data.GetParcelableExtra(CardIOActivity.ExtraScanResult);
                    if (card != null)
                    {
                        _retrieveCardSuccess?.Invoke(card, CardIOActivity.GetCapturedCardImage(data));
                        _retrieveCardSuccess = null;
                    }
                    else
                    {
                        _retrieveCardCancelled?.Invoke();
                        _retrieveCardCancelled = null;
                        System.Diagnostics.Debug.WriteLine("The user canceled.");
                    }
                }
            }
        }

        public Context Context { get; }

        void InitConfig()
        {
            Destroy();

            switch (_formsConfig.Environment)
            {
                case PayPalForms.PayPalEnvironment.NoNetwork:
                    _configEnvironment = PayPalConfiguration.EnvironmentNoNetwork;
                    break;
                case PayPalForms.PayPalEnvironment.Production:
                    _configEnvironment = PayPalConfiguration.EnvironmentProduction;
                    break;
                case PayPalForms.PayPalEnvironment.Sandbox:
                    _configEnvironment = PayPalConfiguration.EnvironmentSandbox;
                    break;
            }

            _configClientId = _formsConfig.PayPalKey;

            _nativeConfig = new PayPalConfiguration()
                .Environment(_configEnvironment)
                .ClientId(_configClientId)
                .AcceptCreditCards(_formsConfig.AcceptCreditCards)
                .MerchantName(_formsConfig.MerchantName)
                .MerchantPrivacyPolicyUri(global::Android.Net.Uri.Parse(_formsConfig.MerchantPrivacyPolicyUri))
                .MerchantUserAgreementUri(global::Android.Net.Uri.Parse(_formsConfig.MerchantUserAgreementUri))
                .RememberUser(_formsConfig.StoreUserData);

            if (!String.IsNullOrEmpty(_formsConfig.Language))
            {
                _nativeConfig = _nativeConfig.LanguageOrLocale(_formsConfig.Language);
            }

            if (!String.IsNullOrEmpty(_formsConfig.PhoneCountryCode))
            {
                _nativeConfig = _nativeConfig.DefaultUserPhoneCountryCode(_formsConfig.PhoneCountryCode);
            }
            StartService();
        }

        void StartService()
        {
            Intent intent = new Intent(Context, typeof(PayPalService));
            intent.PutExtra(PayPalService.ExtraPaypalConfiguration, _nativeConfig);
            Context.StartService(intent);
        }

        PayPalOAuthScopes GetOauthScopes()
        {
            HashSet<string> scopes = new HashSet<string>();
            scopes.Add(PayPalOAuthScopes.PaypalScopeOpenid);
            scopes.Add(PayPalOAuthScopes.PaypalScopeEmail);
            scopes.Add(PayPalOAuthScopes.PaypalScopeAddress);
            scopes.Add(PayPalOAuthScopes.PaypalScopePhone);
            return new PayPalOAuthScopes(scopes.ToList());
        }

        string RoundNumber(double baseNumber)
        {
            var s = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", baseNumber);
            if (s.EndsWith("00", true, CultureInfo.InvariantCulture))
            {
                return ((int)baseNumber).ToString();
            }
            else
            {
                return s;
            }
        }
    }
}
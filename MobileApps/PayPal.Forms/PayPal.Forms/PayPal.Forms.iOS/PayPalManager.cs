using System;
using Xamarin.PayPal.iOS;
using Foundation;
using System.Diagnostics;
using System.Collections.Generic;
using UIKit;
using System.Linq;
using System.Globalization;
using PayPalForms = PayPal.Forms.Abstractions;

namespace PayPal.Forms
{
    public class PayPalManager : PayPalPaymentDelegate
    {
        #region PayPalProfileSharingDelegate

        public void UserDidCancelPayPalProfileSharingViewController(PayPalProfileSharingViewController profileSharingViewController)
        {
            Debug.WriteLine("PayPal Profile Sharing Authorization Canceled");
            profileSharingViewController?.DismissViewController(true, null);
            _onCancelled?.Invoke();
            _onCancelled = null;
            if (!_formsConfig.StoreUserData)
            {
                ClearUserData();
            }
        }

        public void PayPalProfileSharingViewController(PayPalProfileSharingViewController profileSharingViewController, NSDictionary profileSharingAuthorization)
        {
            Debug.WriteLine("PayPal Profile Sharing Authorization Success!");
            profileSharingViewController.DismissViewController(true, () =>
            {
                NSError err = null;
                NSData jsonData = NSJsonSerialization.Serialize(profileSharingAuthorization, NSJsonWritingOptions.PrettyPrinted, out err);
                NSString first = new NSString("");
                if (err == null)
                {
                    first = new NSString(jsonData, NSStringEncoding.UTF8);
                }
                else
                {
                    Debug.WriteLine(err.LocalizedDescription);
                }
                _onSuccess?.Invoke(first.ToString());
                _onSuccess = null;
                if (!_formsConfig.StoreUserData)
                {
                    ClearUserData();
                }
            });
        }

        #endregion

        #region PayPalFuturePaymentDelegate

        public void PayPalFuturePaymentDidCancel(PayPalFuturePaymentViewController futurePaymentViewController)
        {
            Debug.WriteLine("PayPal Future Payment Authorization Canceled");
            futurePaymentViewController?.DismissViewController(true, null);
            _onCancelled?.Invoke();
            _onCancelled = null;
            if (!_formsConfig.StoreUserData)
            {
                ClearUserData();
            }
        }

        void PayPalFuturePaymentViewController(PayPalFuturePaymentViewController futurePaymentViewController, NSDictionary futurePaymentAuthorization)
        {
            Debug.WriteLine("PayPal Future Payment Authorization Success!");
            futurePaymentViewController?.DismissViewController(true, () =>
            {
                NSError err = null;
                NSData jsonData = NSJsonSerialization.Serialize(futurePaymentAuthorization, NSJsonWritingOptions.PrettyPrinted, out err);
                NSString first = new NSString("");
                if (err == null)
                {
                    first = new NSString(jsonData, NSStringEncoding.UTF8);
                }
                else
                {
                    Debug.WriteLine(err.LocalizedDescription);
                }

                _onSuccess?.Invoke(first.ToString());
                _onSuccess = null;
                if (!_formsConfig.StoreUserData)
                {
                    ClearUserData();
                }
            });
        }

        #endregion

        #region ProvideCreditCard

        public void UserDidCancelPaymentViewController(CardIOPaymentViewController paymentViewController)
        {
            paymentViewController.DismissViewController(true, null);
            _retrieveCardCancelled?.Invoke();
            if (!_formsConfig.StoreUserData)
            {
                ClearUserData();
            }
        }

        public void UserDidProvideCreditCardInfo(CardIOCreditCardInfo cardInfo, CardIOPaymentViewController paymentViewController)
        {
            paymentViewController.DismissViewController(true, null);
            _retrieveCardSuccess?.Invoke(cardInfo);
            if (!_formsConfig.StoreUserData)
            {
                ClearUserData();
            }
        }

        #endregion

        PayPalConfiguration _nativeConfig;
        PayPalForms.PayPalConfiguration _formsConfig;
        bool _acceptCreditCards;
        string _environment;
        Action _onCancelled;
        Action<string> _onSuccess;
        Action<string> _onError;
        Action _retrieveCardCancelled;
        Action<CardIOCreditCardInfo> _retrieveCardSuccess;

        public PayPalManager(PayPalForms.PayPalConfiguration xfconfig)
        {
            UpdateConfig(xfconfig);
        }

        public bool AcceptCreditCards
        {
            get => _acceptCreditCards;
            set => _nativeConfig.AcceptCreditCards = _acceptCreditCards = value;
        }

        public string Environment
        {
            get => _environment;
            set
            {
                if (value != _environment)
                {
                    PayPalMobile.PreconnectWithEnvironment(value);
                    _environment = value;
                }
            }
        }

        public void BuyItems(PayPalForms.PayPalItem[] items, Decimal xfshipping, Decimal xftax, PayPalForms.PaymentIntent xfintent, Action onCancelled, Action<string> onSuccess, Action<string> onError, PayPalForms.ShippingAddress address)
        {
            _onCancelled = onCancelled;
            _onSuccess = onSuccess;
            _onError = onError;

            List<PayPalItem> nativeItems = new List<PayPalItem>();
            foreach (var product in items)
            {
                nativeItems.Add(PayPalItem.ItemWithName(
                    product.Name,
                    (nuint)product.Quantity,
                    new NSDecimalNumber(RoundNumber((double)product.Price)),
                    product.Currency,
                    product.SKU)
                );
            }
            var subtotal = PayPalItem.TotalPriceForItems(nativeItems.ToArray());
            var shipping = new NSDecimalNumber(RoundNumber((double)xfshipping));
            var tax = new NSDecimalNumber(RoundNumber((double)xftax));
            var paymentDetails = PayPalPaymentDetails.PaymentDetailsWithSubtotal(subtotal, shipping, tax);
            var total = subtotal.Add(shipping).Add(tax);
            PayPalPaymentIntent paymentIntent;
            switch (xfintent)
            {
                case PayPalForms.PaymentIntent.Authorize:
                    paymentIntent = PayPalPaymentIntent.Authorize;
                    break;
                case PayPalForms.PaymentIntent.Order:
                    paymentIntent = PayPalPaymentIntent.Order;
                    break;
                default:
                    paymentIntent = PayPalPaymentIntent.Sale;
                    break;
            }
            var payment = PayPalPayment.PaymentWithAmount(total, nativeItems.FirstOrDefault().Currency, "Multiple items", paymentIntent);
            payment.Items = nativeItems.ToArray();
            payment.PaymentDetails = paymentDetails;
            if (address != null)
            {
                payment.ShippingAddress = PayPalShippingAddress.ShippingAddressWithRecipientName(
                    address.RecipientName,
                    address.Line1,
                    address.Line2,
                    address.City,
                    address.State,
                    address.PostalCode,
                    address.CountryCode
                );
            }
            if (payment.Processable)
            {
                var paymentViewController = new PayPalPaymentViewController(payment, _nativeConfig, this);
                var top = GetTopViewController(UIApplication.SharedApplication.KeyWindow);
                top.PresentViewController(paymentViewController, true, null);
            }
            else
            {
                _onError?.Invoke("This particular payment will always be processable. If, for example, the amount was negative or the shortDescription was empty, this payment wouldn't be processable, and you'd want to handle that here.");
                Debug.WriteLine("Payment not processalbe:" + payment.Items);
            }
        }

        public void BuyItem(PayPalForms.PayPalItem item, Decimal xftax, PayPalForms.PaymentIntent xfintent, Action onCancelled, Action<string> onSuccess, Action<string> onError, PayPalForms.ShippingAddress address)
        {
            _onCancelled = onCancelled;
            _onSuccess = onSuccess;
            _onError = onError;

            var subTotal = new NSDecimalNumber(RoundNumber((double)item.Price));
            NSDecimalNumber amount = subTotal.Add(new NSDecimalNumber(RoundNumber((double)xftax)));
            var paymentDetails = PayPalPaymentDetails.PaymentDetailsWithSubtotal(subTotal, new NSDecimalNumber(0), new NSDecimalNumber(RoundNumber((double)xftax)));
            PayPalPaymentIntent paymentIntent;
            switch (xfintent)
            {
                case PayPalForms.PaymentIntent.Authorize:
                    paymentIntent = PayPalPaymentIntent.Authorize;
                    break;
                case PayPalForms.PaymentIntent.Order:
                    paymentIntent = PayPalPaymentIntent.Order;
                    break;
                default:
                    paymentIntent = PayPalPaymentIntent.Sale;
                    break;
            }
            var payment = PayPalPayment.PaymentWithAmount(amount, item.Currency, item.Name, paymentIntent);
            payment.PaymentDetails = paymentDetails;
            payment.Items = new NSObject[]{
                PayPalItem.ItemWithName (
                    item.Name,
                    1,
                    new  NSDecimalNumber (RoundNumber((double)item.Price)),
                    item.Currency,
                    item.SKU
                )
            };
            if (address != null)
            {
                payment.ShippingAddress = PayPalShippingAddress.ShippingAddressWithRecipientName(
                    address.RecipientName,
                    address.Line1,
                    address.Line2,
                    address.City,
                    address.State,
                    address.PostalCode,
                    address.CountryCode
                );
            }
            if (payment.Processable)
            {
                var paymentViewController = new PayPalPaymentViewController(payment, _nativeConfig, this);
                var top = GetTopViewController(UIApplication.SharedApplication.KeyWindow);
                top.PresentViewController(paymentViewController, true, null);
            }
            else
            {
                _onError?.Invoke("This particular payment will always be processable. If, for example, the amount was negative or the shortDescription was empty, this payment wouldn't be processable, and you'd want to handle that here.");
                _onError = null;
                Debug.WriteLine("Payment not processalbe:" + payment.Items);
            }
        }

        public override void PayPalPaymentDidCancel(PayPalPaymentViewController paymentViewController)
        {
            Debug.WriteLine("PayPal Payment Cancelled");
            paymentViewController?.DismissViewController(true, null);
            _onCancelled?.Invoke();
            _onCancelled = null;
            if (!_formsConfig.StoreUserData)
            {
                ClearUserData();
            }
        }

        public override void PayPalPaymentViewController(PayPalPaymentViewController paymentViewController, PayPalPayment completedPayment)
        {
            Debug.WriteLine("PayPal Payment Success !");
            paymentViewController.DismissViewController(true, () =>
            {
                NSError err = null;
                NSData jsonData = NSJsonSerialization.Serialize(completedPayment.Confirmation, NSJsonWritingOptions.PrettyPrinted, out err);
                NSString first = new NSString("");
                if (err == null)
                {
                    first = new NSString(jsonData, NSStringEncoding.UTF8);
                }
                else
                {
                    Debug.WriteLine(err.LocalizedDescription);
                }
                _onSuccess?.Invoke(first.ToString());
                _onSuccess = null;
                if (!_formsConfig.StoreUserData)
                {
                    ClearUserData();
                }
            });
        }

        public void FuturePayment(Action onCancelled, Action<string> onSuccess)
        {
            _onCancelled = onCancelled;
            _onSuccess = onSuccess;
            var futurePaymentViewController = new PayPalFuturePaymentViewController(_nativeConfig, new CustomPayPalFuturePaymentDelegate(this));
            var top = GetTopViewController(UIApplication.SharedApplication.KeyWindow);
            top.PresentViewController(futurePaymentViewController, true, null);
        }

        public void AuthorizeProfileSharing(Action onCancelled, Action<string> onSuccess)
        {
            _onCancelled = onCancelled;
            _onSuccess = onSuccess;
            var infoSet = new NSSet(
                Constants.kPayPalOAuth2ScopeOpenId.ToString(),
                Constants.kPayPalOAuth2ScopeEmail.ToString(),
                Constants.kPayPalOAuth2ScopeAddress.ToString(),
                Constants.kPayPalOAuth2ScopePhone.ToString()
            );
            var profileSharingViewController = new PayPalProfileSharingViewController(infoSet, _nativeConfig, new CustomAuthorizeProfileSharingDelegate(this));
            var top = GetTopViewController(UIApplication.SharedApplication.KeyWindow);
            top.PresentViewController(profileSharingViewController, true, null);
        }

        public void RequestCardData(Action onCancelled, Action<CardIOCreditCardInfo> onSuccess, PayPalForms.CardIOLogo scannerLogo)
        {
            _retrieveCardCancelled = onCancelled;
            _retrieveCardSuccess = onSuccess;

            var requireExpiry = _formsConfig == null || _formsConfig.ScanRequiresExpiry;
            var requireCvv = _formsConfig == null || _formsConfig.ScanRequiresCvv;
            var scanExpiry = _formsConfig == null || _formsConfig.ScanExpiry;
            var disableManualEntry = _formsConfig == null || _formsConfig.ScanDisableManualEntry;

            var scanViewController = new CardIOPaymentViewController(new CustomCardIOPaymentViewControllerDelegate(this));

            switch (scannerLogo)
            {
                case PayPalForms.CardIOLogo.CardIO:
                    scanViewController.HideCardIOLogo = false;
                    scanViewController.UseCardIOLogo = true;
                    break;
                case PayPalForms.CardIOLogo.None:
                    scanViewController.HideCardIOLogo = true;
                    scanViewController.UseCardIOLogo = false;
                    break;
            }

            scanViewController.CollectExpiry = requireExpiry;
            scanViewController.CollectCVV = requireCvv;
            scanViewController.ScanExpiry = scanExpiry;
            scanViewController.DisableManualEntryButtons = disableManualEntry;

            var top = GetTopViewController(UIApplication.SharedApplication.KeyWindow);
            top.PresentViewController(scanViewController, true, null);
        }

        public void UpdateConfig(PayPalForms.PayPalConfiguration _newConfig)
        {
            _formsConfig = _newConfig;
            InitConfig();
        }

        public void ClearUserData()
        {
            PayPalMobile.ClearAllUserData();
        }

        void InitConfig()
        {
            NSString key = null;
            NSString value = new NSString(_formsConfig.PayPalKey);
            string env = string.Empty;
            switch (_formsConfig.Environment)
            {
                case PayPalForms.PayPalEnvironment.NoNetwork:
                    key = Constants.PayPalEnvironmentNoNetwork;
                    env = Constants.PayPalEnvironmentNoNetwork.ToString();
                    break;
                case PayPalForms.PayPalEnvironment.Production:
                    key = Constants.PayPalEnvironmentProduction;
                    env = Constants.PayPalEnvironmentProduction.ToString();
                    break;
                case PayPalForms.PayPalEnvironment.Sandbox:
                    key = Constants.PayPalEnvironmentSandbox;
                    env = Constants.PayPalEnvironmentSandbox.ToString();
                    break;
            }

            PayPalMobile.InitializeWithClientIdsForEnvironments(NSDictionary.FromObjectsAndKeys(
                new NSObject[] {
                    value,
                    value,
                    value
                }, new NSObject[] {
                    key,
                    Constants.PayPalEnvironmentProduction,
                    Constants.PayPalEnvironmentSandbox
                }
            ));
            Environment = env;
            _nativeConfig = new PayPalConfiguration();
            AcceptCreditCards = _formsConfig.AcceptCreditCards;
            _nativeConfig.MerchantName = _formsConfig.MerchantName;
            _nativeConfig.MerchantPrivacyPolicyURL = new NSUrl(_formsConfig.MerchantPrivacyPolicyUri);
            _nativeConfig.MerchantUserAgreementURL = new NSUrl(_formsConfig.MerchantUserAgreementUri);
            _nativeConfig.LanguageOrLocale = _formsConfig.Language ?? NSLocale.PreferredLanguages[0];
            if (!String.IsNullOrEmpty(_formsConfig.PhoneCountryCode))
            {
                _nativeConfig.DefaultUserPhoneCountryCode = _formsConfig.PhoneCountryCode;
            }
            switch (_formsConfig.ShippingAddressOption)
            {
                case PayPalForms.ShippingAddressOption.Both:
                    _nativeConfig.PayPalShippingAddressOption = PayPalShippingAddressOption.Both;
                    break;
                case PayPalForms.ShippingAddressOption.None:
                    _nativeConfig.PayPalShippingAddressOption = PayPalShippingAddressOption.None;
                    break;
                case PayPalForms.ShippingAddressOption.PayPal:
                    _nativeConfig.PayPalShippingAddressOption = PayPalShippingAddressOption.PayPal;
                    break;
                case PayPalForms.ShippingAddressOption.Provided:
                    _nativeConfig.PayPalShippingAddressOption = PayPalShippingAddressOption.Provided;
                    break;
            }
        }

        UIViewController GetTopViewController(UIWindow window)
        {
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            var navController = vc as UINavigationController;
            if (navController != null)
            {
                vc = navController.ViewControllers.Last();
            }
            return vc;
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

        class CustomCardIOPaymentViewControllerDelegate : CardIOPaymentViewControllerDelegate
        {
            PayPalManager _payPalManager;

            public CustomCardIOPaymentViewControllerDelegate(PayPalManager manager)
            {
                this._payPalManager = manager;
            }

            public override void UserDidCancelPaymentViewController(CardIOPaymentViewController paymentViewController)
            {
                this._payPalManager.UserDidCancelPaymentViewController(paymentViewController);
            }

            public override void UserDidProvideCreditCardInfo(CardIOCreditCardInfo cardInfo, CardIOPaymentViewController paymentViewController)
            {
                this._payPalManager.UserDidProvideCreditCardInfo(cardInfo, paymentViewController);
            }
        }

        class CustomPayPalFuturePaymentDelegate : PayPalFuturePaymentDelegate
        {
            PayPalManager _payPalManager;

            public CustomPayPalFuturePaymentDelegate(PayPalManager manager)
            {
                this._payPalManager = manager;
            }

            public override void PayPalFuturePaymentDidCancel(PayPalFuturePaymentViewController futurePaymentViewController)
            {
                this._payPalManager.PayPalFuturePaymentDidCancel(futurePaymentViewController);
            }

            public override void PayPalFuturePaymentViewController(PayPalFuturePaymentViewController futurePaymentViewController, NSDictionary futurePaymentAuthorization)
            {
                this._payPalManager.PayPalFuturePaymentViewController(futurePaymentViewController, futurePaymentAuthorization);
            }
        }

        class CustomAuthorizeProfileSharingDelegate : PayPalProfileSharingDelegate
        {
            PayPalManager _payPalManager;

            public CustomAuthorizeProfileSharingDelegate(PayPalManager manager)
            {
                this._payPalManager = manager;
            }

            public override void UserDidCancelPayPalProfileSharingViewController(PayPalProfileSharingViewController profileSharingViewController)
            {
                this._payPalManager.UserDidCancelPayPalProfileSharingViewController(profileSharingViewController);
            }

            public override void PayPalProfileSharingViewController(PayPalProfileSharingViewController profileSharingViewController, NSDictionary profileSharingAuthorization)
            {
                this._payPalManager.PayPalProfileSharingViewController(profileSharingViewController, profileSharingAuthorization);
            }
        }
    }
}
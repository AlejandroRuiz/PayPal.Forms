using System;

namespace PayPal.Forms.Abstractions
{
    public class PayPalConfiguration
    {
        public PayPalEnvironment Environment { get; }

        public string PayPalKey { get; }

        public string PhoneCountryCode { get; set; }

        public string Language { get; set; }

        public string MerchantName { get; set; }

        public string MerchantPrivacyPolicyUri { get; set; }

        public string MerchantUserAgreementUri { get; set; }

        public bool AcceptCreditCards { get; set; }

        public bool StoreUserData { get; set; } = true;

        public ShippingAddressOption ShippingAddressOption { get; set; }

        public bool ScanRequiresExpiry { get; set; } = true;

        public bool ScanRequiresCvv { get; set; } = true;

        public bool ScanExpiry { get; set; } = true;

		public bool ScanDisableManualEntry { get; set; } = false;

        public PayPalConfiguration(PayPalEnvironment environment, string idEnvironment)
        {
            this.Environment = environment;
            this.PayPalKey = idEnvironment;
        }
    }
}
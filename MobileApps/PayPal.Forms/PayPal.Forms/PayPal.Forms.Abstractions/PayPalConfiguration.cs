using PayPal.Forms.Abstractions.Enum;

namespace PayPal.Forms.Abstractions
{
    public class PayPalConfiguration
    {
        public PayPalEnvironment Environment { get; private set; }

        public string PayPalKey { get; private set; }

        public string PhoneCountryCode { get; set; }

        public string Language { get; set; }

        public string MerchantName { get; set; }

        public string MerchantPrivacyPolicyUri { get; set; }

        public string MerchantUserAgreementUri { get; set; }
<<<<<<< HEAD
=======

        public bool AcceptCreditCards { get; set; }

        public PayPalConfiguration(PayPalEnvironment enviroment, string idEnvironment)
        {
            Environment = enviroment;
            PayPalKey = idEnvironment;
        }
    }
}
>>>>>>> d3942ff1a9fa1ee495bb46ef241b36a9d3d5f692

        public bool AcceptCreditCards { get; set; }

        public ShippingAddressOption ShippingAddressOption { get; set; }

        public PayPalConfiguration (PayPalEnvironment environment, string idEnvironment)
        {
            Environment = environment;
            PayPalKey = idEnvironment;
        }
    }
}
using PayPal.Forms.Abstractions.Enum;

namespace PayPal.Forms.Abstractions
{
	public class PayPalConfiguration
	{
		public Environment Environment { get; private set; }

		public string PayPalKey { get; private set; }

		public string MerchantName { get; set; }

		public string MerchantPrivacyPolicyUri { get; set; }

		public string MerchantUserAgreementUri { get; set; }

		public bool AcceptCreditCards { get; set; }

		public PayPalConfiguration (Environment enviroment, string idEnvironment)
		{
			Environment = enviroment;
			PayPalKey = idEnvironment;
		}
	}
}


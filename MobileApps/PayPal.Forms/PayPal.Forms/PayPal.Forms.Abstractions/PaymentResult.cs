using System;
using PayPal.Forms.Abstractions.Enum;

namespace PayPal.Forms.Abstractions
{
	public class PaymentResult
	{
		public class Response
		{
			public string create_time { get; set; }
			public string id { get; set; }
			public string state { get; set; }
			public string intent { get; set; }
		}

		public class Client
		{
			public string paypal_sdk_version { get; set; }
			public string environment { get; set; }
			public string platform { get; set; }
			public string product_name { get; set; }
		}

		public class PayPalResponse
		{
			public Response response { get; set; }
			public Client client { get; set; }
			public string response_type { get; set; }
		}

		public PaymentResultStatus Status { get; private set; }

		public string ErrorMessage { get; private set; }

		public PayPalResponse ServerResponse { get; private set; }

		public PaymentResult (PaymentResultStatus status, string errorMessage = null, PayPalResponse serverResponse = null)
		{
			Status = status;
			ErrorMessage = errorMessage;
			ServerResponse = serverResponse;
		}
	}
}


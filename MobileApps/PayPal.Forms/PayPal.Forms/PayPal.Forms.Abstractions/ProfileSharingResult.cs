using System;
using Newtonsoft.Json;

namespace PayPal.Forms.Abstractions
{
    public class ProfileSharingResult
    {
        public class Response
        {
            [JsonProperty(PropertyName = "code")]
            public string Code { get; set; }
        }

        public class Client
        {
            [JsonProperty(PropertyName = "paypal_sdk_version")]
            public string PaypalSdkVersion { get; set; }

            [JsonProperty(PropertyName = "environment")]
            public string Environment { get; set; }

            [JsonProperty(PropertyName = "platform")]
            public string Platform { get; set; }

            [JsonProperty(PropertyName = "product_name")]
            public string ProductName { get; set; }
        }

        public class PayPalProfileSharingResponse
        {
            [JsonProperty(PropertyName = "response")]
            public Response Response { get; set; }

            [JsonProperty(PropertyName = "client")]
            public Client Client { get; set; }

            [JsonProperty(PropertyName = "response_type")]
            public string ResponseType { get; set; }
        }

        public PayPalStatus Status { get; }

        public string ErrorMessage { get; }

        public PayPalProfileSharingResponse ServerResponse { get; }

        public ProfileSharingResult(PayPalStatus status, string errorMessage = null, PayPalProfileSharingResponse serverResponse = null)
        {
            this.Status = status;
            this.ErrorMessage = errorMessage;
            this.ServerResponse = serverResponse;
        }
    }
}
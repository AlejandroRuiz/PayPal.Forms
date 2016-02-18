# PayPal.Forms
PayPal Plugin for Xamarin.Forms

# Setup

##iOS & Android

In your main application constructor add the following code with your owns values
```
public App ()
{
  CrossPaypalManager.Init(new PayPalConfiguration(
					PayPal.Forms.Abstractions.Enum.Environment.NoNetwork,
					"Your PayPal ID from https://developer.paypal.com/developer/applications/"
					)
					{
					  //If you want to accept credit cards
					  AcceptCreditCards = true,
				  	//Your business name
					  MerchantName = "Test Store",
					  //Your privacy policy Url
					  MerchantPrivacyPolicyUri = "https://www.example.com/privacy",
					  //Your user agreement Url
					  MerchantUserAgreementUri = "https://www.example.com/legal"
				  }
			);
}
```

##iOS

From oficial PayPal SDK Page (https://github.com/paypal/PayPal-iOS-SDK#with-or-without-cocoapods) follow the two steps:

* Add the open source license acknowledgments
* Add squemas into Info.plist

##Android

Is very important to add the following code inside your OnActivityResult and OnDestroy on your main activity class
```
protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
{
	base.OnActivityResult (requestCode, resultCode, data);
	PayPalManagerImplementation.Manager.OnActivityResult(requestCode, resultCode, data);
}

protected override void OnDestroy()
{
	base.OnDestroy();
	PayPalManagerImplementation.Manager.Destroy();
}
```

# Usage

##Single Item

```
var result = await CrossPaypalManager.Current.Buy (new PayPalItem ("Test Product", new BigDecimal (12.50), "USD"), new BigDecimal (0));
if (result.Status == PaymentResultStatus.Cancelled) {
	Console.WriteLine ("Cancelled");
}else if(result.Status == PaymentResultStatus.Error){
	Console.WriteLine (result.ErrorMessage);
}else if(result.Status == PaymentResultStatus.Successful){
	Console.WriteLine (result.ServerResponse.Response.Id);
}
```

##List of Items

```
var result = await CrossPaypalManager.Current.Buy (new PayPalItem[] {
				new PayPalItem ("sample item #1", 2, new BigDecimal (87.50), "USD",
					"sku-12345678"), 
				new PayPalItem ("free sample item #2", 1, new BigDecimal (0.00),
					"USD", "sku-zero-price"),
				new PayPalItem ("sample item #3 with a longer name", 6, new BigDecimal (37.99),
					"USD", "sku-33333") 
			}, new BigDecimal (20.5), new BigDecimal (13.20));
if (result.Status == PaymentResultStatus.Cancelled) {
	Console.WriteLine ("Cancelled");
}else if(result.Status == PaymentResultStatus.Error){
	Console.WriteLine (result.ErrorMessage);
}else if(result.Status == PaymentResultStatus.Successful){
	Console.WriteLine (result.ServerResponse.Response.Id);
}
```

##Future Payments

```
var result = await CrossPaypalManager.Current.RequestFuturePayments();
if (result.Status == PaymentResultStatus.Cancelled) {
	Console.WriteLine ("Cancelled");
}else if(result.Status == PaymentResultStatus.Error){
	Console.WriteLine (result.ErrorMessage);
}else if(result.Status == PaymentResultStatus.Successful){
	//Print Authorization Code
	Console.WriteLine(result.ServerResponse.Response.Code);
}
```

##Obtain a Client Metadata ID

```
//Print Client Metadata Id
Console.WriteLine(CrossPaypalManager.Current.ClientMetadataId);
```
# Nuget
* Nuget Package (https://www.nuget.org/packages/PayPal.Forms)


# TODO
* Add support for PCL Xamarin.Forms App Projects.
* Add app provided shipping address.
* Enable shipping address retrieval.
* Profile sharing

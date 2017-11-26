# PayPal.Forms
PayPal Plugin for Xamarin.Forms

# Setup

## iOS & Android

In MainActivity(Android)/AppDelegate(iOS) after "Forms.Init()"  call the Init method with your PayPal config value
```
...
global::Xamarin.Forms.Forms.Init ();
 CrossPayPalManager.Init(new PayPalConfiguration(
					PayPalEnvironment.NoNetwork,
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
					  MerchantUserAgreementUri = "https://www.example.com/legal",
					  
					  // OPTIONAL - ShippingAddressOption (Both, None, PayPal, Provided)
					  ShippingAddressOption = ShippingAddressOption.Both,
					  
					  // OPTIONAL - Language: Default languege for PayPal Plug-In
					  Language = "es",
					  
					   // OPTIONAL - PhoneCountryCode: Default phone country code for PayPal Plug-In
					  PhoneCountryCode = "52",
				  }
			);
...

```

## iOS

From official PayPal SDK page (https://github.com/paypal/PayPal-iOS-SDK#with-or-without-cocoapods) follow the two steps:

* Add the open source license acknowledgments
* Add squemas into Info.plist

### VERY IMPORTANT FOR IOS 10 IF YOU WANT TO USE THE CAMERA FEATURES

Add "NSCameraUsageDescription" into you Info.plist file.

```
<key>NSCameraUsageDescription</key>
<string>We will use your camera to scan the credit card</string>
```

## Android

YOU MUST NEED TO HAVE INSTALLED THE NEXT PACKAGE

https://www.nuget.org/packages/Square.OkHttp3/

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

## Single Item

```
var result = await CrossPayPalManager.Current.Buy (new PayPalItem ("Test Product", new Decimal (12.50), "USD"), new Decimal (0));
if (result.Status == PayPalStatus.Cancelled) {
	Debug.WriteLine ("Cancelled");
}else if(result.Status == PayPalStatus.Error){
	Debug.WriteLine (result.ErrorMessage);
}else if(result.Status == PayPalStatus.Successful){
	Debug.WriteLine (result.ServerResponse.Response.Id);
}
```

## List of Items

```
var result = await CrossPayPalManager.Current.Buy (new PayPalItem[] {
				new PayPalItem ("sample item #1", 2, new Decimal (87.50), "USD",
					"sku-12345678"), 
				new PayPalItem ("free sample item #2", 1, new Decimal (0.00),
					"USD", "sku-zero-price"),
				new PayPalItem ("sample item #3 with a longer name", 6, new Decimal (37.99),
					"USD", "sku-33333") 
			}, new Decimal (20.5), new Decimal (13.20));
if (result.Status == PayPalStatus.Cancelled) {
	Debug.WriteLine ("Cancelled");
}else if(result.Status == PayPalStatus.Error){
	Debug.WriteLine (result.ErrorMessage);
}else if(result.Status == PayPalStatus.Successful){
	Debug.WriteLine (result.ServerResponse.Response.Id);
}
```

## Shipping Address (Optional)

```
//Optional shipping address parameter into Buy methods.
var result = await CrossPayPalManager.Current.Buy(
			new PayPalItem(
				"Test Product",
				new Decimal(12.50), "USD"),
				new Decimal(0),
				new ShippingAddress("My Custom Recipient Name", "Custom Line 1", "", "My City", "My State", "12345", "MX")
		   );
if (result.Status == PayPalStatus.Cancelled)
{
	Debug.WriteLine("Cancelled");
}
else if (result.Status == PayPalStatus.Error)
{
	Debug.WriteLine(result.ErrorMessage);
}
else if (result.Status == PayPalStatus.Successful)
{
	Debug.WriteLine(result.ServerResponse.Response.Id);
}
```

## Future Payments

```
var result = await CrossPayPalManager.Current.RequestFuturePayments();
if (result.Status == PayPalStatus.Cancelled) {
	Debug.WriteLine ("Cancelled");
}else if(result.Status == PayPalStatus.Error){
	Debug.WriteLine (result.ErrorMessage);
}else if(result.Status == PayPalStatus.Successful){
	//Print Authorization Code
	Debug.WriteLine(result.ServerResponse.Response.Code);
}
```

## Profile sharing

```
var result = await CrossPayPalManager.Current.AuthorizeProfileSharing();
if (result.Status == PayPalStatus.Cancelled) {
	Debug.WriteLine ("Cancelled");
}else if(result.Status == PayPalStatus.Error){
	Debug.WriteLine (result.ErrorMessage);
}else if(result.Status == PayPalStatus.Successful){
	Debug.WriteLine (result.ServerResponse.Response.Code);
}
```

## Obtain a Client Metadata ID

```
//Print Client Metadata Id
Debug.WriteLine(CrossPayPalManager.Current.ClientMetadataId);
```

## Standalone Card Scanner

```
//Optional parameter CardIOLogo("PayPal", "CardIO" or "None") for ScanCard method by default "PayPal" is used
var result = await CrossPayPalManager.Current.ScanCard();
if (result.Status == PayPalStatus.Cancelled)
{
	Debug.WriteLine("Cancelled");
}
else if (result.Status == PayPalStatus.Successful)
{
	if (result.Card.CardImage != null)
	{
		CardImage.Source = result.Card.CardImage;
	}
	Debug.WriteLine($"CardNumber: {result.Card.CardNumber}, CardType: {result.Card.CardType.ToString()}, Cvv: {result.Card.Cvv}, ExpiryMonth: {result.Card.ExpiryMonth}");
	Debug.WriteLine($"ExpiryYear: {result.Card.ExpiryYear}, PostalCode: {result.Card.PostalCode}, RedactedCardNumber: {result.Card.RedactedCardNumber}, Scaned: {result.Card.Scaned}");
}
```

# Nuget
* Nuget Package (https://www.nuget.org/packages/PayPal.Forms)

# Known Issues
* On Xamarin Studio 5.10.X you will receive an alert that xamarin cant resolve "xamarin.paypal" and/or "paypal.forms" lib name this will be fixed by upgrading to Xamarin Studio 6 in beta/alpha channel(recommended) or downgrading to Xamarin Studio 5.9(not recommended)

# TODO
No new tasks to do. All issues or features requests are welcome

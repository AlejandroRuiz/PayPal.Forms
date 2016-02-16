using System;

using UIKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace Xamarin.PayPal.iOS
{
	// The first step to creating a binding is to add your native library ("libNativeLibrary.a")
	// to the project by right-clicking (or Control-clicking) the folder containing this source
	// file and clicking "Add files..." and then simply select the native library (or libraries)
	// that you want to bind.
	//
	// When you do that, you'll notice that MonoDevelop generates a code-behind file for each
	// native library which will contain a [LinkWith] attribute. MonoDevelop auto-detects the
	// architectures that the native library supports and fills in that information for you,
	// however, it cannot auto-detect any Frameworks or other system libraries that the
	// native library may depend on, so you'll need to fill in that information yourself.
	//
	// Once you've done that, you're ready to move on to binding the API...
	//
	//
	// Here is where you'd define your API definition for the native Objective-C library.
	//
	// For example, to bind the following Objective-C class:
	//
	//     @interface Widget : NSObject {
	//     }
	//
	// The C# binding would look like this:
	//
	//     [BaseType (typeof (NSObject))]
	//     interface Widget {
	//     }
	//
	// To bind Objective-C properties, such as:
	//
	//     @property (nonatomic, readwrite, assign) CGPoint center;
	//
	// You would add a property definition in the C# interface like so:
	//
	//     [Export ("center")]
	//     CGPoint Center { get; set; }
	//
	// To bind an Objective-C method, such as:
	//
	//     -(void) doSomething:(NSObject *)object atIndex:(NSInteger)index;
	//
	// You would add a method definition to the C# interface like so:
	//
	//     [Export ("doSomething:atIndex:")]
	//     void DoSomething (NSObject object, int index);
	//
	// Objective-C "constructors" such as:
	//
	//     -(id)initWithElmo:(ElmoMuppet *)elmo;
	//
	// Can be bound as:
	//
	//     [Export ("initWithElmo:")]
	//     IntPtr Constructor (ElmoMuppet elmo);
	//
	// For more information, see http://developer.xamarin.com/guides/ios/advanced_topics/binding_objective-c/
	//

	// @interface PayPalConfiguration : NSObject <NSCopying>
	[BaseType (typeof(NSObject))]
	interface PayPalConfiguration : INSCopying
	{
		// @property (readwrite, copy, nonatomic) NSString * _Nullable defaultUserEmail;
		[NullAllowed, Export ("defaultUserEmail")]
		string DefaultUserEmail { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable defaultUserPhoneCountryCode;
		[NullAllowed, Export ("defaultUserPhoneCountryCode")]
		string DefaultUserPhoneCountryCode { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable defaultUserPhoneNumber;
		[NullAllowed, Export ("defaultUserPhoneNumber")]
		string DefaultUserPhoneNumber { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable merchantName;
		[NullAllowed, Export ("merchantName")]
		string MerchantName { get; set; }

		// @property (readwrite, copy, nonatomic) NSURL * _Nullable merchantPrivacyPolicyURL;
		[NullAllowed, Export ("merchantPrivacyPolicyURL", ArgumentSemantic.Copy)]
		NSUrl MerchantPrivacyPolicyURL { get; set; }

		// @property (readwrite, copy, nonatomic) NSURL * _Nullable merchantUserAgreementURL;
		[NullAllowed, Export ("merchantUserAgreementURL", ArgumentSemantic.Copy)]
		NSUrl MerchantUserAgreementURL { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL acceptCreditCards;
		[Export ("acceptCreditCards")]
		bool AcceptCreditCards { get; set; }

		// @property (assign, readwrite, nonatomic) PayPalShippingAddressOption payPalShippingAddressOption;
		[Export ("payPalShippingAddressOption", ArgumentSemantic.Assign)]
		PayPalShippingAddressOption PayPalShippingAddressOption { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL rememberUser;
		[Export ("rememberUser")]
		bool RememberUser { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable languageOrLocale;
		[NullAllowed, Export ("languageOrLocale")]
		string LanguageOrLocale { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL alwaysDisplayCurrencyCodes;
		[Export ("alwaysDisplayCurrencyCodes")]
		bool AlwaysDisplayCurrencyCodes { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL disableBlurWhenBackgrounding;
		[Export ("disableBlurWhenBackgrounding")]
		bool DisableBlurWhenBackgrounding { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL presentingInPopover;
		[Export ("presentingInPopover")]
		bool PresentingInPopover { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL forceDefaultsInSandbox;
		[Export ("forceDefaultsInSandbox")]
		bool ForceDefaultsInSandbox { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable sandboxUserPassword;
		[NullAllowed, Export ("sandboxUserPassword")]
		string SandboxUserPassword { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable sandboxUserPin;
		[NullAllowed, Export ("sandboxUserPin")]
		string SandboxUserPin { get; set; }
	}

	// typedef void (^PayPalFuturePaymentDelegateCompletionBlock)();
	delegate void PayPalFuturePaymentDelegateCompletionBlock ();

	// @protocol PayPalFuturePaymentDelegate <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface PayPalFuturePaymentDelegate
	{
		// @required -(void)payPalFuturePaymentDidCancel:(PayPalFuturePaymentViewController * _Nonnull)futurePaymentViewController;
		[Abstract]
		[Export ("payPalFuturePaymentDidCancel:")]
		void PayPalFuturePaymentDidCancel (PayPalFuturePaymentViewController futurePaymentViewController);

		// @required -(void)payPalFuturePaymentViewController:(PayPalFuturePaymentViewController * _Nonnull)futurePaymentViewController didAuthorizeFuturePayment:(NSDictionary * _Nonnull)futurePaymentAuthorization;
		[Abstract]
		[Export ("payPalFuturePaymentViewController:didAuthorizeFuturePayment:")]
		void PayPalFuturePaymentViewController (PayPalFuturePaymentViewController futurePaymentViewController, NSDictionary futurePaymentAuthorization);

		// @optional -(void)payPalFuturePaymentViewController:(PayPalFuturePaymentViewController * _Nonnull)futurePaymentViewController willAuthorizeFuturePayment:(NSDictionary * _Nonnull)futurePaymentAuthorization completionBlock:(PayPalFuturePaymentDelegateCompletionBlock _Nonnull)completionBlock;
		[Export ("payPalFuturePaymentViewController:willAuthorizeFuturePayment:completionBlock:")]
		void PayPalFuturePaymentViewController (PayPalFuturePaymentViewController futurePaymentViewController, NSDictionary futurePaymentAuthorization, PayPalFuturePaymentDelegateCompletionBlock completionBlock);
	}

	// @interface PayPalFuturePaymentViewController : UINavigationController
	[BaseType (typeof(UINavigationController))]
	interface PayPalFuturePaymentViewController
	{
		// -(instancetype _Nullable)initWithConfiguration:(PayPalConfiguration * _Nonnull)configuration delegate:(id<PayPalFuturePaymentDelegate> _Nullable)delegate;
		[Export ("initWithConfiguration:delegate:")]
		IntPtr Constructor (PayPalConfiguration configuration, [NullAllowed] PayPalFuturePaymentDelegate @delegate);

		[Wrap ("WeakFuturePaymentDelegate")]
		[NullAllowed]
		PayPalFuturePaymentDelegate FuturePaymentDelegate { get; }

		// @property (readonly, nonatomic, weak) id<PayPalFuturePaymentDelegate> _Nullable futurePaymentDelegate;
		[NullAllowed, Export ("futurePaymentDelegate", ArgumentSemantic.Weak)]
		NSObject WeakFuturePaymentDelegate { get; }
	}

	//[Static]
	//[Verify (ConstantsInterfaceAssociation)]
	partial interface Constants
	{
		[Static]
		// extern NSString *const _Nonnull kPayPalOAuth2ScopeFuturePayments;
		[Field ("kPayPalOAuth2ScopeFuturePayments", "__Internal")]
		NSString kPayPalOAuth2ScopeFuturePayments { get; }

		[Static]
		// extern NSString *const _Nonnull kPayPalOAuth2ScopeProfile;
		[Field ("kPayPalOAuth2ScopeProfile", "__Internal")]
		NSString kPayPalOAuth2ScopeProfile { get; }

		[Static]
		// extern NSString *const _Nonnull kPayPalOAuth2ScopeOpenId;
		[Field ("kPayPalOAuth2ScopeOpenId", "__Internal")]
		NSString kPayPalOAuth2ScopeOpenId { get; }

		[Static]
		// extern NSString *const _Nonnull kPayPalOAuth2ScopePayPalAttributes;
		[Field ("kPayPalOAuth2ScopePayPalAttributes", "__Internal")]
		NSString kPayPalOAuth2ScopePayPalAttributes { get; }

		[Static]
		// extern NSString *const _Nonnull kPayPalOAuth2ScopeEmail;
		[Field ("kPayPalOAuth2ScopeEmail", "__Internal")]
		NSString kPayPalOAuth2ScopeEmail { get; }

		[Static]
		// extern NSString *const _Nonnull kPayPalOAuth2ScopeAddress;
		[Field ("kPayPalOAuth2ScopeAddress", "__Internal")]
		NSString kPayPalOAuth2ScopeAddress { get; }

		[Static]
		// extern NSString *const _Nonnull kPayPalOAuth2ScopePhone;
		[Field ("kPayPalOAuth2ScopePhone", "__Internal")]
		NSString kPayPalOAuth2ScopePhone { get; }
	}

	// @interface PayPalPaymentDetails : NSObject <NSCopying>
	[BaseType (typeof(NSObject))]
	interface PayPalPaymentDetails : INSCopying
	{
		// +(PayPalPaymentDetails * _Nonnull)paymentDetailsWithSubtotal:(NSDecimalNumber * _Nullable)subtotal withShipping:(NSDecimalNumber * _Nullable)shipping withTax:(NSDecimalNumber * _Nullable)tax;
		[Static]
		[Export ("paymentDetailsWithSubtotal:withShipping:withTax:")]
		PayPalPaymentDetails PaymentDetailsWithSubtotal ([NullAllowed] NSDecimalNumber subtotal, [NullAllowed] NSDecimalNumber shipping, [NullAllowed] NSDecimalNumber tax);

		// @property (readwrite, copy, nonatomic) NSDecimalNumber * _Nullable subtotal;
		[NullAllowed, Export ("subtotal", ArgumentSemantic.Copy)]
		NSDecimalNumber Subtotal { get; set; }

		// @property (readwrite, copy, nonatomic) NSDecimalNumber * _Nullable shipping;
		[NullAllowed, Export ("shipping", ArgumentSemantic.Copy)]
		NSDecimalNumber Shipping { get; set; }

		// @property (readwrite, copy, nonatomic) NSDecimalNumber * _Nullable tax;
		[NullAllowed, Export ("tax", ArgumentSemantic.Copy)]
		NSDecimalNumber Tax { get; set; }
	}

	// @interface PayPalItem : NSObject <NSCopying>
	[BaseType (typeof(NSObject))]
	interface PayPalItem : INSCopying
	{
		// +(PayPalItem * _Nonnull)itemWithName:(NSString * _Nonnull)name withQuantity:(NSUInteger)quantity withPrice:(NSDecimalNumber * _Nonnull)price withCurrency:(NSString * _Nonnull)currency withSku:(NSString * _Nullable)sku;
		[Static]
		[Export ("itemWithName:withQuantity:withPrice:withCurrency:withSku:")]
		PayPalItem ItemWithName (string name, nuint quantity, NSDecimalNumber price, string currency, [NullAllowed] string sku);

		// +(NSDecimalNumber * _Nonnull)totalPriceForItems:(NSArray * _Nonnull)items;
		[Static]
		[Export ("totalPriceForItems:")]
		//[Verify (StronglyTypedNSArray)]
		NSDecimalNumber TotalPriceForItems (NSObject[] items);

		// @property (readwrite, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; set; }

		// @property (assign, readwrite, nonatomic) NSUInteger quantity;
		[Export ("quantity")]
		nuint Quantity { get; set; }

		// @property (readwrite, copy, nonatomic) NSDecimalNumber * _Nonnull price;
		[Export ("price", ArgumentSemantic.Copy)]
		NSDecimalNumber Price { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nonnull currency;
		[Export ("currency")]
		string Currency { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable sku;
		[NullAllowed, Export ("sku")]
		string Sku { get; set; }
	}

	// @interface PayPalShippingAddress : NSObject <NSCopying>
	[BaseType (typeof(NSObject))]
	interface PayPalShippingAddress : INSCopying
	{
		// +(PayPalShippingAddress * _Nonnull)shippingAddressWithRecipientName:(NSString * _Nonnull)recipientName withLine1:(NSString * _Nonnull)line1 withLine2:(NSString * _Nullable)line2 withCity:(NSString * _Nonnull)city withState:(NSString * _Nullable)state withPostalCode:(NSString * _Nullable)postalCode withCountryCode:(NSString * _Nonnull)countryCode;
		[Static]
		[Export ("shippingAddressWithRecipientName:withLine1:withLine2:withCity:withState:withPostalCode:withCountryCode:")]
		PayPalShippingAddress ShippingAddressWithRecipientName (string recipientName, string line1, [NullAllowed] string line2, string city, [NullAllowed] string state, [NullAllowed] string postalCode, string countryCode);

		// @property (readwrite, copy, nonatomic) NSString * _Nonnull recipientName;
		[Export ("recipientName")]
		string RecipientName { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nonnull line1;
		[Export ("line1")]
		string Line1 { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable line2;
		[NullAllowed, Export ("line2")]
		string Line2 { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nonnull city;
		[Export ("city")]
		string City { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable state;
		[NullAllowed, Export ("state")]
		string State { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable postalCode;
		[NullAllowed, Export ("postalCode")]
		string PostalCode { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nonnull countryCode;
		[Export ("countryCode")]
		string CountryCode { get; set; }
	}

	// @interface PayPalPayment : NSObject <NSCopying>
	[BaseType (typeof(NSObject))]
	interface PayPalPayment : INSCopying
	{
		// +(PayPalPayment * _Nonnull)paymentWithAmount:(NSDecimalNumber * _Nonnull)amount currencyCode:(NSString * _Nonnull)currencyCode shortDescription:(NSString * _Nonnull)shortDescription intent:(PayPalPaymentIntent)intent;
		[Static]
		[Export ("paymentWithAmount:currencyCode:shortDescription:intent:")]
		PayPalPayment PaymentWithAmount (NSDecimalNumber amount, string currencyCode, string shortDescription, PayPalPaymentIntent intent);

		// @property (readwrite, copy, nonatomic) NSString * _Nonnull currencyCode;
		[Export ("currencyCode")]
		string CurrencyCode { get; set; }

		// @property (readwrite, copy, nonatomic) NSDecimalNumber * _Nonnull amount;
		[Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nonnull shortDescription;
		[Export ("shortDescription")]
		string ShortDescription { get; set; }

		// @property (assign, readwrite, nonatomic) PayPalPaymentIntent intent;
		[Export ("intent", ArgumentSemantic.Assign)]
		PayPalPaymentIntent Intent { get; set; }

		// @property (readwrite, copy, nonatomic) PayPalPaymentDetails * _Nullable paymentDetails;
		[NullAllowed, Export ("paymentDetails", ArgumentSemantic.Copy)]
		PayPalPaymentDetails PaymentDetails { get; set; }

		// @property (readwrite, copy, nonatomic) NSArray * _Nullable items;
		[NullAllowed, Export ("items", ArgumentSemantic.Copy)]
		//[Verify (StronglyTypedNSArray)]
		NSObject[] Items { get; set; }

		// @property (readwrite, copy, nonatomic) PayPalShippingAddress * _Nullable shippingAddress;
		[NullAllowed, Export ("shippingAddress", ArgumentSemantic.Copy)]
		PayPalShippingAddress ShippingAddress { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable invoiceNumber;
		[NullAllowed, Export ("invoiceNumber")]
		string InvoiceNumber { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable custom;
		[NullAllowed, Export ("custom")]
		string Custom { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable softDescriptor;
		[NullAllowed, Export ("softDescriptor")]
		string SoftDescriptor { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable bnCode;
		[NullAllowed, Export ("bnCode")]
		string BnCode { get; set; }

		// @property (readonly, assign, nonatomic) BOOL processable;
		[Export ("processable")]
		bool Processable { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull localizedAmountForDisplay;
		[Export ("localizedAmountForDisplay")]
		string LocalizedAmountForDisplay { get; }

		// @property (readonly, copy, nonatomic) NSDictionary * _Nonnull confirmation;
		[Export ("confirmation", ArgumentSemantic.Copy)]
		NSDictionary Confirmation { get; }
	}

	// typedef void (^PayPalPaymentDelegateCompletionBlock)();
	delegate void PayPalPaymentDelegateCompletionBlock ();

	// @protocol PayPalPaymentDelegate <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface PayPalPaymentDelegate
	{
		// @required -(void)payPalPaymentDidCancel:(PayPalPaymentViewController * _Nonnull)paymentViewController;
		[Abstract]
		[Export ("payPalPaymentDidCancel:")]
		void PayPalPaymentDidCancel (PayPalPaymentViewController paymentViewController);

		// @required -(void)payPalPaymentViewController:(PayPalPaymentViewController * _Nonnull)paymentViewController didCompletePayment:(PayPalPayment * _Nonnull)completedPayment;
		[Abstract]
		[Export ("payPalPaymentViewController:didCompletePayment:")]
		void PayPalPaymentViewController (PayPalPaymentViewController paymentViewController, PayPalPayment completedPayment);

		// @optional -(void)payPalPaymentViewController:(PayPalPaymentViewController * _Nonnull)paymentViewController willCompletePayment:(PayPalPayment * _Nonnull)completedPayment completionBlock:(PayPalPaymentDelegateCompletionBlock _Nonnull)completionBlock;
		[Export ("payPalPaymentViewController:willCompletePayment:completionBlock:")]
		void PayPalPaymentViewController (PayPalPaymentViewController paymentViewController, PayPalPayment completedPayment, PayPalPaymentDelegateCompletionBlock completionBlock);
	}

	// @interface PayPalPaymentViewController : UINavigationController
	[BaseType (typeof(UINavigationController))]
	interface PayPalPaymentViewController
	{
		// -(instancetype _Nullable)initWithPayment:(PayPalPayment * _Nonnull)payment configuration:(PayPalConfiguration * _Nullable)configuration delegate:(id<PayPalPaymentDelegate> _Nonnull)delegate;
		[Export ("initWithPayment:configuration:delegate:")]
		IntPtr Constructor (PayPalPayment payment, [NullAllowed] PayPalConfiguration configuration, PayPalPaymentDelegate @delegate);

		[Wrap ("WeakPaymentDelegate")]
		[NullAllowed]
		PayPalPaymentDelegate PaymentDelegate { get; }

		// @property (readonly, nonatomic, weak) id<PayPalPaymentDelegate> _Nullable paymentDelegate;
		[NullAllowed, Export ("paymentDelegate", ArgumentSemantic.Weak)]
		NSObject WeakPaymentDelegate { get; }

		// @property (readonly, assign, nonatomic) PayPalPaymentViewControllerState state;
		[Export ("state", ArgumentSemantic.Assign)]
		PayPalPaymentViewControllerState State { get; }
	}

	// typedef void (^PayPalProfileSharingDelegateCompletionBlock)();
	delegate void PayPalProfileSharingDelegateCompletionBlock ();

	// @protocol PayPalProfileSharingDelegate <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface PayPalProfileSharingDelegate
	{
		// @required -(void)userDidCancelPayPalProfileSharingViewController:(PayPalProfileSharingViewController * _Nonnull)profileSharingViewController;
		[Abstract]
		[Export ("userDidCancelPayPalProfileSharingViewController:")]
		void UserDidCancelPayPalProfileSharingViewController (PayPalProfileSharingViewController profileSharingViewController);

		// @required -(void)payPalProfileSharingViewController:(PayPalProfileSharingViewController * _Nonnull)profileSharingViewController userDidLogInWithAuthorization:(NSDictionary * _Nonnull)profileSharingAuthorization;
		[Abstract]
		[Export ("payPalProfileSharingViewController:userDidLogInWithAuthorization:")]
		void PayPalProfileSharingViewController (PayPalProfileSharingViewController profileSharingViewController, NSDictionary profileSharingAuthorization);

		// @optional -(void)payPalProfileSharingViewController:(PayPalProfileSharingViewController * _Nonnull)profileSharingViewController userWillLogInWithAuthorization:(NSDictionary * _Nonnull)profileSharingAuthorization completionBlock:(PayPalProfileSharingDelegateCompletionBlock _Nonnull)completionBlock;
		[Export ("payPalProfileSharingViewController:userWillLogInWithAuthorization:completionBlock:")]
		void PayPalProfileSharingViewController (PayPalProfileSharingViewController profileSharingViewController, NSDictionary profileSharingAuthorization, PayPalProfileSharingDelegateCompletionBlock completionBlock);
	}

	// @interface PayPalProfileSharingViewController : UINavigationController
	[BaseType (typeof(UINavigationController))]
	interface PayPalProfileSharingViewController
	{
		// -(instancetype _Nullable)initWithScopeValues:(NSSet * _Nonnull)scopeValues configuration:(PayPalConfiguration * _Nonnull)configuration delegate:(id<PayPalProfileSharingDelegate> _Nullable)delegate;
		[Export ("initWithScopeValues:configuration:delegate:")]
		IntPtr Constructor (NSSet scopeValues, PayPalConfiguration configuration, [NullAllowed] PayPalProfileSharingDelegate @delegate);

		[Wrap ("WeakProfileSharingDelegate")]
		[NullAllowed]
		PayPalProfileSharingDelegate ProfileSharingDelegate { get; }

		// @property (readonly, nonatomic, weak) id<PayPalProfileSharingDelegate> _Nullable profileSharingDelegate;
		[NullAllowed, Export ("profileSharingDelegate", ArgumentSemantic.Weak)]
		NSObject WeakProfileSharingDelegate { get; }
	}

	//[Static]
	//[Verify (ConstantsInterfaceAssociation)]
	partial interface Constants
	{
		[Static]
		// extern NSString *const _Nonnull PayPalEnvironmentProduction;
		[Field ("PayPalEnvironmentProduction", "__Internal")]
		NSString PayPalEnvironmentProduction { get; }

		[Static]
		// extern NSString *const _Nonnull PayPalEnvironmentSandbox;
		[Field ("PayPalEnvironmentSandbox", "__Internal")]
		NSString PayPalEnvironmentSandbox { get; }

		[Static]
		// extern NSString *const _Nonnull PayPalEnvironmentNoNetwork;
		[Field ("PayPalEnvironmentNoNetwork", "__Internal")]
		NSString PayPalEnvironmentNoNetwork { get; }
	}

	// @interface PayPalMobile : NSObject
	[BaseType (typeof(NSObject))]
	interface PayPalMobile
	{
		// +(void)initializeWithClientIdsForEnvironments:(NSDictionary * _Nonnull)clientIdsForEnvironments;
		[Static]
		[Export ("initializeWithClientIdsForEnvironments:")]
		void InitializeWithClientIdsForEnvironments (NSDictionary clientIdsForEnvironments);

		// +(void)preconnectWithEnvironment:(NSString * _Nonnull)environment;
		[Static]
		[Export ("preconnectWithEnvironment:")]
		void PreconnectWithEnvironment (string environment);

		// +(NSString * _Nonnull)clientMetadataID;
		[Static]
		[Export ("clientMetadataID")]
		//[Verify (MethodToProperty)]
		string ClientMetadataID { get; }

		// +(NSString * _Nonnull)applicationCorrelationIDForEnvironment:(NSString * _Nonnull)environment __attribute__((deprecated("Use clientMetadataID instead.")));
		[Static]
		[Export ("applicationCorrelationIDForEnvironment:")]
		string ApplicationCorrelationIDForEnvironment (string environment);

		// +(void)clearAllUserData;
		[Static]
		[Export ("clearAllUserData")]
		void ClearAllUserData ();

		// +(NSString * _Nonnull)libraryVersion;
		[Static]
		[Export ("libraryVersion")]
		//[Verify (MethodToProperty)]
		string LibraryVersion { get; }
	}

	// @interface CardIOCreditCardInfo : NSObject <NSCopying>
	[BaseType (typeof(NSObject))]
	interface CardIOCreditCardInfo : INSCopying
	{
		// @property (readwrite, copy, nonatomic) NSString * cardNumber;
		[Export ("cardNumber")]
		string CardNumber { get; set; }

		// @property (readonly, copy, nonatomic) NSString * redactedCardNumber;
		[Export ("redactedCardNumber")]
		string RedactedCardNumber { get; }

		// @property (assign, readwrite, nonatomic) NSUInteger expiryMonth;
		[Export ("expiryMonth")]
		nuint ExpiryMonth { get; set; }

		// @property (assign, readwrite, nonatomic) NSUInteger expiryYear;
		[Export ("expiryYear")]
		nuint ExpiryYear { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * cvv;
		[Export ("cvv")]
		string Cvv { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * postalCode;
		[Export ("postalCode")]
		string PostalCode { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL scanned;
		[Export ("scanned")]
		bool Scanned { get; set; }

		// @property (readwrite, nonatomic, strong) UIImage * cardImage;
		[Export ("cardImage", ArgumentSemantic.Strong)]
		UIImage CardImage { get; set; }

		// @property (readonly, assign, nonatomic) CardIOCreditCardType cardType;
		[Export ("cardType", ArgumentSemantic.Assign)]
		CardIOCreditCardType CardType { get; }

		// +(NSString *)displayStringForCardType:(CardIOCreditCardType)cardType usingLanguageOrLocale:(NSString *)languageOrLocale;
		[Static]
		[Export ("displayStringForCardType:usingLanguageOrLocale:")]
		string DisplayStringForCardType (CardIOCreditCardType cardType, string languageOrLocale);

		// +(UIImage *)logoForCardType:(CardIOCreditCardType)cardType;
		[Static]
		[Export ("logoForCardType:")]
		UIImage LogoForCardType (CardIOCreditCardType cardType);
	}

	// @protocol CardIOViewDelegate <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface CardIOViewDelegate
	{
		// @required -(void)cardIOView:(CardIOView *)cardIOView didScanCard:(CardIOCreditCardInfo *)cardInfo;
		[Abstract]
		[Export ("cardIOView:didScanCard:")]
		void DidScanCard (CardIOView cardIOView, CardIOCreditCardInfo cardInfo);
	}

	// @interface CardIOView : UIView
	[BaseType (typeof(UIView))]
	interface CardIOView
	{
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		CardIOViewDelegate Delegate { get; set; }

		// @property (readwrite, nonatomic, weak) id<CardIOViewDelegate> _Nullable delegate;
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * languageOrLocale;
		[Export ("languageOrLocale")]
		string LanguageOrLocale { get; set; }

		// @property (readwrite, retain, nonatomic) UIColor * guideColor;
		[Export ("guideColor", ArgumentSemantic.Retain)]
		UIColor GuideColor { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL useCardIOLogo;
		[Export ("useCardIOLogo")]
		bool UseCardIOLogo { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL hideCardIOLogo;
		[Export ("hideCardIOLogo")]
		bool HideCardIOLogo { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL allowFreelyRotatingCardGuide;
		[Export ("allowFreelyRotatingCardGuide")]
		bool AllowFreelyRotatingCardGuide { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * scanInstructions;
		[Export ("scanInstructions")]
		string ScanInstructions { get; set; }

		// @property (readwrite, retain, nonatomic) UIView * scanOverlayView;
		[Export ("scanOverlayView", ArgumentSemantic.Retain)]
		UIView ScanOverlayView { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL scanExpiry;
		[Export ("scanExpiry")]
		bool ScanExpiry { get; set; }

		// @property (assign, readwrite, nonatomic) CardIODetectionMode detectionMode;
		[Export ("detectionMode", ArgumentSemantic.Assign)]
		CardIODetectionMode DetectionMode { get; set; }

		// @property (assign, readwrite, nonatomic) CGFloat scannedImageDuration;
		[Export ("scannedImageDuration")]
		nfloat ScannedImageDuration { get; set; }

		// @property (readonly, assign, nonatomic) CGRect cameraPreviewFrame;
		[Export ("cameraPreviewFrame", ArgumentSemantic.Assign)]
		CGRect CameraPreviewFrame { get; }
	}

	/*[Static]
	[Verify (ConstantsInterfaceAssociation)]
	partial interface Constants
	{
		// extern NSString *const CardIOScanningOrientationDidChangeNotification;
		[Field ("CardIOScanningOrientationDidChangeNotification", "__Internal")]
		NSString CardIOScanningOrientationDidChangeNotification { get; }

		// extern NSString *const CardIOCurrentScanningOrientation;
		[Field ("CardIOCurrentScanningOrientation", "__Internal")]
		NSString CardIOCurrentScanningOrientation { get; }

		// extern NSString *const CardIOScanningOrientationAnimationDuration;
		[Field ("CardIOScanningOrientationAnimationDuration", "__Internal")]
		NSString CardIOScanningOrientationAnimationDuration { get; }
	}*/

	// @protocol CardIOPaymentViewControllerDelegate <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface CardIOPaymentViewControllerDelegate
	{
		// @required -(void)userDidCancelPaymentViewController:(CardIOPaymentViewController *)paymentViewController;
		[Abstract]
		[Export ("userDidCancelPaymentViewController:")]
		void UserDidCancelPaymentViewController (CardIOPaymentViewController paymentViewController);

		// @required -(void)userDidProvideCreditCardInfo:(CardIOCreditCardInfo *)cardInfo inPaymentViewController:(CardIOPaymentViewController *)paymentViewController;
		[Abstract]
		[Export ("userDidProvideCreditCardInfo:inPaymentViewController:")]
		void UserDidProvideCreditCardInfo (CardIOCreditCardInfo cardInfo, CardIOPaymentViewController paymentViewController);
	}

	// @interface CardIOPaymentViewController : UINavigationController
	[BaseType (typeof(UINavigationController))]
	interface CardIOPaymentViewController
	{
		// -(id)initWithPaymentDelegate:(id<CardIOPaymentViewControllerDelegate>)aDelegate;
		[Export ("initWithPaymentDelegate:")]
		IntPtr Constructor (CardIOPaymentViewControllerDelegate aDelegate);

		// -(id)initWithPaymentDelegate:(id<CardIOPaymentViewControllerDelegate>)aDelegate scanningEnabled:(BOOL)scanningEnabled;
		[Export ("initWithPaymentDelegate:scanningEnabled:")]
		IntPtr Constructor (CardIOPaymentViewControllerDelegate aDelegate, bool scanningEnabled);

		// @property (readwrite, copy, nonatomic) NSString * languageOrLocale;
		[Export ("languageOrLocale")]
		string LanguageOrLocale { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL keepStatusBarStyle;
		[Export ("keepStatusBarStyle")]
		bool KeepStatusBarStyle { get; set; }

		// @property (assign, readwrite, nonatomic) UIBarStyle navigationBarStyle;
		[Export ("navigationBarStyle", ArgumentSemantic.Assign)]
		UIBarStyle NavigationBarStyle { get; set; }

		// @property (readwrite, retain, nonatomic) UIColor * navigationBarTintColor;
		[Export ("navigationBarTintColor", ArgumentSemantic.Retain)]
		UIColor NavigationBarTintColor { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL disableBlurWhenBackgrounding;
		[Export ("disableBlurWhenBackgrounding")]
		bool DisableBlurWhenBackgrounding { get; set; }

		// @property (readwrite, retain, nonatomic) UIColor * guideColor;
		[Export ("guideColor", ArgumentSemantic.Retain)]
		UIColor GuideColor { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL suppressScanConfirmation;
		[Export ("suppressScanConfirmation")]
		bool SuppressScanConfirmation { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL suppressScannedCardImage;
		[Export ("suppressScannedCardImage")]
		bool SuppressScannedCardImage { get; set; }

		// @property (assign, readwrite, nonatomic) CGFloat scannedImageDuration;
		[Export ("scannedImageDuration")]
		nfloat ScannedImageDuration { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL maskManualEntryDigits;
		[Export ("maskManualEntryDigits")]
		bool MaskManualEntryDigits { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * scanInstructions;
		[Export ("scanInstructions")]
		string ScanInstructions { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL hideCardIOLogo;
		[Export ("hideCardIOLogo")]
		bool HideCardIOLogo { get; set; }

		// @property (readwrite, retain, nonatomic) UIView * scanOverlayView;
		[Export ("scanOverlayView", ArgumentSemantic.Retain)]
		UIView ScanOverlayView { get; set; }

		// @property (assign, readwrite, nonatomic) CardIODetectionMode detectionMode;
		[Export ("detectionMode", ArgumentSemantic.Assign)]
		CardIODetectionMode DetectionMode { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL collectExpiry;
		[Export ("collectExpiry")]
		bool CollectExpiry { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL collectCVV;
		[Export ("collectCVV")]
		bool CollectCVV { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL collectPostalCode;
		[Export ("collectPostalCode")]
		bool CollectPostalCode { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL scanExpiry;
		[Export ("scanExpiry")]
		bool ScanExpiry { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL useCardIOLogo;
		[Export ("useCardIOLogo")]
		bool UseCardIOLogo { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL allowFreelyRotatingCardGuide;
		[Export ("allowFreelyRotatingCardGuide")]
		bool AllowFreelyRotatingCardGuide { get; set; }

		// @property (assign, readwrite, nonatomic) BOOL disableManualEntryButtons;
		[Export ("disableManualEntryButtons")]
		bool DisableManualEntryButtons { get; set; }

		[Wrap ("WeakPaymentDelegate")]
		[NullAllowed]
		CardIOPaymentViewControllerDelegate PaymentDelegate { get; set; }

		// @property (readwrite, nonatomic, weak) id<CardIOPaymentViewControllerDelegate> _Nullable paymentDelegate;
		[NullAllowed, Export ("paymentDelegate", ArgumentSemantic.Weak)]
		NSObject WeakPaymentDelegate { get; set; }
	}

	[Static]
	//[Verify (ConstantsInterfaceAssociation)]
	partial interface Constants
	{
		// extern NSString *const CardIOScanningOrientationDidChangeNotification;
		[Field ("CardIOScanningOrientationDidChangeNotification", "__Internal")]
		NSString CardIOScanningOrientationDidChangeNotification { get; }

		// extern NSString *const CardIOCurrentScanningOrientation;
		[Field ("CardIOCurrentScanningOrientation", "__Internal")]
		NSString CardIOCurrentScanningOrientation { get; }

		// extern NSString *const CardIOScanningOrientationAnimationDuration;
		[Field ("CardIOScanningOrientationAnimationDuration", "__Internal")]
		NSString CardIOScanningOrientationAnimationDuration { get; }
	}

	// @interface CardIOUtilities : NSObject
	[BaseType (typeof(NSObject))]
	interface CardIOUtilities
	{
		// +(NSString *)libraryVersion;
		[Static]
		[Export ("libraryVersion")]
		//[Verify (MethodToProperty)]
		string LibraryVersion { get; }

		// +(BOOL)canReadCardWithCamera;
		[Static]
		[Export ("canReadCardWithCamera")]
		//[Verify (MethodToProperty)]
		bool CanReadCardWithCamera { get; }

		// +(void)preload;
		[Static]
		[Export ("preload")]
		void Preload ();

		// +(UIImageView *)blurredScreenImageView;
		[Static]
		[Export ("blurredScreenImageView")]
		//[Verify (MethodToProperty)]
		UIImageView BlurredScreenImageView { get; }
	}
}


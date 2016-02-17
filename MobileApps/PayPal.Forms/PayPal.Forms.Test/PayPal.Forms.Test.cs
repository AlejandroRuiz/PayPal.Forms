using System;

using Xamarin.Forms;
using PayPal.Forms.Abstractions;
using Deveel.Math;
using PayPal.Forms.Abstractions.Enum;

namespace PayPal.Forms.Test
{
	public class App : Application
	{
		Button BuyOnethingButton;
		Button BuyManythingsButton;


		public App ()
		{
			BuyOnethingButton = new Button () {
				Text = "Buy One Thing Button"
			};
			BuyOnethingButton.Clicked += BuyOnethingButton_Clicked;

			BuyManythingsButton = new Button () {
				Text = "Buy Many Things Button"
			};
			BuyManythingsButton.Clicked += BuyManythingsButton_Clicked;
				

			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						BuyOnethingButton,
						BuyManythingsButton
					}
				}
			};
		}

		async void BuyManythingsButton_Clicked (object sender, EventArgs e)
		{
			var result = await Forms.CrossPaypalManager.Current.Buy (new PayPalItem[] {
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
		}

		async void BuyOnethingButton_Clicked (object sender, EventArgs e)
		{
			var result = await CrossPaypalManager.Current.Buy (new PayPalItem ("Test Product", new BigDecimal (12.50), "USD"), new BigDecimal (0));
			if (result.Status == PaymentResultStatus.Cancelled) {
				Console.WriteLine ("Cancelled");
			}else if(result.Status == PaymentResultStatus.Error){
				Console.WriteLine (result.ErrorMessage);
			}else if(result.Status == PaymentResultStatus.Successful){
				Console.WriteLine (result.ServerResponse.Response.Id);
			}
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}


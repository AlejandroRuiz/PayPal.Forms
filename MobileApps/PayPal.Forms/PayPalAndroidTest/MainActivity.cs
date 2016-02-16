using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.PayPal.Android;
using Android.Util;
using Org.Json;

namespace PayPalAndroidTest
{
	[Activity (Label = "PayPalAndroidTest", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		PayPalManager MainManager;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			MainManager = new PayPalManager (this);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += Button_Click;
		}

		void Button_Click (object sender, System.EventArgs e)
		{
			MainManager.BuySomething ();
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Android.Content.Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			MainManager.OnActivityResult (requestCode, resultCode, data);
		}

		protected override void OnDestroy ()
		{
			MainManager.Destroy ();
			base.OnDestroy ();
		}
	}
}



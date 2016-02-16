using System;
using PayPal.Forms.Abstractions;
using System.Threading.Tasks;
using Deveel.Math;
using System.Diagnostics;

namespace PayPal.Forms
{
	public class CrossPaypalManager
	{

		static PayPalConfiguration _config;
		public static void Init(PayPalConfiguration config)
		{
			IsInitialized = true;
			_config = config;
		}

		public static bool IsInitialized {
			get;
			private set;
		}

		static Lazy<IPayPalManager> Implementation = new Lazy<IPayPalManager>(() => CreateGameCenterManager(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

		public CrossPaypalManager ()
		{
		}

		/// <summary>
		/// Current paypal manager
		/// </summary>
		public static IPayPalManager Current
		{
			get
			{
				if (!IsInitialized) {
					Debug.WriteLine ("You Must Call PayPal.Forms.CrossPaypalManager.Init() before to use it");
					throw new NotImplementedException ("You Must Call PayPal.Forms.CrossPaypalManager.Init() before to use it");
				}
				var ret = Implementation.Value;
				if (ret == null)
				{
					throw NotImplementedInReferenceAssembly();
				}
				return ret;
			}
		}


		static IPayPalManager CreateGameCenterManager()
		{
			#if PORTABLE
			return null;
			#else
			return new PayPalManagerImplementation(_config);
			#endif
		}

		internal static Exception NotImplementedInReferenceAssembly()
		{
			return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
		}
	}
}


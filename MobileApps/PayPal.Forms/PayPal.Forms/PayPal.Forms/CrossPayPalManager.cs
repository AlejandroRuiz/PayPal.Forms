using System;
using System.Diagnostics;
using PayPal.Forms.Abstractions;

namespace PayPal.Forms
{
    public class CrossPayPalManager
    {

        static PayPalConfiguration _config;
        static Lazy<IPayPalManager> _implementation = new Lazy<IPayPalManager>(() => CreatePayPalManager(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

#if __ANDROID__
        public static void Init(PayPalConfiguration config, global::Android.Content.Context context)
        {
            Context = context;
#else
        public static void Init(PayPalConfiguration config)
        {
#endif
            IsInitialized = true;
            _config = config;
        }

        public static bool IsInitialized
        {
            get;
            private set;
        }

#if __ANDROID__
        public static global::Android.Content.Context Context { get; private set; }
#endif

        /// <summary>
        /// Current paypal manager
        /// </summary>
        public static IPayPalManager Current
        {
            get
            {
                if (!IsInitialized)
                {
                    Debug.WriteLine("You Must Call PayPal.Forms.CrossPaypalManager.Init() before to use it");
                    throw new NotImplementedException("You Must Call PayPal.Forms.CrossPaypalManager.Init() before to use it");
                }
                var ret = _implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }


        static IPayPalManager CreatePayPalManager()
        {
#if PORTABLE
            return null;
#elif __ANDROID__
            return new PayPalManagerImplementation(_config, Context);
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
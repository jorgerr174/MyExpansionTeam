using Android.App;
using Android.Runtime;

namespace MobileApp
{
#if DEBUG                                   // connect to local service on the
    [Application(UsesCleartextTraffic = true)]  // emulator's host for debugging,
#else                                       // access via http://10.0.2.2
    [Application]                               
#endif
    public class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}

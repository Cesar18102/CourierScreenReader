using Android.OS;
using Android.App;
using Android.Runtime;
using Android.Content;
using Android.Support.V7.App;

using Autofac;

using ScreenReaderService.AccessibilityEventProcessors;
using ScreenReaderService.Services;

namespace ScreenReaderService
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderListPageEventProcessor>();

            Intent intentToAccessibility = new Intent(
                this, typeof(ScreenReader)
            );

            StartService(intentToAccessibility);
            StartDelivery();

            DependencyHolder.Dependencies.Resolve<Bot>().Start();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void StartDelivery()
        {
            Intent launchIntent = PackageManager.GetLaunchIntentForPackage("ua.ipost.work");
            launchIntent.SetFlags(ActivityFlags.ClearTop);
            StartActivity(launchIntent);
        }
    }
}
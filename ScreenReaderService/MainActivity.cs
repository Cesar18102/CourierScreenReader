using System;

using Android.OS;
using Android.App;
using Android.Runtime;
using Android.Content;
using Android.Support.V7.App;

using Autofac;

using ScreenReaderService.Telegram;
using ScreenReaderService.Services;
using ScreenReaderService.AccessibilityEventProcessors;

namespace ScreenReaderService
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private BotService BotService = DependencyHolder.Dependencies.Resolve<BotService>();
        private TelegramNotifier TelegramNotifier = DependencyHolder.Dependencies.Resolve<TelegramNotifier>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);

                ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderListPageEventProcessor>();

                Intent intentToAccessibility = new Intent(
                    this, typeof(ScreenReader)
                );

                BotService.StateService.StateInfo.Paused = true;
                BotService.StateService.Save();

                StartService(intentToAccessibility);
                StartDelivery();

                DependencyHolder.Dependencies.Resolve<Bot>().Start();
            }
            catch(Exception e)
            {
                TelegramNotifier.NotifyMessage(
                    $"@{BotService.CredentialsService.Credentials.TelegramUsername}, your bot is stuck due to:\n" + 
                    GetExceptionInfo(e)
                ).GetAwaiter().GetResult();
            }
        }

        private string GetExceptionInfo(Exception e)
        {
            if (e == null)
                return "";

            return $"{e.Message}\n\n{e.StackTrace}\n\n**********************\n\n{GetExceptionInfo(e)}";
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
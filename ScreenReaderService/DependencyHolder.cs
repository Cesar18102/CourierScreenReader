using System.Text;
using System.Security.Cryptography;

using Autofac;

using ScreenReaderService.Util;
using ScreenReaderService.Telegram;
using ScreenReaderService.Services;
using ScreenReaderService.Data.Services;
using ScreenReaderService.AccessibilityEventProcessors;

namespace ScreenReaderService
{
    public static class DependencyHolder
    {
        private static IContainer dependencies = null;
        public static IContainer Dependencies
        {
            get
            {
                if (dependencies == null)
                    dependencies = Build();
                return dependencies;
            }
        }

        private static IContainer Build()
        {
            ContainerBuilder builder = new ContainerBuilder();

            TypedParameter hasher = new TypedParameter(typeof(HashAlgorithm), SHA256.Create());
            TypedParameter encoding = new TypedParameter(typeof(Encoding), Encoding.UTF8);

            builder.RegisterType<HashingService>()
                   .UsingConstructor(typeof(HashAlgorithm), typeof(Encoding))
                   .WithParameters(new TypedParameter[] { hasher, encoding })
                   .AsSelf().SingleInstance();
            builder.RegisterType<SaltService>().AsSelf().SingleInstance();
            builder.RegisterType<AuthService>().AsSelf().SingleInstance();

            builder.RegisterType<StatusCodeHandlerService>().AsSelf().SingleInstance();

            builder.RegisterType<Bot>().AsSelf().SingleInstance();
            builder.RegisterType<BotService>().AsSelf().SingleInstance();

            builder.RegisterType<FileManager>().AsSelf().SingleInstance();

            builder.RegisterType<FilterService>().AsSelf().SingleInstance();
            builder.RegisterType<ConstraintsConfigService>().AsSelf().SingleInstance();
            builder.RegisterType<CredentialsConfigService>().AsSelf().SingleInstance();
            builder.RegisterType<TextPreparationService>().AsSelf().SingleInstance();
            builder.RegisterType<SessionService>().AsSelf().SingleInstance();
            builder.RegisterType<StateService>().AsSelf().SingleInstance();
            builder.RegisterType<WorkService>().AsSelf().SingleInstance();

            builder.RegisterType<OrderListPageEventProcessor>().AsSelf().SingleInstance();
            builder.RegisterType<OrderPageEventProcessor>().AsSelf().SingleInstance();
            builder.RegisterType<TakenOrderPageEventProcessor>().AsSelf().SingleInstance();
            builder.RegisterType<LoginPageEventProcessor>().AsSelf().SingleInstance();

            builder.RegisterType<TelegramNotifier>().AsSelf().SingleInstance();

            return builder.Build();
        }
    }
}
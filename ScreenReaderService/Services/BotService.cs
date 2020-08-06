using ScreenReaderService.Data.Services;

using Autofac;

namespace ScreenReaderService.Services
{
    public class BotService
    {
        public WorkService WorkService = DependencyHolder.Dependencies.Resolve<WorkService>();
        public StateService StateService = DependencyHolder.Dependencies.Resolve<StateService>();
        public FilterService FilterService = DependencyHolder.Dependencies.Resolve<FilterService>();
        public ConstraintsConfigService ConstraintsService = DependencyHolder.Dependencies.Resolve<ConstraintsConfigService>();
        public CredentialsConfigService CredentialsService = DependencyHolder.Dependencies.Resolve<CredentialsConfigService>();
        public TextPreparationService TextPreparationService = DependencyHolder.Dependencies.Resolve<TextPreparationService>();
    }
}
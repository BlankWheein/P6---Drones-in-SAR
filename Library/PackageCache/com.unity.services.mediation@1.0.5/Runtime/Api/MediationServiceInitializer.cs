using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Telemetry.Internal;

namespace Unity.Services.Mediation
{
    class MediationServiceInitializer : IInitializablePackage
    {
        internal const string keyGameId = "com.unity.ads.game-id";
        const string k_NoGameIdWasSet = "No gameId was set for the mediation service. Please make sure your project is linked to the dashboard when you build your application.";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            CoreRegistry.Instance.RegisterPackage(new MediationServiceInitializer())
                .DependsOn<IInstallationId>()
                .DependsOn<IProjectConfiguration>()
                .DependsOn<IMetricsFactory>();
        }

        public async Task Initialize(CoreRegistry registry)
        {
            var installationId = registry.GetServiceComponent<IInstallationId>();
            var projectConfig  = registry.GetServiceComponent<IProjectConfiguration>();

            await Initialize(installationId, projectConfig);
        }

        internal async Task Initialize(IInstallationId installationId, IProjectConfiguration projectConfiguration)
        {
            var installId = installationId.GetOrCreateIdentifier();
            var gameId    = projectConfiguration.GetString(keyGameId);

#if UNITY_ANDROID || UNITY_IOS
            if (!Application.isEditor && string.IsNullOrEmpty(gameId))
            {
                MediationLogger.LogError(k_NoGameIdWasSet);
            }
#endif
            await MediationService.Initialize(gameId, installId);
        }
    }
}

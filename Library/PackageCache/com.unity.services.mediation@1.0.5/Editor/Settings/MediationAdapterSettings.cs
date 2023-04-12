using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Mediation.Adapters.Editor;
using Unity.Services.Mediation.Dashboard.Editor;
using UnityEditor;
using UnityEditor.Advertisements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    class MediationAdapterSettings : EditorWindow
    {
        const string k_DashboardUrl     = @"https://dashboard.unity3d.com/monetization";
        const string k_DocumentationUrl = @"https://docs.unity3d.com/2020.2/Documentation/Manual/";
        const string k_Install          = "Install";
        const string k_Uninstall        = "Uninstall";
        const string k_ApiRangeTooltip  = "This adapter supports API versions: \n" + "Android: {0} to {1} \n" + "iOS: {2} to {3}";
        const string k_SourceConfigUrl  = @"https://dashboard.unity3d.com/organizations/{0}/monetization/mediation/ad-sources";
        const string k_DisableDexingArtifactTransformKey = "DisableDexingArtifactTransform";
        const string k_ForceDynamicLinkingKey = "ForceDynamicLinking";
        const string k_DeterministicBuildsKey = "DeterministicBuilds";
        const string k_ForceDependencyUpdate = "ForceDependencyUpdate";

        const string k_SettingsStyle       = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/SettingsStyle.uss";

        const string k_SettingsTemplate    = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/SettingsTemplate.uxml";
        const string k_AdapterTemplate     = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdapterTemplate.uxml";

#if UNITY_2020_1_OR_NEWER
        const string k_ServiceBaseStyle    = @"StyleSheets/ServicesWindow/ServicesProjectSettingsCommon.uss";
        static readonly string k_SkinStyle = $@"StyleSheets/ServicesWindow/ServicesProjectSettings{(EditorGUIUtility.isProSkin ? "Dark" : "Light")}.uss";
#else
        const string k_ServiceBaseStyle    = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/2019/BaseStyle.uss";
        static readonly string k_SkinStyle = $@"Packages/com.unity.services.mediation/Editor/Settings/Layout/2019/SkinStyle{(EditorGUIUtility.isProSkin ? "Dark" : "Light")}.uss";
#endif

        static Dictionary<string, AdapterInfo> s_AdapterInfos;
        static Dictionary<string, Toggle> s_AdapterSelectToggle;
        static Dictionary<string, VisualElement> s_AdapterInstalledInfo;
        static Dictionary<string, VisualElement> s_AdapterUninstalledInfo;
        static Dictionary<string, VisualElement> s_AdapterInstalledIcon;
        static Dictionary<string, VisualElement> s_AdapterUnavailableConfigInfo;
        static Dictionary<string, VisualElement> s_AdapterConfiguredInfo;
        static Dictionary<string, VisualElement> s_AdapterUnconfiguredInfo;
        static Dictionary<string, VisualElement> s_AdapterConfiguredIcon;
        static Dictionary<string, Button> s_AdapterInstallButton;
        static List<IAdapterSettings> s_AdapterSettings;
        static List<string> s_configuredAdNetworks;
        static bool s_Initialized;
        static bool s_SettingsChanged;
        static ReloadableUserSetting<bool> s_ForceDynamicLinkingSetting = new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
            MediationUserSettingsKeys.forceDynamicLinkingKey, true);
        static ReloadableUserSetting<bool> s_DisableDexingArtifactTransform = new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
            MediationUserSettingsKeys.disableDexingArtifactTransform, true);
        static ReloadableUserSetting<bool> s_DeterministicBuildsSetting = new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
            MediationUserSettingsKeys.deterministicBuildsKey, true);
        static ReloadableUserSetting<bool> s_ForceDependencyUpdateSetting = new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
            MediationUserSettingsKeys.forceDependencyUpdateKey, false);

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            if (s_Initialized) return;
            s_Initialized = true;

            var adapters       = MediationSdkInfo.GetAllAdapters();
            s_AdapterInfos                    = adapters.ToDictionary(info => info.Identifier);
            s_AdapterSettings                 = ConfigureAdapterSettings(adapters);
            s_AdapterSelectToggle             = new Dictionary<string, Toggle>();
            s_AdapterInstalledInfo            = new Dictionary<string, VisualElement>();
            s_AdapterUninstalledInfo          = new Dictionary<string, VisualElement>();
            s_AdapterInstalledIcon            = new Dictionary<string, VisualElement>();
            s_AdapterUnavailableConfigInfo    = new Dictionary<string, VisualElement>();
            s_AdapterConfiguredInfo           = new Dictionary<string, VisualElement>();
            s_AdapterUnconfiguredInfo         = new Dictionary<string, VisualElement>();
            s_AdapterConfiguredIcon           = new Dictionary<string, VisualElement>();
            s_AdapterInstallButton            = new Dictionary<string, Button>();
            MediationSdkInfo.AdaptersChanged += Refresh;
            DashboardClient.GetConfiguredAdNetworksAsync(RefreshConfiguredAdNetworks);
        }

        /// <summary>
        /// Refreshes the list of available adapters, installed adapters and installed versions
        /// Called on MediationSDKInfo.AdaptersChanged
        /// </summary>
        static void Refresh()
        {
            var installedAdapters = MediationSdkInfo.GetInstalledAdapters();
            var changed = false;

            foreach (var adapterSetting in s_AdapterSettings)
            {
                var adapterInfo = installedAdapters
                    .FirstOrDefault(info => info.Identifier == adapterSetting.AdapterId);
                var isInstalled = adapterInfo != null;

                if (isInstalled)
                {
                    if (adapterInfo.InstalledVersion.Identifier != adapterSetting.InstalledVersion.value)
                    {
                        adapterSetting.InstalledVersion.value = adapterInfo.InstalledVersion.Identifier;
                        changed = true;
                    }
                }
                else
                {
                    if (adapterSetting.InstalledVersion.value != "")
                    {
                        changed = true;
                        adapterSetting.InstalledVersion.value = "";
                    }
                }
            }

            if (changed)
            {
                MediationSettingsProvider.instance.Save();
            }

            RefreshAdaptersData();
            RefreshAllInstallButton();
        }

        [MenuItem("Services/" + MediationServiceIdentifier.k_PackageDisplayName + "/Configure", priority = 100)]
        public static void ShowWindow()
        {
            EditorGameServiceAnalyticsSender.SendTopMenuConfigureEvent();
            SettingsService.OpenProjectSettings($"Project/Services/{MediationServiceIdentifier.k_PackageDisplayName}");
        }

        public static List<IAdapterSettings> ConfigureAdapterSettings(IEnumerable<AdapterInfo> adapters)
        {
            var adaptersIds = adapters.Select(info => info.Identifier).ToList();
            var adapterSettings = CreateAdapterSettings(FindAdaptersWithDefinedSettingsClass());

            ConfigureAdapterSettingsWithGenericSettings(adapterSettings, adaptersIds);
            SortAdaptersByOriginalOrder(adapterSettings, adaptersIds);

            return adapterSettings;
        }

        static IEnumerable<Type> FindAdaptersWithDefinedSettingsClass()
        {
            return typeof(MediationAdapterSettings).Assembly.GetTypes()
                .Where(type =>
                    typeof(IAdapterSettings).IsAssignableFrom(type) &&
                    !type.IsAbstract &&
                    !typeof(GenericAdapterSettings).IsAssignableFrom(type))
                .ToList();
        }

        static List<IAdapterSettings> CreateAdapterSettings(IEnumerable<Type> definedTypes)
        {
            return definedTypes
                .Select(type => (IAdapterSettings)Activator.CreateInstance(type))
                .ToList();
        }

        static void ConfigureAdapterSettingsWithGenericSettings(List<IAdapterSettings> adaptersSettings, IEnumerable<string> adaptersIds)
        {
            var adapterIdsWithGenericSettings = adaptersIds
                .Where(id => adaptersSettings.All(settings => settings.AdapterId != id)).ToList();

            adaptersSettings.AddRange(adapterIdsWithGenericSettings.Select(id => new GenericAdapterSettings(id)));
        }

        static void SortAdaptersByOriginalOrder(List<IAdapterSettings> adapters, IList<string> adaptersIds)
        {
            adapters.Sort((settings1, settings2) =>
            {
                var i1 = adaptersIds.IndexOf(settings1.AdapterId);
                var i2 = adaptersIds.IndexOf(settings2.AdapterId);
                return i1 - i2;
            });
        }

        public static VisualElement GenerateUIElementUI()
        {
            var rootElement = new VisualElement();
            LoadExternalAssets(rootElement);

            CreateButton(rootElement, "GoToAdUnits", MediationAdUnitsWindow.ShowWindow);
            CreateButton(rootElement, "GoToCodeGenerator", MediationCodeGeneratorWindow.ShowWindow);

            CreateToggle(rootElement, k_ForceDynamicLinkingKey, ForceDynamicLinkingToggleValueChanged,
                s_ForceDynamicLinkingSetting.value);
            CreateToggle(rootElement, k_DisableDexingArtifactTransformKey, DisableDexingArtifactTransformToggleValueChanged,
                s_DisableDexingArtifactTransform.value);
            CreateToggle(rootElement, k_DeterministicBuildsKey, DeterministicBuildsToggleValueChanged,
                s_DeterministicBuildsSetting.value);
            CreateToggle(rootElement, k_ForceDependencyUpdate, ForceDependencyUpdateToggleValueChanged,
                s_ForceDependencyUpdateSetting.value);

            //Clear references to graphic elements as they will be generated here.
            ClearGraphicElementsReferences();
            FillAdaptersList(rootElement);

            RefreshAdaptersData();
            RefreshAllInstallButton();
#if !ENABLE_EDITOR_GAME_SERVICES
            MediationEditorService.RefreshGameId();
#endif
            CreateTextElement(rootElement, "android-game-id", AdvertisementSettings.GetGameId(RuntimePlatform.Android));
            CreateTextElement(rootElement, "ios-game-id", AdvertisementSettings.GetGameId(RuntimePlatform.IPhonePlayer));
            return rootElement;
        }

        static void LoadExternalAssets(VisualElement rootElement)
        {
            var settingsTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_SettingsTemplate);

            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(k_SettingsStyle);
            rootElement.styleSheets.Add(stylesheet);

            stylesheet = EditorGUIUtility.Load(k_ServiceBaseStyle) as StyleSheet;
            rootElement.styleSheets.Add(stylesheet);

            stylesheet = EditorGUIUtility.Load(k_SkinStyle) as StyleSheet;
            rootElement.styleSheets.Add(stylesheet);

            settingsTemplate.CloneTree(rootElement);
        }

        static void ClearGraphicElementsReferences()
        {
            s_AdapterSelectToggle.Clear();
            s_AdapterInstalledInfo.Clear();
            s_AdapterUninstalledInfo.Clear();
            s_AdapterInstalledIcon.Clear();
            s_AdapterUnavailableConfigInfo.Clear();
            s_AdapterConfiguredInfo.Clear();
            s_AdapterUnconfiguredInfo.Clear();
            s_AdapterConfiguredIcon.Clear();
            s_AdapterInstallButton.Clear();
        }

        static void FillAdaptersList(VisualElement rootElement)
        {
           var adapterListRoot = rootElement.Q<VisualElement>("AdapterList");

            var isEven = true;
            foreach (var adapterSetting in s_AdapterSettings)
            {
                adapterListRoot.Add(FillSingleAdapter(adapterSetting, isEven));
                isEven = !isEven;
            }
        }

        static VisualElement FillSingleAdapter(IAdapterSettings adapterSetting, bool isEven)
        {
            var adapterTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdapterTemplate);
            var adapterInfo = s_AdapterInfos[adapterSetting.AdapterId];
            var adapter = new VisualElement();

            adapterTemplate.CloneTree(adapter);

            var adapterContainer = adapter.Q<VisualElement>("Adapter");
            var backgroundUssPrefix = EditorGUIUtility.isProSkin ? "dark" : "light";
            var rowUssPrefix = isEven ? "even" : "odd";

            adapterContainer.AddToClassList($"{backgroundUssPrefix}-{rowUssPrefix}-background");

            adapter.Q<TextElement>("AdapterName").text = adapterInfo.DisplayName;
            adapter.Q<Button>("InstallButton").clickable.clickedWithEventInfo += evt =>
                AdapterInstallClicked(s_AdapterInstallButton.FirstOrDefault(pair => pair.Value == evt.target).Key);

            // Keep a reference to the graphic elements we will need to update.
            s_AdapterInstalledInfo.Add(adapterSetting.AdapterId, adapter.Q<VisualElement>("InstalledInfo"));
            s_AdapterUninstalledInfo.Add(adapterSetting.AdapterId, adapter.Q<VisualElement>("UninstalledInfo"));
            s_AdapterInstalledIcon.Add(adapterSetting.AdapterId, adapter.Q<VisualElement>("InstalledNetworkWarning"));
            s_AdapterUnavailableConfigInfo.Add(adapterSetting.AdapterId, adapter.Q<VisualElement>("UnavailableConfigInfo"));
            s_AdapterConfiguredInfo.Add(adapterSetting.AdapterId, adapter.Q<VisualElement>("ConfiguredInfo"));
            s_AdapterUnconfiguredInfo.Add(adapterSetting.AdapterId, adapter.Q<VisualElement>("UnconfiguredInfo"));
            s_AdapterConfiguredIcon.Add(adapterSetting.AdapterId, adapter.Q<VisualElement>("ConfiguredNetworkWarning"));

            adapter.Q<VisualElement>("SourceConfigLinkContainer").AddManipulator(new Clickable(LinkToSourceConfig));

            s_AdapterInstallButton.Add(adapterSetting.AdapterId, adapter.Q<Button>("InstallButton"));

            adapterSetting.OnAdapterSettingsGui("", adapter.Q<VisualElement>("Adapter"));

            return adapter;
        }

        static void CreateButton(VisualElement rootElement, string name, Action callback)
        {
            rootElement.Q<Button>(name).clickable.clicked += callback;
        }

        static void CreateToggle(VisualElement rootElement, string name, EventCallback<ChangeEvent<bool>> callback, bool toggleValue)
        {
            var toggle = rootElement.Q<Toggle>(name);
            toggle.RegisterValueChangedCallback(callback);
            toggle.value = toggleValue;
        }

        static void CreateTextElement(VisualElement rootElement, string name, string text)
        {
            rootElement.Q<TextElement>(name).text = text;
        }

        static void LinkToSourceConfig()
        {
            var orgId = Core.Editor.OrganizationHandler.OrganizationProvider.Organization.Key;
            Application.OpenURL(string.IsNullOrWhiteSpace(orgId)
                ? k_DashboardUrl
                : string.Format(k_SourceConfigUrl, orgId));
        }

        /// <summary>
        /// Refresh the values displayed in the adapters section
        /// </summary>
        static void RefreshAdaptersData()
        {
            foreach (var adapterSetting in s_AdapterSettings)
            {
                // If the UI has not been created yet, skip this adapter
                if (!s_AdapterInstalledInfo.ContainsKey(adapterSetting.AdapterId))
                    continue;

                var adapterInfo = s_AdapterInfos[adapterSetting.AdapterId];
                var isInstalled = !string.IsNullOrEmpty(adapterSetting.InstalledVersion.value);
                var configAvailable = s_configuredAdNetworks != null;
                var isConfigured = configAvailable && s_configuredAdNetworks.Contains(adapterInfo.DashboardId);
                var installedVersionInfo = Array.Find(adapterInfo.Versions, x => x.Identifier == adapterSetting.InstalledVersion.value);
                s_AdapterInstalledInfo[adapterSetting.AdapterId].visible = isInstalled;
                s_AdapterUninstalledInfo[adapterSetting.AdapterId].visible = !isInstalled;
                s_AdapterInstalledIcon[adapterSetting.AdapterId].visible = (!isInstalled && isConfigured);
                s_AdapterUnavailableConfigInfo[adapterSetting.AdapterId].visible = !configAvailable;
                s_AdapterConfiguredInfo[adapterSetting.AdapterId].visible = isConfigured;
                s_AdapterUnconfiguredInfo[adapterSetting.AdapterId].visible = configAvailable && !isConfigured;
                s_AdapterConfiguredIcon[adapterSetting.AdapterId].visible = configAvailable && (!isConfigured && isInstalled);
                s_AdapterInstallButton[adapterSetting.AdapterId].visible = !(adapterSetting is UnityAdsSettings);

                RefreshAllInstallButton();
            }
        }

        /// <summary>
        /// Sets the appropriate text on the Install button ie Install or Uninstall for each adapter.
        /// </summary>
        static void RefreshAllInstallButton()
        {
            foreach (var adapterSetting in s_AdapterSettings)
            {
                var isInstalled = !string.IsNullOrEmpty(adapterSetting.InstalledVersion.value);

                if (s_AdapterInstallButton.ContainsKey(adapterSetting.AdapterId))
                {
                    s_AdapterInstallButton[adapterSetting.AdapterId].text = isInstalled ? k_Uninstall : k_Install;
                }
            }
        }

        /// <summary>
        /// Sets the appropriate text on the Install button ie Install or Uninstall for a specific adapter.
        /// </summary>
        static void RefreshInstallButton(string adapterIdentifier)
        {
            bool isInstalled = IsAdapterInstalled(adapterIdentifier);

            s_AdapterInstallButton[adapterIdentifier].text = isInstalled ? k_Uninstall : k_Install;
        }

        static void RefreshConfiguredAdNetworks(List<string> adNetworks)
        {
            s_configuredAdNetworks = adNetworks;
            Refresh();
        }

        /// <summary>
        /// Installs/updates to a selected version of the adapter
        /// </summary>
        static void AdapterInstallClicked(string adapterIdentifier)
        {
            if (adapterIdentifier != default)
            {
                bool isInstalled = IsAdapterInstalled(adapterIdentifier);

                if (!isInstalled)
                {
                    EditorGameServiceAnalyticsSender.SendProjectSettingsAdapterInstallEvent(adapterIdentifier);
                    MediationSdkInfo.Install(adapterIdentifier);
                }
                else
                {
                    EditorGameServiceAnalyticsSender.SendProjectSettingsAdapterUninstallEvent(adapterIdentifier);
                    MediationSdkInfo.Uninstall(adapterIdentifier);
                }

                RefreshInstallButton(adapterIdentifier);
            }
        }

        static bool IsAdapterInstalled(string adapterIdentifier)
        {
            return !string.IsNullOrEmpty(s_AdapterSettings.
                FirstOrDefault(adapterSettings => adapterSettings.AdapterId == adapterIdentifier)?.
                InstalledVersion.value);
        }

        static void ForceDynamicLinkingToggleValueChanged(ChangeEvent<bool> evt)
        {
            EditorGameServiceAnalyticsSender.SendBuildSettingsEvent(k_ForceDynamicLinkingKey, evt.newValue);
            s_ForceDynamicLinkingSetting.value = evt.newValue;
        }

        static void DeterministicBuildsToggleValueChanged(ChangeEvent<bool> evt)
        {
            EditorGameServiceAnalyticsSender.SendBuildSettingsEvent(k_DeterministicBuildsKey, evt.newValue);
            s_DeterministicBuildsSetting.value = evt.newValue;
        }

        static void DisableDexingArtifactTransformToggleValueChanged(ChangeEvent<bool> evt)
        {
            EditorGameServiceAnalyticsSender.SendBuildSettingsEvent(k_DisableDexingArtifactTransformKey, evt.newValue);
            s_DisableDexingArtifactTransform.value = evt.newValue;
        }

        static void ForceDependencyUpdateToggleValueChanged(ChangeEvent<bool> evt)
        {
            EditorGameServiceAnalyticsSender.SendBuildSettingsEvent(k_ForceDependencyUpdate, evt.newValue);
            s_ForceDependencyUpdateSetting.value = evt.newValue;
        }
    }
}

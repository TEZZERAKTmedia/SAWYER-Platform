extern "C" void RegisterStaticallyLinkedModulesGranular()
{
	void RegisterModule_SharedInternals();
	RegisterModule_SharedInternals();

	void RegisterModule_Core();
	RegisterModule_Core();

	void RegisterModule_AI();
	RegisterModule_AI();

	void RegisterModule_AR();
	RegisterModule_AR();

	void RegisterModule_Accessibility();
	RegisterModule_Accessibility();

	void RegisterModule_AndroidJNI();
	RegisterModule_AndroidJNI();

	void RegisterModule_Animation();
	RegisterModule_Animation();

	void RegisterModule_AssetBundle();
	RegisterModule_AssetBundle();

	void RegisterModule_Audio();
	RegisterModule_Audio();

	void RegisterModule_Cloth();
	RegisterModule_Cloth();

	void RegisterModule_ContentLoad();
	RegisterModule_ContentLoad();

	void RegisterModule_CrashReporting();
	RegisterModule_CrashReporting();

	void RegisterModule_DSPGraph();
	RegisterModule_DSPGraph();

	void RegisterModule_Director();
	RegisterModule_Director();

	void RegisterModule_GI();
	RegisterModule_GI();

	void RegisterModule_GameCenter();
	RegisterModule_GameCenter();

	void RegisterModule_Grid();
	RegisterModule_Grid();

	void RegisterModule_HierarchyCore();
	RegisterModule_HierarchyCore();

	void RegisterModule_HotReload();
	RegisterModule_HotReload();

	void RegisterModule_IMGUI();
	RegisterModule_IMGUI();

	void RegisterModule_ImageConversion();
	RegisterModule_ImageConversion();

	void RegisterModule_Input();
	RegisterModule_Input();

	void RegisterModule_InputLegacy();
	RegisterModule_InputLegacy();

	void RegisterModule_InputForUI();
	RegisterModule_InputForUI();

	void RegisterModule_JSONSerialize();
	RegisterModule_JSONSerialize();

	void RegisterModule_Localization();
	RegisterModule_Localization();

	void RegisterModule_Marshalling();
	RegisterModule_Marshalling();

	void RegisterModule_Multiplayer();
	RegisterModule_Multiplayer();

	void RegisterModule_ParticleSystem();
	RegisterModule_ParticleSystem();

	void RegisterModule_PerformanceReporting();
	RegisterModule_PerformanceReporting();

	void RegisterModule_Physics();
	RegisterModule_Physics();

	void RegisterModule_Physics2D();
	RegisterModule_Physics2D();

	void RegisterModule_Properties();
	RegisterModule_Properties();

	void RegisterModule_ScreenCapture();
	RegisterModule_ScreenCapture();

	void RegisterModule_ShaderVariantAnalytics();
	RegisterModule_ShaderVariantAnalytics();

	void RegisterModule_SpriteShape();
	RegisterModule_SpriteShape();

	void RegisterModule_SpriteMask();
	RegisterModule_SpriteMask();

	void RegisterModule_Streaming();
	RegisterModule_Streaming();

	void RegisterModule_Substance();
	RegisterModule_Substance();

	void RegisterModule_Subsystems();
	RegisterModule_Subsystems();

	void RegisterModule_TLS();
	RegisterModule_TLS();

	void RegisterModule_Terrain();
	RegisterModule_Terrain();

	void RegisterModule_TerrainPhysics();
	RegisterModule_TerrainPhysics();

	void RegisterModule_TextRendering();
	RegisterModule_TextRendering();

	void RegisterModule_TextCoreFontEngine();
	RegisterModule_TextCoreFontEngine();

	void RegisterModule_TextCoreTextEngine();
	RegisterModule_TextCoreTextEngine();

	void RegisterModule_Tilemap();
	RegisterModule_Tilemap();

	void RegisterModule_UI();
	RegisterModule_UI();

	void RegisterModule_UIElements();
	RegisterModule_UIElements();

	void RegisterModule_Umbra();
	RegisterModule_Umbra();

	void RegisterModule_UnityAnalyticsCommon();
	RegisterModule_UnityAnalyticsCommon();

	void RegisterModule_UnityConnect();
	RegisterModule_UnityConnect();

	void RegisterModule_UnityAnalytics();
	RegisterModule_UnityAnalytics();

	void RegisterModule_UnityCurl();
	RegisterModule_UnityCurl();

	void RegisterModule_UnityTestProtocol();
	RegisterModule_UnityTestProtocol();

	void RegisterModule_UnityWebRequest();
	RegisterModule_UnityWebRequest();

	void RegisterModule_UnityWebRequestAssetBundle();
	RegisterModule_UnityWebRequestAssetBundle();

	void RegisterModule_UnityWebRequestAudio();
	RegisterModule_UnityWebRequestAudio();

	void RegisterModule_UnityWebRequestTexture();
	RegisterModule_UnityWebRequestTexture();

	void RegisterModule_UnityWebRequestWWW();
	RegisterModule_UnityWebRequestWWW();

	void RegisterModule_VFX();
	RegisterModule_VFX();

	void RegisterModule_VR();
	RegisterModule_VR();

	void RegisterModule_Vehicles();
	RegisterModule_Vehicles();

	void RegisterModule_Video();
	RegisterModule_Video();

	void RegisterModule_Wind();
	RegisterModule_Wind();

	void RegisterModule_XR();
	RegisterModule_XR();

}

template <typename T> void RegisterUnityClass(const char*);
template <typename T> void RegisterStrippedType(int, const char*, const char*);

void InvokeRegisterStaticallyLinkedModuleClasses()
{
	void RegisterStaticallyLinkedModuleClasses();
	RegisterStaticallyLinkedModuleClasses();
}

void RegisterAllClasses()
{
	void RegisterAllClassesGranular();
	RegisterAllClassesGranular();
}

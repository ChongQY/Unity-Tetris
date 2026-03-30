using HybridCLR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;          // YooAsset 核心命名空间

/// <summary>
/// YooAsset 资源系统引导管理器
/// 负责初始化资源包、检查更新、下载资源并最终进入游戏。
/// 挂载在启动场景（如 Splash/Init）的一个 GameObject 上。
/// </summary>
public class YooAssetBoot : MonoBehaviour {
    [Header("运行模式")]
    [Tooltip("编辑器模拟模式（无需真机资源），仅在编辑器中有效")]
    public bool editorSimulateMode = false;

    [Tooltip("资源包名称（通常为 DefaultPackage）")]
    public string packageName = "DefaultPackage";

    [Header("服务器地址（联机模式必需）")]
    [Tooltip("主服务器基础地址，例如：https://your-cdn.com/PC/v1.0")]
    public string defaultHostServer;
    [Tooltip("备用服务器基础地址，可选，可与主服务器相同")]
    public string fallbackHostServer;

    [Header("下载设置")]
    [Tooltip("最大并发下载数")]
    public int downloadingMaxNum = 10;
    [Tooltip("下载失败重试次数")]
    public int failedTryAgain = 3;
    [Tooltip("是否在更新清单后自动下载所有资源")]
    public bool autoDownloadAll = true;

    [Header("完成后加载的场景")]
    [Tooltip("资源准备就绪后要加载的场景名称")]
    public string nextSceneName = "Main";

    [Header("UI相关")]
    [Tooltip("进度条")]
    public Slider slider;
    [Tooltip("进度条文字")]
    public TextMeshProUGUI sliderText;

    // 资源包引用
    private ResourcePackage _package;

    /// <summary>
    /// 需要补充元数据的AOT程序集文件列表（不含路径，但包含 .dll 后缀）。
    /// 注意：列表中的文件名必须与打包生成的补充元数据文件名完全一致。
    /// </summary>
    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>() {
        "mscorlib.dll",
        "UnityEngine.CoreModule.dll",
        "UnityEngine.UI.dll",
        "UnityEngine.AudioModule.dll",
        "YooAsset.dll",
        "PhotonRealtime.dll",
        "Unity.TextMeshPro.dll",
        "PhotonUnityNetworking.dll",
        "DOTweenLib.dll",
        "DOTween.dll",
    };

    /// <summary>
    /// 热更新程序集的反射引用，用于后续调用入口方法。
    /// </summary>
    private static Assembly _hotUpdateAss;

    /// <summary>
    /// 存储从 AssetBundle 中加载出来的程序集字节数据，键为文件名（如 "mscorlib.dll"），值为 TextAsset 对象。
    /// </summary>
    private static Dictionary<string, TextAsset> s_assetDatas = new Dictionary<string, TextAsset>();

    IEnumerator Start() {
        // 防止被销毁，保证常驻
        DontDestroyOnLoad(gameObject);

        // 1. 初始化 YooAsset 全局系统
        YooAssets.Initialize();
        Debug.Log("YooAssets 初始化完成");

        // 2. 初始化事件系统（如果你使用了 UniFramework.Event）
        // UniEvent.Initalize();

        // 3. 创建或获取资源包
        _package = YooAssets.TryGetPackage(packageName);
        if (_package == null)
            _package = YooAssets.CreatePackage(packageName);
        Debug.Log($"资源包 {packageName} 已创建/获取");

        // 4. 根据模式初始化资源包
        if (editorSimulateMode) {
            yield return InitEditorSimulateMode();
        } else {
            // 默认使用联机模式，你也可以扩展 OfflinePlayMode / WebPlayMode
            yield return InitHostPlayMode();
        }

        // 5. 请求远程资源版本号
        var versionOp = _package.RequestPackageVersionAsync(false);
        yield return versionOp;
        if (versionOp.Status != EOperationStatus.Succeed) {
            Debug.LogError($"获取资源版本失败：{versionOp.Error}");
            yield break;
        }
        string latestVersion = versionOp.PackageVersion;
        Debug.Log($"最新资源版本：{latestVersion}");

        // 6. 更新资源清单到最新版本
        var manifestOp = _package.UpdatePackageManifestAsync(latestVersion);
        yield return manifestOp;
        if (manifestOp.Status != EOperationStatus.Succeed) {
            Debug.LogError($"更新资源清单失败：{manifestOp.Error}");
            yield break;
        }
        Debug.Log("资源清单更新成功");

        // 7. 如果需要，自动下载所有资源
        if (autoDownloadAll) {
            yield return DownloadAllResources();
        }

        // 8. 设置默认资源包（可选，方便后续用静态方法加载）
        YooAssets.SetDefaultPackage(_package);

        // 9. 加载所有必要的程序集（包括 AOT 元数据补充和热更新 DLL）
        yield return StartCoroutine(LoadDlls());

        // 10. 加载游戏配置数据(ScriptableObject)
        yield return StartCoroutine(CallHotUpdateConfigInit());

        // 11. 所有准备工作完成，进入下一个场景
        Debug.Log($"YooAsset 初始化完成，准备加载场景：{nextSceneName}");
        YooAssets.LoadSceneAsync(nextSceneName);
    }


    /// <summary>
    /// 初始化编辑器模拟模式（无需真机资源，只在编辑器下有效）
    /// </summary>
    private IEnumerator InitEditorSimulateMode() {
        // 获取模拟构建的根目录（需要先执行 YooAsset/AssetBundle Builder 构建一次）
        var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
        string packageRoot = buildResult.PackageRootDirectory;

        // 创建编辑器文件系统参数
        var fileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
        var initParams = new EditorSimulateModeParameters();
        initParams.EditorFileSystemParameters = fileSystemParams;

        var initOp = _package.InitializeAsync(initParams);
        yield return initOp;

        if (initOp.Status == EOperationStatus.Succeed)
            Debug.Log("编辑器模拟模式初始化成功");
        else
            Debug.LogError($"编辑器模拟模式初始化失败：{initOp.Error}");
    }

    /// <summary>
    /// 初始化联机运行模式（支持热更新）
    /// </summary>
    private IEnumerator InitHostPlayMode() {
        // 构建完整的基础路径（假设 defaultHostServer 输入到 "/content" 为止，末尾无斜杠）
        string fullDefaultHost = defaultHostServer + "/CDN/PC/v1.0";
        string fullFallbackHost = fallbackHostServer + "/CDN/PC/v1.0";
        IRemoteServices remoteServices = new RemoteServices(fullDefaultHost, fullFallbackHost);

        // 内置文件系统（读取 StreamingAssets 内的初始资源）
        var buildinParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        // 缓存文件系统（存放下载的热更新资源）
        var cacheParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);

        var initParams = new HostPlayModeParameters();
        initParams.BuildinFileSystemParameters = buildinParams;
        initParams.CacheFileSystemParameters = cacheParams;

        var initOp = _package.InitializeAsync(initParams);
        yield return initOp;

        if (initOp.Status == EOperationStatus.Succeed)
            Debug.Log("联机模式初始化成功");
        else
            Debug.LogError($"联机模式初始化失败：{initOp.Error}");
    }

    /// <summary>
    /// 下载所有需要更新的资源
    /// </summary>
    private IEnumerator DownloadAllResources() {
        // 创建下载器，下载所有需要更新的文件
        var downloader = _package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        sliderText.text = "更新中";
        slider.value = 0;

        if (downloader.TotalDownloadCount == 0) {
            Debug.Log("没有资源需要下载");

            // 更新UI
            sliderText.text = "更新完成";
            slider.value = 1;
            yield break;
        }

        Debug.Log($"需要下载 {downloader.TotalDownloadCount} 个文件，总大小：{downloader.TotalDownloadBytes} 字节");

        // 注册回调（可根据需要显示进度）
        downloader.DownloadUpdateCallback = (updateData) => {
            Debug.Log($"下载进度：{updateData.CurrentDownloadCount}/{updateData.TotalDownloadCount}，已下载：{updateData.CurrentDownloadBytes}/{updateData.TotalDownloadBytes}");

            // 这里可以更新 UI 进度条，例如 progressSlider.value = (float)updateData.CurrentDownloadBytes / updateData.TotalDownloadBytes;
            float currentMB = updateData.CurrentDownloadBytes / (1024f * 1024f);
            float totalMB = updateData.TotalDownloadBytes / (1024f * 1024f);
            slider.value = (float)updateData.CurrentDownloadBytes / updateData.TotalDownloadBytes;
            sliderText.text = $"{currentMB:F1}/{totalMB:F1} MB";
        };

        downloader.DownloadErrorCallback = (errorData) =>   // 关键修正：参数为 DownloadErrorData
        {
            Debug.LogError($"文件 {errorData.FileName} 下载失败：{errorData.ErrorInfo}");
            slider.value = 0;
            sliderText.text = $"下载失败";
        };

        downloader.DownloadFinishCallback = (finishData) => // 根据实际委托，可能接受 DownloaderFinishData，这里先用 finishData 占位
        {
            Debug.Log("所有资源下载完成");
            // 可访问 finishData.Success 判断是否成功
            if (finishData.Succeed) {
                sliderText.text = "更新完成";
                slider.value = 1;
            } else {
                sliderText.text = "更新失败";
                slider.value = 0;
            }
        };

        downloader.DownloadFileBeginCallback = (beginData) => {
            Debug.Log($"开始下载：{beginData.FileName}，大小：{beginData.FileSize} 字节");
        };

        downloader.BeginDownload();
        yield return downloader;

        if (downloader.Status == EOperationStatus.Succeed)
            Debug.Log("资源下载成功");
        else
            Debug.LogError($"资源下载失败，状态：{downloader.Status}");
    }

    /// <summary>
    /// 内部类：实现 IRemoteServices 接口，用于构建远程 URL
    /// </summary>
    private class RemoteServices : IRemoteServices {
        private readonly string _defaultHost;
        private readonly string _fallbackHost;

        public RemoteServices(string defaultHost, string fallbackHost) {
            _defaultHost = defaultHost;
            _fallbackHost = fallbackHost;
        }

        string IRemoteServices.GetRemoteMainURL(string fileName) {
            return $"{_defaultHost}/{fileName}";
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName) {
            return $"{_fallbackHost}/{fileName}";
        }
    }

    /// <summary>
    /// 从默认资源包中加载所有需要的程序集文件（AOT 元数据 + 热更新 DLL），
    /// 并依次执行元数据补充和热更新 DLL 加载。
    /// </summary>
    private IEnumerator LoadDlls() {
        // 合并需要加载的所有文件名：热更新 DLL 和 AOT 元数据文件
        var assets = new List<string> { "HotUpdate.dll" }.Concat(AOTMetaAssemblyFiles);

        // 逐个加载程序集字节数据
        foreach (var asset in assets) {
            // 通过 YooAsset 异步加载 TextAsset 类型的资源（即 .bytes 文件）
            var handle = YooAssets.LoadAssetAsync<TextAsset>(asset);
            yield return handle;

            if (handle.Status == EOperationStatus.Succeed) {
                TextAsset textAsset = handle.AssetObject as TextAsset;
                s_assetDatas[asset] = textAsset;
                Debug.Log(asset + "加载成功, 大小: " + textAsset.bytes.Length + "字节");
            } else {
                Debug.LogError($"加载程序集 {asset} 失败");
            }

            // 释放资源句柄，减少内存占用
            handle.Release();
        }

        // 第一步：加载 AOT 补充元数据（必须在热更新程序集加载之前）
        LoadMetadataForAOTAssemblies();

        // 第二步：加载热更新程序集（HotUpdate.dll）
        LoadHotUpdateDlls();
    }

    /// <summary>
    /// 补充元数据
    /// 作用：在加载热更新程序集之前，预先加载必要的AOT程序集的元数据，
    /// 使得热更新代码能够正确使用泛型、反射等特性，避免运行时出现ExecutionEngineException等错误。
    /// </summary>
    private void LoadMetadataForAOTAssemblies() {
        // 设置模式，一般都使用 SuperSet 模式
        // HomologousImageMode 是一个枚举，表示加载的元数据与当前AOT镜像的关系：
        // - SuperSet: 允许加载比当前AOT版本更新的元数据（通常就用这个）
        // - ExactMatch: 要求精确匹配版本（很少用）
        HomologousImageMode mode = HomologousImageMode.SuperSet;

        // 遍历需要补充元数据的AOT程序集名称列表（如 "mscorlib.dll", "System.dll" 等）
        foreach (var aotDllName in AOTMetaAssemblyFiles) {
            // 从预先加载好的资源数据字典中，根据程序集名称取出对应的字节数组
            // s_assetDatas 中存储的是 TextAsset，通过 .bytes 属性获取原始字节
            byte[] dllBytes = s_assetDatas[aotDllName].bytes;

            // 调用 HybridCLR 的运行时API，将当前AOT程序集的元数据加载到运行时中
            // 参数1: 程序集元数据的字节数组
            // 参数2: 元数据匹配模式（通常用 SuperSet）
            // 返回值: 一个枚举值，表示加载结果（OK 表示成功，其他值表示错误）
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);

            // 建议：检查返回值并处理错误，例如：
            if (err != LoadImageErrorCode.OK) {
                Debug.LogError($"加载 AOT 元数据失败：{aotDllName}，错误码：{err}");
                // 可根据项目需要决定是否中止流程
            }
        }
    }

    /// <summary>
    /// 加载热更新程序集（HotUpdate.dll），并获取其 Assembly 对象。
    /// 在真机环境下从字节数组加载，在编辑器环境下直接从已加载的程序集中查找。
    /// </summary>
    private void LoadHotUpdateDlls() {
        if (s_assetDatas.Count == 0) {
            Debug.LogError("程序集字节数据为空，无法加载热更新 DLL");
            return;
        }

#if !UNITY_EDITOR
        // 真机：从字节数组加载程序集
        _hotUpdateAss = Assembly.Load(s_assetDatas["HotUpdate.dll"].bytes);
        Debug.Log("热更新程序集加载成功，程序集名称：" + _hotUpdateAss.FullName);
#else
        // 编辑器：热更新程序集已经由 Unity 编译并加载，直接查找
        // 注意：程序集名称需与 .asmdef 中定义的名称一致，默认为 "HotUpdate"
        _hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies()
            .First(a => a.GetName().Name == "HotUpdate");
        Debug.Log("编辑器模式下找到热更新程序集：" + _hotUpdateAss.FullName);
#endif
    }

    // ScriptableObject 在HotUpdate
    // 这里是AOT, 调用初始化得反射去调用
    // 反射调用热更层的 ConfigManager 初始化
    // 【核心修复】直接从已加载的热更程序集里找类型，不再用脆弱的 Type.GetType
    private IEnumerator CallHotUpdateConfigInit() {
        // 1. 安全检查：确保热更程序集已加载
        if (_hotUpdateAss == null) {
            Debug.LogError("热更程序集未加载，无法初始化 ConfigManager！");
            yield break;
        }

        // 2. 【关键】直接从 _hotUpdateAss 程序集里找 ConfigManager 类
        // 注意：这里要写对类名，如果有命名空间要带上命名空间
        System.Type configType = _hotUpdateAss.GetType("ConfigManager");

        if (configType == null) {
            Debug.LogError("在热更程序集中找不到 ConfigManager 类！请检查类名是否拼写正确，且类是 public 的。");

            // 【调试代码】打印程序集里所有的类名，帮你找问题
            Debug.Log("--- 热更程序集里的所有类名如下 ---");
            foreach (var type in _hotUpdateAss.GetTypes()) {
                Debug.Log(type.FullName);
            }
            yield break;
        }

        Debug.Log($"成功找到 ConfigManager 类：{configType.FullName}");

        // 3. 获取单例实例
        object instance = configType.GetProperty("Instance").GetValue(null);

        // 4. 获取初始化协程方法
        System.Reflection.MethodInfo initMethod = configType.GetMethod("InitializeCoroutine");

        // 5. 【关键】调用并等待初始化完成
        IEnumerator initCoroutine = (IEnumerator)initMethod.Invoke(instance, null);
        yield return StartCoroutine(initCoroutine);

        // 6. 二次确认：检查 GameConfig 是否真的加载成功了
        // 这里通过反射再检查一下 GameConfig 属性
        var gameConfigProp = configType.GetProperty("GameConfig");
        object gameConfigValue = gameConfigProp.GetValue(instance);

        if (gameConfigValue == null) {
            Debug.LogError("ConfigManager 初始化完成，但 GameConfig 仍然是 null！请检查 YooAsset 里的资源地址是否正确。");
        } else {
            Debug.Log("ConfigManager 初始化成功，GameConfig 已加载！");
        }
    }
}
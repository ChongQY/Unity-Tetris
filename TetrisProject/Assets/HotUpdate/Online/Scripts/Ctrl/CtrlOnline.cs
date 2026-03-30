using Photon.Pun;
using System.Collections;
using UnityEngine;
using YooAsset;

public class CtrlOnline : MonoBehaviour {
    // 在 检查器面板 隐藏

    [HideInInspector] public ModelOnline modelOnline;// 模型数据
    [HideInInspector] public ViewOnline viewOnline;// 视图
    [HideInInspector] public GameManagerOnline gameManagerOnline;// 游戏管理器
    [HideInInspector] public AudioManagerOnline audioManagerOnline;// 声音管理器
    [HideInInspector] public NetworkCtrl networkCtrl;// 联机
    [HideInInspector] public CameraManagerOnline cameraManagerOnline;// 摄像机管理器

    private void Awake() {
        // 初始化
        StartCoroutine(InitLocalGame());
    }

    private IEnumerator InitLocalGame() {
        // 初始化 MV层引用
        yield return StartCoroutine(LoadResources());

        // 添加脚本
        AddScripts();

        // 游戏界面
        GameInit();
    }

    /// <summary>
    /// 初始化 MV层、引用
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadResources() {
        // 加载 Model 层
        AssetHandle modelHandle = YooAssets.LoadAssetAsync<GameObject>("ModelOnline");
        yield return modelHandle;
        if (modelHandle.Status != EOperationStatus.Succeed) {
            Debug.LogError("ModelOnline 加载失败");
            yield break;
        }
        // 加载View 层
        AssetHandle viewHandle = YooAssets.LoadAssetAsync<GameObject>("ViewOnline");
        yield return viewHandle;
        if (viewHandle.Status != EOperationStatus.Succeed) {
            Debug.LogError("View 加载失败");
            yield break;
        }

        // 实例化 Mode、View
        Instantiate(modelHandle.AssetObject as GameObject).gameObject.name = "ModelOnline";
        Instantiate(viewHandle.AssetObject as GameObject).gameObject.name = "ViewOnline";

        // 获取组件
        modelOnline = GameObject.FindGameObjectWithTag("ModelOnline").GetComponent<ModelOnline>();
        viewOnline = GameObject.FindGameObjectWithTag("ViewOnline").GetComponent<ViewOnline>();
    }

    /// <summary>
    /// 添加脚本
    /// </summary>
    /// <returns></returns>
    private void AddScripts() {
        // 添加 游戏管理器
        gameManagerOnline = gameObject.AddComponent<GameManagerOnline>();

        // 添加 摄像机管理器
        cameraManagerOnline = gameObject.AddComponent<CameraManagerOnline>();

        // 添加 音频管理器
        audioManagerOnline = gameObject.AddComponent<AudioManagerOnline>();

        // 添加 事件管理器
        gameObject.AddComponent<EventManagerOnline>();

        // 添加 网络化资源管理器
        networkCtrl = gameObject.AddComponent<NetworkCtrl>();
    }

    private void GameInit() {
        // 设置UI初始界面
        viewOnline.SetRecordText(modelOnline.playerTrueCount, modelOnline.enemyTrueCount);
        viewOnline.InitUI();
        cameraManagerOnline.ZoomOut();
        PhotonNetwork.IsMessageQueueRunning = true; // 避免永久阻塞
        Debug.Log("欢迎来到联机对战界面");
    }

    


}

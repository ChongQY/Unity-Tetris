using System.Collections;
using UnityEngine;
using YooAsset;

public class Ctrl : MonoBehaviour {
    // 在 检查器面板 隐藏

    [HideInInspector] public Model model;// 模型数据

    [HideInInspector] public View view;// 视图
    [HideInInspector] public CameraManager cameraManager;// 相机管理器
    [HideInInspector] public GameManager gameManager;// 游戏管理器
    [HideInInspector] public AudioManager audioManager;// 声音管理器
    [HideInInspector] public NetworkingManager networkingManager;// 网络化管理器

    // 有限状态机
    private FSMSystem fsm;

    
    private void Awake() {
        // 初始化
        StartCoroutine(InitLocalGame());
    }

    private IEnumerator InitLocalGame() {
        // 初始化引用
        yield return StartCoroutine(LoadResources());

        // 添加脚本
        AddScripts();

        // 读取上次数据
        model.LoadData();

        // 初始化有限状态机
        MakeFSM();

        Debug.Log("热更新成功");
    }

    
    /// <summary>
    /// 初始化 MV层、引用
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadResources() {
        // 加载 Model 层
        AssetHandle modelHandle = YooAssets.LoadAssetAsync<GameObject>("Model");
        yield return modelHandle;
        if (modelHandle.Status != EOperationStatus.Succeed) {
            Debug.LogError("Model 加载失败");
            yield break;
        }
        // 加载View 层
        AssetHandle viewHandle = YooAssets.LoadAssetAsync<GameObject>("View");
        yield return viewHandle;
        if (viewHandle.Status != EOperationStatus.Succeed) {
            Debug.LogError("View 加载失败");
            yield break;
        }

        // 实例化 Mode、View
        Instantiate(modelHandle.AssetObject as GameObject).gameObject.name = "Model";
        Instantiate(viewHandle.AssetObject as GameObject).gameObject.name = "View";

        // 获取组件
        model = GameObject.FindGameObjectWithTag("Model").GetComponent<Model>();
        view = GameObject.FindGameObjectWithTag("View").GetComponent<View>();
        
        
    }

    /// <summary>
    /// 添加脚本
    /// </summary>
    /// <returns></returns>
    private void AddScripts() {
        // 添加 游戏管理器
        gameManager = gameObject.AddComponent<GameManager>();

        // 添加 摄像机管理器
        cameraManager = gameObject.AddComponent<CameraManager>();

        // 添加 音频管理器
        audioManager = gameObject.AddComponent<AudioManager>();

        // 添加 UI事件管理器
        gameObject.AddComponent<UIEventManager>();

        // 添加 网络化资源管理器
        networkingManager = gameObject.AddComponent<NetworkingManager>();



    }
    /// <summary>
    /// 初始化有限状态机
    /// </summary>
    private void MakeFSM() {
        fsm = new FSMSystem();

        FSMState[] states = GetComponentsInChildren<FSMState>();

        // 添加所有状态
        foreach (FSMState state in states) {
            fsm.AddState(state, this);
        }

        // 设置默认状态
        MenuState menuState = GetComponentInChildren<MenuState>();
        fsm.SetCurrentState(menuState);
    }
}

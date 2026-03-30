using System.Collections;
using UnityEngine;
using YooAsset;

// 如果不加 public，默认是 internal，反射可能找不到
public class ConfigManager {
    private static ConfigManager _instance;
    public static ConfigManager Instance => _instance ??= new ConfigManager();

    public GameConfigSO GameConfig { get; private set; }

    private ConfigManager() { }

    public IEnumerator InitializeCoroutine() {
        var handle = YooAssets.LoadAssetAsync<GameConfigSO>("GameConfig");
        yield return handle;
        if (handle.Status == EOperationStatus.Succeed) {
            Debug.Log("资源加载成功");
            GameConfig = handle.AssetObject as GameConfigSO;
        } else {
            Debug.LogError("加载配置失败");
        }
            
    }
}
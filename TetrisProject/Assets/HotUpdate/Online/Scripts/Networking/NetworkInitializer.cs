using Photon.Pun;
using UnityEngine;

public class NetworkInitializer : MonoBehaviour {
    public GameObject[] spawnablePrefabs;   // 在 Inspector 中拖入需要网络生成的预制体

    private void Awake() {
        DontDestroyOnLoad(gameObject);      // 确保场景切换后仍然存在
        RegisterPrefabs();
    }

    private void RegisterPrefabs() {
        var pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool == null) return;

        foreach (var prefab in spawnablePrefabs) {
            if (prefab != null && !pool.ResourceCache.ContainsKey(prefab.name)) {
                // 检查预制体上是否有 PhotonView
                if (prefab.GetComponent<PhotonView>() == null) {
                    Debug.LogError($"预制体 {prefab.name} 缺少 PhotonView 组件！");
                } else {
                    pool.ResourceCache.Add(prefab.name, prefab);
                    Debug.Log($"注册预制体: {prefab.name} (含 PhotonView)");
                }
            }
        }
    }
}
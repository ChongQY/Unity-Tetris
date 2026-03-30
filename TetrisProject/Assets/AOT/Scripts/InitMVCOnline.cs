using Photon.Pun;
using System.Collections;
using UnityEngine;
using YooAsset;

public class InitMVCOnline : MonoBehaviour {
    private void Awake() {
        StartCoroutine(LoadCtrl());
    }

    private IEnumerator LoadCtrl() {
        // 1. 用 YooAsset 加载资源（这部分保持不变）
        AssetHandle ctrlHandle = YooAssets.LoadAssetAsync<GameObject>("CtrlOnline");
        yield return ctrlHandle;

        if (ctrlHandle.Status != EOperationStatus.Succeed) {
            Debug.LogError($"加载 CtrlOnline 失败: {ctrlHandle.LastError}");
            yield break;
        }

        // 2. 用 Unity 普通方法生成物体（这部分也不变）
        GameObject ctrlObj = Instantiate(ctrlHandle.AssetObject as GameObject);
        ctrlObj.name = "CtrlOnline";

        // 3. ? 所有 PhotonView 检查、分配 ID 的代码全部删掉！
        // 现在不需要了，代码清爽多了
    }
}
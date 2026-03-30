using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class InitMVC : MonoBehaviour
{

    private void Awake() {
        StartCoroutine(LoadCtrl());
    }

    /// <summary>
    ///  初始化Ctrl
    ///  目前也就是实例化出来Ctrl
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadCtrl() {
        AssetHandle ctrlHandle = YooAssets.LoadAssetAsync<GameObject>("Ctrl");
        yield return ctrlHandle;
        Instantiate(ctrlHandle.AssetObject as GameObject).gameObject.name = "Ctrl";
    }
}

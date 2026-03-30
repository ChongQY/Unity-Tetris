// 引入 Photon 网络功能的核心命名空间，用于网络同步和 RPC 等
using Photon.Pun;
// 引入 Unity 基础功能
using UnityEngine;

// 定义一个 PlayerController 类，继承自 MonoBehaviourPun
// MonoBehaviourPun 是 Photon 提供的类，它内部封装了 PhotonView 组件，并提供了 photonView 属性
public class PlayerController : MonoBehaviourPun {
    public float HP = 100;
    public int ID;

    void Start() {
        if (photonView == null) {
            Debug.LogError("PlayerController: photonView is null! 请确保预制体上挂载了 PhotonView 组件。");
        }

        ID = photonView.ViewID;
    }
    // Update 是 Unity 的生命周期方法，每帧调用一次
    void Update() {
        // ===== 网络权限检查 =====
        // photonView.IsMine 判断当前客户端是否拥有这个物体的控制权
        // 只有本地玩家控制的物体才能接收输入
        // 如果不是本地玩家（即对手），则直接返回，不执行后续移动代码
        if (!photonView.IsMine) return;

        // ===== 获取输入 =====
        // Input.GetAxis("Horizontal") 返回 -1 到 1 之间的值，对应左右方向键或 AD 键
        float h = Input.GetAxis("Horizontal");
        // Input.GetAxis("Vertical") 返回 -1 到 1 之间的值，对应上下方向键或 WS 键
        float v = Input.GetAxis("Vertical");

        // ===== 移动物体 =====
        // 使用 Translate 方法移动物体，方向是 (h, 0, v)，乘以 Time.deltaTime 保证移动速度与帧率无关
        // 再乘以 5f 作为速度系数
        transform.Translate(new Vector3(h, 0, v) * Time.deltaTime * 5f);

        // 注意：这个移动只在本地客户端执行，Photon 不会自动同步 Transform
        // 如果需要同步位置，需要在 PhotonView 的 Observed Components 中添加 Transform
        // 或者通过 RPC 手动同步

        if (Input.GetKeyDown(KeyCode.Space)) {
            photonView.RPC("Att", RpcTarget.All, photonView.ViewID);
        }
    }


    [PunRPC]
    public void Att(int senderID, PhotonMessageInfo info) {
        Debug.Log($"Hite 执行，当前客户端：{PhotonNetwork.LocalPlayer.NickName}，物体所有者：{photonView.Owner?.NickName}");

        var players = FindObjectsOfType<PlayerController>();
        foreach (var player in players) {
            if (senderID != player.photonView.ViewID) {
                player.HP -= 25; 
                player.transform.Rotate(new Vector3(10, 10, 10));
                Debug.Log("senderID:" + senderID + "Mid_HP_PlayerID: " + player.photonView.ViewID);
            }
        }

    }
}
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

// 为了使用回调函数 所以继承MonoBehaviourPunCallbacks
public class NetworkingManager : MonoBehaviourPunCallbacks {
    private Ctrl ctrl;

    private void Awake() {
        ctrl = GameObject.FindGameObjectWithTag("Ctrl").GetComponent<Ctrl>();
    }

    #region 连接服务器 及其 回调函数
    /// <summary>
    /// 连接 服务器
    /// </summary>
    public void ConnectServer() {
        // 连接到光子云服务器
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia"; // 例如亚洲
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// 当成功连接到主服务器时自动调用
    /// </summary>
    public override void OnConnectedToMaster() {
        Debug.Log("已连接到光子服务器");
        ctrl.view.onlineSalaStartText.text = "已连接，可以创建或加入房间";

        // 开启 创建房间/加入房间
        ctrl.view.onlineSalaCreateBtn.interactable = true;
        ctrl.view.onlineSalaAddBtn.interactable = true;
    }
    /// <summary>
    /// 当与主服务器断开时调用
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("已和服务器断开连接");

        ctrl.view.onlineSalaStartText.text = "连接失败, 请重试";

        // 禁用 创建房间/加入房间
        ctrl.view.onlineSalaCreateBtn.interactable = false;
        ctrl.view.onlineSalaAddBtn.interactable = false;
    }
    #endregion


    #region 创建、加入、离开房间 及其 回调函数
    /// <summary>
    /// 创建房间方法
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="options"></param>
    public void CreateRoom(string roomName, RoomOptions options) {
        // 创建房间
        PhotonNetwork.CreateRoom(roomName, options);
    }

    /// <summary>
    /// 加入指定房间方法
    /// </summary>
    /// <param name="roomName"></param>
    public void JoinRoom(string roomName) {
        // 调用 PhotonNetwork.JoinRoom() 尝试加入指定名称的房间
        PhotonNetwork.JoinRoom(roomName);
        
    }

    /// <summary>
    /// 离开房间
    /// </summary>
    public void ExitRomm() {
        // 离开当前房间
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// 当离开房间时调用
    /// </summary>
    public override void OnLeftRoom() {
        // 开启 创建房间/加入房间
        ctrl.view.onlineSalaCreateBtn.interactable = true;
        ctrl.view.onlineSalaAddBtn.interactable = true;
        // 修改状态
        ctrl.view.onlineSalaStartText.text = "已退出房间，可以创建或加入房间";
        ctrl.view.onlineSalaCreateBtnText.text = "创建房间";

    }

    /// <summary>
    /// 当房间创建成功时调用
    /// </summary>
    public override void OnCreatedRoom() {
        // 禁用加入房间按钮 开启离开房间按钮
        ctrl.view.onlineSalaAddBtn.interactable = false;
        ctrl.view.onlineSalaCreateBtn.interactable = true;
        // 修改状态
        ctrl.view.onlineSalaStartText.text = "房间创建成功，等待玩家...";
        ctrl.view.onlineSalaCreateBtnText.text = "退出房间";
    }
    /// <summary>
    /// 当成功加入房间时调用（无论是创建者还是加入者都会触发）
    /// </summary>
    public override void OnJoinedRoom() {
        ctrl.view.onlineSalaStartText.text = "已加入房间，玩家数量：" + PhotonNetwork.CurrentRoom.PlayerCount;

        // 直接跳转 联机场景
        //SceneManager.LoadScene("OnlineGame");
        StartCoroutine(OnSceneLoaded());

        // 禁用加入按钮 开启离开按钮
        ctrl.view.onlineSalaAddBtn.interactable = false;
        ctrl.view.onlineSalaCreateBtn.interactable = true;
        // 修改状态
        ctrl.view.onlineSalaCreateBtnText.text = "退出房间";
    }

    /// <summary>
    /// 当创建房间失败时调用
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message) {
        ctrl.view.onlineSalaStartText.text = "创建房间失败：" + message;
        
        // 开启加入/加入房间按钮
        ctrl.view.onlineSalaCreateBtn.interactable = true;
        ctrl.view.onlineSalaAddBtn.interactable = true;
        // 修改状态
        ctrl.view.onlineSalaCreateBtnText.text = "创建房间";
    }

    /// <summary>
    /// 当加入房间失败时调用
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message) {
        ctrl.view.onlineSalaStartText.text = "加入房间失败：" + message;

        // 开启加入/加入房间按钮
        ctrl.view.onlineSalaCreateBtn.interactable = true;
        ctrl.view.onlineSalaAddBtn.interactable = true;
        // 修改状态
        ctrl.view.onlineSalaCreateBtnText.text = "创建房间";
    }
    #endregion


    #region 房间内玩家变化回调

    // 当有其他玩家进入房间时调用（所有已有玩家都会收到）
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        ctrl.view.onlineSalaStartText.text = "有玩家加入，玩家数量：" + PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(newPlayer.NickName + " 加入了房间");

        // 当房间人数达到2且自己是房主时，加载游戏场景
        //if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2) {
        //    Debug.Log("房主开始加载场景");
        //    ctrl.viewOnline.onlineSalaStartText.text = "匹配成功, 正常进入对战场景...";

        //    // 通过 PhotonView 发送 RPC 给所有客户端
        //    // RPC_LoadGameScene 方法会在所有客户端上执行
        //    photonView.RPC("RPC_LoadGameScene", RpcTarget.All);
        //}
    }

    // 当有其他玩家离开房间时调用
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        ctrl.view.onlineSalaStartText.text = "有玩家离开，玩家数量：" + PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(otherPlayer.NickName + " 离开了房间");
    }

    #endregion


    #region RPC 方法

    // 这个方法由房主调用，让所有客户端加载游戏场景
    [PunRPC]
    void RPC_LoadGameScene() {
        Debug.Log("RPC 触发加载场景，当前客户端：" + PhotonNetwork.LocalPlayer.NickName);

        //// 暂停消息队列，避免加载过程中处理网络消息导致问题
        //PhotonNetwork.IsMessageQueueRunning = false;

        //// 使用 YooAsset 异步加载场景（假设场景地址为 "OnlineGame"）
        //SceneHandle handle = YooAssets.LoadSceneAsync("OnlineGame");
        //StartCoroutine(OnSceneLoaded(handle));
    }
    /*
     private IEnumerator OnSceneLoaded(SceneHandle handle) {
        yield return handle;

        if (handle.Status == EOperationStatus.Succeed) {
            Debug.Log("场景加载成功");
            // 加载完成后重新开启消息队列
            PhotonNetwork.IsMessageQueueRunning = true;
        } else {
            Debug.LogError($"场景加载失败：{handle.LastError}");
            // 可在此处添加重试逻辑或错误提示
            PhotonNetwork.IsMessageQueueRunning = true; // 避免永久阻塞
        }
    }
     */



    #endregion

    /// <summary>
    ///  跳转到联机场景
    ///  这里关闭了消息队列: PhotonNetwork.IsMessageQueueRunning = false;
    ///  需要再跳转后打开
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnSceneLoaded() {
        // 暂停消息队列，避免加载过程中处理网络消息导致问题
        PhotonNetwork.IsMessageQueueRunning = false;

        // 异步加载OnlineGame场景
        SceneHandle handle = YooAssets.LoadSceneAsync("OnlineGame");

        yield return handle;

        if (handle.Status == EOperationStatus.Succeed) {
            Debug.Log("场景加载成功");
        } else {
            Debug.LogError($"场景加载失败：{handle.LastError}");
        }


    }
}

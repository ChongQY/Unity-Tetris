// 引入 Photon 网络核心功能
using Photon.Pun;
// 引入 Photon 的基本类型，如 RoomOptions
using Photon.Realtime;
// 引入 TextMeshPro
using TMPro;
// 引入 Unity 基础功能
using UnityEngine;
// 引入场景管理
using UnityEngine.SceneManagement;
// 引入 UI 组件
using UnityEngine.UI;

// 定义一个名为 Launcher 的类，继承自 MonoBehaviourPunCallbacks
// MonoBehaviourPunCallbacks 是 Photon 提供的回调类，可以让我们重写连接事件的方法
public class Launcher : MonoBehaviourPunCallbacks {
    [Header("UI 组件")]
    [SerializeField] private TMP_InputField roomNameInput;  // 输入房间名的输入框
    [SerializeField] private Button createBtn;              // 创建房间按钮
    [SerializeField] private Button joinBtn;                // 加入房间按钮
    [SerializeField] private TextMeshProUGUI statusText;    // 显示状态信息的文本


    // 用于记录准备数量的变量（只在房主端使用）
    private int readyCount = 0;

    // =================== Unity 生命周期 ===================
    void Awake() {
        // 确保这个物体在场景切换时不会被销毁
        // 这样 Launcher 物体可以在多个场景中持续存在，用于接收 RPC 和回调
        //DontDestroyOnLoad(gameObject);

        // 注册场景加载完成事件
        // 当任何场景加载完成后，OnSceneLoaded 方法会被调用
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start() {
        // 为创建房间按钮添加点击监听事件，点击时执行 CreateRoom 方法
        createBtn.onClick.AddListener(CreateRoom);
        // 为加入房间按钮添加点击监听事件，点击时执行 JoinRoom 方法
        joinBtn.onClick.AddListener(JoinRoom);

        // 更新状态文本，提示正在连接
        statusText.text = "正在连接...";

        // 连接到光子云服务器
        // ConnectUsingSettings() 会读取 Project Settings 里配置的 AppId
        // 然后尝试连接到 Photon 的云端服务器
        PhotonNetwork.ConnectUsingSettings();
    }

    void OnDestroy() {
        // 取消注册场景加载事件，避免内存泄漏
        // 当 Launcher 物体被销毁时，移除事件监听
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // =================== 场景加载完成回调 ===================
    // 当场景加载完成后自动调用（通过 SceneManager.sceneLoaded 事件注册）
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"场景加载完成：{scene.name}，当前客户端：{PhotonNetwork.LocalPlayer?.NickName ?? "未知"}");
    }

    // =================== 按钮点击方法 ===================
    void CreateRoom() {
        // 获取输入框中的房间名
        string roomName = roomNameInput.text;
        // 如果房间名为空，则自动生成一个随机房间名（格式：Room_四位数字）
        if (string.IsNullOrEmpty(roomName))
            roomName = "Room_" + Random.Range(1000, 9999);

        // 创建 RoomOptions 对象，用于设置房间属性
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };  // 设置最大玩家数为2

        // 调用 PhotonNetwork.CreateRoom() 尝试创建一个新房间
        PhotonNetwork.CreateRoom(roomName, options);
        statusText.text = "创建房间中...";
    }

    void JoinRoom() {
        // 获取输入框中的房间名
        string roomName = roomNameInput.text;
        // 如果房间名为空，提示用户输入
        if (string.IsNullOrEmpty(roomName)) {
            statusText.text = "请输入房间名";
            return;
        }

        // 调用 PhotonNetwork.JoinRoom() 尝试加入指定名称的房间
        PhotonNetwork.JoinRoom(roomName);
        statusText.text = "加入房间中...";
    }

    // =================== Photon 连接回调 ===================
    // 当成功连接到主服务器时自动调用
    public override void OnConnectedToMaster() {
        Debug.Log("已连接到光子服务器");
        statusText.text = "已连接，可以创建或加入房间";
    }

    // 当房间创建成功时调用
    public override void OnCreatedRoom() {
        statusText.text = "房间创建成功，等待玩家...";
    }

    // 当成功加入房间时调用（无论是创建者还是加入者都会触发）
    public override void OnJoinedRoom() {
        statusText.text = "已加入房间，玩家数量：" + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    // 当创建房间失败时调用
    public override void OnCreateRoomFailed(short returnCode, string message) {
        statusText.text = "创建房间失败：" + message;
    }

    // 当加入房间失败时调用
    public override void OnJoinRoomFailed(short returnCode, string message) {
        statusText.text = "加入房间失败：" + message;
    }

    // =================== 房间内玩家变化回调 ===================
    // 当有其他玩家进入房间时调用（所有已有玩家都会收到）
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        statusText.text = "已加入房间，玩家数量：" + PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(newPlayer.NickName + " 加入了房间");

        // 当房间人数达到2且自己是房主时，加载游戏场景
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            Debug.Log("房主开始加载场景");

            // 通过 PhotonView 发送 RPC 给所有客户端
            // RPC_LoadGameScene 方法会在所有客户端上执行
            photonView.RPC("RPC_LoadGameScene", RpcTarget.All);
        }
    }

    // 当有其他玩家离开房间时调用
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        statusText.text = "已加入房间，玩家数量：" + PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(otherPlayer.NickName + " 离开了房间");
    }

    // =================== RPC 方法 ===================
    // RPC 方法必须用 [PunRPC] 特性标记
    // 这个方法由房主调用，让所有客户端加载游戏场景
    [PunRPC]
    void RPC_LoadGameScene() {
        Debug.Log("RPC 触发加载场景，当前客户端：" + PhotonNetwork.LocalPlayer.NickName);

        // PhotonNetwork.LoadLevel 会加载指定名称的场景
        // 并确保所有客户端都加载同一个场景
        PhotonNetwork.LoadLevel("OnlineGame");
    }

    // RPC 方法：客户端通知房主自己已准备好
    // 只有房主会收到这个 RPC
    [PunRPC]
    void RPC_ClientReady() {
        // 只有房主会执行这里的代码
        readyCount++;
        Debug.Log($"客户端准备，当前准备数：{readyCount} / {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

}
// 引入 Photon 核心命名空间
using Photon.Pun;
// 引入 Photon 房间和玩家相关的命名空间
using Photon.Realtime;
// 引入 Photon 底层事件数据的命名空间（用于 RaiseEvent）
using ExitGames.Client.Photon;
using UnityEngine;
using YooAsset;
using System.Collections;

// ==========================================
// 1. 定义事件码（就像“广播暗号”）
// ==========================================
/// <summary>
/// 事件码枚举：用来区分不同的网络消息
/// 为什么用 byte？因为省流量，而且 Photon 要求事件码是 byte 类型
/// </summary>
public enum EventCodes : byte {
    PlayerReady = 1,           // 暗号1：我准备好了
    StartGame = 2,             // 暗号2：游戏开始
    GameTrue = 3,              // 暗号3：我赢了
    GameTrueIsEnemyToFalse = 4,// 暗号4：我认输了
    Attack = 5,                // 暗号5：发动攻击（给对手加垃圾行）
    UpdateEnemyMap = 6,        // 暗号6：同步我的地图给对手看
    ExitOnline = 7             // 暗号7：我要退出房间了
}

// ==========================================
// 2. 类定义
// ==========================================
/// <summary>
/// 网络控制器：负责所有网络消息的发送和接收
/// 【继承说明】
/// 1. MonoBehaviourPunCallbacks：可以自动接收 Photon 的常用回调（如玩家加入、离开房间）
/// 2. IOnEventCallback：【关键】必须实现这个接口，才能接收 RaiseEvent 发来的“广播消息”
/// </summary>
public class NetworkCtrl : MonoBehaviourPunCallbacks, IOnEventCallback {
    // 持有 MVC 的核心控制器，方便调用游戏逻辑
    private CtrlOnline ctrl;

    private void Awake() {
        // 【重要】把当前脚本注册为 Photon 的回调目标
        // 就像你打开了收音机，并调到了 Photon 频道，这样才能听到广播
        PhotonNetwork.AddCallbackTarget(this);

        // 获取场景里的 CtrlOnline 组件
        ctrl = GameObject.FindGameObjectWithTag("CtrlOnline").GetComponent<CtrlOnline>();
    }

    private void OnDestroy() {
        // 【重要】物体销毁时，必须移除回调目标
        // 就像你离开房间前要关掉收音机，避免内存泄漏
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // ==========================================
    // 3. 发送消息的“对外接口”
    // ==========================================
    // 这些方法是给 EventManagerOnline 调用的
    // 它们本身不做复杂逻辑，只是把具体的“暗号”和“内容”传给下面的 SendEvent 辅助方法

    /// <summary>
    /// 通知其他人：我准备好了
    /// </summary>
    public void SendReady() {
        SendEvent(EventCodes.PlayerReady);
    }

    /// <summary>
    /// 通知所有人：游戏开始
    /// </summary>
    public void StartGame() {
        SendEvent(EventCodes.StartGame);
    }

    /// <summary>
    /// 通知对手：我赢了
    /// </summary>
    public void GameTrue() {
        SendEvent(EventCodes.GameTrue);
    }

    /// <summary>
    /// 通知对手：我认输了
    /// </summary>
    public void GameTrueIsEnemyToFalse() {
        SendEvent(EventCodes.GameTrueIsEnemyToFalse);
    }

    /// <summary>
    /// 通知对手：我要攻击你（给你加几行垃圾）
    /// </summary>
    /// <param name="count">要加的垃圾行数量</param>
    public void Attack(int count) {
        SendEvent(EventCodes.Attack, count);
    }

    /// <summary>
    /// 通知对手：这是我的地图数据
    /// </summary>
    /// <param name="map">代表地图的 int 数组</param>
    public void UpdateEnemyMap(int[] map) {
        SendEvent(EventCodes.UpdateEnemyMap, map);
    }

    /// <summary>
    /// 通知所有人：我要退出了
    /// </summary>
    public void ExitOnline() {
        SendEvent(EventCodes.ExitOnline);
    }

    // ==========================================
    // 4. 【核心】发送消息的辅助方法 (SendEvent)
    // ==========================================
    /// <summary>
    /// 封装 PhotonNetwork.RaiseEvent，简化调用
    /// </summary>
    /// <param name="code">事件码（暗号）</param>
    /// <param name="content">【可选】要发送的具体内容（可以是 int、string、数组、类对象等）</param>
    private void SendEvent(EventCodes code, object content = null) {
        // ------------------------------------------------
        // 参数1：决定发给谁 (ReceiverGroup)
        // ------------------------------------------------
        // ReceiverGroup.All：发给房间里所有人（包括自己）
        // ReceiverGroup.Others：发给除了自己以外的所有人
        // ReceiverGroup.MasterClient：只发给房主
        // 这里的逻辑：如果是“开始游戏”或“退出”，发给所有人；否则只发给对手
        ReceiverGroup target = (code == EventCodes.StartGame || code == EventCodes.ExitOnline)
            ? ReceiverGroup.All
            : ReceiverGroup.Others;

        // ------------------------------------------------
        // 参数2：配置发送选项 (RaiseEventOptions)
        // ------------------------------------------------
        // 这里只设置了 Receivers（发给谁）
        // 这个类还可以设置 CachingOption（是否缓存消息，给晚进房间的人看）等
        RaiseEventOptions options = new RaiseEventOptions { Receivers = target };

        // ------------------------------------------------
        // 参数3：具体发送 (PhotonNetwork.RaiseEvent)
        // ------------------------------------------------
        // 第1个参数 (eventCode): byte 类型的暗号
        // 第2个参数 (customData): object 类型的内容，你想传什么就传什么
        // 第3个参数 (raiseEventOptions): 上面配置的“发给谁”
        // 第4个参数 (sendOptions): 发送可靠性
        //    - SendOptions.SendReliable: 【默认】可靠传输（像打电话，确保对方一定收到，丢包会重发）
        //    - SendOptions.SendUnreliable: 不可靠传输（像广播，没收到就算了，用于同步位置这种高频但丢一两次无所谓的）
        PhotonNetwork.RaiseEvent((byte)code, content, options, SendOptions.SendReliable);
    }


    // ==========================================
    // 5. 【核心】接收消息的方法 (OnEvent)
    // ==========================================
    /// <summary>
    /// 【IOnEventCallback 接口方法】
    /// 只要房间里有人发 RaiseEvent，这个方法就会自动被调用
    /// </summary>
    /// <param name="photonEvent">包含了收到的所有信息（暗号、内容、谁发的等）</param>
    public void OnEvent(EventData photonEvent) {
        // 5.1 提取“暗号”：看看对方发的是什么指令
        byte code = photonEvent.Code;

        // 5.2 提取“内容”：看看对方发的具体数据是什么（可能是 null）
        object data = photonEvent.CustomData;

        // 5.3 根据不同的“暗号”，执行不同的游戏逻辑
        switch (code) {
            case (byte)EventCodes.PlayerReady:
                Debug.Log("[事件] 接收到对手准备 ");
                // 通过 EventCenter 广播给本地的 UI 和游戏逻辑
                EventCenter.Broadcast(EventType.EnemyPrepare);
                break;

            case (byte)EventCodes.StartGame:
                Debug.Log("[事件] 接收到游戏开始");
                EventCenter.Broadcast(EventType.StartGame);
                break;

            case (byte)EventCodes.GameTrue:
                Debug.Log("[事件] 接收到游戏胜利");
                EventCenter.Broadcast(EventType.GameTrue);
                break;

            case (byte)EventCodes.GameTrueIsEnemyToFalse:
                Debug.Log("[事件] 接收到游戏胜利(对手认输)");
                EventCenter.Broadcast(EventType.EnemyToFalse);
                break;

            case (byte)EventCodes.Attack:
                Debug.Log("[事件] 接收到攻击");
                // 这里的 data 是我们发过来的 int count，记得强制转换类型
                EventCenter.Broadcast(EventType.Attack, (int)data);
                break;

            case (byte)EventCodes.UpdateEnemyMap:
                Debug.Log("[事件] 接收到敌人地图");
                // 这里的 data 是 int[] map
                EventCenter.Broadcast(EventType.UpdateEnemyMap, (int[])data);
                break;

            case (byte)EventCodes.ExitOnline:
                Debug.Log("[事件] 接收到退出命令");
                EventCenter.Broadcast(EventType.ExitOnlineGame);
                break;
        }
    }

    // ==========================================
    // 6. Photon 常用回调函数 (继承自 MonoBehaviourPunCallbacks)
    // ==========================================
    // 这些方法会在特定时机自动被 Photon 调用，你不需要手动调用它们

    /// <summary>
    /// 当与 Photon 服务器断开连接时自动调用
    /// </summary>
    /// <param name="cause">断开原因（枚举类型，比如超时、被踢出等）</param>
    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("已和服务器断开连接, 自动返回单机场景。断开原因: " + cause);
    }

    /// <summary>
    /// 当“我”成功离开房间时自动调用
    /// </summary>
    public override void OnLeftRoom() {
        Debug.Log("主动返回单机场景");
        // 离开成功后，通过 YooAsset 加载单机场景
        StartCoroutine(LoadScene("LocalGame"));
    }

    /// <summary>
    /// 当“其他玩家”离开房间时自动调用
    /// </summary>
    /// <param name="otherPlayer">离开的那个玩家的信息</param>
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        // 修改 UI 状态
        ctrl.viewOnline.SetStateText("有玩家离开");
        // 广播本地事件，重置游戏
        EventCenter.Broadcast(EventType.PlayerExit);
        // 重置准备人数
        ctrl.modelOnline.prepareCount = 0;
    }

    /// <summary>
    /// 当“其他玩家”进入房间时自动调用
    /// </summary>
    /// <param name="newPlayer">刚进来的那个玩家的信息</param>
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        ctrl.viewOnline.SetStateText("有玩家加入\r\n(双方准备后将自动开始游戏)");
        ctrl.viewOnline.UpdateRoomCount();
        ctrl.viewOnline.PrepareGameButtonAction(true);
        ctrl.viewOnline.prepareGameButton.gameObject.SetActive(true);
        ctrl.modelOnline.prepareCount = 0;
    }

    // ==========================================
    // 7. 场景切换
    // ==========================================
    /// <summary>
    /// 异步加载场景的协程
    /// </summary>
    /// <param name="name">要加载的场景名称</param>
    private IEnumerator LoadScene(string name) {
        // 使用 YooAsset 异步加载
        SceneHandle handle = YooAssets.LoadSceneAsync(name);

        // 等待加载完成（每帧检查一次）
        while (!handle.IsDone) {
            Debug.Log("正在加载场景...");
            Debug.Log($"加载进度: {handle.Progress * 100:F1}%");
            yield return null; // 挂起一帧
        }

        Debug.Log("场景加载完成");
    }
}
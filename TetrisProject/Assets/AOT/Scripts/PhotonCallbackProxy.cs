using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonCallbackProxy : MonoBehaviourPunCallbacks {
    public static PhotonCallbackProxy Instance { get; private set; }

    // 定义事件，供热更新订阅
    public event System.Action OnLeftRoomEvent;
    public event System.Action<DisconnectCause> OnDisconnectedEvent;
    public event System.Action<Player> OnPlayerLeftRoomEvent;
    public event System.Action<Player> OnPlayerEnteredRoomEvent;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保证跨场景存活
            PhotonNetwork.AddCallbackTarget(this); // 确保回调注册
        } else {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // 实现 Photon 回调，触发事件
    public override void OnLeftRoom() {
        OnLeftRoomEvent?.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        OnDisconnectedEvent?.Invoke(cause);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        OnPlayerLeftRoomEvent?.Invoke(otherPlayer);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        OnPlayerEnteredRoomEvent?.Invoke(newPlayer);
    }

    // 可根据需要添加其他回调（如 OnMasterClientSwitched 等）
}
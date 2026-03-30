using Photon.Pun;
using UnityEngine;

public class EventManagerOnline : MonoBehaviour
{
    private CtrlOnline ctrl;

    private void Awake() {
        ctrl = GetComponent<CtrlOnline>();

    }

    private void Start() {
        // 订阅事件
        ctrl.viewOnline.prepareGameButton.onClick.AddListener(OnPrepareGameButtonClick);
        ctrl.viewOnline.exitButton.onClick.AddListener(OnExitOnlineGameButtonClick);
        ctrl.viewOnline.toGameFalseButton.onClick.AddListener(OnToGameFalseClick);
        ctrl.viewOnline.settingButtonIn.onClick.AddListener(OnSettingButtonIn);
        ctrl.viewOnline.settingButtonOut.onClick.AddListener(OnSettingButtonOut);
        ctrl.viewOnline.settingPreviewButton.onClick.AddListener(OnSettingPreviewButtonClick);
        ctrl.viewOnline.settingParticleskButton.onClick.AddListener(OnSettingParticleskButtonClick);
        ctrl.viewOnline.settingFallDirectlyButton.onClick.AddListener(OnSettingFallDirectlyButtonClick);

        EventCenter.AddListener(EventType.GameOver, OnGameFalse);
        EventCenter.AddListener<int>(EventType.Attack, OnAttack);
        EventCenter.AddListener(EventType.EnemyPrepare, OnEnemyPrepare);
        EventCenter.AddListener(EventType.StartGame, StartGame);
        EventCenter.AddListener(EventType.GameTrue, OnGameTrue);
        EventCenter.AddListener(EventType.PlayerExit, OnPlayerExit);
        EventCenter.AddListener(EventType.EnemyToFalse, OnEnemyIsFalse);
        EventCenter.AddListener<int[]>(EventType.UpdateEnemyMap, OnUpdateEnemyMap);
    }

    private void OnDestroy() {
        // 取消订阅
        if (ctrl.viewOnline != null && ctrl.viewOnline.prepareGameButton != null) ctrl.viewOnline.prepareGameButton.onClick.RemoveListener(OnPrepareGameButtonClick);
        if (ctrl.viewOnline != null && ctrl.viewOnline.exitButton != null) ctrl.viewOnline.exitButton.onClick.RemoveListener(OnExitOnlineGameButtonClick);
        if (ctrl.viewOnline != null && ctrl.viewOnline.toGameFalseButton != null) ctrl.viewOnline.toGameFalseButton.onClick.RemoveListener(OnToGameFalseClick);
        if (ctrl.viewOnline != null && ctrl.viewOnline.settingButtonIn != null) ctrl.viewOnline.settingButtonIn.onClick.RemoveListener(OnSettingButtonIn);
        if (ctrl.viewOnline != null && ctrl.viewOnline.settingButtonOut != null) ctrl.viewOnline.settingButtonOut.onClick.RemoveListener(OnSettingButtonOut);
        if (ctrl.viewOnline != null && ctrl.viewOnline.settingPreviewButton != null) ctrl.viewOnline.settingPreviewButton.onClick.RemoveListener(OnSettingPreviewButtonClick);
        if (ctrl.viewOnline != null && ctrl.viewOnline.settingParticleskButton != null) ctrl.viewOnline.settingParticleskButton.onClick.RemoveListener(OnSettingParticleskButtonClick);
        if (ctrl.viewOnline != null && ctrl.viewOnline.settingFallDirectlyButton != null) ctrl.viewOnline.settingFallDirectlyButton.onClick.RemoveListener(OnSettingFallDirectlyButtonClick);

        EventCenter.RemoveListener(EventType.GameOver, OnGameFalse);
        EventCenter.RemoveListener<int>(EventType.Attack, OnAttack);
        EventCenter.RemoveListener(EventType.EnemyPrepare, OnEnemyPrepare);
        EventCenter.RemoveListener(EventType.StartGame, StartGame);
        EventCenter.RemoveListener(EventType.GameTrue, OnGameTrue);
        EventCenter.RemoveListener(EventType.PlayerExit, OnPlayerExit);
        EventCenter.RemoveListener(EventType.EnemyToFalse, OnEnemyIsFalse);
        EventCenter.RemoveListener<int[]>(EventType.UpdateEnemyMap, OnUpdateEnemyMap);

    }

    /// <summary>
    /// 准备按钮点击
    /// </summary>
    public void OnPrepareGameButtonClick() {
        // 修改状态
        ctrl.viewOnline.SetStateText("已准备\r\n(双方准备后将自动开始游戏)");

        // 重置游戏数据
        ctrl.gameManagerOnline.ResetDate();

        // 准备按钮锁上
        ctrl.viewOnline.PrepareGameButtonAction(false);

        // 告诉 "网络总控制器" 我准备了
        ctrl.networkCtrl.SendReady();

        // 房主统计人数
        CheckPrepareCount();

        // 播放声音
        ctrl.audioManagerOnline.PlayCursor();
    }

    /// <summary>
    /// 游戏胜利
    /// </summary>
    public void OnGameTrue() {
        // 暂停游戏
        ctrl.gameManagerOnline.PauseGame();

        // 修改状态为 "获胜"
        ctrl.viewOnline.SetStateText("获胜, 胜场+1");

        // 修改战绩
        ctrl.modelOnline.playerTrueCount++;
        ctrl.viewOnline.SetRecordText(ctrl.modelOnline.playerTrueCount, ctrl.modelOnline.enemyTrueCount);

        // 解锁准备按钮
        ctrl.viewOnline.PrepareGameButtonAction(true);

        // 游戏结束UI
        ctrl.viewOnline.GameOverUI();

        // 摄像机缩小
        ctrl.cameraManagerOnline.ZoomOut();
        
        // 播放音效 (游戏胜利暂时没有音效，先用消除方块的)
        ctrl.audioManagerOnline.PlayLineclear();

        // 待定
    }

    /// <summary>
    /// 游戏失败
    /// </summary>
    public void OnGameFalse() {
        // 暂停游戏
        ctrl.gameManagerOnline.PauseGame();

        // 修改状态为 "获胜"
        ctrl.viewOnline.SetStateText("失败, 对方胜场+1");

        // 修改战绩
        ctrl.modelOnline.enemyTrueCount++;
        ctrl.viewOnline.SetRecordText(ctrl.modelOnline.playerTrueCount, ctrl.modelOnline.enemyTrueCount);

        // 解锁准备按钮
        ctrl.viewOnline.PrepareGameButtonAction(true);

        // 游戏结束UI
        ctrl.viewOnline.GameOverUI();

        // 摄像机缩小
        ctrl.cameraManagerOnline.ZoomOut();

        // 播放音效
        ctrl.audioManagerOnline.PlayGameOver();

        // 待定
    }

    /// <summary>
    /// 对手准备就绪
    /// </summary>
    public void OnEnemyPrepare() {
        // 设置状态信息
        ctrl.viewOnline.SetStateText("对手准备就绪\r\n(双方准备后将自动开始游戏)");

        // 房主统计人数
        CheckPrepareCount();
    }

    /// <summary>
    /// 攻击对方
    /// </summary>
    public void OnAttack(int count) {
        ctrl.modelOnline.AddRubbisRowCount(ConfigManager.Instance.GameConfig.rubbisonline, ctrl.gameManagerOnline.Temp.transform, count);
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    public void StartGame() {
        Debug.Log("开始游戏");

        // 修改状态信息
        ctrl.viewOnline.SetStateText("游戏中...");

        // 摄像机放大
        ctrl.cameraManagerOnline.ZoomIn();

        // 游戏开始UI
        ctrl.viewOnline.StartGameUI();

        // 开始游戏
        ctrl.gameManagerOnline.StartGame();
    }
    
    /// <summary>
    /// 更新敌人地图
    /// </summary>
    /// <param name="client"></param>
    public void OnUpdateEnemyMap(int[] map) {
        ctrl.viewOnline.UpdateEnemyMap(map, ModelOnline.CURRENT_MAP_ROWS, ModelOnline.CURRENT_MAP_COLUMNS);
    }


    /// <summary>
    /// 只有房主才能调用这个方法进行统计准备人数
    /// 人数达到 2 人则可以广播 开始游戏
    /// </summary>
    /// <param name="isMasterClient"></param>
    private void CheckPrepareCount() {
        // 不是房主的退出去
        if (!PhotonNetwork.IsMasterClient) return;

        // 房主统计准备人数
        ctrl.modelOnline.prepareCount++;
        Debug.Log("当前准备人数: " + ctrl.modelOnline.prepareCount);

        if (ctrl.modelOnline.prepareCount == 2) {
            // 通过NetworkCtrl 广播开始游戏
            ctrl.networkCtrl.StartGame();

            ctrl.modelOnline.prepareCount = 0;
        }
    }

    /// <summary>
    /// 返回单人联机模式按钮点击
    /// </summary>
    public void OnExitOnlineGameButtonClick() {
        //Debug.Log("准备1");
        //// 离开当前房间
        //PhotonNetwork.LeaveRoom();
        //Debug.Log("准备2");

        // ========== 核心修复1：强制开启Photon消息队列（最常见的坑！）==========
        // 消息队列关闭时，Photon不会处理任何服务器回调，OnLeftRoom永远不会触发
        if (!PhotonNetwork.IsMessageQueueRunning) {
            Debug.LogError("=== 发现Photon消息队列被关闭，已强制开启 ===");
            PhotonNetwork.IsMessageQueueRunning = true;
        }

        Debug.Log($"LeaveRoom 前 - InRoom: {PhotonNetwork.InRoom}, State: {PhotonNetwork.NetworkClientState}");
        PhotonNetwork.LeaveRoom();
        Debug.Log($"LeaveRoom 后 - InRoom: {PhotonNetwork.InRoom}, State: {PhotonNetwork.NetworkClientState}");

        // 修改状态
        ctrl.viewOnline.SetStateText("退出房间中...");

        // 播放音效
        ctrl.audioManagerOnline.PlayCursor();

        //StartCoroutine(LoadScene("LocalGame"));
    }

    //// 测试
    //private IEnumerator LoadScene(string name) {
    //    SceneHandle handle = YooAssets.LoadSceneAsync(name);
    //    while (!handle.IsDone) {
    //        Debug.Log("正在加载");
    //        Debug.Log($"加载进度: {handle.Progress * 100:F1}%");
    //        yield return null;
    //    }
    //    Debug.Log("加载完成");

    //    // 跳转
    //}

    /// <summary>
    /// 当有玩家离开时
    /// </summary>
    public void OnPlayerExit() {
        // 暂停游戏
        ctrl.gameManagerOnline.PauseGame();

        // UI恢复初始状态
        ctrl.viewOnline.ResetUI();

        // 数据重置
        ctrl.modelOnline.ResetToInitDate();

        // 重置相机
        ctrl.cameraManagerOnline.ZoomOut();
    }

    /// <summary>
    /// 认输按钮点击
    /// </summary>
    public void OnToGameFalseClick() {
        // 暂停游戏
        ctrl.gameManagerOnline.PauseGame();

        // 修改状态为 "获胜"
        ctrl.viewOnline.SetStateText("失败, 对方胜场+1\r\n你认输了, 懦夫");

        // 修改战绩
        ctrl.modelOnline.enemyTrueCount++;
        ctrl.viewOnline.SetRecordText(ctrl.modelOnline.playerTrueCount, ctrl.modelOnline.enemyTrueCount);

        // 解锁准备按钮
        ctrl.viewOnline.PrepareGameButtonAction(true);

        // 游戏结束UI
        ctrl.viewOnline.GameOverUI();

        // 摄像机缩小
        ctrl.cameraManagerOnline.ZoomOut();

        // 发送 我认输了
        ctrl.networkCtrl.GameTrueIsEnemyToFalse();

        // 播放音效
        ctrl.audioManagerOnline.PlayCursor();

        // 待定
    }


    /// <summary>
    /// 游戏胜利(对手认输)
    /// </summary>
    public void OnEnemyIsFalse() {
        // 暂停游戏
        ctrl.gameManagerOnline.PauseGame();

        // 修改状态为 "获胜"
        ctrl.viewOnline.SetStateText("获胜, 胜场+1\r\n对手由于自己的懦弱, 已认输");

        // 修改战绩
        ctrl.modelOnline.playerTrueCount++;
        ctrl.viewOnline.SetRecordText(ctrl.modelOnline.playerTrueCount, ctrl.modelOnline.enemyTrueCount);

        // 解锁准备按钮
        ctrl.viewOnline.PrepareGameButtonAction(true);

        // 游戏结束UI
        ctrl.viewOnline.GameOverUI();

        // 摄像机缩小
        ctrl.cameraManagerOnline.ZoomOut();

        // 待定
        // 播放音效 (游戏胜利暂时没有音效，先用消除方块的)
        ctrl.audioManagerOnline.PlayLineclear();
    }


    /// <summary>
    /// 打开设置按钮点击
    /// </summary>
    private void OnSettingButtonIn() {
        ctrl.viewOnline.ShowSettingUI();

        // 播放音效
        ctrl.audioManagerOnline.PlayCursor();
    }

    /// <summary>
    /// 关闭设置按钮点击
    /// </summary>
    private void OnSettingButtonOut() {
        ctrl.viewOnline.HideSettingUI();

        // 播放音效
        ctrl.audioManagerOnline.PlayCursor();
    }

    /// <summary>
    /// 预览按钮点击
    /// </summary>
    private void OnSettingPreviewButtonClick() {
        ModelOnline.Instance.isPreview = !ModelOnline.Instance.isPreview;
        ctrl.viewOnline.SetSettingPreviewActive();

        // 播放音效
        ctrl.audioManagerOnline.PlayCursor();
    }

    /// <summary>
    /// 投特效按钮点击
    /// </summary>
    private void OnSettingParticleskButtonClick() {
        ModelOnline.Instance.isParticles = !ModelOnline.Instance.isParticles;
        ctrl.viewOnline.SetSettingParticleskActive();

        // 播放音效
        ctrl.audioManagerOnline.PlayCursor();
    }

    /// <summary>
    /// 直接下落按钮点击
    /// </summary>
    private void OnSettingFallDirectlyButtonClick() {
        ModelOnline.Instance.isFallDirectly = !ModelOnline.Instance.isFallDirectly;
        ctrl.viewOnline.SetSettingFallDirectlyButton();

        // 播放音效
        ctrl.audioManagerOnline.PlayCursor();
    }

}

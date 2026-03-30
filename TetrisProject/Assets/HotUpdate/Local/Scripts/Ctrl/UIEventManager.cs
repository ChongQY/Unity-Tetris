using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class UIEventManager : MonoBehaviour
{
    private Ctrl ctrl;

    private void Awake() {
        ctrl = GetComponent<Ctrl>();

    }

    private void Start() {
        // -- 订阅事件

        // - 主界面
        // > 按钮点击事件
        ctrl.view.menuExitButton.onClick.AddListener(OnMenuGameExitButtonClick);
        ctrl.view.menuGameOnlineButton.onClick.AddListener(OnMenuGameOnlineButtonClick);
        ctrl.view.menuStartButton.onClick.AddListener(OnmMenuStartButtonClick);
        ctrl.view.menuSettingButton.onClick.AddListener(OnMenuSettingClick);
        ctrl.view.menuRankButton.onClick.AddListener(OnMenuRankButtonClick);


        // - 暂停界面
        // > 按钮点击事件
        ctrl.view.pauseStartButton.onClick.AddListener(OnPauseStartButtonClick);
        ctrl.view.pauseRestartButton.onClick.AddListener(OnPauseRestartButtonClick);
        ctrl.view.pauseSettingButton.onClick.AddListener(OnPauseSettingClick);
        ctrl.view.pauseHomeButton.onClick.AddListener(OnPauseHomeButtonClick);


        // - 运行时界面
        // > 按钮点击事件
        ctrl.view.gamePauseButton.onClick.AddListener(OnGamePauseButtonClick);


        // - 游戏结束界面
        // > 按钮点击事件
        ctrl.view.gameOverRestartButton.onClick.AddListener(OnGameOverRestartButtonClick);
        ctrl.view.gameOverHomeButton.onClick.AddListener(OnGameOverHomeButtonClick);


        // - 设置界面
        // > 按钮点击事件
        ctrl.view.settingbkBtn.onClick.AddListener(OnSettingUIbkBtnClick);
        ctrl.view.settingAudioButton.onClick.AddListener(OnSettingUIAudioButtonClick);
        ctrl.view.settingPreviewButton.onClick.AddListener(OnSettingPreviewButtonClick);
        ctrl.view.settingParticlesButton.onClick.AddListener(OnSettingParticlesButtonClick);
        ctrl.view.settingFallDirectlyButton.onClick.AddListener(OnSettingFallDirectlyButtonClick);
        // > 滑动条值改变事件
        ctrl.view.settingStepTimeSSlider.onValueChanged.AddListener(OnSettingStepTimeSSliderValueChanged);
        ctrl.view.settingHorizontalMoveTimeKeySlider.onValueChanged.AddListener(OnSettingHorizontalMoveTimeKeySliderValueChanged);


        // - 历史记录面板
        // > 按钮点击事件
        ctrl.view.rankbkButton.onClick.AddListener(OnRankbkButtonClick);
        ctrl.view.rankReleButton.onClick.AddListener(OnRankReleButtonClick);

        // - 联机大厅
        // > 按钮点击事件
        ctrl.view.onlineSalaExitBtn.onClick.AddListener(OnOnlineExitButtonClick);
        ctrl.view.onlineSalaCreateBtn.onClick.AddListener(OnOnlineCreateRoomButtonClike);
        ctrl.view.onlineSalaAddBtn.onClick.AddListener(OnOnlineAddRoomButtonClike);

        
    }

    private void OnDestroy() {
        // -- 取消订阅

        // - 主界面
        // > 按钮点击事件
        if (ctrl.view != null && ctrl.view.menuExitButton != null) { ctrl.view.menuExitButton.onClick.RemoveListener(OnMenuGameExitButtonClick); }
        if (ctrl.view != null && ctrl.view.menuGameOnlineButton != null) { ctrl.view.menuGameOnlineButton.onClick.RemoveListener(OnMenuGameOnlineButtonClick); }
        if (ctrl.view != null && ctrl.view.menuStartButton != null) { ctrl.view.menuStartButton.onClick.RemoveListener(OnmMenuStartButtonClick); }
        if (ctrl.view != null && ctrl.view.menuSettingButton != null) { ctrl.view.menuSettingButton.onClick.RemoveListener(OnMenuSettingClick); }
        if (ctrl.view != null && ctrl.view.menuRankButton != null) { ctrl.view.menuRankButton.onClick.RemoveListener(OnMenuRankButtonClick); }


        // - 暂停界面
        // > 按钮点击事件
        if (ctrl.view != null && ctrl.view.pauseStartButton != null) { ctrl.view.pauseStartButton.onClick.RemoveListener(OnPauseStartButtonClick); }
        if (ctrl.view != null && ctrl.view.pauseRestartButton != null) { ctrl.view.pauseRestartButton.onClick.RemoveListener(OnPauseRestartButtonClick); }
        if (ctrl.view != null && ctrl.view.pauseSettingButton != null) { ctrl.view.pauseSettingButton.onClick.RemoveListener(OnPauseSettingClick); }
        if (ctrl.view != null && ctrl.view.pauseHomeButton != null) { ctrl.view.pauseHomeButton.onClick.RemoveListener(OnPauseHomeButtonClick); }


        // - 运行时界面
        // > 按钮点击事件
        if (ctrl.view != null && ctrl.view.gamePauseButton != null) { ctrl.view.gamePauseButton.onClick.RemoveListener(OnGamePauseButtonClick); }


        // - 游戏结束界面
        // > 按钮点击事件
        if (ctrl.view != null && ctrl.view.gameOverRestartButton != null) { ctrl.view.gameOverRestartButton.onClick.RemoveListener(OnGameOverRestartButtonClick); }
        if (ctrl.view != null && ctrl.view.gameOverHomeButton != null) { ctrl.view.gameOverHomeButton.onClick.RemoveListener(OnGameOverHomeButtonClick); }


        // - 设置界面
        // > 按钮点击事件
        if (ctrl.view != null && ctrl.view.settingbkBtn != null) { ctrl.view.settingbkBtn.onClick.RemoveListener(OnSettingUIbkBtnClick); }
        if (ctrl.view != null && ctrl.view.settingAudioButton != null) { ctrl.view.settingAudioButton.onClick.RemoveListener(OnSettingUIAudioButtonClick); }
        if (ctrl.view != null && ctrl.view.settingPreviewButton != null) { ctrl.view.settingPreviewButton.onClick.RemoveListener(OnSettingPreviewButtonClick); }
        if (ctrl.view != null && ctrl.view.settingParticlesButton != null) { ctrl.view.settingParticlesButton.onClick.RemoveListener(OnSettingParticlesButtonClick); }
        if (ctrl.view != null && ctrl.view.settingFallDirectlyButton != null) { ctrl.view.settingFallDirectlyButton.onClick.RemoveListener(OnSettingFallDirectlyButtonClick); }
        // > 滑动条值改变事件
        if (ctrl.view != null && ctrl.view.settingStepTimeSSlider != null) { ctrl.view.settingStepTimeSSlider.onValueChanged.RemoveListener(OnSettingStepTimeSSliderValueChanged); }
        if (ctrl.view != null && ctrl.view.settingHorizontalMoveTimeKeySlider != null) { ctrl.view.settingHorizontalMoveTimeKeySlider.onValueChanged.RemoveListener(OnSettingHorizontalMoveTimeKeySliderValueChanged); }

        // - 历史记录面板
        // > 按钮点击事件
        if (ctrl.view != null && ctrl.view.rankbkButton != null) { ctrl.view.rankbkButton.onClick.RemoveListener(OnRankbkButtonClick); }
        if (ctrl.view != null && ctrl.view.rankReleButton != null) { ctrl.view.rankReleButton.onClick.RemoveListener(OnRankReleButtonClick); }


        // - 联机大厅
        // > 按钮点击事件
        if (ctrl.view != null && ctrl.view.onlineSalaExitBtn != null) { ctrl.view.onlineSalaExitBtn.onClick.RemoveListener(OnOnlineExitButtonClick); }
        if (ctrl.view != null && ctrl.view.onlineSalaCreateBtn != null) { ctrl.view.onlineSalaCreateBtn.onClick.RemoveListener(OnOnlineCreateRoomButtonClike); }
        if (ctrl.view != null && ctrl.view.onlineSalaAddBtn != null) { ctrl.view.onlineSalaAddBtn.onClick.RemoveListener(OnOnlineAddRoomButtonClike); }

       }

    #region 主界面

    // 开始按钮点击
    public void OnmMenuStartButtonClick() {
        // 播放点击声音
        ctrl.audioManager.PlayCursor();

        // 广播开始按钮点击按钮点击
        EventCenter.Broadcast(EventType.MenuStartButtonClick);
        Debug.Log("广播了开始按钮点击");
    }

    /// <summary>
    /// 设置按钮点击
    /// </summary>
    public void OnMenuSettingClick() {
        // 显示面板
        ctrl.view.ShowSettingUI();

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
        Debug.Log("显示了设置面板");
    }

    /// <summary>
    /// 历史记录按钮点击
    /// </summary>
    public void OnMenuRankButtonClick() {
        // 显示面板
        ctrl.view.ShowRankUI();

        // 更新面板
        ctrl.view.UpdateRankUI(ctrl.model.scoreLabel, ctrl.model.higeScoreLabel, ctrl.model.gameCount);

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    /// <summary>
    /// 游戏退出按钮点击
    /// </summary>
    public void OnMenuGameExitButtonClick() {
        // 播放点击声音
        ctrl.audioManager.PlayCursor();

        // 退出游戏
        Application.Quit();
    }

    /// <summary>
    /// 双人对战按钮点击
    /// </summary>
    public void OnMenuGameOnlineButtonClick() {
        Debug.Log("进入双人对战");

        // 显示联机大厅UI
        ctrl.view.onlineSalaUI.gameObject.SetActive(true);
        // 开启创建/加入按钮
        ctrl.view.onlineSalaCreateBtn.interactable = false;
        ctrl.view.onlineSalaAddBtn.interactable = false;

        if (!PhotonNetwork.IsConnected) {
            // 更新状态文字
            ctrl.view.onlineSalaStartText.text = "正在连接服务器...";
            // 连接服务器
            ctrl.networkingManager.ConnectServer();
        } else {
            ctrl.view.onlineSalaStartText.text = "已连接，可以创建或加入房间";
            // 开启创建/加入按钮
            ctrl.view.onlineSalaCreateBtn.interactable = true;
            ctrl.view.onlineSalaAddBtn.interactable = true;
        }

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    #endregion

    #region 暂停界面

    // (继续)按钮点击
    public void OnPauseStartButtonClick() {
        // 广播暂停界面暂停按钮点击
        EventCenter.Broadcast(EventType.PauseStartButtonClick);

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
        Debug.Log("暂停界面开始按钮点击");
    }

    // 重新开始按钮点击
    public void OnPauseRestartButtonClick() {
        // 保存数据
        ctrl.model.SaveData();
        // 重置数据
        ctrl.gameManager.ResetDate();

        // 广播暂停界面重新开始按钮点击
        EventCenter.Broadcast(EventType.PauseRestartButtonClick);

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    // 设置按钮点击
    public void OnPauseSettingClick() {
        ctrl.view.ShowSettingUI();

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
        Debug.Log("显示了设置面板");

    }

    // 主界面按钮点击
    public void OnPauseHomeButtonClick() {
        // 保存数据
        ctrl.model.SaveData();

        // 广播暂停界面返回主界面按钮点击
        EventCenter.Broadcast(EventType.PauseHomeButtonClick);

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    #endregion

    #region 运行时游戏界面
    /// 暂停按钮点击
    public void OnGamePauseButtonClick() {
        // 播放点击声音
        ctrl.audioManager.PlayCursor();

        // 广播游戏运行界面暂停按钮点击
        EventCenter.Broadcast(EventType.GamePauseButtonButtonClick);
        
    }
    #endregion

    #region 游戏结束界面
    // 游戏结束界面重新开始按钮点击
    private void OnGameOverRestartButtonClick() {
        // 重置数据
        // 离开的时候重置可以看到上一次的堆叠状态
        ctrl.gameManager.ResetDate();

        // 广播游戏结束界面重新开始按钮点击
        EventCenter.Broadcast(EventType.GameOverRestartButtonClick);

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    // 游戏结束界面返回大厅按钮点击
    private void OnGameOverHomeButtonClick() {
        // 广播游戏结束界面重新开始按钮点击
        EventCenter.Broadcast(EventType.GameOverHomeButtonClick);

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    #endregion

    #region 设置界面

    /// <summary>
    /// 设置界面背景点击
    /// 退出设置界面
    /// </summary>
    public void OnSettingUIbkBtnClick() {
        // 隐藏设置界面
        ctrl.view.HideSettingUI();

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    /// <summary>
    /// 下落参数改变时
    /// </summary>
    /// <param name="v"></param>
    public void OnSettingStepTimeSSliderValueChanged(float v) {
        // 更新系数
        ctrl.model.stepTimeS = v;

        // 更新UI
        ctrl.view.UpdateSettingStepTimeSText(v);
    }

    /// <summary>
    /// 下落参数改变时
    /// </summary>
    /// <param name="v"></param>
    public void OnSettingHorizontalMoveTimeKeySliderValueChanged(float v) {
        // 更新系数
        ctrl.model.horizontalMoveTimeKey = v;

        // 更新UI
        ctrl.view.UpdateSettingHorizontalMoveTimeKeyTextt(v);
    }


    /// <summary>
    /// 声音开关
    /// </summary>
    public void OnSettingUIAudioButtonClick() {
        // 更新静音状态
        ctrl.model.isMute = !ctrl.model.isMute;
        ctrl.view.SetSettingAudioButtonActive();
    }

    /// <summary>
    /// 预览按钮点击
    /// </summary>
    public void OnSettingPreviewButtonClick() {
        ctrl.model.isPreview = !ctrl.model.isPreview;
        ctrl.view.SetSettingPreviewActive();
    }

    /// <summary>
    /// 粒子特效按钮点击
    /// </summary>
    public void OnSettingParticlesButtonClick() {
        ctrl.model.isParticles = !ctrl.model.isParticles;
        ctrl.view.SetSettingParticlesActive();
    }

    /// <summary>
    /// 下落按钮点击
    /// </summary>
    public void OnSettingFallDirectlyButtonClick() {
        ctrl.model.isFallDirectly = !ctrl.model.isFallDirectly;
        ctrl.view.SetSettingFallDirectlyButton();
    }

    #endregion

    #region 历史记录面板

    /// <summary>
    /// 历史记录界面背景点击
    /// 退出历史记录界面
    /// </summary>
    public void OnRankbkButtonClick() {
        // 隐藏历史记录面板
        ctrl.view.HideRankUI();

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    /// <summary>
    /// 数据重置按钮点击
    /// </summary>
    public void OnRankReleButtonClick() {
        // 清除之前的数据
        ctrl.model.ClearData();

        // 更新UI
        ctrl.view.UpdateRankUI(ctrl.model.scoreLabel, ctrl.model.higeScoreLabel, ctrl.model.gameCount);

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    #endregion

    #region 联机大厅

    /// <summary>
    /// 退出按钮点击
    /// </summary>
    public void OnOnlineExitButtonClick() {
        // 隐藏联机大厅UI
        ctrl.view.onlineSalaUI.gameObject.SetActive(false);

        // 与服务器断开连接
        PhotonNetwork.Disconnect();

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }


    /// <summary>
    /// 创建房间按钮点击
    /// </summary>
    public void OnOnlineCreateRoomButtonClike() {
        // 房间名称
        string name = ctrl.view.onlineSalaInputField.text;

        if (string.IsNullOrEmpty(name)) {
            // 提示输入房间名
            ctrl.view.onlineSalaStartText.text = "房间名不能为空";
            return;
        }

        // 是否在房间内
        if (PhotonNetwork.InRoom) {
            // 离开房间                                                           
            ctrl.networkingManager.ExitRomm();
            // 修改状态
            ctrl.view.onlineSalaStartText.text = "离开房间中...";
        } else {
            // 设置房间人数
            RoomOptions options = new RoomOptions { MaxPlayers = 2 };  // 设置最大玩家数为2
            // 创建房间                                                           
            ctrl.networkingManager.CreateRoom(name, options);
            // 修改状态
            ctrl.view.onlineSalaStartText.text = "创建房间中...";
        }

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }

    /// <summary>
    /// 加入 房间按钮点击
    /// </summary>
    public void OnOnlineAddRoomButtonClike() {
        // 房间名称
        string name = ctrl.view.onlineSalaInputField.text;

        if (string.IsNullOrEmpty(name)) {
            // 提示输入房间名
            ctrl.view.onlineSalaStartText.text = "房间名不能为空";
            return;
        }

        // 加入房间
        ctrl.networkingManager.JoinRoom(name);
        // 修改状态
        ctrl.view.onlineSalaStartText.text = "加入房间中...";

        // 播放点击声音
        ctrl.audioManager.PlayCursor();
    }
    #endregion


}

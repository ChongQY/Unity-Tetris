using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    [Header("--- 标题 ---")]
    [Tooltip("主界面标题\"俄罗斯方块\"")]
    [SerializeField] private RectTransform logoName;

    [Tooltip("暂停标题\"游戏暂停\"")]
    [SerializeField] private RectTransform pauseTitle;


    [Space(15)]
    [Header("--- 主界面 ---")]
    [Tooltip("主界面UI")]
    [SerializeField] private RectTransform menuUI;

    [Tooltip("主界面 - 退出按钮")]
    public Button menuExitButton;

    [Tooltip("主界面 - 双人对战按钮")]
    public Button menuGameOnlineButton;

    [Tooltip("主界面 - 开始游戏按钮")]
    public Button menuStartButton;

    [Tooltip("主界面 - 设置按钮")]
    public Button menuSettingButton;

    [Tooltip("主界面 - 记录按钮")]
    public Button menuRankButton;


    [Space(15)]
    [Header("--- 暂停界面 ---")]
    [Tooltip("暂停界面 - 暂停界面UI")]
    [SerializeField] private RectTransform pauseUI;

    [Tooltip("暂停界面 - 重新开始按钮")]
    public Button pauseRestartButton;

    [Tooltip("暂停界面 - 继续游戏按钮")]
    public Button pauseStartButton;

    [Tooltip("暂停界面 - 设置按钮")]
    public Button pauseSettingButton;

    [Tooltip("暂停界面 - 返回主界面按钮")]
    public Button pauseHomeButton;


    [Space(15)]
    [Header("--- 游戏时界面 ---")]
    [Tooltip("游戏时界面 - 游戏时UI")]
    [SerializeField] private RectTransform gameUI;

    [Tooltip("游戏时界面 - 当前游戏分数")]
    [SerializeField] private TextMeshProUGUI gameScoreLabeltxt;

    [Tooltip("游戏时界面 - 历史最高游戏分数")]
    [SerializeField] private TextMeshProUGUI gameHigeScoreLabeltxt;

    [Tooltip("游戏时界面 - 暂停按钮")]
    public Button gamePauseButton;


    [Space(15)]
    [Header("--- 游戏结束界面 ---")]
    [Tooltip("游戏结束界面 - 游戏结束UI")]
    [SerializeField] private RectTransform gameOverUI;

    [Tooltip("游戏结束界面 - 游戏结束分数")]
    [SerializeField] private TextMeshProUGUI gameOverScoretxt;

    [Tooltip("游戏结束界面 - 重新开始按钮")]
    public Button gameOverRestartButton;

    [Tooltip("游戏结束界面 - 返回主界面按钮")]
    public Button gameOverHomeButton;


    [Space(15)]
    [Header("--- 设置界面 ---")]
    [Tooltip("设置界面 - 设置界面UI")]
    [SerializeField] private RectTransform settingUI;

    [Tooltip("设置界面 - 背景按钮")]
    public Button settingbkBtn;

    [Tooltip("设置界面 - 开关静音按钮")]
    public Button settingAudioButton;

    [Tooltip("设置界面 - 开关预览按钮")]
    public Button settingPreviewButton;

    [Tooltip("设置界面 - 开关粒子特效按钮")]
    public Button settingParticlesButton;

    [Tooltip("设置界面 - 开关直接下落按钮")]
    public Button settingFallDirectlyButton;

    [Tooltip("设置界面 - 下降系数")]
    public Slider settingStepTimeSSlider;

    [Tooltip("设置界面 - 左右移动系数")]
    public Slider settingHorizontalMoveTimeKeySlider;

    [Tooltip("设置界面 - 下降系数文字")]
    [SerializeField] private TextMeshProUGUI settingStepTimeSText;

    [Tooltip("设置界面 - 左右移动系数文字")]
    [SerializeField] private TextMeshProUGUI settingHorizontalMoveTimeKeyText;


    [Space(15)]
    [Header("--- 历史记录界面 ---")]
    [Tooltip("历史记录界面 - 历史记录UI")]
    [SerializeField] private RectTransform rankUI;

    [Tooltip("历史记录界面 - 当前分数")]
    [SerializeField] private TextMeshProUGUI rankUIScoreLabeltxt;

    [Tooltip("历史记录界面 - 最高分数")]
    [SerializeField] private TextMeshProUGUI rankUIHighLabeltxt;

    [Tooltip("历史记录界面 - 游戏次数")]
    [SerializeField] private TextMeshProUGUI rankUINumberGamesLabeltxt;

    [Tooltip("历史记录界面 - 清空按钮")]
    public Button rankReleButton;

    [Tooltip("历史记录界面 - 背景按钮")]
    public Button rankbkButton;


    [Space(15)]
    [Header("--- 联机大厅 ---")]
    [Tooltip("联机大厅 - 联机大厅UI")]
    public RectTransform onlineSalaUI;

    [Tooltip("联机大厅 - 退出按钮")]
    public Button onlineSalaExitBtn;

    [Tooltip("联机大厅 - 创建房间按钮")]
    public Button onlineSalaCreateBtn;

    [Tooltip("联机大厅 - 创建房间按钮文本")]
    public TextMeshProUGUI onlineSalaCreateBtnText;

    [Tooltip("联机大厅 - 加入房间按钮")]
    public Button onlineSalaAddBtn;

    [Tooltip("联机大厅 - 状态文本")]
    public TextMeshProUGUI onlineSalaStartText;

    [Tooltip("联机大厅 - 输入信息文本")]
    public TMP_InputField onlineSalaInputField;


    [Space(15)]
    [Header("--- 按键说明 ---")]
    [Tooltip("按键说明 - 按键组界面")]
    [SerializeField] private RectTransform keyGroup;

    [Tooltip("按键说明 - W")]
    [SerializeField] private RectTransform keyW;

    [Tooltip("按键说明 - S")]
    [SerializeField] private RectTransform keyS;

    [Tooltip("按键说明 - A")]
    [SerializeField] private RectTransform keyA;

    [Tooltip("按键说明 - D")]
    [SerializeField] private RectTransform keyD;

    [Tooltip("按键说明 - Space")]
    [SerializeField] private RectTransform keySpace;

    [Space(15)]
    [Header("--- 其他 ---")]
    [Tooltip("游戏地图 - 地图")]
    public Transform map;


    private void Awake() {}

    // 显示主界面UI
    public void ShowMenuUI() {
        // 显示标题 "俄罗斯方块"
        logoName.gameObject.SetActive(true);
        logoName.DOAnchorPosY(-162.2854f, 0.5f).SetEase(Ease.OutBack);

        // 显示下面几个主界面按钮
        menuUI.gameObject.SetActive(true);
        menuUI.DOAnchorPosY(110.02f, 0.5f).SetEase(Ease.OutBack);

        // 显示按键说明
        ShowKeyGroup();
    }

    // 隐藏主界面UI
    public void HideMenuUI() {
        // 隐藏标题 "俄罗斯方块"
        logoName.DOAnchorPosY(162.29f, 0.5f).SetEase(Ease.InBack).OnComplete(() => { 
            logoName.gameObject.SetActive(false); 
        });

        // 隐藏下面几个主界面按钮
        menuUI.DOAnchorPosY(-110.02f, 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            menuUI.gameObject.SetActive(false);
        });

        // 隐藏按键说明
        HideKeyGroup();
    }

    // 显示暂停界面UI
    public void ShowPauseUI() {
        // 显示标题 "游戏暂停"
        pauseTitle.gameObject.SetActive(true);
        pauseTitle.DOAnchorPosY(-162.2854f, 0.5f).SetEase(Ease.OutBack);

        // 显示下面几个暂停界面按钮
        pauseUI.gameObject.SetActive(true);
        pauseUI.DOAnchorPosY(110.02f, 0.5f).SetEase(Ease.OutBack);
    }

    // 隐藏暂停界面UI
    public void HidePauseUI() {
        // 隐藏标题 "游戏暂停"
        pauseTitle.DOAnchorPosY(162.29f, 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            pauseTitle.gameObject.SetActive(false);
        });

        // 隐藏下面几个暂停界面按钮
        pauseUI.DOAnchorPosY(-110.02f, 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            pauseUI.gameObject.SetActive(false);
        });
    }

    // 显示主界面 重新开始按钮
    public void ShowMenuRestartButton() {
        pauseRestartButton.gameObject.SetActive(true);
    }

    // 隐藏主界面 重新开始按钮
    public void HideMenuRestartButton() {
        pauseRestartButton.gameObject.SetActive(false);
    }

    // 显示游戏中的UI
    public void ShowGameUI() {
        // 更新分数
        UpdateGameScoreUI(Model.Instance.scoreLabel, Model.Instance.higeScoreLabel);

        // 显示分数UI
        gameUI.gameObject.SetActive(true);
        gameUI.DOAnchorPosY(-164.09f, 0.5f);
    }

    // 隐藏游戏中的UI
    public void HideGameUI() {
        // 分数UI
        gameUI.DOAnchorPosY(164.09f, 0.5f).OnComplete(() => {
            gameUI.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// 显示游戏结束UI
    /// </summary>
    /// <param name="scoreLabel"></param>
    public void ShowGameOverUI() {
        // 显示游戏结束UI
        gameOverUI.gameObject.SetActive(true);

        // 更新分数
        UpdateGameOverScoreUI(Model.Instance.scoreLabel);
    }

    /// <summary>
    /// 隐藏显示游戏结束UI
    /// </summary>
    public void HideGameOverUI() {
        // 显示游戏结束UI
        gameOverUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示设置面板UI
    /// </summary>
    public void ShowSettingUI() {
        settingUI.gameObject.SetActive(true);
        UpdateSettingStepTimeSText(Model.Instance.stepTimeS);
        UpdateSettingHorizontalMoveTimeKeyTextt(Model.Instance.horizontalMoveTimeKey);
        SetSettingPreviewActive();
        SetSettingParticlesActive();
        SetSettingAudioButtonActive();
        SetSettingFallDirectlyButton();
    }

    /// <summary>
    /// 隐藏设置面板UI
    /// </summary>
    public void HideSettingUI() {
        settingUI.gameObject.SetActive(false);
        Model.Instance.SaveData();// 关闭的时候保存一下数据
    }

    /// <summary>
    /// 显示历史面板UI
    /// </summary>
    public void ShowRankUI() {
        rankUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏历史面板UI
    /// </summary>
    public void HideRankUI() {
        rankUI.gameObject.SetActive(false);
    }

    

    /// <summary>
    /// 更新游戏结束界面分数
    /// </summary>
    /// <param name="scoreLabel"></param>
    public void UpdateGameOverScoreUI(int scoreLabel) {
        gameOverScoretxt.text = scoreLabel.ToString();
    }

    /// <summary>
    /// 更新游戏分数
    /// </summary>
    /// <param name="scoreLabel"></param>
    /// <param name="higeScoreLabel"></param>
    public void UpdateGameScoreUI(int scoreLabel , int higeScoreLabel) {
        gameScoreLabeltxt.text = scoreLabel.ToString();
        gameHigeScoreLabeltxt.text = higeScoreLabel.ToString();
    }

    /// <summary>
    /// 更新历史记录UI
    /// </summary>
    public void UpdateRankUI(int score , int highScore , int numberGames) {
        rankUIScoreLabeltxt.text = score.ToString();
        rankUIHighLabeltxt.text = highScore.ToString();
        rankUINumberGamesLabeltxt.text = numberGames.ToString();
    }

    /// <summary>
    /// 更新设置面板下降系数文字
    /// </summary>
    public void UpdateSettingStepTimeSText(float v) {
        settingStepTimeSSlider.value = v;
        settingStepTimeSText.text = "加速下落系数: " + ((int)(v * 1000)).ToString();
    }

    /// <summary>
    /// 更新设置面板下降系数文字
    /// </summary>
    public void UpdateSettingHorizontalMoveTimeKeyTextt(float v) {
        settingHorizontalMoveTimeKeySlider.value = v;
        settingHorizontalMoveTimeKeyText.text = "左右移动系数: " + ((int)(v * 100)).ToString();
    }

    /// <summary>
    /// 生成方块消除粒子特效
    /// </summary>
    /// <param name="rows"></param>
    public void CreateDestroyParticles() {
        foreach (int row in Model.Instance.deleteRowList) {
            GameObject game = Instantiate(ConfigManager.Instance.GameConfig.destroyParticles, map);
            Vector3 pos = game.transform.position;
            pos.y = row;
            game.transform.localPosition = pos;
        }

    }

    /// <summary>settingAudioButton
    /// 设置界面 - 静音按钮
    /// </summary>
    public void SetSettingAudioButtonActive() {
        Image image = settingAudioButton.gameObject.GetComponent<Image>();
        if (!Model.Instance.isMute) {
            image.color = new Color32(237, 149, 74, 255);
        } else {
            image.color = new Color32(128, 128, 128, 128);
        }
    }

    /// <summary>
    /// 设置界面 - 预览按钮
    /// </summary>
    public void SetSettingPreviewActive() {
        Image image = settingPreviewButton.gameObject.GetComponent<Image>();
        if (Model.Instance.isPreview) {
            image.color = new Color32(220, 102, 85, 255);
        } else {
            image.color = new Color32(128, 128, 128, 128);
        }
    }

    /// <summary>
    /// 设置界面 - 粒子特效按钮
    /// </summary>
    public void SetSettingParticlesActive() {
        Image image = settingParticlesButton.gameObject.GetComponent<Image>();
        if (Model.Instance.isParticles) {
            image.color = new Color32(92, 190, 228, 255);
        } else {
            image.color = new Color32(128, 128, 128, 128);
        }
    }

    /// <summary>
    /// 设置界面 - 下落按钮
    /// </summary>
    public void SetSettingFallDirectlyButton() {
        Image image = settingFallDirectlyButton.gameObject.GetComponent<Image>();
        if (Model.Instance.isFallDirectly) {
            image.color = new Color32(67, 186, 154, 255);
        } else {
            image.color = new Color32(128, 128, 128, 128);
        }
    }

    private Sequence mySequence;
    /// <summary>
    /// 显示按钮组界面
    /// </summary>
    private void ShowKeyGroup() {
        // 显示按钮组界面
        keyGroup.gameObject.SetActive(true);

        // 先停止并销毁之前的动画 
        if (mySequence != null) {
            mySequence.Kill();
            mySequence = null;
        }

        // 创建新的
        mySequence = DOTween.Sequence();
        mySequence.Append(keySpace.DOAnchorPosY(-91f, 0.1f).SetEase(Ease.OutBack))
            .Append(keyA.DOAnchorPosY(19.1f, 0.1f).SetEase(Ease.OutBounce))
            .Append(keyD.DOAnchorPosY(19.1f, 0.1f).SetEase(Ease.OutBounce))
            .Append(keyS.DOAnchorPosY(19.1f, 0.1f).SetEase(Ease.OutBounce))
            .Append(keyW.DOAnchorPosY(125.6f, 0.1f).SetEase(Ease.OutBounce))
            .AppendCallback(() => mySequence = null)
            .Play();


    }
    /// <summary>
    /// 隐藏按钮组界面
    /// </summary>
    private void HideKeyGroup() {
        // 先停止并销毁之前的动画 
        if (mySequence != null) {
            mySequence.Kill();
            mySequence = null;
        }

        // 创建新的
        mySequence = DOTween.Sequence();
        mySequence.Append(keySpace.DOAnchorPosY(-600f, 0.1f).SetEase(Ease.OutQuad))
            .Append(keyA.DOAnchorPosY(600f, 0.1f).SetEase(Ease.OutQuad))
            .Append(keyD.DOAnchorPosY(600f, 0.1f).SetEase(Ease.OutQuad))
            .Append(keyW.DOAnchorPosY(706.5f, 0.1f).SetEase(Ease.OutQuad))
            .Append(keyS.DOAnchorPosY(600f, 0.1f).SetEase(Ease.OutQuad))
            .AppendCallback(() => {
                keyGroup.gameObject.SetActive(false);
                mySequence = null;
            })
            .Play();
    }
}

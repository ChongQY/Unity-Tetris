using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewOnline : MonoBehaviour
{
    [Header("--- 标题 ---")]
    [Tooltip("主界面标题\"俄罗斯方块\"")]
    [SerializeField] private RectTransform logoName;

    [Space(15)]
    [Header("--- 房间信息 ---")]
    [Tooltip("房间信息框UI")]
    [SerializeField] private RectTransform roomInfoUI;
    [Tooltip("房间信息框 - 房间名")]
    public TextMeshProUGUI roomName;
    [Tooltip("房间信息框 - 房间当前人数")]
    public TextMeshProUGUI roomCount;
    [Tooltip("房间信息框 - 房间状态")]
    public TextMeshProUGUI roomState;

    [Space(15)]
    [Header("--- 按钮组 ---")]
    [Tooltip("下方按钮组UI")]
    [SerializeField] private RectTransform buttonsUI;
    [Tooltip("准备按钮")]
    public Button prepareGameButton;
    [Tooltip("返回单机主界面按钮")]
    public Button exitButton;
    [Tooltip("认输按钮")]
    public Button toGameFalseButton;
    [Tooltip("认输UI")]
    [SerializeField] private RectTransform toGameFalseUI;

    [Space(15)]
    [Header("--- 信息UI ---")]
    [Tooltip("信息UI")]
    [SerializeField] private RectTransform infoUI;
    [Tooltip("状态文本")]
    public TextMeshProUGUI stateText;
    [Tooltip("战绩文本")]
    public TextMeshProUGUI recordText;

    [Space(15)]
    [Header("--- 设置UI ---")]
    [Tooltip("设置UI")]
    public RectTransform settingUI;
    [Tooltip("设置按钮展开")]
    public Button settingButtonIn;
    [Tooltip("设置按钮收起")]
    public Button settingButtonOut;
    [Tooltip("预览按钮")]
    public Button settingPreviewButton;
    [Tooltip("特效按钮")]
    public Button settingParticleskButton;
    [Tooltip("直接下落按钮")]
    public Button settingFallDirectlyButton;
    

    [Space(15)]
    [Header("--- 地图相关 ---")]
    [Tooltip("玩家地图")]
    public Transform map;
    [Tooltip("敌人地图组件")]
    public EnemyMap enemyMap;



    private void Awake() {
        //buttonsUI = transform.Find("Canvas/ButtonsUI") as RectTransform;
        //infoUI = transform.Find("Canvas/InfoUI") as RectTransform;
        //logoName = transform.Find("Canvas/LogoName") as RectTransform;

        //roomInfoUI = transform.Find("Canvas/RoomInfo") as RectTransform;
        //roomName = transform.Find("Canvas/RoomInfo/hor/RoomName").GetComponent<TextMeshProUGUI>();
        //roomCount = transform.Find("Canvas/RoomInfo/hor/RoomCount").GetComponent<TextMeshProUGUI>();
        //roomState = transform.Find("Canvas/RoomInfo/hor/RoomState").GetComponent<TextMeshProUGUI>();

        //prepareGameButton = transform.Find("Canvas/ButtonsUI/PrepareGameButton").GetComponent<Button>();
        //exitButton = transform.Find("Canvas/ButtonsUI/ExitOnlineGameButton").GetComponent<Button>();
        //toGameFalseButton = transform.Find("Canvas/ToGameFalse").GetComponent<Button>();
        //toGameFalseUI = transform.Find("Canvas/ToGameFalse") as RectTransform;

        //stateText = transform.Find("Canvas/InfoUI/StateText").GetComponent<TextMeshProUGUI>();
        //recordText = transform.Find("Canvas/InfoUI/Record/RecordText").GetComponent<TextMeshProUGUI>();

        //map = transform.Find("Map").transform;
        //enemyMap = transform.Find("EnemyMap").GetComponent<EnemyMap>();
    }

    /// <summary>
    /// 修改状态信息
    /// </summary>
    /// <param name="text"></param>
    public void SetStateText(string text) {
        stateText.text = text;
    }

    // <summary>
    /// 修改战绩信息
    /// </summary>
    /// <param name="text"></param>
    public void SetRecordText(int playerCount, int enemyCount) {
        recordText.text = "我: " + playerCount + "<br>他: " + enemyCount;
    }

    // 更新敌人地图
    public void UpdateEnemyMap(int[] map, int h, int w) {
        enemyMap.UpdateMap(map, h , w);
    }

    /// <summary>
    /// 显示标题"俄罗斯方块"
    /// </summary>
    public void ShowLogoNameUI() {
        // 显示
        logoName.gameObject.SetActive(true);
        logoName.DOAnchorPosY(0, 0.5f);
    }

    /// <summary>
    /// 隐藏标题"俄罗斯方块"
    /// </summary>
    public void HideLogoNameUI() {
        // 隐藏
        logoName.DOAnchorPosY(324.57f, 0.5f).OnComplete(() => {
            logoName.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// 显示认输按钮
    /// </summary>
    public void ShowToGameFalseUI() {
        // 显示
        toGameFalseUI.gameObject.SetActive(true);
        toGameFalseUI.DOAnchorPosX(-87f, 0.5f);
    }

    /// <summary>
    /// 隐藏认输按钮
    /// </summary>
    public void HideToGameFalseUI() {
        // 隐藏
        toGameFalseUI.DOAnchorPosX(151f, 0.5f).OnComplete(() => {
            toGameFalseUI.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// 显示下方 按钮组UI
    /// </summary>
    public void ShowButtonsUI() {
        // 显示
        buttonsUI.gameObject.SetActive(true);
        buttonsUI.DOAnchorPosY(0, 0.5f);
    }

    /// <summary>
    /// 隐藏下方 按钮组UI
    /// </summary>
    public void HideButtonsUI() {
        // 隐藏
        buttonsUI.DOAnchorPosY(-192.35f, 0.5f).OnComplete(() => {
            buttonsUI.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// 显示信息UI
    /// </summary>
    public void ShowInfoUI() {
        // 显示
        infoUI.gameObject.SetActive(true);
        infoUI.DOAnchorPosY(0, 0.5f);
    }

    /// <summary>
    /// 隐藏信息UI
    /// </summary>
    public void HideInfoUI() {
        // 隐藏
        infoUI.DOAnchorPosY(435.55f, 0.5f).OnComplete(() => {
            infoUI.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// 显示房间信息UI
    /// </summary>
    public void ShowRoomInfo() {
        // 显示
        roomInfoUI.gameObject.SetActive(true);
        roomInfoUI.DOAnchorPosX(-438f, 0.5f);
    }

    /// <summary>
    /// 隐藏房间信息UI
    /// </summary>
    public void HideRoomInfo() {
        // 隐藏
        roomInfoUI.DOAnchorPosX(0, 0.5f).OnComplete(() => {
            roomInfoUI.gameObject.SetActive(true);
        });
    }

    /// <summary>
    /// 显示主UI
    /// 目前就两个所以写个总方法显示隐藏
    /// </summary>
    public void ShowMain() {
        ShowButtonsUI();
        ShowInfoUI();
        ShowRoomInfo();
        ShowLogoNameUI();
    }
    public void HideMain() {
        HideButtonsUI();
        HideInfoUI();
        HideRoomInfo();
        HideLogoNameUI();
    }

    public void UpdateRoomCount() {
        roomCount.text = "房间当前人数: " + PhotonNetwork.CurrentRoom.PlayerCount;

    }

    public void PrepareGameButtonAction(bool action) {
        if (action) {
            prepareGameButton.interactable = true;
            prepareGameButton.GetComponent<Image>().color = new Color(1, 0.5f, 0.5f, 1);
        } else {
            prepareGameButton.interactable = false;
            prepareGameButton.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }
    }

    /// <summary>
    /// 第一次进来初始化
    /// </summary>
    public void InitUI() {
        // 显示UI
        ShowMain();

        // 如果当前房间人数小于等于1 隐藏按钮组
        if(PhotonNetwork.CurrentRoom.PlayerCount <= 1) {
            prepareGameButton.gameObject.SetActive(false);
            SetStateText("等待其他玩家加入\r\n(私密房间可以通过分享房间名让其他人加入)");
        } else {
            SetStateText("请准备(双方准备后将自动开始游戏)");
        }

        // 初始化房间信息
        roomName.text = "房间名: " + PhotonNetwork.CurrentRoom.Name;
        roomCount.text = "房间当前人数: " + PhotonNetwork.CurrentRoom.PlayerCount;
        roomState.text = "房间状态: 私密";// 以后更新匹配

        // 隐藏认输按钮
        HideToGameFalseUI();

    }

    /// <summary>
    /// 重置UI信息
    /// </summary>
    public void ResetUI() {
        // 显示UI
        ShowMain();

        // 如果当前房间人数小于等于1 隐藏按钮组
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) {
            prepareGameButton.gameObject.SetActive(false);
            SetStateText("等待其他玩家加入\r\n(私密房间可以通过分享房间名让其他人加入)");
        } else {
            SetStateText("请准备(双方准备后将自动开始游戏)");
        }

        // 初始化房间信息
        roomName.text = "房间名: " + PhotonNetwork.CurrentRoom.Name;
        roomCount.text = "房间当前人数: " + PhotonNetwork.CurrentRoom.PlayerCount;
        roomState.text = "房间状态: 私密";// 以后更新匹配

        // 初始化战绩
        SetRecordText(0, 0);
    }

    /// <summary>
    /// 游戏结束UI
    /// </summary>
    public void GameOverUI() {
        // 显示下方UI
        ShowButtonsUI();

        // 显示标题"俄罗斯方块"
        ShowLogoNameUI();

        // 隐藏认输按钮
        HideToGameFalseUI();
    }

    /// <summary>
    /// 游戏开始UI
    /// </summary>
    public void StartGameUI() {
        // 隐藏下方UI
        HideButtonsUI();

        // 隐藏标题"俄罗斯方块"
        HideLogoNameUI();

        // 显示人数按钮
        ShowToGameFalseUI();
    }


    /// <summary>
    /// 生成方块消除粒子特效
    /// </summary>
    /// <param name="rows"></param>
    public void CreateDestroyParticles() {
        Debug.Log("生成特效");
        foreach (int row in ModelOnline.Instance.deleteRowList) {
            GameObject game = Instantiate(ConfigManager.Instance.GameConfig.destroyParticles, map);
            Vector3 pos = game.transform.position;
            pos.y = row;
            game.transform.localPosition = pos;
        }

    }

    /// <summary>
    /// 显示设置面板
    /// </summary>
    public void ShowSettingUI() {
        float targetWidth = 400f;
        settingUI.DOSizeDelta(new Vector2(targetWidth, settingUI.sizeDelta.y), 0.5f).OnComplete(() => {
            settingButtonIn.gameObject.SetActive(false);
            settingButtonOut.gameObject.SetActive(true);
        });
    }

    /// <summary>
    /// 隐藏设置面板
    /// </summary>
    public void HideSettingUI() {
        float targetWidth = 0f;
        settingUI.DOSizeDelta(new Vector2(targetWidth, settingUI.sizeDelta.y), 0.5f).OnComplete(() => {
            settingButtonIn.gameObject.SetActive(true);
            settingButtonOut.gameObject.SetActive(false);
        }); ;
    }



    /// <summary>
    /// 设置界面 - 预览按钮
    /// </summary>
    public void SetSettingPreviewActive() {
        Image image = settingPreviewButton.gameObject.GetComponent<Image>();
        if (ModelOnline.Instance.isPreview) {
            image.color = new Color32(220, 102, 85, 255);
        } else {
            image.color = new Color32(128, 128, 128, 128);
        }
    }

    /// <summary>
    /// 设置界面 - 粒子特效按钮
    /// </summary>
    public void SetSettingParticleskActive() {
        Image image = settingParticleskButton.gameObject.GetComponent<Image>();
        if (ModelOnline.Instance.isParticles) {
            image.color = new Color32(220, 102, 85, 255);
        } else {
            image.color = new Color32(128, 128, 128, 128);
        }
    }

    /// <summary>
    /// 设置界面 - 下落按钮
    /// </summary>
    public void SetSettingFallDirectlyButton() {
        Image image = settingFallDirectlyButton.gameObject.GetComponent<Image>();
        if (ModelOnline.Instance.isFallDirectly) {
            image.color = new Color32(220, 102, 85, 255);
        } else {
            image.color = new Color32(128, 128, 128, 128);
        }
    }
}   


using System.Configuration;
using UnityEditor;
using UnityEngine;

public class GameManagerOnline : MonoBehaviour {
    private CtrlOnline ctrl;
    // 是否暂停
    private bool isPause = true;

    // 当前图形
    private ShapeOnline currentShape = null;

    // 图形层
    [HideInInspector] public GameObject Temp {  get; private set; }

    // 资源中心文件
    private GameConfigSO gameConfig = ConfigManager.Instance.GameConfig;


    private void Awake() {
        ctrl = GetComponent<CtrlOnline>();
        Temp = new GameObject("temp");
    }
    private void Start() {
        Temp.transform.position = ctrl.viewOnline.map.transform.position;
    }
    void Update() {
        // 暂停
        if (isPause) return;

        // 生成新的图形
        if (currentShape == null) SpawnShape();
    }

    // 开始游戏
    public void StartGame() {
        isPause = false;
        if (currentShape != null) currentShape.StartGame();
    }

    // 暂停游戏
    public void PauseGame() {
        isPause = true;
        if (currentShape != null) currentShape.PauseGame();
    }

    // 随机生成图形
    public void SpawnShape() {
        // 随机一个图形
        int index = Random.Range(0, gameConfig.shapesOnline.Length);
        // 随机一个颜色
        int indexColor = Random.Range(0, gameConfig.colors.Length);
        // 生成图形
        currentShape = Instantiate(gameConfig.shapesOnline[index].gameObject).GetComponent<ShapeOnline>();
        currentShape.Init(gameConfig.colors[indexColor], this, ctrl);
        currentShape.transform.SetParent(Temp.transform, false);
    }

    /// <summary>
    /// 下落完成
    /// </summary>
    public void CurrentShapeComplete() {

        // 判断是否游戏结束
        // 1. 行放满了
        if (ctrl.modelOnline.IsGameOver()) {
            // 游戏结束
            Debug.Log("玩家死亡");
            GameOver();

        } else {
            // - 游戏继续
            // 可以生成下一个形状了
            currentShape = null;
        }

        // 发送 地图给对方
        Debug.Log("发送地图");
        ctrl.networkCtrl.UpdateEnemyMap(ctrl.modelOnline.GetInterMap());
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver() {
        // - 所有玩家都停止
        // 相对手发送消息 你赢了
        ctrl.networkCtrl.GameTrue();

        // 自己输了
        EventCenter.Broadcast(EventType.GameOver);

    }

    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetDate() {

        // --- 可以优化 ---
        // 清空 图形
        foreach (Transform it in Temp.transform) {
            Destroy(it.gameObject);
        }

        // 重置model数据
        ctrl.modelOnline.MapResetToInitDate();
    }
}

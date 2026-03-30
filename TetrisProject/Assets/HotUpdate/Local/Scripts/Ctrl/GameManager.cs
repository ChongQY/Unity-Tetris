using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Ctrl ctrl;
    // 是否暂停
    private bool isPause = true;

    // 当前图形
    private Shape currentShape = null;

    // 图形层
    private GameObject temp;
    public GameObject Temp {  get { return temp; } }

    // 资源中心文件
    private GameConfigSO gameConfig = ConfigManager.Instance.GameConfig;

    private void Awake() {
        ctrl = GetComponent<Ctrl>();
        temp = new GameObject("temp");

        temp.transform.position = ctrl.view.map.transform.position;
    }
    private void Start() {
        
    }
    void Update()
    {
        // 暂停
        if (isPause) return;
        
        // 生成新的图形
        if (currentShape == null) SpawnShape();
    }

    // 开始游戏
    public void StartGame() {
        isPause = false;
        if(currentShape != null) currentShape.StartGame();
    }

    // 暂停游戏
    public void PauseGame() {
        isPause = true;
        if (currentShape != null) currentShape.PauseGame();
    }

    // 随机生成图形
    public void SpawnShape() {
        // 随机一个图形
        int index = Random.Range(0 , gameConfig.shapes.Length);
        // 随机一个颜色
        int indexColor = Random.Range(0 , gameConfig.colors.Length);
        // 生成图形
        currentShape = Instantiate(gameConfig.shapes[index].gameObject).GetComponent<Shape>();
        currentShape.Init(gameConfig.colors[indexColor] , this, ctrl);
        currentShape.transform.SetParent(temp.transform, false);
    }

    /// <summary>
    /// 下落完成
    /// </summary>
    public void CurrentShapeComplete() {
        // 更新分数
        ctrl.view.UpdateGameScoreUI(ctrl.model.scoreLabel, ctrl.model.higeScoreLabel);

        // 判断是否游戏结束
        // 1. 行放满了
        if (ctrl.model.IsGameOver()) {
            // 游戏结束
            GameOver();

        } else {
            // - 游戏继续
            // 可以生成下一个形状了
            currentShape = null;
        }
        
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver() {
        // 广播 "游戏失败"
        EventCenter.Broadcast(EventType.GameOver);
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetDate() {
        
        // --- 可以优化 ---
        // 清空 图形
        foreach(Transform it in temp.transform) {
            Destroy(it.gameObject);
        }

        // 重置model数据
        ctrl.model.MapResetToInitDate();
    }
}

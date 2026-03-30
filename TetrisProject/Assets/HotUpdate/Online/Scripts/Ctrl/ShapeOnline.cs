using System.Collections.Generic;
using UnityEngine;

public class ShapeOnline : MonoBehaviour
{
    private CtrlOnline ctrl;
    // 游戏管理器
    private GameManagerOnline gameManager;

    // 旋转中心点
    private Transform pivot;

    // 是否暂停
    private bool isPause = false;

    // 下落时间累计  
    private float time = 0f;
    // 下落时间key
    private float stepTime = 0.3f;
    // 下落替换
    private float stepTimeTemp;

    // 下落预览方块
    private GameObject previewTier;// 预览层
    private List<GameObject> shapePreviewList;

    /// <summary>
    /// 初始化 -> 颜色、游戏管理器 
    /// </summary>
    /// <param name="color"></param>
    public void Init(Color color, GameManagerOnline gameManager, CtrlOnline ctrl) {
        // 遍历子物体修改方块颜色
        foreach (Transform t in transform) {
            if (t.CompareTag("Block")) {
                t.gameObject.GetComponent<SpriteRenderer>().color = color;
            }
        }

        // 游戏管理器
        this.gameManager = gameManager;
        this.ctrl = ctrl;

        // 游戏中心点
        pivot = transform.Find("Pivot");

        // 下落系数
        stepTimeTemp = stepTime;

        // 生成预览层
        previewTier = new GameObject();
        previewTier.transform.SetParent(gameManager.Temp.transform);
        // 生成下落预览方块
        shapePreviewList = new List<GameObject>();
        foreach (SpriteRenderer item in GetComponentsInChildren<SpriteRenderer>()) {
            GameObject game = Instantiate(ConfigManager.Instance.GameConfig.shapePreview, previewTier.transform);
            game.transform.localPosition = item.transform.localPosition;
            game.SetActive(false);
            shapePreviewList.Add(game);
        }
    }



    private void Update() {
        // 暂停了
        if (isPause) return;

        // 下落
        time += Time.deltaTime;
        if (time >= stepTime) {
            // 方块下落
            Fall();
            // 清零
            time = 0f;

            
        }

        // 左右移动控制、旋转、直接下落
        InputControl();
    }

    // 下落
    private void Fall() {
        // 下落一格
        Vector3 pos = transform.position;
        pos.y -= 1f;
        transform.position = pos;

        // 判断是否合法
        // 1. 判断边界
        // 2. 位置上是否有 方块
        // 返回true 代表合法
        while (!ctrl.modelOnline.IsValidMapPosition(this.transform)) {
            // 不合法 回退(上升一格)
            pos.y += 1f;
            transform.position = pos;

            // 下楼完成
            FallComplete();

            // 退出
            return;
        }

        // 更新预览图形
        UpdateBlockPreview();

        // 播放下落音效
        ctrl.audioManagerOnline.PlayDrop();
    }


    // 下落完成
    public void FallComplete() {
        // 停止下落
        isPause = true;

        // 放置方块
        if (ModelOnline.Instance.PlaceShape(this.transform)) {
            // 如果有销毁播放销毁音效
            ctrl.audioManagerOnline.PlayLineclear();

            // 发送 攻击
            ctrl.networkCtrl.Attack(ModelOnline.Instance.deleteRowList.Count);

            // 播放粒子特效
            if (ModelOnline.Instance.isParticles) {
                ctrl.viewOnline.CreateDestroyParticles();
            }
        }

        // 清除预览图形
        ClearPreviewShape();
        // 通知 GameManager 该方块下落完成
        gameManager.CurrentShapeComplete();
    }

    /// <summary>
    /// 更新预览方块
    /// </summary>
    private void UpdateBlockPreview() {
        if (ModelOnline.Instance.isPreview) {
            // 同步位置旋转
            previewTier.gameObject.SetActive(true);
            previewTier.transform.SetPositionAndRotation(transform.position, transform.rotation);
            int downLength = 0;

            // 一直往下降, 直到不满足
            while (true) {
                downLength++;
                Vector3 pos = previewTier.transform.position;
                pos.y -= 1;
                previewTier.transform.position = pos;

                // 不行 回退+退出循环
                if (!ctrl.modelOnline.IsPreviewValidMapPosition(previewTier.transform)) {
                    downLength--;
                    pos = previewTier.transform.position;
                    pos.y += 1;
                    previewTier.transform.position = pos;
                    break;
                }
            }

            // 如果和当前图形重合就不显示
            if (downLength <= 1) {
                foreach (GameObject game in shapePreviewList) game.SetActive(false);
            } else {
                foreach (GameObject game in shapePreviewList) game.SetActive(true);
            }
        } else {
            previewTier.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 清除预览图形
    /// </summary>
    private void ClearPreviewShape() {
        if (previewTier != null) Destroy(previewTier);
    }

    /// <summary>
    /// 控制方块的 左右移动、旋转、直接下落
    /// </summary>
    private void InputControl() {
        // 下降
        ColorStet1();
        // 更新预览图形
        UpdateBlockPreview();
    }
    /// <summary>
    /// S 为加速下降, 加速后还可以控制
    /// </summary>
    private void ColorStet1() {
        ctrl.modelOnline.horizontalMoveTime += Time.deltaTime;

        // 控制左右移动
        // 水平轴 返回 -1 0 1
        int h = 0;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) h = -1;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) h = 1;
        if (h != 0) {
            if (ctrl.modelOnline.horizontalMoveTime > ctrl.modelOnline.horizontalMoveTimeKey) {
                // 左右移动
                Vector3 pos = transform.position;
                pos.x += h;
                transform.position = pos;

                // 判断是否合法
                // 1. 判断边界
                // 2. 位置上是否有 方块
                // 返回true 代表合法
                if (!ctrl.modelOnline.IsValidMapPosition(this.transform)) {
                    // 不合法 -> 移动失败 回退回去
                    pos.x -= h;
                    transform.position = pos;
                    return;
                }

                // 播放音效
                ctrl.audioManagerOnline.PlayControl();

                // 重置
                ctrl.modelOnline.horizontalMoveTime = 0f;
            }
        }

        // 控制旋转
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            // 旋转
            // 这里得围绕Vector3.forward(0,0,1) 旋转
            transform.RotateAround(pivot.position, Vector3.forward, 90);
            // 判断是否合法
            if (!ctrl.modelOnline.IsValidMapPosition(this.transform)) {
                // 旋转失败
                transform.RotateAround(pivot.position, Vector3.forward, -90);
                return;
            }
            ctrl.audioManagerOnline.PlayControl();
        }

        // 控制下降
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            stepTime = ctrl.modelOnline.stepTimeS;
        } else {
            stepTime = stepTimeTemp;
        }

        // 直接下降
        if (Input.GetKeyDown(KeyCode.Space) && ModelOnline.Instance.isFallDirectly) {
            FallDirectly();
            ctrl.audioManagerOnline.PlayFallDirectly();
        }
    }


    /// <summary>
    /// 直接落下
    /// </summary>
    private void FallDirectly() {
        // 一直下落直到不能下为止
        while (true) {
            // 下落一格
            Vector3 pos = transform.position;
            pos.y -= 1f;
            transform.position = pos;

            if (!ModelOnline.Instance.IsValidMapPosition(this.transform)) {
                // 不合法 回退(上升一格)
                pos.y += 1f;
                transform.position = pos;
                break;
            }
        }

        // 下楼完成
        FallComplete();

        // 播放下落音效
        ctrl.audioManagerOnline.PlayDrop();
    }

    // 游戏暂停
    public void PauseGame() {
        isPause = true;
    }

    // 游戏开始
    public void StartGame() {
        isPause = false;
    }
}

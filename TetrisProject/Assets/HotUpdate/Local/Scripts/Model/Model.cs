using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Model : MonoBehaviour
{
    public static Model Instance { get; private set; }


    // 最大长宽
    public const int MAX_ROWS = 25;
    public const int MAX_COLUMNS = 10;

    // 当前地图的长宽
    public const int CURRENT_MAP_ROWS = 20;
    public const int CURRENT_MAP_COLUMNS = 20;

    // 左右持续移动
    [HideInInspector] public float horizontalMoveTimeKey = 0.1f;
    [HideInInspector] public float horizontalMoveTime = 0;

    private Transform[,] map = new Transform[MAX_ROWS, MAX_COLUMNS];// 地图
    [HideInInspector] public int gameCount;// 游戏次数
    [HideInInspector] public int scoreLabel;// 当前分数
    [HideInInspector] public int higeScoreLabel;// 历史最大分数
    [HideInInspector] public float stepTimeS;// 下落系数
    [HideInInspector] public bool isRowFall = false;// 行是否满了(行满了则游戏结束)
    [HideInInspector] public List<int> deleteRowList;// 消除的行(用于在指定位置播放消除粒子特效)

    [HideInInspector] public bool isMute = false;// 是否静音
    [HideInInspector] public bool isPreview = true;// 是否开启预览
    [HideInInspector] public bool isParticles = true;// 是否开启粒子特效
    [HideInInspector] public bool isFallDirectly = true;// 是否直接下落

    private void Awake() {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        // 读取游戏数据
        LoadData();
    }

    /// <summary>
    /// 检查 方块 在地图上是否 合法
    /// </summary>
    /// <param name="tran"></param>
    /// <returns></returns>
    public bool IsValidMapPosition(Transform tran) {
        foreach(Transform it in tran) {
            if (it.CompareTag("Block")) {
                // 获取当前物体相对于指定祖先物体的坐标 所以用GetRelativePositionToAncestor
                Vector2 pos = it.transform.GetRelativePositionToAncestor(it.transform.parent.parent).Round();

                // 超出地图了
                if (!IsInsideMap(pos)) return false;

                // 这里有 方块
                if (map[(int)pos.y, (int)pos.x] != null) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 检查 预览方块 在地图上是否 合法
    /// </summary>
    /// <param name="tran"></param>
    /// <returns></returns>
    public bool IsPreviewValidMapPosition(Transform tran) {
        foreach (Transform it in tran) {
            if (it.CompareTag("BlockPreview")) {
                // 获取当前物体相对于指定祖先物体的坐标 所以用GetRelativePositionToAncestor
                Vector2 pos = it.transform.GetRelativePositionToAncestor(it.transform.parent.parent).Round();

                // 超出地图了
                if (!IsInsideMap(pos)) return false;

                // 这里有 方块
                if (map[(int)pos.y, (int)pos.x] != null) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 检查坐标是否在地图内
    /// </summary>
    /// <returns></returns>
    public bool IsInsideMap(Vector2 v) {
        return (v.y >= 0 && v.y < MAX_ROWS) && (v.x >= 0  && v.x < MAX_COLUMNS);
    }

    /// <summary>
    /// 放置方块
    /// </summary>
    /// <param name="tran"></param>
    /// <returns>是否有方块销毁</returns>
    public bool PlaceShape(Transform tran) {
        foreach (Transform it in tran) {
            if (it.CompareTag("Block")) {
                // 获取当前物体相对于指定祖先物体的坐标 所以用GetRelativePositionToAncestor
                Vector2 pos = it.transform.GetRelativePositionToAncestor(it.transform.parent.parent).Round();
                // 超出地图了
                if (!IsInsideMap(pos)) {
                    Debug.LogWarning("[放置方块失败]超出边界了: x = " + pos.y + " y = " + pos.x);
                    return false;
                }

                // 这里有 方块
                if (map[(int)pos.y, (int)pos.x] != null) {
                    Debug.LogWarning("[放置方块失败]重复放置方块: x = " + pos.y + " y = " + pos.x);
                    return false;
                }

                // 超出了游戏失败
                if ((int)pos.y >= CURRENT_MAP_ROWS) isRowFall = true;

                map[(int)pos.y, (int)pos.x] = it;
            }
        }

        // 检查地图
        return CheckMap();
    }

    /// <summary>
    /// 检查地图(消除满行方块)
    /// </summary>
    /// <returns>是否有销毁</returns>
    public bool CheckMap() {
        bool res = false;
        deleteRowList = new List<int>();// 消除的行

        // 从上到下遍历(可以防止一些特殊情况)
        for (int i = MAX_ROWS - 1; i >= 0; i--) {
            if (CheckIsRowFull(i)) {
                // 这一层放满了
                // 销毁这一层
                DeleteRow(i);
                // 上面的落下来
                MoveDownRowsAbove(i);

                deleteRowList.Add(i);
                res = true;
            }
        }

        return res;
    }

    /// <summary>
    /// 检查一行是否放满
    /// </summary>
    /// <returns></returns>
    private bool CheckIsRowFull(int row) {
        for(int i = 0; i < MAX_COLUMNS; i++) {
            if (map[row, i] == null) return false;
        }

        // 一行得分 +100
        AddOneRowScore();

        return true;
    }

    /// <summary>
    /// 清除一行
    /// </summary>
    /// <param name="row"></param>
    private void DeleteRow(int row) {
        for (int i = 0; i < MAX_COLUMNS; i++) {
            if (map[row, i] == null) {
                Debug.LogWarning("错误的清除行(有空行), 行 = " + (row + 1));
            } else {
                // ==== 这个删除方法可以优化 ====
                // Shape整体并没有被销毁
                Destroy(map[row, i].gameObject);
                map[row,i] = null;
            }

        }

    }

    /// <summary>
    /// row上面的整体落下来
    /// </summary>
    /// <param name="row"></param>
    private void MoveDownRowsAbove(int row) {
        row++;
        for(int i = row; i < MAX_ROWS; i++) {
            MoveDownRow(i);
        }
    }

    /// <summary>
    /// 这一行往下落一行
    /// </summary>
    /// <param name="row"></param>
    private void MoveDownRow(int row) {
        if(row <= 0) {
            Debug.LogWarning("行下落错误 : " + row);
        } else {
            for(int i = 0; i < MAX_COLUMNS; i++) {
                if(map[row, i] == null) continue;
                
                // 判断下方是否有方块
                if(map[row - 1, i] != null) {
                    Debug.LogWarning("行下落错误 : " + row);
                } else {
                    // 下降一格
                    Vector3 pos = map[row, i].transform.GetRelativePositionToAncestor(map[row, i].transform.parent.parent);
                    pos.y -= 1;
                    map[row, i].transform.SetRelativePositionToAncestor(map[row, i].transform.parent.parent, pos);
                    // 掉下去
                    map[row - 1, i] = map[row, i];
                    map[row, i] = null;
                }
            }
        }
    }

    public bool IsGameOver() {
        // 行满了则游戏结束
        if(isRowFall) return true;
        
        return false;
    }

    /// <summary>
    /// 获得一行得分 +100
    /// </summary>
    private void AddOneRowScore() {
        scoreLabel += 100;
        if(scoreLabel > higeScoreLabel) {
            higeScoreLabel = scoreLabel;
        }
    }

    /// <summary>
    /// 地图整体重置为初始数据
    /// </summary>
    public void MapResetToInitDate() {
        // 重置为初始数据
        scoreLabel = 0;

        // 初始化地图
        for (int i = 0; i < MAX_ROWS; i++) {
            for (int j = 0; j < MAX_COLUMNS; j++) {
                map[i, j] = null;
            }
        }

        // 没放满
        isRowFall = false;
    }

    /// <summary>
    /// 数据重置为初始状态
    /// </summary>
    private void ResetToInitDate() {
        // 重置
        scoreLabel = 0;
        higeScoreLabel = 0;
        gameCount = 0;
        stepTimeS = 0.05f;
        horizontalMoveTimeKey = 0.1f;
        isMute = false;
        isPreview = true;
        isParticles = true;
        isFallDirectly = true;
    }

    /// <summary>
    /// 加载游戏数据: 分数、最大分数
    /// </summary>
    public void LoadData() {
        // 加载分数、最大分数
        //PlayerPrefs.GetInt("ScoreLabel");
        scoreLabel = 0;
        higeScoreLabel = PlayerPrefs.GetInt("HigeScoreLabel");
        gameCount = PlayerPrefs.GetInt("GameCount");
        stepTimeS = PlayerPrefs.GetFloat("StepTimeS");if(stepTimeS <= 0.01) stepTimeS = 0.05f;
        horizontalMoveTimeKey = PlayerPrefs.GetFloat("HorizontalMoveTimeKey"); if (horizontalMoveTimeKey <= 0.01) horizontalMoveTimeKey = 0.1f;
        isPreview = PlayerPrefs.GetInt("IsPreview") == 1;
        isParticles = PlayerPrefs.GetInt("IsParticles") == 1;
        isFallDirectly = PlayerPrefs.GetInt("IsFallDirectly") == 1;
        isMute = PlayerPrefs.GetInt("IsMute") == 1;
    }

    /// <summary>
    /// 保存游戏数据: 分数、最大分数
    /// </summary>
    public void SaveData() {
        // 外面调用一次保存数据视为 "游戏一次"
        gameCount++;

        // 保存分数、最大分数
        //PlayerPrefs.SetInt("ScoreLabel", scoreLabel);
        PlayerPrefs.SetInt("HigeScoreLabel", higeScoreLabel);
        PlayerPrefs.SetInt("GameCount", gameCount);
        PlayerPrefs.SetFloat("StepTimeS", stepTimeS);
        PlayerPrefs.SetFloat("HorizontalMoveTimeKey", horizontalMoveTimeKey);
        PlayerPrefs.SetInt("IsPreview", isPreview ? 1 : 0);
        PlayerPrefs.SetInt("IsParticles", isParticles ? 1 : 0);
        PlayerPrefs.SetInt("IsFallDirectly", isFallDirectly ? 1 : 0);
        PlayerPrefs.SetInt("IsMute", isMute ? 1 : 0);
    }
    
    /// <summary>
    /// 清空所有数据
    /// </summary>
    public void ClearData() {
        // 清空之前的数据
        PlayerPrefs.DeleteAll();

        // 重置数据
        ResetToInitDate();
    }


    


}

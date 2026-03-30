
using System.Collections.Generic;
using UnityEngine;

public class ModelOnline : MonoBehaviour {
    public static ModelOnline Instance { get; private set; }

    // 最大长宽
    public const int MAX_ROWS = 30;
    public const int MAX_COLUMNS = 10;

    // 当前地图的长宽
    public const int CURRENT_MAP_ROWS = 20;
    public const int CURRENT_MAP_COLUMNS = 10;
    
    private Transform[,] map = new Transform[MAX_ROWS, MAX_COLUMNS];// 地图


    [HideInInspector] public bool isRowFall = false;// 行是否满了(行满了则游戏结束)

    [HideInInspector] public int playerTrueCount;// 我获胜了几次
    [HideInInspector] public int enemyTrueCount;// 对面获胜了几次
    [HideInInspector] public int prepareCount;// 目前准备的数量
    [HideInInspector] public int rubbishRowCount;// 垃圾行的数量
    [HideInInspector] public List<int> deleteRowList;// 消除的行(用于在指定位置播放消除粒子特效)
    [HideInInspector] public bool isMute = false;// 是否静音
    [HideInInspector] public bool isPreview = true;// 是否开启预览
    [HideInInspector] public bool isParticles = true;// 是否开启粒子特效
    [HideInInspector] public bool isFallDirectly = true;// 是否直接下落
    [HideInInspector] public float stepTimeS = 0.05f;// 下落系数
    // 左右持续移动
    [HideInInspector] public float horizontalMoveTimeKey = 0.1f;
    [HideInInspector] public float horizontalMoveTime = 0;

    private void Awake() {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        // 初始化
        InitDate();
    }

    private void InitDate() {
        stepTimeS = 0.05f;
        horizontalMoveTimeKey = 0.1f;
        horizontalMoveTime = 0;
        playerTrueCount = 0;
        enemyTrueCount = 0;
        isMute = false;
        isPreview = true;
        isParticles = true;
        isFallDirectly = true;
    }

    /// <summary>
    /// 检查tran在地图上是否 合法
    /// </summary>
    /// <param name="tran"></param>
    /// <returns></returns>
    public bool IsValidMapPosition(Transform tran) {
        foreach (Transform it in tran) {
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
        return (v.y >= 0 && v.y < MAX_ROWS) && (v.x >= 0 && v.x < MAX_COLUMNS);
    }

    /// <summary>
    /// 放置方块
    /// </summary>
    /// <param name="tran"></param>
    /// <returns>是否有消除</returns>
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
    /// <returns>消除了多少行</returns>
    public bool CheckMap() {
        bool res = false;
        deleteRowList = new List<int>();// 消除的行

        // 从上到下遍历(可以防止一些特殊情况)
        for (int i = MAX_ROWS - 1; i >= rubbishRowCount; i--) {
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


        // 消除垃圾行
        // 一行得分 -> 一行垃圾
        if(deleteRowList.Count != 0) {
            SubtractRubbisRow(deleteRowList.Count);
            for (int i = MAX_ROWS - 1; i >= rubbishRowCount; i--) {
                if (CheckIsRowFull(i)) {
                    // 这一层放满了
                    // 销毁这一层
                    DeleteRow(i);
                    // 上面的落下来
                    MoveDownRowsAbove(i);
                }
            }
        }

        return res;
    }

    /// <summary>
    /// 检查一行是否放满
    /// </summary>
    /// <returns></returns>
    private bool CheckIsRowFull(int row) {
        for (int i = 0; i < MAX_COLUMNS; i++) {
            if (map[row, i] == null) return false;
        }

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
                map[row, i] = null;
            }

        }
    }

    /// <summary>
    /// row上面的整体落下来
    /// </summary>
    /// <param name="row"></param>
    private void MoveDownRowsAbove(int row) {
        row++;
        for (int i = row; i < MAX_ROWS; i++) {
            MoveDownRow(i);
        }
    }

    /// <summary>
    /// 这一行往下落一行
    /// </summary>
    /// <param name="row"></param>
    private void MoveDownRow(int row) {
        if (row <= 0) {
            Debug.LogWarning("行下落错误 : " + row);
        } else {
            for (int i = 0; i < MAX_COLUMNS; i++) {
                if (map[row, i] == null) continue;

                // 判断下方是否有方块
                if (map[row - 1, i] != null) {
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

    /// <summary>
    /// 增加一行垃圾行
    /// </summary>
    private void AddRubbisRow(GameObject rubbis, Transform temp) {
        // 整体上升一格
        for(int i = MAX_ROWS - 1; i > 0; i--) {
            for(int j = 0; j < MAX_COLUMNS; j++) {
                // 保持当前行为null
                if (map[i, j] != null) map[i, j] = null;
                // 下面有方块就上来 没有就 continue
                if (map[i - 1, j] == null) continue;

                if(i == CURRENT_MAP_ROWS - 1) {
                    // 行满了
                    isRowFall = true;
                }

                // 上升一格
                if (map[i - 1, j].CompareTag("Block")) {
                    Vector3 pos = new Vector3(j, i, 0);
                    map[i - 1, j].transform.SetRelativePositionToAncestor(map[i - 1, j].transform.parent.parent, pos);
                    map[i, j] = map[i - 1, j];
                } else {
                    // 垃圾行
                    Vector3 pos = new Vector3(j, i, 0);
                    map[i - 1, j].localPosition = pos;
                    map[i, j] = map[i - 1, j];

                }
            }
        }
        // 最后一行增加垃圾行
        for (int j = 0; j < MAX_COLUMNS; j++) {
            // 保持当前行为null
            if (map[0, j] != null)  map[0, j] = null;
            
            // 实例化垃圾
            GameObject item = Instantiate(rubbis);
            // 设置父物体
            item.transform.SetParent(temp);
            // 设置坐标
            item.transform.localPosition = new Vector3(j, 0, 0);
            // 放进地图
            map[0, j] = item.transform;
        }
    }

    /// <summary>
    /// 减少row行垃圾行
    /// </summary>
    public void SubtractRubbisRow(int row) {
        rubbishRowCount -= row;
        if (rubbishRowCount < 0) rubbishRowCount = 0;
    }

    /// <summary>
    /// 增加n行垃圾
    /// </summary>
    public void AddRubbisRowCount(GameObject rubbis, Transform temp, int countRow) {
        for(int i = 0; i < countRow; i++) {
            rubbishRowCount++;
            AddRubbisRow(rubbis, temp);
        }
    }

    public bool IsGameOver() {
        // 行满了则游戏结束
        if (isRowFall) return true;

        return false;
    }


    /// <summary>
    /// 地图整体重置为初始数据
    /// </summary>
    public void MapResetToInitDate() {

        // 初始化地图
        for (int i = 0; i < MAX_ROWS; i++) {
            for (int j = 0; j < MAX_COLUMNS; j++) {
                map[i, j] = null;
            }
        }

        // 没放满
        isRowFall = false;

        // 垃圾行的数量
        rubbishRowCount = 0;
    }

    /// <summary>
    /// 获取数字化的地图
    /// </summary>
    public int[] GetInterMap() {
        int[] newMap = new int[CURRENT_MAP_ROWS * CURRENT_MAP_COLUMNS];
        for(int i = 0; i < CURRENT_MAP_ROWS; i++) {
            for(int j = 0; j < CURRENT_MAP_COLUMNS; j++) {
                // 0 是空地
                // 1 是正常方块
                // 2 是垃圾
                int type = 0;
                if (map[i,j] != null) {
                    if (map[i, j].CompareTag("Block")) type = 1;
                    else type = 2;
                }
                newMap[i * CURRENT_MAP_COLUMNS + j] = type;
            }
        }

        return newMap;  
    }



    /// <summary>
    /// 重置为初始数据
    /// </summary>
    public void ResetToInitDate() {
        // 我获胜了几次
        playerTrueCount = 0;
        // 对面获胜了几次
        enemyTrueCount = 0;
        // 目前准备的数量
        prepareCount = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public void LoadData() {
    }

    /// <summary>
    /// 
    /// </summary>
    public void SaveData() {
    }

    /// <summary>
    /// 清空所有数据
    /// </summary>
    public void ClearData() {
    }





}

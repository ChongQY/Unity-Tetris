
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMap : MonoBehaviour
{
    private Dictionary<int, Queue<GameObject>> pool;

    // 0: 空地
    // 1: 敌人放置
    // 2: 垃圾
    private GameObject[] floors;
    private GameObject[,] Map;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="h"></param>
    /// <param name="w"></param>
    public void Init(int h, int w) {
        Map = new GameObject[h, w];
        pool = new Dictionary<int, Queue<GameObject>>();
        floors = ConfigManager.Instance.GameConfig.floors;
    }

    /// <summary>
    /// 更新地图
    /// </summary>
    /// <param name="map"></param>
    public void UpdateMap(int[] map, int row, int columns) {
        if(Map == null) {
            Debug.Log("初始化敌人地图");
            Init(row, columns);
        }
        // 清理之前的
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < columns; j++) {
                if (Map[i, j] != null) PutPool(Map[i, j].gameObject);
            }
        }

        // 放入新的
        for (int i = 0; i < row; i++) {
            for(int j = 0; j <  columns; j++) {
                GameObject item = GetPoolItem(map[i * columns + j]);
                item.transform.SetParent(this.transform);
                item.transform.localPosition = new Vector3(j, i);
                item.transform.localScale = Vector3.one;
                Map[i, j] = item.gameObject;
            }
        }
    }

    // ========== 对象池优化 ==========

    /// <summary>
    /// 在池中获取一个物体
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetPoolItem(int type) {
        GameObject item = null;
        // 没有这个键就创建一个
        if (!pool.ContainsKey(type)){
            pool.Add(type, new Queue<GameObject>());
        }
        // 池里没有物体了就创建一个
        if (pool[type].Count == 0) {
            item = CreatePoolItem(type);
            pool[type].Enqueue(item);
        }

        // 取出并删除 (恢复初始数据)
        item = pool[type].Dequeue();
        item.gameObject.SetActive(true);
        return item;
    }

    /// <summary>
    /// 放回池中
    /// </summary>
    /// <param name="item"></param>
    private void PutPool(GameObject item) {
        item.SetActive(false);
        int type = item.GetComponent<FloorOnline>().type;
        pool[type].Enqueue(item);
    }

    /// <summary>
    /// 根据类型创建一个
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject CreatePoolItem(int type) {
        GameObject item = Instantiate(floors[type]);
        item.transform.SetParent(this.transform);
        item.gameObject.SetActive(false);
        item.GetComponent<FloorOnline>().type = type;
        return item;
    }
}

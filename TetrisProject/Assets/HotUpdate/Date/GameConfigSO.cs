// HotUpdate/Scripts/Data/GameConfigSO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "HotUpdate/GameConfig", fileName = "GameConfig")]
public class GameConfigSO : ScriptableObject {

    [Header("下落图形预制体 (单机)")]
    public GameObject[] shapes;
    [Header("下落图形预制体 (联机)")]
    public GameObject[] shapesOnline;
    [Header("下落预览图形")]
    public GameObject shapePreview;
    [Header("下落图形颜色")]
    public Color[] colors;


    [Header("垃圾预制体")]
    [Tooltip("这个目前就联机模式使用")]
    public GameObject rubbisonline;

    [Header("敌方方块预制体")]
    public GameObject[] floors;

    [Header("点击音效")]
    public AudioClip cursor;
    [Header("\"直接落下\"音效")]
    public AudioClip drop;
    [Header("左右移动音效")]
    public AudioClip controlMove;
    [Header("消除音效")]
    public AudioClip lineclear;
    [Header("游戏结束音效")]
    public AudioClip gameOver;
    [Header("游戏结束音效")]
    public AudioClip fallDirectly;
    [Header("方块消除粒子特效")]
    public GameObject destroyParticles;
}
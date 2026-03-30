using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerOnline : MonoBehaviour
{
    private AudioSource audioSource;
    // 点击音效
    public AudioClip cursor;
    // "直接落下"音效
    public AudioClip drop;
    // 左右移动音效
    public AudioClip controlMove;
    // 消除音效
    public AudioClip lineclear;
    // 游戏结束音效
    public AudioClip gameOver;
    // 直接下落音效
    public AudioClip fallDirectly;


    private void Awake() {
        // 获取 AudioSource 组件
        audioSource = GetComponent<AudioSource>();

        // 加载音频
        LoadAudioClip();
    }

    private void LoadAudioClip() {
        // 加载点击音效
        cursor = ConfigManager.Instance.GameConfig.cursor;

        // 加载"直接落下"音效
        drop = ConfigManager.Instance.GameConfig.drop;

        // 加载点击音效
        controlMove = ConfigManager.Instance.GameConfig.controlMove;

        // 加载点击音效
        lineclear = ConfigManager.Instance.GameConfig.lineclear;

        // 加载点击音效
        gameOver = ConfigManager.Instance.GameConfig.gameOver;

        // 加载点击音效
        fallDirectly = ConfigManager.Instance.GameConfig.fallDirectly;
    }

    // 方块直接下落音效
    public void PlayFallDirectly() {
        PlayOneShot(fallDirectly);
    }

    // 播放鼠标点击音效
    public void PlayCursor() {
        PlayOneShot(cursor);
    }

    // 方块下落音效
    public void PlayDrop() {
        PlayOneShot(drop);
    }

    // 游戏结束音效
    public void PlayGameOver() {
        PlayAudio(gameOver);
    }

    // 左右移动音效
    public void PlayControl() {
        PlayOneShot(controlMove);
    }

    // 消除音效
    public void PlayLineclear() {
        PlayAudio(lineclear);
    }

    // 播放指定音乐
    private void PlayAudio(AudioClip clip) {
        if (ModelOnline.Instance.isMute) return;
        audioSource.clip = clip;
        audioSource.Play();
    }

    // 播放指定音效
    private void PlayOneShot(AudioClip clip) {
        if (ModelOnline.Instance.isMute) return;
        audioSource.PlayOneShot(clip);
    }
}

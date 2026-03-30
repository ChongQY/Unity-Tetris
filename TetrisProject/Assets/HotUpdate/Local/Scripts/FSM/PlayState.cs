using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [游戏中状态]
public class PlayState : FSMState
{
    private void Awake() {
        stateID = StateID.Play;

        // - 添加转换规则
        // [游戏中状态]暂停按钮点击 -> 暂停状态
        AddTransition(Transition.PauseButtonClick, StateID.Pause);
        // [游戏中状态]游戏结束 -> 游戏结束状态
        AddTransition(Transition.GameOver, StateID.GameOver);
    }

    // [游戏中状态]进入
    public override void DoBeforeEntering() {
        // 显示游戏中的UI
        ctrl.view.ShowGameUI();

        // 相机放大
        ctrl.cameraManager.ZoomIn();

        // 开始游戏
        ctrl.gameManager.StartGame();

        // 监听 "游戏结束"
        EventCenter.AddListener(EventType.GameOver, OnGameOver);
        EventCenter.AddListener(EventType.GamePauseButtonButtonClick, OnPauseButtonClick);
    }

    // [游戏中状态]离开
    public override void DoBeforeLeaving() {
        // 隐藏游戏中的UI
        ctrl.view.HideGameUI();

        // 移除监听事件
        EventCenter.RemoveListener(EventType.GameOver, OnGameOver);
        EventCenter.RemoveListener(EventType.GamePauseButtonButtonClick, OnPauseButtonClick);
    }



    /// <summary>
    /// 暂停按钮点击
    /// </summary>
    public void OnPauseButtonClick() {
        // 触发 "暂停"按钮 
        fsm.PerformTransition(Transition.PauseButtonClick);
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void OnGameOver() {
        // 触发 "游戏结束"
        fsm.PerformTransition(Transition.GameOver);
    }


}

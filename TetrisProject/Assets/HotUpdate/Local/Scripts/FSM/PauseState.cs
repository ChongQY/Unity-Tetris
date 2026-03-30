using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [游戏暂停状态]
public class PauseState : FSMState
{
    private void Awake() {
        stateID = StateID.Pause;

        // - 添加转换规则
        // [游戏暂停状态] 开始(继续)按钮点击 -> 游戏中状态
        AddTransition(Transition.StartButtonClick, StateID.Play);
        // [游戏暂停状态] 重新开始按钮点击 -> 游戏中状态
        AddTransition(Transition.RestartButtonClick, StateID.Play);
        // [游戏暂停状态] 主界面按钮点击 -> 主界面状态
        AddTransition(Transition.HomeButtonClick, StateID.Menu);
    }

    // [游戏暂停状态]进入
    public override void DoBeforeEntering() {
        // 显示暂停界面
        ctrl.view.ShowPauseUI();

        // 相机缩小， 使俄罗斯方块背景在中间 不挡UI
        ctrl.cameraManager.ZoomOut();

        // 暂停游戏
        ctrl.gameManager.PauseGame();

        // 监听事件
        EventCenter.AddListener(EventType.PauseStartButtonClick, OnStartButtonClick);
        EventCenter.AddListener(EventType.PauseRestartButtonClick, OnRestartButtonClick);
        EventCenter.AddListener(EventType.PauseHomeButtonClick, OnHomeButtonClick);
    }

    // [游戏暂停状态]离开
    public override void DoBeforeLeaving() {
        // 隐藏暂停界面
        ctrl.view.HidePauseUI();

        // 移除监听
        EventCenter.RemoveListener(EventType.PauseStartButtonClick, OnStartButtonClick);
        EventCenter.RemoveListener(EventType.PauseRestartButtonClick, OnRestartButtonClick);
        EventCenter.RemoveListener(EventType.PauseHomeButtonClick, OnHomeButtonClick);
    }

    /// <summary>
    /// 继续按钮点击
    /// </summary>
    private void OnStartButtonClick() {
        // 触发 "继续按钮点击"
        fsm.PerformTransition(Transition.StartButtonClick);
    }

    /// <summary>
    /// 重新开始按钮点击
    /// </summary>
    private void OnRestartButtonClick() {
        // 触发 "重新开始按钮点击"
        fsm.PerformTransition(Transition.RestartButtonClick);
    }


    /// <summary>
    /// 返回大厅按钮点击
    /// </summary>
    private void OnHomeButtonClick() {
        // 触发 "主界面按钮点击事件"
        fsm.PerformTransition(Transition.HomeButtonClick);
    }
}

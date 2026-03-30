using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [游戏结束状态]
public class GameOverState : FSMState
{
    private void Awake() {
        stateID = StateID.GameOver;

        // - 添加转换规则
        // [游戏结束状态] 重新开始按钮点击 -> 游戏中
        AddTransition(Transition.RestartButtonClick, StateID.Play);
        // [游戏结束状态] 返回主界面按钮点击 -> 返回主界面
        AddTransition(Transition.HomeButtonClick, StateID.Menu);

    }

    // [游戏结束状态]进入
    public override void DoBeforeEntering() {
        // 暂停游戏
        ctrl.gameManager.PauseGame();

        // 显示游戏结束UI
        ctrl.view.ShowGameOverUI();

        // 播放游戏结束音效
        ctrl.audioManager.PlayGameOver();

        // 监听事件
        EventCenter.AddListener(EventType.GameOverRestartButtonClick, OnRestartButtonClick);
        EventCenter.AddListener(EventType.GameOverHomeButtonClick, OnHomeButtonClick);
    }

    // [游戏结束状态]离开
    public override void DoBeforeLeaving() {
        // 隐藏游戏结束UI
        ctrl.view.HideGameOverUI();

        // 保存数据
        ctrl.model.SaveData();

        // 移除事件
        EventCenter.RemoveListener(EventType.GameOverRestartButtonClick, OnRestartButtonClick);
        EventCenter.RemoveListener(EventType.GameOverHomeButtonClick, OnHomeButtonClick);
    }

    /// <summary>
    /// 重新开始按钮点击
    /// </summary>
    public void OnRestartButtonClick() {
        // 触发 "重置按钮点击"
        fsm.PerformTransition(Transition.RestartButtonClick);
    }

    /// <summary>
    /// 主界面按钮点击
    /// </summary>
    public void OnHomeButtonClick() {
        // 触发 "主界面按钮点击"
        fsm.PerformTransition(Transition.HomeButtonClick);
    }
}

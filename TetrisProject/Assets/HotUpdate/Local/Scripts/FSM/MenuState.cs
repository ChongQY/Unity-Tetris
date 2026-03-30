
using UnityEngine;

//[主界面状态]
public class MenuState : FSMState
{
    private void Awake() {
        stateID = StateID.Menu;
        
        // - 添加转换规则
        // [主界面状态]开始按钮点击 -> 游戏中状态
        AddTransition(Transition.StartButtonClick, StateID.Play);
    }

    // [主界面状态]进入
    public override void DoBeforeEntering() {
        // 显示主界面
        ctrl.view.ShowMenuUI();

        // 相机缩小， 使俄罗斯方块背景在中间 不挡UI
        ctrl.cameraManager.ZoomOut();

        // 暂停游戏
        ctrl.gameManager.PauseGame();

        // 添加监听
        EventCenter.AddListener(EventType.MenuStartButtonClick, OnStartButtonClick);
    }

    // [主界面状态]离开
    public override void DoBeforeLeaving() {
        // 隐藏主界面
        ctrl.view.HideMenuUI();

        // 重置数据
        // 离开的时候重置可以看到上一次的堆叠状态
        ctrl.gameManager.ResetDate();

        // 这边可以在PlayState里控制，遵循"单一职责"原则
        // 游戏中状态他想要什么自己去搞, "我"主界面状态除了自己不管其他事情
        // 相机放大
        // ctrl.cameraManagerOnline.ZoomIn();

        // 移除监听
        EventCenter.RemoveListener(EventType.MenuStartButtonClick, OnStartButtonClick);
    }

    
    /// <summary>
    /// 开始按钮点击
    /// </summary>
    private void OnStartButtonClick() {
        // 切换到 "开始点击"按钮 点击后的状态
        fsm.PerformTransition(Transition.StartButtonClick);
    }

}

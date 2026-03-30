using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 基于 Eric Dybsand 的《Game Programming Gems 1》第3.1章的有限状态机系统
 * 作者：Roberto Cezar Bianchini, 2010年7月
 * 
 * 使用方法：
 * 1. 在 Transition 和 StateID 枚举中定义你的状态转换标签和状态ID。
 * 2. 创建继承自 FSMState 的新类，并在每个类中通过 AddTransition 方法添加（转换-状态）对。
 *    这些对表示：当处于当前状态时，如果触发某个转换，FSM 应该进入的目标状态。
 *    注意：这是确定性FSM，一个转换不能指向两个不同的状态。
 * 3. 可以重写 Reason 方法来决定何时触发转换，或者在其他地方触发转换（例如通过事件）。
 *    可以重写 Act 方法来执行该状态下的行为。
 * 4. 创建 FSMSystem 实例，并向其中添加状态对象。
 * 5. 在 Update 或 FixedUpdate 中调用当前状态的 Reason 和 Act 方法（或你自己定义的行为方法）。
 * 
 * 也可以使用 Unity 的异步事件（如 OnTriggerEnter、SendMessage）来触发转换，
 * 只需在事件发生时调用 FSMSystem 的 PerformTransition 方法，并传入正确的 Transition。
 * 
 * 本软件按“原样”提供，不提供任何明示或暗示的担保。
 */

/// <summary>
/// 定义所有可能的转换（Transition）的枚举。
/// 不要改变第一个标签 NullTransition，FSMSystem 类会用到它。
/// </summary>
public enum Transition {
    NullTransition = 0,     // 表示不存在的转换（系统保留）
    StartButtonClick,       // 开始按钮点击
    PauseButtonClick,       // 暂停按钮点击
    RestartButtonClick,     // 重新开始按钮点击
    HomeButtonClick,        // 返回主界面按钮点击
    GameOver                // 游戏结束
    // 在这里添加你的其他转换...
}

/// <summary>
/// 定义所有可能的状态ID的枚举。
/// 不要改变第一个标签 NullStateID，FSMSystem 类会用到它。
/// </summary>
public enum StateID {
    NullStateID = 0,        // 表示不存在的状态（系统保留）
    Menu,                   // 主菜单状态
    Play,                   // 游戏中状态
    Pause,                  // 暂停状态
    GameOver                // 游戏结束状态
    // 在这里添加你的其他状态...
}

/// <summary>
/// 状态基类，代表有限状态机中的一个状态。
/// 每个状态包含一个字典（map），记录了从该状态出发，通过某个转换可以到达的目标状态。
/// </summary>
public abstract class FSMState : MonoBehaviour {
    // 控制器引用，可用于访问游戏对象或共享数据（具体类型由你的项目定义）
    protected Ctrl ctrl;
    public Ctrl CTRL { set { ctrl = value; } }

    // 所属的有限状态机引用
    // fsm 字段的主要作用是在子类中使用，让状态能够触发转换：fsm.PerformTransition
    protected FSMSystem fsm;
    public FSMSystem FSM { set { fsm = value; } }

    // 转换 -> 目标状态ID 的映射表
    protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();

    // 当前状态的唯一标识ID
    protected StateID stateID;
    public StateID ID { get { return stateID; } }

    /// <summary>
    /// 向当前状态添加一个转换规则。
    /// </summary>
    /// <param name="trans">触发转换的事件</param>
    /// <param name="id">转换后要进入的目标状态ID</param>
    public void AddTransition(Transition trans, StateID id) {
        // 检查参数是否有效（不能使用 NullTransition 或 NullStateID）
        if (trans == Transition.NullTransition) {
            Debug.LogError("FSMState 错误：不允许使用 NullTransition 作为实际转换");
            return;
        }

        if (id == StateID.NullStateID) {
            Debug.LogError("FSMState 错误：不允许使用 NullStateID 作为实际状态ID");
            return;
        }

        // 检查该转换是否已经被添加（确定性FSM要求每个转换只能指向一个目标状态）
        if (map.ContainsKey(trans)) {
            Debug.LogError("FSMState 错误：状态 " + stateID.ToString() + " 已经定义了转换 " + trans.ToString() +
                           "，无法再分配给另一个状态");
            return;
        }

        map.Add(trans, id);
    }

    /// <summary>
    /// 从当前状态中删除一个转换规则。
    /// </summary>
    /// <param name="trans">要删除的转换</param>
    public void DeleteTransition(Transition trans) {
        // 检查是否尝试删除 NullTransition
        if (trans == Transition.NullTransition) {
            Debug.LogError("FSMState 错误：不允许删除 NullTransition");
            return;
        }

        // 如果存在则删除，否则报错
        if (map.ContainsKey(trans)) {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState 错误：状态 " + stateID.ToString() + " 的转换列表中不存在转换 " + trans.ToString());
    }

    /// <summary>
    /// 根据给定的转换，获取应该转换到的目标状态ID。
    /// </summary>
    /// <param name="trans">触发的转换</param>
    /// <returns>目标状态ID，如果没有定义该转换则返回 NullStateID</returns>
    public StateID GetOutputState(Transition trans) {
        if (map.ContainsKey(trans)) {
            return map[trans];
        }
        return StateID.NullStateID;
    }

    /// <summary>
    /// 进入该状态前会自动调用此方法（由 FSMSystem 在切换状态时调用）。
    /// 可重写此方法执行进入前的准备工作，例如重置变量、播放动画等。
    /// </summary>
    public virtual void DoBeforeEntering() { }

    /// <summary>
    /// 离开该状态前会自动调用此方法（由 FSMSystem 在切换状态时调用）。
    /// 可重写此方法执行清理工作，例如停止声音、保存数据等。
    /// </summary>
    public virtual void DoBeforeLeaving() { }
    
    /// <summary>
    /// 决定当前状态是否应该触发转换。通常在每帧更新中调用。
    /// 你可以在这里编写条件判断，并调用 fsm.PerformTransition() 来执行转换。
    /// </summary>
    public virtual void Reason() { }

    /// <summary>
    /// 执行当前状态下的主要行为（如移动、攻击等）。通常在每帧更新中调用。
    /// 问: 既然继承了这个类(FSMState)的函数也继承了MonoBehaviour, 那么也就拥有了Updata方法,为什么还要这个Act方法呢?
    /// 答: 所有状态都会运行 Update，但只有一个是当前状态
    /// </summary>
    public virtual void Act() { }

} // class FSMState


/// <summary>
/// 有限状态机系统类，管理一组状态，并处理状态之间的转换。
/// </summary>
public class FSMSystem {
    private List<FSMState> states;           // 存储所有已添加的状态

    private StateID currentStateID;           // 当前状态ID
    public StateID CurrentStateID { get { return currentStateID; } }

    private FSMState currentState;            // 当前状态实例
    public FSMState CurrentState { get { return currentState; } }

    public FSMSystem() {
        states = new List<FSMState>();
    }

    /// <summary>
    /// 设置初始状态（一般在添加完所有状态后调用）。
    /// </summary>
    /// <param name="s">初始状态对象</param>
    public void SetCurrentState(FSMState s) {
        currentState = s;
        currentStateID = s.ID;
        s.DoBeforeEntering();  // 调用进入逻辑
    }

    /// <summary>
    /// 向状态机添加一个新状态。第一个添加的状态会被自动设为初始状态。
    /// </summary>
    /// <param name="s">要添加的状态实例</param>
    /// <param name="ctrl">与该状态关联的控制器对象（可根据需要传递）</param>
    public void AddState(FSMState s, Ctrl ctrl) {
        if (s == null) {
            Debug.LogError("FSM 错误：不允许添加空状态引用");
            return;
        }

        // 设置状态所属的 FSM 和控制器引用
        s.FSM = this;
        s.CTRL = ctrl;

        // 如果当前还没有任何状态，则第一个添加的状态即为初始状态
        if (states.Count == 0) {
            states.Add(s);
            return;
        }

        // 检查是否已经添加过相同 ID 的状态（每个状态ID必须唯一）
        foreach (FSMState state in states) {
            if (state.ID == s.ID) {
                Debug.LogError("FSM 错误：无法添加状态 " + s.ID.ToString() + "，因为该状态ID已存在");
                return;
            }
        }
        states.Add(s);
    }

    /// <summary>
    /// 从状态机中删除指定ID的状态。
    /// </summary>
    /// <param name="id">要删除的状态ID</param>
    public void DeleteState(StateID id) {
        if (id == StateID.NullStateID) {
            Debug.LogError("FSM 错误：不允许删除 NullStateID");
            return;
        }

        for (int i = 0; i < states.Count; i++) {
            if (states[i].ID == id) {
                states.RemoveAt(i);
                return;
            }
        }
        Debug.LogError("FSM 错误：无法删除状态 " + id.ToString() + "，该状态不在列表中");
    }

    /// <summary>
    /// 执行一次状态转换：根据当前状态和给定的转换，切换到目标状态。
    /// 如果当前状态没有定义该转换，则输出错误并保持当前状态。
    /// </summary>
    /// <param name="trans">要执行的转换</param>
    public void PerformTransition(Transition trans) {
        // 不允许使用 NullTransition 进行实际转换
        if (trans == Transition.NullTransition) {
            Debug.LogError("FSM 错误：不允许使用 NullTransition 进行实际转换");
            return;
        }

        // 获取当前状态针对该转换的目标状态ID
        StateID id = currentState.GetOutputState(trans);
        if (id == StateID.NullStateID) {
            Debug.LogError("FSM 错误：状态 " + currentStateID.ToString() + " 没有为转换 " + trans.ToString() + " 定义目标状态");
            return;
        }

        // 查找目标状态对象
        FSMState targetState = null;
        foreach (FSMState state in states) {
            if (state.ID == id) {
                targetState = state;
                break;
            }
        }

        if (targetState == null) {
            Debug.LogError("FSM 错误：目标状态ID " + id.ToString() + " 不存在于状态列表中");
            return;
        }

        // 执行状态切换
        currentState.DoBeforeLeaving();   // 旧状态退出前处理
        currentState = targetState;        // 更新当前状态
        currentStateID = id;               // 更新当前状态ID
        currentState.DoBeforeEntering();   // 新状态进入前处理
    }

} // class FSMSystem
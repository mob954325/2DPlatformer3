using UnityEngine;

// 진입
// 업데이트
// 종료

public abstract class StateBase : MonoBehaviour
{
    /// <summary>
    /// 상태 진입 시 호출되는 함수
    /// </summary>
    public abstract void StateEnter();

    /// <summary>
    /// 상태 업데이트 함수
    /// </summary>
    public abstract void StateUpdate();

    /// <summary>
    /// 상태 변경 시 변경 전 호출되는 함수
    /// </summary>
    public abstract void StateExit();
}

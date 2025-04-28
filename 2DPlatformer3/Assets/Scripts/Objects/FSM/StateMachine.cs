using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private StateBase currentState;

    /// <summary>
    /// 현재 상태 접근 및 수정용 프로퍼티
    /// </summary>
    public StateBase CurrentState 
    { 
        get => currentState;
        set
        {
            isStateChanging = true;
            currentState.StateExit();

            currentState = value;

            currentState.StateEnter();
            isStateChanging = false;
        }
    }

    private List<StateBase> StateList;

    bool isInitialzed = false;
    bool isStateChanging = false;

    private void Awake()
    {
        isInitialzed = false;

        // Load states
        StateList = GetComponentsInChildren<StateBase>().ToList();
        currentState = StateList[0]; // 리스트의 첫 번째로 초기화

        isInitialzed = true;
    }

    private void Update()
    {
        if (!isInitialzed) return;

        if(!isStateChanging)
        {
            CurrentState.StateUpdate();   
        }
    }

    private void FixedUpdate()
    {
        if (!isInitialzed) return;

        if (!isStateChanging)
        {
            CurrentState.StateFixedUpdate();
        }
    }

    public void StateChange(int listIndex)
    {
        CurrentState = StateList[listIndex];
    }
}

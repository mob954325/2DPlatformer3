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
            if(!isBusy) currentState.StateExit();

            currentState = value;

            if (!isBusy) currentState.StateEnter();
            isStateChanging = false;
        }
    }

    [SerializeField] private List<StateBase> StateList;

    private bool isInitialzed = false;
    private bool isStateChanging = false;
    private bool isBusy = false;

    private void Awake()
    {
        isInitialzed = false;        
    }

    private void OnEnable()
    {
        // Load states
        if(!isInitialzed)
        {
            StateList = GetComponentsInChildren<StateBase>().ToList();
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if(StateList.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} has zero states");
        }
        else
        {
            currentState = StateList[0]; // 리스트의 첫 번째로 초기화
            isInitialzed = true;
        }
    }

    private void Update()
    {
        if (!isInitialzed) return;

        if (isBusy) return;

        if(!isStateChanging)
        {
            CurrentState.StateUpdate();   
        }
    }

    private void FixedUpdate()
    {
        if (!isInitialzed) return;

        if (isBusy) return;

        if (!isStateChanging)
        {
            CurrentState.StateFixedUpdate();
        }
    }

    public void StateChange(int listIndex)
    {
        if (listIndex >= StateList.Count) return;
        CurrentState = StateList[listIndex];
    }

    public void SetTransitionBlocked(bool value)
    {
        isBusy = value;
    }
}

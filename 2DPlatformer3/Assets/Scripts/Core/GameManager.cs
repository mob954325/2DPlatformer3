using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    BeforeStart = 0, // 프로그램 시작 후 State 호출 전 상태
    Menu,
    Play,
}

public class GameManager : Singleton<GameManager>
{
    [Tooltip("PoolType 순서대로 오브젝트를 배치 할 것")]
    public GameObject[] poolPrefab = new GameObject[(int)PoolType.PoolTypeCount];

    private GameState state;
    public GameState State
    {
        get => state;
        set
        {
            if (state == value) return; // 중복 호출 방지

            state = value;
            Initialize(state);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        State = GameState.BeforeStart;
    }

    private void Start()
    {
        State = GameState.Menu;
    }

    private void Initialize(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                SetMenuScene();
                break;
            case GameState.Play:
                SetPlayScene();
                break;
            default:
                break;
        }
    }

    private void SetMenuScene()
    {
        Button StartButton = GameObject.Find("Start").GetComponent<Button>();
        StartButton.onClick.AddListener(() => 
        {
            Debug.Log("start");
            SceneChange(1);
        });

        Button ExitButton = GameObject.Find("Exit").GetComponent<Button>();
        ExitButton.onClick.AddListener(ExitGame);
    }

    private void SetPlayScene()
    {
        SetPoolManager();
    }

    private void SetPoolManager()
    {
        for(int i = 0; i < (int)PoolType.PoolTypeCount; i++)
        {
            PoolManager.Instacne.Register(((PoolType)i).ToString(), poolPrefab[i]);
        }
    }

    private void ExitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    public void SceneChange(int sceneIndex)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 중복 호출 방지
        PoolManager.Instacne.ClearPoolData();
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            State = GameState.Menu;
        }
        else
        {
            State = GameState.Play;
        }
    }
}

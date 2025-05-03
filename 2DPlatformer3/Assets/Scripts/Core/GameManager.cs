using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Cinemachine;

public enum GameState
{
    BeforeStart = 0, // 프로그램 시작 후 State 호출 전 상태
    Menu,
    Play,
    PlayEnd, // 플레이어 사망이나 게임 클리어일 때 
}

public class GameManager : Singleton<GameManager>
{
    DefeatPanel defeatPanel;

    public GameObject playerPrefab;
    public GameObject playerVCamPrefab;
    private Player spawnedPlayer;

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

        defeatPanel = GetComponentInChildren<DefeatPanel>();

        // 리소스 가져오기
        playerPrefab = Resources.Load<GameObject>("Prefab/Player/Player");

        poolPrefab = new GameObject[(int)PoolType.PoolTypeCount];
        poolPrefab[0] = Resources.Load<GameObject>("Prefab/Enemy/EnemyLight");

        playerVCamPrefab = Resources.Load<GameObject>("Prefab/Core/PlayerCam");

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

    public void SpawnPlayer(Vector3 position)
    {
        // 플레이어 중복 생성 방지
        if(spawnedPlayer != null)
        {
            Destroy(spawnedPlayer.gameObject);
            spawnedPlayer = null;
        }

        spawnedPlayer = Instantiate(playerPrefab, position, Quaternion.identity).GetComponent<Player>();
        CinemachineCamera vCam = Instantiate(playerVCamPrefab).GetComponent<CinemachineCamera>();

        vCam.Target.TrackingTarget = spawnedPlayer.gameObject.transform;
        vCam.Target.LookAtTarget = spawnedPlayer.gameObject.transform;
        vCam.gameObject.AddComponent<CinemachineFollow>();

        CinemachineConfiner2D confiner = vCam.gameObject.GetComponent<CinemachineConfiner2D>();
        confiner.BoundingShape2D = GameObject.Find("CameraBound").GetComponent<Collider2D>();
        confiner.InvalidateBoundingShapeCache();

        spawnedPlayer.OnDeadPerformed += () => { defeatPanel.OpenPanel(); State = GameState.PlayEnd; };
    }
}

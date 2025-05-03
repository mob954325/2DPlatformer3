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
    VictoryPanel victoryPanel;
    PlayPanel playPanel;

    public GameObject playerPrefab;
    public GameObject playerVCamPrefab;
    private Player spawnedPlayer;

    [Tooltip("PoolType 순서대로 오브젝트를 배치 할 것")]
    public GameObject[] poolPrefabs = new GameObject[(int)PoolType.PoolTypeCount];
    public AudioClip[] audioClips; 

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

    private int enemyCount = 0;
    public int EnemyCount
    {
        get => enemyCount;
        set
        {
            enemyCount = value;
            OnEnemyCountChange?.Invoke(EnemyCount);

            if(State == GameState.Play && enemyCount == 0)
            {
                State = GameState.PlayEnd;
                victoryPanel.OpenPanel();
            }
        }
    }

    public Action<int> OnEnemyCountChange;

    protected override void Awake()
    {
        base.Awake();

        defeatPanel = GetComponentInChildren<DefeatPanel>();
        victoryPanel = GetComponentInChildren<VictoryPanel>();
        playPanel = GetComponentInChildren<PlayPanel>();
        playPanel.ClosePanel();

        // 리소스 가져오기
        playerPrefab = Resources.Load<GameObject>("Prefab/Player/Player");

        poolPrefabs = new GameObject[(int)PoolType.PoolTypeCount];
        poolPrefabs[0] = Resources.Load<GameObject>("Prefab/Enemy/EnemyLight");
        poolPrefabs[1] = Resources.Load<GameObject>("Prefab/FX/HitEffect");

        playerVCamPrefab = Resources.Load<GameObject>("Prefab/Core/PlayerCam");

        State = GameState.BeforeStart;
    }

    private void Start()
    {
        OnEnemyCountChange += playPanel.EnemyCountUI.SetCount;
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
            SceneChange(1);
        });

        Button ExitButton = GameObject.Find("Exit").GetComponent<Button>();
        ExitButton.onClick.AddListener(ExitGame);

        playPanel.ClosePanel();
        EnemyCount = 0;

        SoundManager.Instacne.PlayBGM(BGMType.Menu);
    }

    private void SetPlayScene()
    {
        SetPoolManager();
        playPanel.OpenPanel();
        SoundManager.Instacne.PlayBGM(BGMType.Battle);
    }

    private void SetPoolManager()
    {
        for(int i = 0; i < (int)PoolType.PoolTypeCount; i++)
        {
            PoolManager.Instacne.Register(((PoolType)i).ToString(), poolPrefabs[i]);
        }
    }

    private void ExitGame()
    {
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
        spawnedPlayer.OnHpChange += playPanel.PlayerHpGauge.SetValue;
        spawnedPlayer.Initialize();
    }
}

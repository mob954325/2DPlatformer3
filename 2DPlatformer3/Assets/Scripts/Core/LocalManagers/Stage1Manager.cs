using UnityEngine;

public class Stage1Manager : MonoBehaviour
{
    public Transform playerSpawn;
    public Transform[] enemyspawns;

    private void Start()
    {
        // 스폰 지점 찾기
        playerSpawn = GameObject.Find("PlayerSpawnPoint").transform;

        GameObject enemySpawnPoints = GameObject.Find("EnemySpawnPoints");
        enemyspawns = enemySpawnPoints.GetComponentsInChildren<Transform>();

        GameManager.Instacne.SpawnPlayer(playerSpawn.position);

        for(int i = 1; i < enemyspawns.Length; i++)
        {
            EnemyBase enemy = PoolManager.Instacne.Pop(PoolType.EnemyMelee, enemyspawns[i].position).GetComponent<EnemyBase>();
            GameManager.Instacne.EnemyCount++;
            enemy.OnDeadPerformed += () => { GameManager.Instacne.EnemyCount--; };
        }
    }
}

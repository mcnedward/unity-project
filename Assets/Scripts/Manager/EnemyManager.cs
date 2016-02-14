using Assets.Scripts.Status;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class EnemyManager : MonoBehaviour
    {
        public Health PlayerHealth;
        public GameObject Enemy;
        public float SpawnTime = 3f;
        public Transform[] SpawnPoints;
        public int _enemyLimit = 4;

        private int _enemyCount = 0;

        // Use this for initialization
        void Start()
        {
            InvokeRepeating("Spawn", SpawnTime, SpawnTime);
        }

        void Spawn()
        {
            if (PlayerHealth.IsPlayerDead() || _enemyCount == 4)
                return;

            var spawnPointIndex = Random.Range(0, SpawnPoints.Length);

            Instantiate(Enemy, SpawnPoints[spawnPointIndex].position, Quaternion.identity);
            _enemyCount++;
        }

        public void KillEnemy()
        {
            _enemyCount--;
        }
    }
}
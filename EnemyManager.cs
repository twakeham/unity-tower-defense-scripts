using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// Tracks all enemies currently alive

public class EnemyManager : Singleton<EnemyManager> {

    public List<string> enemyTypes;

    [HideInInspector]
    public static List<Enemy> enemies = new List<Enemy>(16);

    protected EnemyManager() { }
 
    public static Enemy spawnEnemy(GameObject prefab, Vector3 position, Quaternion rotation) {
        /// Create enemy at given location and rotation.
        GameObject enemyObject = (GameObject)GameObject.Instantiate(prefab, position, rotation);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
      
        /// Store reference to enemy and give it an id.
        EnemyManager.enemies.Add(enemy);

        return enemy;
    }

    public static void destroyEnemy(Enemy enemy) {
        /// Remove enemy from manager. 
        EnemyManager.enemies.Remove(enemy);
        GameObject.Destroy(enemy.gameObject);
    }
}

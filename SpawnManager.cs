using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Enemy waves

// Waves are made up of a number of wave components that can
// be delayed and assigned to specific paths
[System.Serializable]
public class WaveComponent {
    public GameObject enemyPrefab;
    public Path path;
    public int enemiesInWave;
    public int startTimeDelay;
    public float timeBetweenEnemies;
}


[System.Serializable]
public class Wave {
    public WaveComponent[] waveComponents;
}


//[System.Serializable]
//public class Wave1 {
//    public GameObject enemyPrefab;
//    public int enemiesInWave;
//    public float timeBetweenEnemies;
//    public float timeBeforeNextWave;
//}

// Spawn Manager controls instantiation of enemies in correct position/rotation and attaching them to
// the correct path
    
public class SpawnManager : MonoBehaviour {
   
    public float startTime;
    public float timeBetweenWaves;
    public List<Wave> waves;

    private Task _spawner;

    public void Start() {
        _spawner = new Task(spawnWaves());
    }

    public void spawnEnemy(GameObject enemyPrefab, Path path) { 
        Vector3 position = path.getPoint(0f);
        Vector3 rotation = path.getTangent(0f);

        Enemy enemy = EnemyManager.spawnEnemy(enemyPrefab, position, Quaternion.LookRotation(rotation));
        enemy.path = path;
    }

    public IEnumerator spawnWaveComponent(WaveComponent waveComponent) {
        yield return new WaitForSeconds(waveComponent.startTimeDelay);
        
        for (int idx = 0; idx < waveComponent.enemiesInWave; idx++) {
            spawnEnemy(waveComponent.enemyPrefab, waveComponent.path);
            yield return new WaitForSeconds(waveComponent.timeBetweenEnemies);
        }
    }

    public IEnumerator spawnWaves() {
        /// delay for number of seconds specified
        yield return new WaitForSeconds(startTime);

        foreach (Wave wave in waves) {
            List<Task> waveComponentTasks = new List<Task>();

            foreach (WaveComponent waveComponent in wave.waveComponents) {
                Task waveComponentTask = new Task(spawnWaveComponent(waveComponent));
                waveComponentTasks.Add(waveComponentTask);
            }

            /// wait until all tasks have finished running
            yield return new WaitUntil(() => waveComponentTasks.FindAll((Task waveComponentTask) => { return waveComponentTask.running; }).Count == 0 );

            /// wait for time between waves
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}
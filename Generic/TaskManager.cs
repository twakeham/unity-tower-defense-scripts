using UnityEngine;
using System;
using System.Collections;


/// Task manager for starting and managing Unity coroutines
// Enables pausing and stopping of coroutines.

public class TaskManager : MonoBehaviour {

    // Task manager is a singleton class that is used by Task instances
    // to start coroutines.

    private static TaskManager _instance = null;

    private TaskManager() { }

    public static TaskManager instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType(typeof(TaskManager)) as TaskManager;

                if (_instance == null) {
                    GameObject obj = new GameObject("TaskManager");
                    _instance = obj.AddComponent<TaskManager>();
                }
            }
            return _instance;
        }
    }
}


public class Task {

    #region PublicMembers
    public event System.Action<bool> jobComplete;
    #endregion

    #region PrivateMembers
    private System.Action _callback;
    private IEnumerator _coroutine;
    #endregion

    #region Properties
    private bool _running;
    public bool running { get { return _running; } }

    private bool _paused;
    public bool paused { get { return _paused; } }

    private bool _killed;
    public bool killed { get { return _killed; } }
    #endregion

    #region Initialisers
    public Task(IEnumerator coroutine, System.Action callback, bool startCoroutine=true) : this(coroutine, startCoroutine) {
        _callback = callback;
    }

    public Task(IEnumerator coroutine, bool startCoroutine=true) {
        _coroutine = coroutine;
        if (startCoroutine) {
            start();
        }
    }
    #endregion

    #region PublicMethods
    public void start() {
        _running = true;
        TaskManager.instance.StartCoroutine(managedTask());
    }

    public IEnumerator startAsCoroutine() {
        _running = true;
        yield return TaskManager.instance.StartCoroutine(managedTask());
    }

    public void pause() {
        _paused = true;
    }

    public void unpause() {
        _paused = false;
    }

    public void kill() {
        _killed = true;
        _running = false;
        _paused = false;
    }

    public void kill(int delay) {
        // delay in milliseconds
        new System.Threading.Timer(obj =>
        {
            lock (this) {
                kill();
            }
        }, null, delay, System.Threading.Timeout.Infinite);
    }

    public Task then(Task task) {
        jobComplete += (bool killed) => {
            task.start();
        };
        return task;
    }

    public Task then(IEnumerator coroutine) {
        Task task = new Task(coroutine, false);
        jobComplete += (bool killed) => {
            task.start();
        };
        return task;
    }

    public IEnumerator managedTask() {
        // need this in case we start paused
        yield return null;

        while (_running) {
            if (_paused) {
                yield return null;
            }
            else {
                if (_coroutine.MoveNext()) {
                    yield return _coroutine.Current;
                }
                else {
                    _running = false;
                }
            }
        }
        if (jobComplete != null) {
            jobComplete(_killed);
        }
        if (_callback != null) {
            _callback();
        }
    }
    #endregion
}

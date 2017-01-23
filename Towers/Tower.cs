using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Game objects for managing buying/upgrading/selling towers

[System.Serializable]
public class TowerLevel {

    public string name;
    public GameObject tower;
    public int buildTime;
    public float buyValue;
    public float sellValue;

}

public class Tower : MonoBehaviour {

    public string towerName;
    public string description;
    public Sprite icon;

    public List<TowerLevel> towerLevels;

}
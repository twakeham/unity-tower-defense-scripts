using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Basic tower properties

public class BaseTower : MonoBehaviour {

    [Header("Tower Properties")]
    public bool targetsGround = true;
    public bool targetsAir = true;

    [HideInInspector]
    public TowerModifier modifiers;
}
    
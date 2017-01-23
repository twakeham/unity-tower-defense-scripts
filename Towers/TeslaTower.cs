using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Electricity tower - shoots nearest enemy with chance for beam to jump to other near enemies

public class TeslaTower : BaseTower {

    [Header("Firing Properties")]
    public float range;
    public float damage;
    public float cooldownTime;

    private Task _firing;

	// Use this for initialization
	void Start () {
        _firing = new Task(firing());
	}
	
    protected IEnumerator firing() {
        while(true) {
            List<Enemy> enemies = getTargets();
            if (enemies.Count == 0) continue;
            enemies.ForEach((enemy) => {
                enemy.inflictDamage(damage);
            });

            yield return new WaitForSeconds(cooldownTime);
        }
    }

    private List<Enemy> getTargets() {
        return EnemyManager.enemies.FindAll((enemy) => {
            return Vector3.Distance(transform.position, enemy.transform.position) <= range;    
        });
    }

}

using UnityEngine;
using System.Collections;

// Old towers - keeping around for now but don't use!

public class Turret : MonoBehaviour {

    public float targetRange;
    public float firingRange;
    public float rateOfTurn;

    public GameObject bullet;
    public GameObject firingPoint;

    public int rateOfFire;

    private float cooldown;
    private float cooldownRemaining;

    [System.Serializable]
    public struct FOV {
        public float minimumRotation;
        public float maximumRotation;
    }

    public FOV fieldOfView;

    Transform turret;

    bool lockedOn = false;

	void Start () {
        turret = transform.Find("Turret");
        cooldown = 1f / rateOfFire;
	}
	
	// Update is called once per frame
	void Update () {
	    //
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();

        Enemy nearestEnemy = null;
        float distance = Mathf.Infinity;

        foreach (Enemy enemy in enemies) {
            if (enemy == null) {
                continue;
            }
            float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if (nearestEnemy == null || enemyDistance < distance) {
                distance = enemyDistance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy == null) {
            // no enemies
            return;
        }

        Vector3 direction = nearestEnemy.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        float yRotation = lookRotation.eulerAngles.y;

        //if (lookRotation.eulerAngles.y < fieldOfView.minimumRotation) {
        //    yRotation = fieldOfView.minimumRotation;
        //}

        //if (lookRotation.eulerAngles.y > fieldOfView.maximumRotation) {
        //    yRotation = fieldOfView.maximumRotation;
        //}

        Quaternion targetRotation = Quaternion.Euler(0, yRotation, 0);

        // TODO: smooth this out.
        turret.rotation = targetRotation;

        if (distance > firingRange) {
            // out of range for firing
            return;
        }

        cooldownRemaining -= Time.deltaTime;
        if (cooldownRemaining <= 0) {
            cooldownRemaining = cooldown;
            Shoot(nearestEnemy);
        }

	}

    void Shoot(Enemy enemy) {
        GameObject bulletObject = (GameObject)Instantiate(bullet, firingPoint.transform.position, transform.rotation);
        Projectile projectile = bulletObject.GetComponent<Projectile>();
        if (enemy.target == null) {
            projectile.target = enemy.transform;
        } else {
            projectile.target = enemy.target.transform;
        }
        projectile.enemy = enemy;
    }
}

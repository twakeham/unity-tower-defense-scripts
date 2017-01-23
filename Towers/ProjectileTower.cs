using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ProjectileTower : BaseTower {

    [Flags]
    public enum TargetingMode {
        Nearest = 1,
        Furthest = 2,
        Weakest = 4,
        Strongest = 8,
        Slowest = 16,
        Fastest = 32,
        Newest = 64,
        Oldest = 128
    }

    [Header("Field of View")]
    public bool restrictFieldOfView;
    public int fieldOfViewAngle;

    [Header("Targeting Properties")]
    [Tooltip("Should the turret rotate back to it's home position when there are no enemies")]
    public bool homeWhenNoEnemies;
    [Tooltip("This attribute determines whether a tower will track a target until it is locked or switch targets it a better one becomes available during the tracking process")]
    public bool requireTargetingLock;
    public TargetingMode targetingMode;
    public float rateOfTurn;

    [Header("Firing Properties")]
    public float firingRange;
    public float firingRate;
    public float damagePerHit;

    [Header("Game objects")]
    public GameObject tower;
    public GameObject turret;
    public GameObject weapon;
    public GameObject firingPoint;
    public GameObject projectile = null;
    public GameObject muzzleFlash = null;

    // targeting delegate
    protected delegate Enemy EnemyTargetingFunction(List<Enemy> enemies);

    // Current target enemy.
    protected Enemy _target;
    public Enemy target { get { return _target; } }
    protected bool _targetLock;
    // Secondary target enemy.
    protected Enemy _secondaryTarget;

    // Tasks to run on Tower.
    protected Task _tracking;
    protected Task _targeting;
    protected Task _firing;

    // particle systems that we need to turn on and off
    protected ParticleSystem[] particles;

    protected virtual void Start () {
        if (muzzleFlash) particles = muzzleFlash.GetComponentsInChildren<ParticleSystem>();

        EnemyTargetingFunction targetingDelegate = null;

        switch(targetingMode) {
            case TargetingMode.Nearest: 
                targetingDelegate = new EnemyTargetingFunction(targetNearest);
                break;

            case TargetingMode.Furthest:
                targetingDelegate = new EnemyTargetingFunction(targetFurthest);
                break;
        }

        _targeting = new Task(targeting(new EnemyTargetingFunction(targetingDelegate)));
        _tracking = new Task(tracking());
        _firing = new Task(firing());
    
         // hide muzzle flash
        setMuzzleFlashVisible(false);

    }

//    protected void OnDrawGizmosSelected() {
//        UnityEditor.Handles.color = new Color(1f, 1f, 1f, 0.1f);
//        UnityEditor.Handles.DrawSolidArc(tower.transform.position, tower.transform.up, tower.transform.right, 360, firingRange);
//    }

    protected virtual IEnumerator tracking() {

        /// rotation step size
        float step = 0.25f * rateOfTurn;

        // waiting until targeting has found a target before starting to track
        yield return new WaitUntil(() => _secondaryTarget != null);
        _target = _secondaryTarget;

        while (true) {

            /// if the rotateToHomePosition property is set, return to original rotation when
            /// there are no more enemies in the firing range.
            if (_target == null && homeWhenNoEnemies) {
                Quaternion homeRotation = Quaternion.Euler(0, 0, 0);
                turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, homeRotation, step);
            }

            while (distanceToTarget() <= firingRange && !_target.isDead) {

                Vector3 direction;
                // find the direction to our current target and calculate look at rotation
                if (target) {
                    direction = target.transform.position - turret.transform.position;
                } else {
                    direction = _target.transform.position - turret.transform.position;
                }
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                float yRotation = lookRotation.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0, yRotation, 0);

                // rotate towards current target
                turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, targetRotation, step);

                // if a separate weapon is specified, point it at the enemy;
                if (weapon != null) {
                    Quaternion weaponRotation = Quaternion.Euler(lookRotation.eulerAngles.x, 0, 0);
                    Quaternion interpolatedRotation = Quaternion.RotateTowards(weapon.transform.rotation, weaponRotation, rateOfTurn);
                    interpolatedRotation.y = 0;
                    interpolatedRotation.z = 0;
                    weapon.transform.localRotation = interpolatedRotation;
                }

                // check if we are within a degree of the target for target lock
                _targetLock = Vector3.Angle(direction, weapon.transform.forward) < 2.5f;
                //Console.Log(_targetLock, Vector3.Angle(direction, weapon.transform.forward));

                if (!requireTargetingLock) {
                    // if we haven't got a lock on this target yet, switch to closer enemy if there is one
                    if (!_targetLock && _secondaryTarget != _target) {
                        _target = _secondaryTarget;
                    }
                }
                    
                yield return null;
            }

            _target = _secondaryTarget;

            // target outside range, turn off particles
            setMuzzleFlashVisible(false);
            yield return null;
        }

    }

    protected virtual IEnumerator targeting(EnemyTargetingFunction targetingFunction) {
        while (true) {
            /// filter the list of all enemies to only contain enemies within firing range
            /// of this turret.
            List<Enemy> enemiesInRange = EnemyManager.enemies.FindAll((enemy) => {
                if (enemy == null) return false;
                float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
                return enemyDistance <= firingRange;
            });

            /// if we are utilising field of view, filter out all enemies outside FOV
            if (restrictFieldOfView) {
                enemiesInRange = enemiesInRange.FindAll((enemy) => {
                    float angleToTurret = Vector3.Angle(turret.transform.forward, enemy.transform.position - transform.position);
                    return angleToTurret < fieldOfViewAngle / 2f;
                });
            }
                
            _secondaryTarget = targetingFunction(enemiesInRange);

            yield return null;
        }
    }

    protected Enemy targetNearest(List<Enemy> enemies) {
        /// targeting function that targets the enemy with the least angle between their position
        /// and the current turret rotation

        float closestAngle = Mathf.Infinity;
        Enemy closestEnemy = null;

        /// find enemy target with the least turret rotation required to track it
        enemies.ForEach((enemy) => {
            /// enemy might have been destroyed already
            if (enemy == null) {
                return;
            }
            /// create enemy vector in the same Y plane as the firing point
            Vector3 enemyLocation = new Vector3(enemy.transform.position.x, firingPoint.transform.position.y, enemy.transform.position.z);
            float angleToTurret = Vector3.Angle(turret.transform.forward, enemyLocation);
            if (angleToTurret < closestAngle) {
                closestAngle = angleToTurret;
                closestEnemy = enemy;
            }
        });

        return closestEnemy;
    }

    protected Enemy targetFurthest(List<Enemy> enemies) {
        /// targeting function that targets the enemy with the least angle between their position
        /// and the current turret rotation

        float furthestAngle = 0;
        Enemy furthestEnemy = null;

        /// find enemy target with the least turret rotation required to track it
        enemies.ForEach((enemy) => {
            /// enemy might have been destroyed already
            if (enemy == null) {
                return;
            }
            /// create enemy vector in the same Y plane as the firing point
            Vector3 enemyLocation = new Vector3(enemy.transform.position.x, 
                firingPoint.transform.position.y, 
                enemy.transform.position.z);
            float angleToTurret = Vector3.Angle(turret.transform.forward, enemyLocation);
            if (angleToTurret > furthestAngle) {
                furthestAngle = angleToTurret;
                furthestEnemy = enemy;
            }
        });

        return furthestEnemy;
    }
        
    protected virtual IEnumerator firing() {
        while(true) {
            if (_targetLock) {
                setMuzzleFlashVisible(true);
                if (_target != null && !_target.isDead) _target.inflictDamage(damagePerHit);
            } else {
                setMuzzleFlashVisible(false);
            }
            yield return new WaitForSeconds(1f / firingRate);    
        }
    }

    /// Calculate distance to current target
    public float distanceToTarget() {
        return _target == null ? Mathf.Infinity : Vector3.Distance(firingPoint.transform.position, _target.transform.position);
    }

    /// Sets the play status of any particle systems attached to muzzle flash
    public void setMuzzleFlashVisible(bool visible) {
        if (muzzleFlash != null) {
            foreach (ParticleSystem particle in particles) {
                if (visible) {
                    if (!particle.isPlaying) particle.Play();
                } else {               
                    particle.Stop();
                }
            }
        }
    }

}


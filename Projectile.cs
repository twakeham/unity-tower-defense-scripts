using UnityEngine;
using System.Collections;

/// GameObject component that manages projectile properties

public class Projectile : MonoBehaviour {

    [Header("Projectile properties")]
    public float speed = 15f;
    public float damage = 1f;

    [Header("Area of effect")]
    public bool AOE = false;
    public float radius;
    public float falloff;

    [Header("Target object")]
    public Transform target;
    public Enemy enemy;

    [Header("Particles")]
    public GameObject explosion;

    protected Task _moving;

    private Vector3 _target;

    public void fire() {
        _moving = new Task(moving());
    }

    protected IEnumerator moving() {
        _target = new Vector3(target.position.x, 0, target.position.z);
        Vector3 dir = _target - transform.position;
        while(true) {
            float deltaDist = speed * Time.deltaTime;
            transform.Translate(dir.normalized * deltaDist, Space.World);
           
            yield return null;
        }
    }

    public void OnCollisionEnter(Collision hitPoint) {
        /// detect collision with enemy and terrain mesh colliders
        /// collision with anything else is ignored
        if (hitPoint.gameObject.tag != "Enemy" && hitPoint.gameObject.tag != "Terrain") return;

        if (AOE == false) {
            enemy.inflictDamage(damage);
        } else {
            /// calculates hitbox and damage with falloff for AOE projectiles
            Collider[] colliders = Physics.OverlapSphere(hitPoint.transform.position, radius);

            foreach(Collider collider in colliders) {
                if (collider.gameObject.tag == "Enemy") {
                    Enemy hitEnemy = collider.GetComponent<Enemy>();
                    if (falloff == 0) {
                        hitEnemy.inflictDamage(damage);
                    } else {
                        float distance = Vector3.Distance(collider.transform.position, transform.position);
                        float falloffDamage = (1f - falloff) * distance / radius * damage;
                        hitEnemy.inflictDamage(falloffDamage);
                    }
                }
            }
        }
        _moving.kill();
        Destroy(gameObject);

        /// run particle effect for explosion
        if (explosion) {
            //Vector3 rotation = transform.LookAt(transform.position + Camera.main.transform.forward, Camera.main.transform.up);
            GameObject explosionObject = (GameObject)Instantiate(explosion, hitPoint.transform.position, hitPoint.transform.rotation); 
        }
    }
}

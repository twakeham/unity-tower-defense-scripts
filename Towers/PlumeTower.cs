using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// AOE damage turret that triggers on collision with detection box
// Plume towers are static

public class PlumeTower : BaseTower {

    [Header("Firing Properties")]
	public float damagePerSecond;
    public float firingTime;
	public float cooldownTime;

    private bool _firing = false;
    private int _enemyCount = 0;
    private List<Enemy> enemies = new List<Enemy>();
    private ParticleSystem[] particles;

	private Task _fireDamage;
    private Task _fireParticles;

	void Start () {
        particles = GetComponentsInChildren<ParticleSystem>();
        setParticlesVisible(false);

        _fireDamage = new Task(fireDamage());
        _fireParticles = new Task(fireParticles());
	}

	void OnTriggerEnter(Collider collider) {
	    // adds enemy to those being damaged on collision with collider
		if (collider.gameObject.tag == "Enemy") {
			Enemy enemy = collider.gameObject.GetComponent<Enemy>();
			enemies.Add(enemy);
		}
	}

    void OnTriggerExit(Collider collider) {
        // removes enemy from being damaged on exit from collision area
        if (collider.gameObject.tag == "Enemy") {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            enemies.Remove(enemy);
        }
    }

    private IEnumerator fireDamage() {

        while(true) {
            // clears references to dead enemies
            enemies = enemies.FindAll((enemy) => enemy != null);

            if (enemies.Count > 0) {
                if (_firing) {
                    float damage = damagePerSecond * Time.deltaTime;
                    enemies.ForEach((Enemy enemy) => {
                        enemy.inflictDamage(damage);
                    });
                }
            }
            yield return null;
        }
    }

    private IEnumerator fireParticles() {
        while(true) {
            if (enemies.Count > 0) {
                if (!_firing) {
                    _firing = true;
                    setParticlesVisible(true);
                    yield return new WaitForSeconds(firingTime);
                    _firing = false;
                    setParticlesVisible(false);
                    yield return new WaitForSeconds(cooldownTime);
                }
            } else {
                yield return null;
            }
        }
    }

    public void setParticlesVisible(bool visible) {
        foreach (ParticleSystem particle in particles) {
            if (visible) {
                if (!particle.isPlaying) particle.Play();
            } else {               
                particle.Stop();
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// Enemies! Every game needs 'em

public class Enemy : MonoBehaviour {

    /// EnemyType denotes whether the enemy is a ground or air type enemy.
    /// This is used by turrets in targeting to ensure that they only target
    /// the correct enemy type.
    public enum EnemyType {
        Ground,
        Air
    }

    [Header("Enemy properties")]
    public string enemyName;
    public EnemyType enemyType;
    public int speed;
    /// if provided, turrets will use this game object as the target rather than the object o
    public GameObject target;

    [Header("Health")]
    public float hp;
    public Slider healthBar;

    [Header("Animations")]
    public AnimationClip walkAnimation;
    public AnimationClip tauntAnimation;
    public AnimationClip deathAnimation;
    [Tooltip("Requires transparent materials in the second spot on all mesh objects")]
    public bool fadeOutOnDeath;

    [Header("Modifiers")]
    public List<TowerModifier> towerModifiers;

    [HideInInspector]
    public bool isDead = false;

    /// Pathing information required by the enemy to walk the path
    /// assigned by the SpawnManager
    [NonSerialized]
    public Path path;

    /// Tasks used by enemy
    private Task _walk;
    private Task _die;
    private Task _fade;

    /// Reference to enemy component
    private Enemy _enemy;

    /// store the full HP for health bar calcs
    private float _maxHp;

    void Start() {
        _enemy = GetComponent<Enemy>();
        _walk = new Task(walkTask());
        _die = new Task(dieTask(), false);

        _maxHp = hp;
    }

    void Update() {
    }

    /// Inflicts damage on enemy.  Returns true if it kills enemy, else false.
    public bool inflictDamage(float damage) {
        hp -= damage;

        if (hp <= 0) {
            isDead = true;
            _walk.kill();
            _die.start();
            return true;
        }

        float percentageHp = hp / _maxHp;
        healthBar.value = percentageHp;
        return false;
    }

    public Transform getTarget() {
        return target?.transform ?? transform;
    }

    public IEnumerator walkTask() {
        // wait for path to be set before starting
        yield return new WaitUntil(() => path != null);

        Animation animation = GetComponentInChildren<Animation>();
        animation.Play(walkAnimation.name);

        int segments = path.segments.Count;
        float stepSize = 1f / path.pathAccuracy;

        // this should be generalised away from enemies and into a specific path follower class
        for (float step = stepSize; step < segments; step += stepSize) {
            Vector3 point = path.getPoint(step);
            while (true) {
                // move towards point
                Vector3 dir = point - transform.localPosition;
                float deltaDist = speed * Time.deltaTime;
                //Debug.Log(String.Format("{0} {1}", dir.magnitude, deltaDist));
                if (dir.magnitude <= deltaDist) break;

                transform.Translate(dir.normalized * deltaDist, Space.World);
                Quaternion rotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
                yield return null; 
            }
        }
        EnemyManager.destroyEnemy(_enemy);
    }

    public IEnumerator dieTask() {
        if (fadeOutOnDeath) new Task(fadeOut(deathAnimation.length));
        /// get rid of the health bar when the enemy is dying
        Destroy(GetComponentInChildren<Canvas>().gameObject);
        Animation animation = GetComponentInChildren<Animation>();
        animation.CrossFade(deathAnimation.name);
        while (animation.IsPlaying(deathAnimation.name)) {
            yield return null;
        }
    }

    public IEnumerator fadeOut(float duration) {
        /// it's surprisingly problematic to get Unity to fade out an object
        /// so we have a custom shader that supports alpha channels.  The shader looks
        /// odd because the interaction between moving verts and transparent materials
        /// so we only swap it in when the enemy dies and fades out

        // GameObjects regularly have nested objects and multiple mesh renderers that need to be swapped out
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers) {
            Material[] materials = skinnedMeshRenderer.materials;
            skinnedMeshRenderer.material = materials[1];
        }

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers) {
            Material[] materials = meshRenderer.materials;
            meshRenderer.material = materials[1];
        }

        bool done = false;

        while(!done) {

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers) {
                Color colour = skinnedMeshRenderer.material.color;
                colour.a -= Time.deltaTime / duration;
                skinnedMeshRenderer.material.color = colour;
                if (colour.a <= 0f) done = true;
            }

            foreach (MeshRenderer meshRenderer in meshRenderers) {
                Color colour = meshRenderer.material.color;
                colour.a -= Time.deltaTime / duration;
                meshRenderer.material.color = colour;
            }
                
            yield return null;

        }

        EnemyManager.destroyEnemy(_enemy);

    }
}


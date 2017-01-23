using UnityEngine;
using System.Collections;

// Extension of Projectile tower where projectiles cause splash damage

public class ExplodingProjectileTower : ProjectileTower {

    protected override IEnumerator firing() {
        while(true) {
            if (_targetLock) {
                GameObject projectileObject = (GameObject)Instantiate(projectile, firingPoint.transform.position, transform.rotation);
                Projectile projectileComponent = projectileObject.GetComponent<Projectile>();
                if (_target) {
                    if (_target.target == null) {
                        projectileComponent.target = _target.transform;
                    } else {
                        projectileComponent.target = _target.target.transform;
                    }
                    projectileComponent.enemy = _target;
                    projectileComponent.fire();
                }
            }
            yield return new WaitForSeconds(1f / firingRate);    
        }
    }
}

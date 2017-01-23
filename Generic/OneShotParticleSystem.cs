using UnityEngine;
using System.Collections;

/// Component : Plays a particle system once through then destroys it

public class OneShotParticleSystem : MonoBehaviour {

    public ParticleSystem particleSystem;

    private Task _playing;

	// Use this for initialization
	void Start () {
        _playing = new Task(playing());
	}

    protected IEnumerator playing() {
        yield return new WaitForSeconds(particleSystem.duration);
        Destroy(gameObject);
    }
}

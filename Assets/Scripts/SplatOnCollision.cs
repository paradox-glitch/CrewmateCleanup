using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatOnCollision : MonoBehaviour {

	public ParticleSystem particleLauncher;
	public Gradient particleColorGradient;
	public ParticleDecalPool dropletDecalPool;
	public GameObject m_LevelManager;
	List<ParticleCollisionEvent> collisionEvents;


	void Start () 
	{
		collisionEvents = new List<ParticleCollisionEvent> ();
		m_LevelManager = GameObject.FindGameObjectWithTag("Manager.Level");
	}

	void OnParticleCollision(GameObject other)
	{
		if (other.gameObject.layer == 11)
			return;

		int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents (particleLauncher, other, collisionEvents);

		int i = 0;
		while (i < numCollisionEvents) 
		{
            dropletDecalPool.ParticleHit(collisionEvents[i], particleColorGradient);
            i++;
		}
		m_LevelManager.GetComponent<LevelManager>().SendMessage("DoCheck");
	}

}

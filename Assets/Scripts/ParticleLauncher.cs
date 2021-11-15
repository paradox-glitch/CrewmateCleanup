using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParticleLauncher : MonoBehaviour
{
    public ParticleSystem m_ParticleLauncher;
    public ParticleSystem m_SplatterParticle;

    public ParticleDecalPool pool;

    public Gradient m_ParticleColor;

    List<ParticleCollisionEvent> m_CollisionEvents;

    void Start()
    {
        pool = GameObject.FindGameObjectWithTag("SpaltDecalPool").GetComponent<ParticleDecalPool>();
        m_SplatterParticle = GameObject.FindGameObjectWithTag("SplatParticles").GetComponent<ParticleSystem>();
        m_CollisionEvents = new List<ParticleCollisionEvent>();
        m_ParticleLauncher = this.gameObject.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        ParticleSystem.MainModule _psMain = m_ParticleLauncher.main;
        _psMain.startColor = m_ParticleColor.Evaluate(Random.Range(0f, 1f));
    }

    void EmitAtSplatterLocation(ParticleCollisionEvent particle)
    {
        m_SplatterParticle.transform.position = particle.intersection;
        m_SplatterParticle.transform.rotation = Quaternion.LookRotation(particle.normal);
        ParticleSystem.MainModule _psMain = m_SplatterParticle.main;
        _psMain.startColor = m_ParticleColor.Evaluate(Random.Range(0f, 1f));
        m_SplatterParticle.Emit(1);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.layer == 11)
            return;

        ParticlePhysicsExtensions.GetCollisionEvents(m_ParticleLauncher, other, m_CollisionEvents);
    for (int i = 0; i < m_CollisionEvents.Count; i++)
    {
            pool.ParticleHit(m_CollisionEvents[i], m_ParticleColor);
            EmitAtSplatterLocation(m_CollisionEvents[i]);
    }
    }
}
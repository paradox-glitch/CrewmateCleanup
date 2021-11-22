using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sys = System;

public class ParticleDecalPool : MonoBehaviour
{
    private int _ppdIndex;

    private ParticleDecalData[] particleData;

    public float m_DecalSizeMin = 0.5f, m_DecalSizeMax = 1.5f;

    public int _MaxDecals;

    public Gradient m_BloodGradient;

    private ParticleSystem.Particle[] m_particles;

    private ParticleSystem decalsystem;

    void Start()
    {
        decalsystem = GetComponent<ParticleSystem>();
        m_particles = new ParticleSystem.Particle[_MaxDecals];
        particleData = new ParticleDecalData[_MaxDecals];
        for(int i =0; i < _MaxDecals; i++)
        {
            particleData[i] = new ParticleDecalData();
        }
    }

    public void ParticleHit(ParticleCollisionEvent _pce, Gradient _cg)
    {
        SetParticleData(_pce, _cg);
        DisplayParticles();
    }

    void SetParticleData(ParticleCollisionEvent _pce, Gradient _cg)
    {
        if(_ppdIndex >= _MaxDecals)
        {
            _ppdIndex = 0;
        }

        

        particleData[_ppdIndex].position = _pce.intersection;
        Vector3 DecalRotQual = Quaternion.LookRotation(_pce.normal).eulerAngles;
        DecalRotQual.z = Random.Range(0, 360);
        particleData[_ppdIndex].rotation = DecalRotQual;
        particleData[_ppdIndex].size = Random.Range(m_DecalSizeMin, m_DecalSizeMax);
        particleData[_ppdIndex].color = _cg.Evaluate(Random.Range(0f, 1f));
        particleData[_ppdIndex].clean = false;
        _ppdIndex++;
    }

    public void SetParticalDataDirect(Vector3 a_Position, Vector2 a_Rotation)
    {
        if (_ppdIndex >= _MaxDecals)
        {
            _ppdIndex = 0;
        }
        particleData[_ppdIndex].position = a_Position;
        Vector3 DecalRotQual = Quaternion.LookRotation(a_Rotation).eulerAngles;
        DecalRotQual.z = Random.Range(0, 360);
        particleData[_ppdIndex].rotation = DecalRotQual;
        particleData[_ppdIndex].size = Random.Range(m_DecalSizeMin, m_DecalSizeMax);
        particleData[_ppdIndex].color = m_BloodGradient.Evaluate(Random.Range(0f, 1f));
        particleData[_ppdIndex].clean = false;
        _ppdIndex++;
        DisplayParticles();
    }

    void DisplayParticles()
    {
        for (int i = 0; i < particleData.Length; i++)
        {
            if (!particleData[i].clean)
            {
                m_particles[i].position = particleData[i].position;
                m_particles[i].rotation3D = particleData[i].rotation;
                m_particles[i].startSize = particleData[i].size;
                m_particles[i].startColor = particleData[i].color;
            }
            else
            {
                m_particles[i].startSize = 0;
            }
        }

        decalsystem.SetParticles(m_particles, m_particles.Length);
    }

    void SortArray()
    {
        
    }

    public int DecalsLeft()
    {
        int l_DecalsLeft = 0;

        for (int i = 0; i < particleData.Length; i++)
        {
            if (particleData[i].clean == false)
            {
                l_DecalsLeft++;
            }
        }

        return l_DecalsLeft;
    }

    void TryClean(Vector3 a_CleanPos)
    {
        for (int i = 0; i < particleData.Length; i++)
        {
            float l_Distance = Vector3.Distance(a_CleanPos, particleData[i].position);

            if (l_Distance < 1.5)
            {
                particleData[i].clean = true;
            }
        }
        DisplayParticles();
    }
}

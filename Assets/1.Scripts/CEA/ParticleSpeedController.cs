using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpeedController : MonoBehaviour
{
    private ParticleSystem[] particleSystems;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }

    void Update()
    {
        foreach(var particleSystem in particleSystems)
        {
           var main = particleSystem.main;
            main.simulationSpeed = SlowMotion.speed;
        }
    }
}

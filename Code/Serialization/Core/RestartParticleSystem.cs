using UnityEngine;
using System.Collections;

public class RestartParticleSystem : MonoBehaviour
{
    ParticleSystem _particleSystem;
    void OnEnable()
    {
        if (_particleSystem == null)
        {
            _particleSystem = gameObject.GetComponent<ParticleSystem>();
        }
        _particleSystem.Clear(true);
        _particleSystem.Play();
    }
}

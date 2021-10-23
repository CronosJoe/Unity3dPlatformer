using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public ParticleSystem currentParticleSystem;
    // Start is called before the first frame update
    void Start()
    {
        currentParticleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("particle collided");
        other.GetComponent<PlayerController>().ApplyDashReset();
    }
}

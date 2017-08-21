using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class UnscaledTimeParticle : MonoBehaviour
{

    protected ParticleSystem ps;

    protected bool _isPlaying = true;

    public bool isPlaying
    {
        set
        {
            _isPlaying = value;
            if (!value) ps.Clear();
        }
        get
        {
            return _isPlaying;
        }
    }


    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    
    void Update()
    {
        if (Time.timeScale < 0.01f && _isPlaying)
            ps.Simulate(Time.unscaledDeltaTime, true, false, false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    public Light2D solarLight;
    public float time = 0;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        // intensity = a * sin ( b * time ) + c; a = min/max relative to c; b = stretch along x axis; c = x axis of symmetry 
        // 0.6 * sin ( pi * 0.01 * time ) + 0.3
        // @ time = 0: intensity = 0.3, @ time = 50: intensity = 0.9, @ time = 100: intensity = 0.3, @ time = 150: intensity = -0.3
        solarLight.intensity = 0.6f * Mathf.Sin(Mathf.PI * 0.01f * time) + 0.3f;
        // Day length = 200 secs (3.33min)
        if (time > 200)
            time = 0;
    }
}

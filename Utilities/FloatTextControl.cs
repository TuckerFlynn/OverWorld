using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatTextControl : MonoBehaviour
{
    public Text txt;
    float lifetime;
    float age = 0.0f;
    // Public method to set floating text parameters
    public void Create(string text, float lifetime)
    {
        txt.text = text;
        this.lifetime = lifetime;
    }

    void Update()
    {
        // Increase the age every frame
        age += Time.deltaTime;
        // Slight movement
        transform.position += new Vector3(-0.01f, -0.01f);
        // End of lifetime effect -> shrink font
        float end = lifetime > 1.0f ? 0.5f : lifetime * 0.1f;
        if (age >= lifetime - end)
        {
            transform.localScale *= 0.8f;
        }
        // Destroy text once it has reached lifetime
        if (age >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}

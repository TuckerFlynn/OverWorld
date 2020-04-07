using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public static FloatingText floatingText;

    public GameObject floatingTextPrefab;

    private void Awake()
    {
        if (floatingText == null)
        {
            floatingText = this;
        }
    }

    public void CreateText(string txt, Vector3 position, float lifetime)
    {
        GameObject text = Instantiate(floatingTextPrefab, this.transform);
        text.transform.position = position;
        text.GetComponent<FloatTextControl>().Create(txt, lifetime);
    }
}

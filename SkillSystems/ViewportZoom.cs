using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewportZoom : MonoBehaviour
{
    public GameObject content;
    public Scrollbar vert;
    public Scrollbar hor;

    public void ZoomOut()
    {
        if (content.transform.localScale.x > 0.5f)
            content.transform.localScale -= new Vector3(0.1f, 0.1f);
    }

    public void ZoomIn()
    {
        if (content.transform.localScale.x < 1.5f)
            content.transform.localScale += new Vector3(0.1f, 0.1f);
    }

    public void Reset()
    {
        content.transform.localScale = Vector3.one;
        vert.value = 0.5f;
        hor.value = 0.5f;
    }
}

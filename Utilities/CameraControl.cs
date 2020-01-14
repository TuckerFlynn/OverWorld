using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    GameObject charObject;
    float minX;
    float maxX;
    float minY;
    float maxY;

    private void Start()
    {
        charObject = CharacterManager.characterManager.charObject;
        float vertExtent = Camera.main.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;

        // Calculations assume map is position at the origin
        minX = horzExtent;
        maxX = 64 - horzExtent;
        minY = vertExtent;
        maxY = 64 - vertExtent;
    }

    void LateUpdate()
    {
        var v3 =  new Vector3(charObject.transform.position.x, charObject.transform.position.y, -10.0f);
        v3.x = Mathf.Clamp(v3.x, minX, maxX);
        v3.y = Mathf.Clamp(v3.y, minY, maxY);
        transform.position = v3;
    }
}

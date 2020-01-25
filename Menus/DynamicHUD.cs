using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicHUD : MonoBehaviour
{
    CharacterManager charMngr;
    public Toggle dynamicToggle;
    public GameObject upperRight;

    private void Start()
    {
        charMngr = CharacterManager.characterManager;
    }

    void FixedUpdate()
    {
        if (dynamicToggle.isOn)
        {
            RectTransform minimapRect = upperRight.GetComponent<RectTransform>();

            if (charMngr.charObject.transform.position.x > 50.0f && charMngr.charObject.transform.position.y > 50.0f)
            {
                minimapRect.anchorMin = Vector2.up;
                minimapRect.anchorMax = Vector2.up;
                minimapRect.pivot = Vector2.up;
            }
            else
            {
                minimapRect.anchorMin = Vector2.one;
                minimapRect.anchorMax = Vector2.one;
                minimapRect.pivot = Vector2.one;
            }
        }
    }
}

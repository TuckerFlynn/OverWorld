using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadParameters : MonoBehaviour
{
    public static LoadParameters loadParameters;

    public Character activeChar;

    private void Awake()
    {
        if (loadParameters == null)
        {
            loadParameters = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsUp : MonoBehaviour
{
    CharacterManager charMngr;
    void Start()
    {
        charMngr = CharacterManager.characterManager;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControl player))
        {
            if (Input.GetKey(KeyCode.Return))
            {
                StartCoroutine("DelayedReturnToSurface");
            }
        }
    }

    IEnumerator DelayedReturnToSurface()
    {
        yield return new WaitForEndOfFrame();

        charMngr.ExitDungeon();
        MapManager.mapManager.LoadField(MapManager.mapManager.worldPos);
    }
}

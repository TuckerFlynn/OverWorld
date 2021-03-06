﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsUp : MonoBehaviour
{
    CharacterManager charMngr;
    DungeonMaster dungeonMaster;

    void Start()
    {
        charMngr = CharacterManager.characterManager;
        dungeonMaster = DungeonMaster.dungeonMaster;
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

        dungeonMaster.CurrentDepth--;

        if (dungeonMaster.CurrentDepth == 0)
        {
            charMngr.ExitDungeon();
            MapManager.mapManager.LoadField(MapManager.mapManager.worldPos);
        }
        else
        {
            dungeonMaster.LoadDungeon(false);
        }
    }
}

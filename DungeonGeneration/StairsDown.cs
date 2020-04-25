using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsDown : MonoBehaviour
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
                StartCoroutine("DelayedStairsDown");
            }
        }
    }

    IEnumerator DelayedStairsDown()
    {
        yield return new WaitForEndOfFrame();

        dungeonMaster.CurrentDepth++;
        dungeonMaster.LoadDungeon(true);
    }
}

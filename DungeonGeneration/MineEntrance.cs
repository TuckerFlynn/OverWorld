using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MineEntrance : MonoBehaviour
{
    [Header("Dungeon Parameters")]
    public bool configSet;
    public DungeonConfig config;

    private DungeonMaster dungeonMaster;
    private CharacterManager charMgr;

    public void SelfStart()
    {
        dungeonMaster = DungeonMaster.dungeonMaster;
        charMgr = CharacterManager.characterManager;

        CreateOrLoadConfig();
        configSet = true;
    }

    public void CreateOrLoadConfig()
    {
        //Vector3 pos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 pos = Vector3Int.FloorToInt(transform.position);
        // Check if there is already save for this dungeon by looking for file named worldX_worldY_fieldX_fieldY
        StringBuilder builder = new StringBuilder();
        builder.Append(charMgr.activeChar.worldPos.x).Append("_").Append(charMgr.activeChar.worldPos.y).Append("_");
        builder.Append(pos.x).Append("_").Append(pos.y);
        builder.Append(".config");

        if (File.Exists(Application.persistentDataPath + "/Dungeons/" + builder))
        {
            string JsonIn = File.ReadAllText(Application.persistentDataPath + "/Dungeons/" + builder);
            config = JsonConvert.DeserializeObject<DungeonConfig>(JsonIn);
            Debug.Log("Loaded dungeon config " + builder);
        }
        else
        {
            config = new DungeonConfig(builder.ToString())
            {
                BaseLevel = charMgr.activeChar.level
                // Will add initial value of SkillLevel once the skill exists
            };
            string JsonOut = JsonConvert.SerializeObject(config);
            File.WriteAllText(Application.persistentDataPath + "/Dungeons/" + builder, JsonOut);
            Debug.Log("Saved dungeon config " + builder);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Effectively disables the trigger until the dungeon Config has been set
        if (!configSet)
            SelfStart();
        // If the player moves into the mine entrance, open the mine UI
        if (collision.gameObject.TryGetComponent<PlayerControl>(out PlayerControl playerControl))
        {
            dungeonMaster.EnableMineUI(true, config);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Effectively disables the trigger until the dungeon Config has been set
        //if (!configSet) return;
        // If the player moves into the mine entrance, open the mine UI
        if (collision.gameObject.TryGetComponent<PlayerControl>(out PlayerControl playerControl))
        {
            dungeonMaster.EnableMineUI(false, config);
        }
    }
}
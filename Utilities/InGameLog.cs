using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class InGameLog : MonoBehaviour
{
    public static InGameLog inGameLog;

    public Text log;
    public InputField commandLine;
    private List<string> logList = new List<string>();

    private InventoryManager invenMngr;

    public Tilemap[] maps;

    public GlobalLight globalLight;

    private void Awake()
    {
        if (inGameLog == null)
        {
            DontDestroyOnLoad(gameObject);
            inGameLog = this;
        }
        else if (inGameLog != this)
        {
            Destroy(this.gameObject);
        }

        commandLine.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            commandLine.enabled = true;
            Time.timeScale = 0;
            commandLine.ActivateInputField();
        }
        if (commandLine.enabled && Input.GetKeyDown(KeyCode.Escape))
        {
            commandLine.text = "";
            Time.timeScale = 1;
            commandLine.enabled = false;
        }
        if (commandLine.enabled && Input.GetKeyDown(KeyCode.Return))
        {
            SubmitCommand(commandLine.text);
            commandLine.text = "";
            Time.timeScale = 1;
            commandLine.enabled = false;
        }
    }

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }
    // Adds Debug messages to the Log List, removes msgs when the display is full, and skips repeated msgs in rapid succession
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[").Append(System.DateTime.Now.ToShortTimeString()).Append("] ");
        builder.Append(logString).AppendLine();
        // Don't log if the same message is repeating
        if (logList.Count > 0)
        {
            if (!builder.ToString().Equals(logList[logList.Count - 1]))
                logList.Add(builder.ToString());
        }
        else
        {
            logList.Add(builder.ToString());
        }

        if (logList.Count > 6)
            logList.RemoveAt(0);
        log.text = "";
        for (int i = 0; i < logList.Count; i++)
        {
            log.text += logList[i];
        }
    }
    // Process input commands
    void SubmitCommand (string command)
    {
        Debug.Log(command);
        // Command to add items to inven: 'additem <ID> <Quantity>'
        if (command.StartsWith("additem", System.StringComparison.OrdinalIgnoreCase))
        {
            invenMngr = FindObjectOfType<InventoryManager>();

            string[] s = command.Split(' ');
            if (s.Length < 3 || s.Length > 3)
            {
                Debug.Log("Wrong number of parameters for additem command");
                return;
            }
            if (invenMngr != null)
            {
                for(int i = 0; i < int.Parse(s[2]); i++)
                {
                    invenMngr.AddToInventory(int.Parse(s[1]));
                }
            }
            return;
        }
        // Command to add a prefab to the map: 'addprefab <Name>'
        // The bottom left corner of the prefab set just above the player's position
        if (command.StartsWith("addprefab", System.StringComparison.OrdinalIgnoreCase))
        {
            maps = FindObjectsOfType<Tilemap>();

            string[] s = command.Split(' ');
            if (s.Length < 2 || s.Length > 2)
            {
                Debug.Log("Wrong number of parameters for addprefab command");
                return;
            }
            // Prefab position is set to one tile above the player
            Vector3Int pos = Vector3Int.FloorToInt( CharacterManager.characterManager.charObject.transform.position );

            PrefabLoader loader = new PrefabLoader();
            PrefabData prefab = loader.LoadPrefab(s[1], pos + Vector3Int.up);

            if (prefab == null) return;

            for (int i = 0; i < maps.Length; i++)
            {
                switch (maps[i].gameObject.name)
                {
                    case "Ground":
                        maps[i].SetTiles(prefab.positions, prefab.groundArray);
                        break;
                    case "Roof":
                        maps[i].SetTiles(prefab.positions, prefab.roofArray);
                        break;
                    case "Objects":
                        maps[i].SetTiles(prefab.positions, prefab.objectsArray);
                        break;
                }
            }

            for (int i = 0; i < prefab.areaObjectsArray.Length; i++)
            {
                TiledObject tObj = prefab.areaObjectsArray[i];
                // The area type is used to determine what gameobject needs to be instantiated, and passes the other area properties to that gameobject
                if (tObj.type == "roof")
                {
                    // The TiledObject data is passed to the attached script to handle additonal setup
                    GameObject instance = Instantiate(MapManager.mapManager.AreaObjects[0]);
                    Vector3Int roofPos = pos + Vector3Int.up + new Vector3Int(0, prefab.height - 64, 0);
                    instance.transform.SetParent(MapManager.mapManager.Areas.transform);
                    instance.GetComponent<RoofArea>().CreateRoofArea(tObj, MapManager.mapManager.Roof, roofPos, prefab.height);
                }
            }

            return;
        }
        // Command to set time
        if (command.StartsWith("time", System.StringComparison.OrdinalIgnoreCase))
        {
            globalLight = FindObjectOfType<GlobalLight>();

            string[] s = command.Split(' ');
            if (s.Length < 2 || s.Length > 2)
            {
                Debug.Log("Wrong number of parameters for time command");
                return;
            }

            globalLight.time = float.Parse(s[1]);
            return;
        }
        Debug.Log("Command unknown or improperly formatted");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDataLoader : MonoBehaviour
{
    public static TileDataLoader tileDataLoader;

    public Tilemap Objects;

    private void Awake()
    {
        if (tileDataLoader == null)
            tileDataLoader = this;
        else if (tileDataLoader != this)
            Destroy(this);

        // Check for the dungeons file directory
        if (!Directory.Exists(Application.persistentDataPath + "/Fields/Configs"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Fields/Configs");
        }
    }

    public void SaveTileData (Vector2Int pos)
    {
        List<GameobjectData> dataList = new List<GameobjectData>();

        string path = Application.persistentDataPath + "/Fields/Configs/" + pos.x + "_" + pos.y + ".config";
        // Loop through objects checking for any that have information that needs to persist the next time that field is loaded
        foreach (Transform child in Objects.transform)
        {
            if (child.TryGetComponent<CropTimer>(out CropTimer timer))
            {
                PlantData toAdd = new PlantData
                {
                    X = Mathf.FloorToInt(child.position.x),
                    Y = Mathf.FloorToInt(child.position.y),
                    Age = timer.age
                };

                dataList.Add(toAdd);
            }
            if (child.TryGetComponent<ContainerTile>(out ContainerTile containerTile))
            {
                ContainerData toAdd = new ContainerData
                {
                    X = Mathf.FloorToInt(child.position.x),
                    Y = Mathf.FloorToInt(child.position.y),
                    Items = containerTile.Items
                };

                dataList.Add(toAdd);
            }
        }

        if (dataList.Count > 0)
        {
            TypeNameSerializationBinder binder = new TypeNameSerializationBinder
            {
                KnownTypes = new List<Type> { typeof(GameobjectData), typeof(PlantData), typeof(ContainerData) }
            };

            string jsonOut = JsonConvert.SerializeObject(dataList, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = binder
            });

            File.WriteAllText(path, jsonOut);
        }
        else
        {
            File.Delete(path);
        }
    }

    public void LoadTileData(Vector2Int pos)
    {
        GameobjectData[] dataArray;
        string path = Application.persistentDataPath + "/Fields/Configs/" + pos.x + "_" + pos.y + ".config";

        if (File.Exists(path))
        {
            string JsonIn = File.ReadAllText(path);
            // Allows deserializing into multiple classes based on the $type value in the Json file
            TypeNameSerializationBinder binder = new TypeNameSerializationBinder
            {
                KnownTypes = new List<Type> { typeof(GameobjectData), typeof(PlantData), typeof(ContainerData) }
            };
            // The imnportant bit! Deserialize list of objects into the appropriate classes
            dataArray = JsonConvert.DeserializeObject<GameobjectData[]>(JsonIn, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = binder
            });

            // Assign gameobject data to the corresponding gameobject
            foreach (Transform child in Objects.transform)
            {
                if (child.TryGetComponent<CropTimer>(out CropTimer timer))
                {
                    foreach (GameobjectData objData in dataArray)
                    {
                        if (objData is PlantData plantData)
                        {
                            if (plantData.X == Mathf.FloorToInt(child.position.x) && plantData.Y == Mathf.FloorToInt(child.position.y))
                            {
                                timer.age = plantData.Age;
                            }
                        }

                    }
                }
                else if (child.TryGetComponent<ContainerTile>(out ContainerTile containerTile))
                {
                    foreach (GameobjectData objData in dataArray)
                    {
                        if (objData is ContainerData containerData)
                        {
                            if (containerData.X == Mathf.FloorToInt(child.position.x) && containerData.Y == Mathf.FloorToInt(child.position.y))
                            {
                                containerTile.Items = containerData.Items;
                            }
                        }
                    }
                }
            }
        }
    }
}
[Serializable]
public class GameobjectData
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Type { get; set; }
}
[Serializable]
public class PlantData : GameobjectData
{
    public float Age { get; set; }

    public PlantData()
    {
        Type = "plantData";
    }

    public override string ToString()
    {
        return string.Format("({0},{1}) - Age: {2}", X, Y, Age);
    }
}
[Serializable]
public class ContainerData : GameobjectData
{
    public InvenItem[] Items;

    public ContainerData()
    {
        Type = "containerData";
    }

    public override string ToString()
    {
        return string.Format("({0},{1}) - Inven slots: {2}", X, Y, Items.Length);
    }
}
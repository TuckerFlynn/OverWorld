using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PrefabLoader
{
    /// <summary>
    /// Checks for a text asset named path, returns null if there isn't one.
    /// </summary>
    /// <param name="prefab">Name of the the prefab file</param>
    /// <param name="origin">Position of the bottom left corner of the prefab</param>
    /// <returns></returns>
    public PrefabData LoadPrefab(string prefab, Vector3Int origin)
    {
        // First check for the requested prefeb in the resources folder (aka built-int prefabs)
        if (Resources.Load<TextAsset>("Tilesets/Prefabs/" + prefab) != null)
        {
            Debug.Log("Found prefab in resources");

            string jsonIn = Resources.Load<TextAsset>("Tilesets/Prefabs/" + prefab).text;
            TiledMap tiled = JsonConvert.DeserializeObject<TiledMap>(jsonIn);

            Vector3Int[] positions = new Vector3Int[tiled.height * tiled.width];
            TileBase[] groundArray = new TileBase[tiled.height * tiled.width];
            TileBase[] roofArray = new TileBase[tiled.height * tiled.width];
            TileBase[] objectsArray = new TileBase[tiled.height * tiled.width];

            TiledObject[] areaObjectsArray = new TiledObject[tiled.layers[3].objects.Count];

            // GROUND & ROOF LAYERS
            for (int i = 0; i < tiled.height * tiled.width; i++)
            {
                // Get the tilemap position vector from the index, adjusted so that the bottom left corner of the prefab is the first tile
                int X = i % tiled.width + origin.x;
                int Y = (tiled.height-1) - Mathf.FloorToInt(i / tiled.width) + origin.y;
                positions[i] = new Vector3Int(X, Y, 0);

                // GROUND
                TiledLayer groundLayer = tiled.layers[0];
                groundArray[i] = TiledHelper.ReadTileLayer(tiled, groundLayer, i);

                // ROOF
                TiledLayer roofLayer = tiled.layers[1];
                roofArray[i] = TiledHelper.ReadTileLayer(tiled, roofLayer, i);
            }

            TiledLayer objectLayer = tiled.layers[2];
            for (int i = 0; i < objectLayer.objects.Count; i++)
            {
                ObjectRef obj = TiledHelper.ReadObjectLayer(tiled, objectLayer, i);
                objectsArray[obj.index] = obj.tileBase;
            }

            TiledLayer areaLayer = tiled.layers[3];
            for (int i = 0; i < areaLayer.objects.Count; i++)
            {
                areaObjectsArray[i] = areaLayer.objects[i];
            }

            return new PrefabData { width = tiled.width, height = tiled.height, positions = positions, groundArray = groundArray, roofArray = roofArray, objectsArray = objectsArray, areaObjectsArray = areaObjectsArray };
        }
        // Second check for the requested prefab in the persistant data folder (aka custom prefabs)
        else if (File.Exists(Application.persistentDataPath + "/Prefabs/" + prefab))
        {
            Debug.Log("Found prefab in persistant data, but didn't load it because I don't want to.");
            return null;
        }
        // If nothing is found
        else
        {
            Debug.Log("Unable to find prefab '" + prefab + "'");
            return null;
        }
    }
}

public class PrefabData
{
    public int width;
    public int height;
    public Vector3Int[] positions;
    public TileBase[] groundArray;
    public TileBase[] roofArray;
    public TileBase[] objectsArray;
    public TiledObject[] areaObjectsArray;
}

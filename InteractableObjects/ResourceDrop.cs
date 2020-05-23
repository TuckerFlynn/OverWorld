using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceDrop : MonoBehaviour
{
    GameObject GroundItems;
    [Header("RESOURCE ATTRIBUTES")]
    public float durability = 10.0f;
    /// <summary>
    /// 0 = none, 1 = axe, 2 = pickaxe, 3 = shovel, 4 = melee weapon, 5 = range weapon
    /// </summary>
    public int resourceType = 0;
    public GameObject itemPrefab;
    public int dropID;
    public int dropQuantity;
    public float dropExp;
    [Header("REPLACEMENT TILE")]
    public bool replace;
    public string tileset;
    public int[] index;

    Tilemap tilemap;

    private void Start()
    {
        GroundItems = GameObject.Find("GroundItems");
        tilemap = GetComponentInParent<Tilemap>();
    }

    public void DamageResource(float dmg)
    {
        durability -= dmg;
        if (durability <= 0)
        {
            if (dropID != 0)
            {
                GameObject obj = Instantiate(itemPrefab);

                obj.GetComponent<SpriteRenderer>().sprite = ItemsDatabase.itemsDatabase.GetItem(dropID).Sprite;
                obj.GetComponent<GroundItem>().ID = dropID;
                obj.GetComponent<GroundItem>().Quantity = dropQuantity;
                obj.transform.position = transform.position;
                obj.transform.SetParent(GroundItems.transform);
                // Give small amount of xp for gathering a resource
                CharacterManager.characterManager.AddExperience(dropExp);
            }
            StartCoroutine("DelayedDestroy");
        }
    }
    // Tile must be destroyed via coroutine in the following frame to prevent an error
    IEnumerator DelayedDestroy()
    {
        yield return new WaitForEndOfFrame();

        Destroy(this.gameObject);
        DestroyTile();
    }

    void DestroyTile ()
    {
        Vector3Int pos = Vector3Int.FloorToInt( transform.position );
        if (!replace)
            tilemap.SetTile(pos, null);
        else
        {
            TileBase tile;
            // If multiple replacement tiles are set, one will be choosen randomly from possibilities
            int RandIndex = index[Random.Range(0, index.Length)];
            // Check whether loaded tile should be an ObjTile or EnvrTile
            if (tileset == "PropTiles" || tileset == "PlantTiles")
                tile = TilesetLoader.GetTilesetByString<List<ObjTile>>(tileset)[RandIndex] as TileBase;
            else
                tile = TilesetLoader.GetTilesetByString<List<EnvrTile>>(tileset)[RandIndex] as TileBase;
            
            tilemap.SetTile(pos, tile); // This is effectively DestroyImmediate for this script
        }
    }
}

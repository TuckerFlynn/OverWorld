using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceDrop : MonoBehaviour
{
    public float durability = 10.0f;
    /// <summary>
    /// 0 = none, 1 = axe, 2 = pickaxe, 3 = shovel, 4 = melee weapon, 5 = range weapon
    /// </summary>
    public int resourceType = 0;

    GameObject GroundItems;
    public GameObject itemPrefab;
    public int dropID;
    public int dropQuantity;
    public float dropExp;

    private void Start()
    {
        GroundItems = GameObject.Find("GroundItems");
    }

    public void DamageResource(float dmg)
    {
        durability -= dmg;
        if (durability <= 0)
        {
            GameObject obj = Instantiate(itemPrefab);

            obj.GetComponent<SpriteRenderer>().sprite = ItemsDatabase.itemsDatabase.GetItem(dropID).Sprite;
            obj.GetComponent<GroundItem>().ID = dropID;
            obj.GetComponent<GroundItem>().Quantity = dropQuantity;
            obj.transform.position = transform.position;
            obj.transform.SetParent(GroundItems.transform);
            // Give small amount of xp for gathering a resource
            CharacterManager.characterManager.AddExperience(dropExp);

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
        Tilemap tilemap = GetComponentInParent<Tilemap>();
        Vector3Int pos = Vector3Int.FloorToInt( transform.position );
        tilemap.SetTile(pos, null);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldMinimap : MonoBehaviour
{
    // HUD objects and related values
    [Header("HUD")]
    public GameObject HUD;
    public Image minimapDisplay;
    public Text coordText;
    Texture2D minimapTex;
    public int zoom = 10;

    public void UpdateMinimap(Vector2Int worldPos)
    {
        MapManager mapManager = MapManager.mapManager;

        Vector2Int posLim = new Vector2Int(Mathf.Clamp(worldPos.x, zoom, mapManager.worldSize - zoom), Mathf.Clamp(worldPos.y, zoom, mapManager.worldSize - zoom));

        minimapTex = new Texture2D(2 * zoom + 1, 2 * zoom + 1);
        Color[] minimapPix = new Color[(2 * zoom + 1) * (2 * zoom + 1)];

        minimapDisplay.enabled = false;
        minimapDisplay.material.mainTexture = minimapTex;

        int pixIndex = 0;
        for (int y = -zoom; y <= zoom; y++)
        {
            for (int x = -zoom; x <= zoom; x++)
            {
                Vector2Int pos = new Vector2Int(posLim.x + x, posLim.y + y);
                //int pixIndex = Mathf.FloorToInt(minimapPix.Length/2) + CoordToId(new Vector2Int(x,y), zoom*2+1);

                int i = CoordToId(pos, mapManager.worldSize);
                Color color = Color.black;
                if (i >= 0 && i < mapManager.masterMap.Length)
                {
                    MapField field = mapManager.masterMap[i];
                    color = BiomeTexureColor(field.MainBiome);
                    if (field.Modifier.ContainsKey("River"))
                        color = Color.cyan;
                    if (pos == worldPos)
                        color = Color.red;
                }
                minimapPix[pixIndex] = color;
                pixIndex++;
            }
        }
        minimapTex.SetPixels(minimapPix);
        minimapTex.Apply();
        minimapDisplay.enabled = true;

        coordText.text = worldPos.ToString();
    }

    // Convert a Vector2 map coordinate to the corresponding field index
    int CoordToId(Vector2Int vect, int size)
    {
        return vect.y * size + vect.x;
    }

    Color BiomeTexureColor(string biome)
    {
        Color texColor;
        switch (biome)
        {
            case "arcticWaste":
                texColor = new Color(0.639f, 0.655f, 0.773f);
                break;
            case "tundra":
                texColor = new Color(143f / 255f, 77f / 255f, 87f / 255f);
                break;
            case "desert":
                texColor = new Color(240f / 255f, 181f / 255f, 65f / 255f);
                break;
            case "borealForest":
                texColor = new Color(82f / 255f, 51f / 255f, 63f / 255f);
                break;
            case "temperateForest":
                texColor = new Color(99f / 255f, 171f / 255f, 63f / 255f);
                break;
            case "bog":
                texColor = new Color(58f / 255f, 63f / 255f, 94f / 255f);
                break;
            case "swamp":
                texColor = new Color(43f / 255f, 43f / 255f, 69f / 255f);
                break;
            case "temperateJungle":
                texColor = new Color(47f / 255f, 87f / 255f, 83f / 255f);
                break;
            case "grassland":
                texColor = new Color(200f / 255f, 212f / 255f, 93f / 255f);
                break;
            case "arctic":
                texColor = new Color(254f / 255f, 255f / 255f, 232f / 255f);
                break;
            case "tropicalJungle":
                texColor = new Color(40f / 255f, 53f / 255f, 64f / 255f);
                break;
            default:
                texColor = new Color(0f, 0f, 0f);
                break;
        }
        return texColor;
    }
}

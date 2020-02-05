using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IngameMenu : MonoBehaviour
{
    public GameObject escapeMenu;
    public GameObject[] disableOnQuit;
    public GameObject settingsMenu;
    // HUD objects and related values
    [Header("HUD")]
    public GameObject HUD;
    public Image minimapDisplay;
    public Text coordText;
    Texture2D minimapTex;
    public int zoom = 10;
    // GUI objects and related values
    [Header("GUI")]
    public GameObject inventory;
    public GameObject logger;

    private void Start()
    {
        escapeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        HUD.SetActive(true);
        inventory.SetActive(false);

        logger = GameObject.Find("Logger");
    }

    void Update()
    {
        // Prevents normal key actions while the looger input field is active
        if (logger != null)
        {
            if (logger.GetComponentInChildren<InputField>().enabled)
            {
                return;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventory.activeSelf)
            {
                inventory.SetActive(false);
            }
            else
            {
                escapeMenu.SetActive(!escapeMenu.activeSelf);
                if (System.Math.Abs(Time.timeScale) > float.Epsilon)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
                settingsMenu.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.SetActive(!inventory.activeSelf);
        }
    }

    public void QuitToMain(Camera main)
    {
        foreach(GameObject obj in disableOnQuit)
        {
            obj.SetActive(false);
        }
        Destroy(CharacterManager.characterManager.gameObject);
        Destroy(MapManager.mapManager.gameObject);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void Settings()
    {
        escapeMenu.SetActive(!escapeMenu.activeSelf);
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
    // Settings options
    public void AdjustHudTransparency (Slider slider)
    {
        Image[] images = HUD.GetComponentsInChildren<Image>();
        Text[] texts = HUD.GetComponentsInChildren<Text>();
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, slider.value);
        }
        foreach (Text text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, slider.value);
        }
    }

    public void UpdateMinimap(Vector2Int worldPos)
    {
        MapManager mapManager = MapManager.mapManager;

        Vector2Int posLim = new Vector2Int(Mathf.Clamp(worldPos.x, zoom, mapManager.worldSize - zoom), Mathf.Clamp(worldPos.y, zoom, mapManager.worldSize - zoom));

        minimapTex = new Texture2D(2*zoom+1, 2*zoom+1);
        Color[] minimapPix = new Color[ (2*zoom+1) * (2*zoom+1)];

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

                if (i >= 0 && i < mapManager.masterMap.Length)
                {
                    MapField field = mapManager.masterMap[i];
                    minimapPix[pixIndex] = BiomeTexureColor(field.MainBiome);
                    if (pos == worldPos)
                    {
                        minimapPix[pixIndex] = Color.red;
                    }
                }
                else
                {
                    minimapPix[pixIndex] = Color.black;
                }
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
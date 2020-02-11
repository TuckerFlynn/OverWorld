using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class FieldDemo : MonoBehaviour
{
    public static FieldDemo fieldDemo;

    private MapField field = new MapField(45, 10);
    public string biome;
    public Slider precipSlider;
    private float precip;
    public Slider tempSlider;
    private float temp;
    public float elevation = 0.5f;
    public Text biomeText;

    private Dictionary<int, string> biomeKey = new Dictionary<int, string>();
    
    public Tilemap Ground;
    public Tilemap Objects;
    public FieldGenerator fieldGenerator;

    private void Awake()
    {
        if (fieldDemo == null)
        {
            DontDestroyOnLoad(gameObject);
            fieldDemo = this;
        }
        else if (fieldDemo != this)
        {
            Destroy(this);
        }

        precipSlider.value = PlayerPrefs.GetFloat("demoPrecip", 0.3f);
        tempSlider.value = PlayerPrefs.GetFloat("demoTemp", 0.5f);

        DefineBiomes();
        TilesetLoader.LoadTiles();
    }

    private void Start()
    {
        ReadUI();
        biomeText.text = "current biome: " + biome;
        StepDemo();
    }

    private void Update()
    {
        ReadUI();
        biomeText.text = "current biome: " + biome;
    }

    public void RunDemo()
    {
        StartCoroutine("Demo");
    }

    public void StepDemo()
    {
        field.Precip = precip;
        field.Temp = temp;
        field.Elevation = elevation;
        biome = DetermineBiome(field);
        field.MainBiome = biome;

        fieldGenerator.MainBiomeGen(field);
    }

    public void PauseDemo()
    {
        StopCoroutine("Demo");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("demoPrecip", precip);
        PlayerPrefs.SetFloat("demoTemp", temp);
    }

    void ReadUI()
    {
        precip = precipSlider.value;
        temp = tempSlider.value;
    }

    IEnumerator Demo()
    {
        for ( ;; )
        {
            field.Precip = precip;
            field.Temp = temp;
            field.Elevation = elevation;
            biome = DetermineBiome(field);
            field.MainBiome = biome;

            fieldGenerator.MainBiomeGen(field);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Some functions borrowed from WorldGenerator to generate an accurate MapField
    /// </summary>
    void DefineBiomes()
    {
        biomeKey.Add(0, "arcticWaste");
        biomeKey.Add(1, "arcticWaste");
        biomeKey.Add(2, "tundra");
        biomeKey.Add(3, "tundra");
        biomeKey.Add(4, "grassland");
        biomeKey.Add(5, "grassland");
        biomeKey.Add(6, "desert");
        biomeKey.Add(7, "desert");
        biomeKey.Add(8, "desert");
        biomeKey.Add(9, "desert");
        biomeKey.Add(10, "arcticWaste");
        biomeKey.Add(11, "tundra");
        biomeKey.Add(12, "tundra");
        biomeKey.Add(13, "tundra");
        biomeKey.Add(14, "grassland");
        biomeKey.Add(15, "grassland");
        biomeKey.Add(16, "grassland");
        biomeKey.Add(17, "desert");
        biomeKey.Add(18, "desert");
        biomeKey.Add(19, "desert");
        biomeKey.Add(20, "arcticWaste");
        biomeKey.Add(21, "tundra");
        biomeKey.Add(22, "tundra");
        biomeKey.Add(23, "tundra");
        biomeKey.Add(24, "grassland");
        biomeKey.Add(25, "grassland");
        biomeKey.Add(26, "grassland");
        biomeKey.Add(27, "grassland");
        biomeKey.Add(28, "grassland");
        biomeKey.Add(29, "desert");
        biomeKey.Add(30, "arcticWaste");
        biomeKey.Add(31, "tundra");
        biomeKey.Add(32, "borealForest");
        biomeKey.Add(33, "borealForest");
        biomeKey.Add(34, "grassland");
        biomeKey.Add(35, "grassland");
        biomeKey.Add(36, "grassland");
        biomeKey.Add(37, "grassland");
        biomeKey.Add(38, "grassland");
        biomeKey.Add(39, "grassland");
        biomeKey.Add(40, "arctic");
        biomeKey.Add(41, "borealForest");
        biomeKey.Add(42, "borealForest");
        biomeKey.Add(43, "borealForest");
        biomeKey.Add(44, "borealForest");
        biomeKey.Add(45, "temperateForest");
        biomeKey.Add(46, "temperateForest");
        biomeKey.Add(47, "temperateForest");
        biomeKey.Add(48, "temperateForest");
        biomeKey.Add(49, "temperateForest");
        biomeKey.Add(50, "arctic");
        biomeKey.Add(51, "borealForest");
        biomeKey.Add(52, "borealForest");
        biomeKey.Add(53, "borealForest");
        biomeKey.Add(54, "temperateForest");
        biomeKey.Add(55, "temperateForest");
        biomeKey.Add(56, "temperateForest");
        biomeKey.Add(57, "temperateForest");
        biomeKey.Add(58, "temperateForest");
        biomeKey.Add(59, "temperateForest");
        biomeKey.Add(60, "arctic");
        biomeKey.Add(61, "borealForest");
        biomeKey.Add(62, "borealForest");
        biomeKey.Add(63, "borealForest");
        biomeKey.Add(64, "temperateForest");
        biomeKey.Add(65, "temperateForest");
        biomeKey.Add(66, "temperateForest");
        biomeKey.Add(67, "temperateForest");
        biomeKey.Add(68, "tropicalJungle");
        biomeKey.Add(69, "tropicalJungle");
        biomeKey.Add(70, "arctic");
        biomeKey.Add(71, "arctic");
        biomeKey.Add(72, "bog");
        biomeKey.Add(73, "temperateJungle");
        biomeKey.Add(74, "temperateJungle");
        biomeKey.Add(75, "temperateJungle");
        biomeKey.Add(76, "temperateJungle");
        biomeKey.Add(77, "tropicalJungle");
        biomeKey.Add(78, "tropicalJungle");
        biomeKey.Add(79, "tropicalJungle");
        biomeKey.Add(80, "arctic");
        biomeKey.Add(81, "arctic");
        biomeKey.Add(82, "bog");
        biomeKey.Add(83, "bog");
        biomeKey.Add(84, "temperateJungle");
        biomeKey.Add(85, "temperateJungle");
        biomeKey.Add(86, "temperateJungle");
        biomeKey.Add(87, "tropicalJungle");
        biomeKey.Add(88, "tropicalJungle");
        biomeKey.Add(89, "tropicalJungle");
        biomeKey.Add(90, "arctic");
        biomeKey.Add(91, "arctic");
        biomeKey.Add(92, "bog");
        biomeKey.Add(93, "bog");
        biomeKey.Add(94, "swamp");
        biomeKey.Add(95, "swamp");
        biomeKey.Add(96, "swamp");
        biomeKey.Add(97, "swamp");
        biomeKey.Add(98, "tropicalJungle");
        biomeKey.Add(99, "tropicalJungle");
    }

    // Get the biomeKey ID based on the temperature and precipitation
    string DetermineBiome(MapField map)
    { 
        float p = Mathf.Floor(Mathf.Clamp(map.Precip, 0.0f, 0.99f) * 10.0f) * 0.1f;
        float t = Mathf.Floor(Mathf.Clamp(map.Temp, 0.0f, 0.99f) * 10.0f) * 0.1f;
        int biomeID = Mathf.FloorToInt(100f * p + 10f * t);

        return biomeKey[biomeID];
    }
}

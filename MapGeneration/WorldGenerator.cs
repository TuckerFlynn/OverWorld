using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldGenerator : MonoBehaviour {
    // Seed for use in random gen elements
    [Header("Hash Seed")]
    public InputField seedInput;
    int hashSeed = 902;
    XXHash hash;

    [Header("Basic Settings")]
    // Maps will always be square so only one dimension is needed
    public Slider worldSizeSlider;
    int worldSize = 64;
    // To get two different noise maps the samples will be taken from different places chosen by hash
    float precipOrg;
    float tempOrg;
    // Different scaling for the noise maps for variety in biome size
    public Slider biomeScaleSlider;
    float precipScale = 10.0f;
    float tempScale = 20.0F;

    // Equatorial gradient values
    [Header("Equator Gradient Variables")]
    public Slider poleFracSlider;
    public Slider equatorFracSlider;
    float poleFrac = 0.1F;
    float equatorFrac = 1.9f;

    // Mountain gen values
    [Header("Mountain Gen Variables")]
    public Slider hikerCountSlider;
    public Slider minLengthSlider;
    public Slider maxLengthSlider;
    public Slider slopeConstSlider;
    int hikerCount = 10;
    public int maxElevation = 100;
    int minHikeLength = 20;
    int maxHikeLength = 80;
    float slopeConst = 0.8f;

    // Water gen values
    [Header("Water Gen Variables")]
    public int swimmerCount = 5;

    // Textures to view the worldGen results
    private Texture2D surfaceMainTex;
    private Texture2D heightMainTex;
    private Color[] surfacePix;
    private Color[] heightPix;
    [Header("GameObjects w/ Renderer")]
    public Image biomeImage;
    public Image heightImage;

    private Dictionary<int, string> biomeKey = new Dictionary<int, string>();

    private MapField[] map;

    void Start()
    {
        // Set the biomeKey dictionary
        DefineBiomes();
    }

    public void RunWorldGen() {
        // Get settings input from UI
        GenSetup();
        // Set and/or clear the masterMap array size
        map = new MapField[worldSize * worldSize];
        // Begin world creation
        SurfaceGenMain();
        HeightMapGen();
        ElevLatAdjustment();
        DetermineBiome();
        RunRivers();
        DrawTex();
        Debug.Log("Updated @ " + Time.time);
    }

    // Get all map gen values from the UI
    void GenSetup () {
        // Check if the seed string can be parsed to an int and set as hash seed, else quit the generation
        bool goodSeed = int.TryParse(seedInput.text, out hashSeed);
        if (!goodSeed) {
            Debug.Log("Seed failed to parse as int: " + seedInput.text);
            return;
        } else {
            hash = new XXHash(hashSeed);
        }
        precipOrg = hash.Range(0, 1000, 0);
        tempOrg = hash.Range(1000, 2000, 1);
        precipScale = 10.0f * (0.5f + biomeScaleSlider.value);
        tempScale = 20.0f * (0.5f + biomeScaleSlider.value);

        worldSize = Mathf.RoundToInt(worldSizeSlider.value) * 16;

        poleFrac = poleFracSlider.value;
        equatorFrac = equatorFracSlider.value;

        hikerCount = (int)hikerCountSlider.value;
        minHikeLength = (int)minLengthSlider.value;
        maxHikeLength = (int)maxLengthSlider.value;

        slopeConst = slopeConstSlider.value;
    }

    // Looks ugly and long, but allows for much faster editing of biome conditions and adding new biomes
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

    // Primary function for defining initial global temperatures and precipitations
    void SurfaceGenMain()
    {
        // For each field within in the world...
        float y = 0.0F;
        int index = 0;

        while (y < worldSize)
        {
            float x = 0.0F;
            while (x < worldSize)
            {
                // Create each MapField and set the index
                map[index] = new MapField(index, worldSize);

                float pxCoord = precipOrg + x / precipScale;
                float pyCoord = precipOrg + y / precipScale;
                float pValue = Mathf.PerlinNoise(pxCoord, pyCoord);

                float txCoord = tempOrg + x / tempScale;
                float tyCoord = tempOrg + y / tempScale;
                float tValue = Mathf.PerlinNoise(txCoord, tyCoord);

                map[index].Temp = tValue;
                map[index].Precip = pValue;
            
                index++;
                x++;
            }
            y++;
        }
    }

    // Run Hikers to create mountain ranges
    void HeightMapGen()
    {
        List<Peak> peaks = new List<Peak>();
        // start counting h from hikerCount, so changing the number of hikers while using the same seed will create new mountains
        int h = hikerCount, i = 0;
        while (i < hikerCount)
        {
            // Create the Hiker
            Hiker hiker = new Hiker(h);
            // The starting point of the walker is set with a random (x,y) coordinate, end is found from a random 
            Vector2Int start = new Vector2Int(hash.Range(0, worldSize, h+1), hash.Range(0, worldSize, h+2));
            Vector2Int end = HikerRange(start, hash.Range(minHikeLength, maxHikeLength, h+3), hash.Range(0.0f, 2*Mathf.PI, h+4));
            // Get the list of steps for the Hiker
            List<Vector2Int> steps = hiker.LooseGuide(start, end);
            // Loop through the steps, moving the Hiker each time
            int s = 0;
            Vector2Int pos = start;
            while (s < steps.Count) {
                pos += steps[s];
                // A peak is placed every 12 steps ** this is a placeholder until a better method is written! **
                if (s % 12 == 0) {
                    // ** Also placeholder, elevation is currently set using a quadratic that ranges from 0 to maxElevation ... not nice :'(
                    float a = 2 * maxElevation / (end-start).magnitude;
                    float b = 4 * maxElevation / (end-start).sqrMagnitude;
                    float elevation = maxElevation + a - b * (pos - start).magnitude;
                    map[CoordToId(pos)].Elevation = elevation;
                    // Create and add a peak to the peaks list
                    Peak peak = new Peak(pos) {
                        Elevation = elevation
                    };
                    peaks.Add(peak);
                }
                s++;
            }
            h += 5;
            i++;
        }

        /* After the list of all peaks on the map has been created, loop through them to create heightMap.
         * Keep in mind the peaks list is MUCH shorter than the masterMap array and takes no time to loop.
         * First loop finds the maximum height, which is required to normalize all values.
         * This has little effect at this point but once some noise is added to the elevation it (maybe?) will be necessary
         */
        int p = 0;
        float min = maxElevation * 10.0f;
        float max = 0.0f;
        while (p < peaks.Count) {
            if (peaks[p].Elevation > max) max = peaks[p].Elevation;
            if (peaks[p].Elevation < min) min = peaks[p].Elevation;
            p++;
        }
        // Second loop normalizes all intial elevation values (which are > 1.0)
        p = 0;
        while (p < peaks.Count) {
            // Normalize heights

            //peaks[p].Elevation = (peaks[p].Elevation - min) / (max - min);

            // ** with current eqn for creating elevation, normalizing is nasty so this is temp solution
            peaks[p].Elevation /= max;
            peaks[p].CurrentHeight = peaks[p].Elevation;
            peaks[p].SlopeConst = slopeConst;
            p++;
        }
        // Third loop works the magic
        p = 0;
        float heightCount = 0.9f;
        while (heightCount >= 0 && peaks.Count > 0) {
            // Continuously loop through all peaks until the heightCounter has decreased to 0
            while (p < peaks.Count) {
                // Decrease the height of each peak until it reaches the heightCounter, then reduce the heightCounter and start again
                while (peaks[p].CurrentHeight > heightCount) {
                    peaks[p] = ElevationStep(peaks[p]);
                    // If no changes to the map are made by an elevationStep, the peak can be removed from the list
                    if (peaks[p].AffectedFields == 0 && peaks[p].StepCount > 1) {
                        peaks.RemoveAt(p);
                        p--;
                        break;
                    }
                }
                p++;
            }
            heightCount -= 0.01f;
            p = 0;
        }
    }

    // Get an endpoint of a vector from a random angle and magnitude, clamped to the map boundaries
    Vector2Int HikerRange (Vector2Int start, int length, float angle) {
        int x = Mathf.RoundToInt(Mathf.Clamp(start.x + length * Mathf.Cos(angle), 1.0f, worldSize-1));
        int y = Mathf.RoundToInt(Mathf.Clamp(start.y + length * Mathf.Sin(angle), 1.0f, worldSize-1));
        return new Vector2Int(x, y);
    }

    // Convert a Vector2Int map coordinate to the corresponding field index
    int CoordToId (Vector2Int coord) {
        return Mathf.RoundToInt(coord.y * worldSize + coord.x);
    }

    // Expands areas of elevations around peaks to create a heightmap
    Peak ElevationStep (Peak peak) {
        Peak tempPeak = peak;
        tempPeak.CurrentRadius++;
        // Equation for calculating the change in elevation: e = eMax / radius ^ const w/ const < 1.0
        tempPeak.CurrentHeight = tempPeak.Elevation / Mathf.Pow(tempPeak.CurrentRadius, tempPeak.SlopeConst);
        tempPeak.AffectedFields = 0;

        // Get min/max coordinates of mapFields within the currentRadius surrounding a peak
        int yMin = Mathf.RoundToInt(tempPeak.position.y) - tempPeak.CurrentRadius;
        yMin = yMin < 0 ? 0 : yMin;
        int yMax = Mathf.RoundToInt(tempPeak.position.y) + tempPeak.CurrentRadius;
        yMax = yMax > worldSize ? worldSize : yMax;
        int xMin = Mathf.RoundToInt(tempPeak.position.x) - tempPeak.CurrentRadius;
        xMin = xMin < 0 ? 0 : xMin;
        int xMax = Mathf.RoundToInt(tempPeak.position.x) + tempPeak.CurrentRadius;
        xMax = xMax > worldSize ? worldSize : xMax;

        for (int y = yMin; y < yMax; y++) {
            for (int x = xMin; x < xMax; x++) {
                Vector2Int pos = new Vector2Int(x, y);
                // If the mapField elevation is not zero (aka it has already been set) or if it is outside the circular radius, skip it
                bool boolA = map[CoordToId(pos)].Elevation > 0;
                bool boolB = Mathf.Round((map[CoordToId(pos)].Position - tempPeak.position).magnitude) > tempPeak.CurrentRadius;
                if (boolA || boolB) {
                    continue;
                }
                // Otherwise, set the mapField elevation and increase the number of fields affected this step
                map[CoordToId(pos)].Elevation = tempPeak.CurrentHeight;
                tempPeak.AffectedFields++;
            }
        }
        tempPeak.StepCount++;
        return tempPeak;
    }

    // Adjust temperature and precipitation values based on elevation and latitude (y-value) values must be clamped between 0.0-1.0
    void ElevLatAdjustment () {
        int i = 0;
        while (i < map.Length) {
            // Elevation changes
            map[i].Temp = Mathf.Clamp01(map[i].Temp - map[i].Elevation);

            // Latitude changes
            float tValue = map[i].Temp;
            // First a hard increase/decrease of temp based on latitude
            if (LatitudeCurve(map[i].Position.y) > 1.0f) {
                if (tValue < 0.5f) {
                    tValue += LatitudeCurve(map[i].Position.y) / (equatorFrac * 10.0f);
                }
            } else {
                if (tValue < 0.5f){
                    tValue -= LatitudeCurve(map[i].Position.y) / (equatorFrac * 10.0f);
                }
            }
            // Followed by a softer scaling
            tValue *= LatitudeCurve(map[i].Position.y);

            if (tValue > 1.0f) {
                float pValue = map[i].Precip;
                pValue = Mathf.Clamp01((pValue - (tValue-1.0f)));
                map[i].Precip = pValue;
            }
            tValue = Mathf.Clamp01(tValue);
            map[i].Temp = tValue;

            i++;
        }
    }

    float LatitudeCurve (float y) {
        float a = (4F * (poleFrac - equatorFrac) / (worldSize * worldSize));
        float b = (4F * (equatorFrac - poleFrac) / worldSize);
        float c = poleFrac;
        float t2 = a * y * y + b * y + c;
        return t2;
    }

    // Get the biomeKey ID based on the temperature and precipitation
    void DetermineBiome () {
        int i = 0;
        while (i < map.Length) {
            float precip = map[i].Precip;
            float temp = map[i].Temp;

            float p = Mathf.Floor(Mathf.Clamp(precip, 0.0f, 0.99f) * 10.0f) * 0.1f;
            float t = Mathf.Floor(Mathf.Clamp(temp, 0.0f, 0.99f) * 10.0f) * 0.1f;
            int biomeID = Mathf.FloorToInt(100f * p + 10f * t);
            string biome = biomeKey[biomeID];

            map[i].MainBiome = biome;

            i++;
        }
    }

    // Create river systems
    void RunRivers () {
        int h = hikerCount * maxHikeLength;
        float swimCount = swimmerCount;
        int seedAttempts = 0;
        while (swimCount > 0 && seedAttempts < swimmerCount * 100) {
            // The starting point of the Swimmer is set with a random (x,y) coordinate similar to the Hikers
            Vector2Int start = new Vector2Int(hash.Range(0, worldSize, h + 1), hash.Range(0, worldSize, h + 2));
            // Crude (but still fast enoguh) method to find starting points with a high elevation
            if (map[CoordToId(start)].Elevation > 0.8f) {
                object[] river = { 0.01f };
                map[CoordToId(start)].AddMod("River", river);

                Vector2Int pos = start;
                // Hard limit amount of strokes for loop
                int strokeLim = 0;
                while (GetLowestNeighbour(pos) != Vector2Int.zero && strokeLim < worldSize*worldSize) {
                    pos += GetLowestNeighbour(pos);
                    if (CoordToId(pos) < 0 || CoordToId(pos) > map.Length) {
                        break;
                    }
                    map[CoordToId(pos)].AddMod("River", river);
                    strokeLim++;
                }
                swimCount--;
            }
            seedAttempts++;
            h += 2;
        }
    }

    // Compare the elevation of the current MapField to the elevation of each surrounding field
    // If the lowest field is the current position, Vector2Int.zero will be returned
    Vector2Int GetLowestNeighbour (Vector2Int pos) {
        Vector2Int lowField = Vector2Int.zero;
        float lowElevation = map[CoordToId(pos)].Elevation;
        // All eight directions
        Vector2Int[] neighbours =
        {
            Vector2Int.right, 
            new Vector2Int(1,-1), 
            Vector2Int.down, 
            new Vector2Int(-1, -1), 
            Vector2Int.left, 
            new Vector2Int(-1, 1), 
            Vector2Int.up, 
            new Vector2Int(1,1)
        };
        // Loop through surrounding fields, saving the lowest elevation
        int i = 0;
        while (i < neighbours.Length) {
            // Skip coords outside the map limits
            if (CoordToId(pos + neighbours[i]) < 0 || CoordToId(pos + neighbours[i]) > (map.Length - 1)) {
                i++;
                continue;
            }
            if (map[CoordToId(pos + neighbours[i])].Elevation < lowElevation) {
                lowElevation = map[CoordToId(pos + neighbours[i])].Elevation;
                lowField = neighbours[i];
            }
            i++;
        }
        return lowField;
    }

    void DrawTex () {
        // Disable images
        biomeImage.enabled = false;
        heightImage.enabled = false;
        // Set up the texture and a Color array to hold pixels during processing.
        surfaceMainTex = new Texture2D(worldSize, worldSize);
        heightMainTex = new Texture2D(worldSize, worldSize);

        surfacePix = new Color[worldSize * worldSize];
        heightPix = new Color[worldSize * worldSize];

        biomeImage.material.mainTexture = surfaceMainTex;
        heightImage.material.mainTexture = heightMainTex;

        int i = 0;
        while (i < map.Length) {
            string biome = map[i].MainBiome;
            Color biomeColor = BiomeTexureColor(biome);

            surfacePix[i] = biomeColor;

            if (map[i].Modifier.ContainsKey("River")) {
                surfacePix[i] = Color.cyan; 
            }

            heightPix[i] = new Color(map[i].Elevation, map[i].Elevation, map[i].Elevation);
            // Copy the pixel data to the texture and load it into the GPU.
            surfaceMainTex.SetPixels(surfacePix);
            surfaceMainTex.Apply();

            heightMainTex.SetPixels(heightPix);
            heightMainTex.Apply();

            i++;
        }
        // Re-enable images
        biomeImage.enabled = true;
        heightImage.enabled = true;
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

    public void save ()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/worldMap.dat", FileMode.OpenOrCreate);

        bf.Serialize(file, map);
        file.Close();
        Debug.Log("World map saved to system");
    }

    public void load ()
    {
        if (File.Exists(Application.persistentDataPath + "/worldMap.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.OpenRead(Application.persistentDataPath + "/worldMap.dat");

            map = (MapField[])bf.Deserialize(file);
            file.Close();
            Debug.Log("World map loaded from system");

            DrawTex();
        }
        else
        {
            Debug.Log("Unable to find worldMap.dat");
        }
    }

    public void back ()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
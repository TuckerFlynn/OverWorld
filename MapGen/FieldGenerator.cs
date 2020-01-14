using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldGenerator : MonoBehaviour
{
    public Tilemap Ground;
    public Tilemap Objects;

    public int mapSize = 64;
    Vector3Int[] positions;
    TileBase[] tileArray;
    // Dictionary holding all tiles and associated prefabs
    Dictionary<string, TileBase> AllTiles = new Dictionary<string, TileBase>();

    public void MainBiomeGen(MapField field)
    {
        Ground.ClearAllTiles();
        Objects.ClearAllTiles();

        string biome = field.MainBiome;

        switch (biome)
        {
            case "arcticWaste":
                ArcticWasteGen(field);
                break;
            case "tundra":
                TundraGen(field);
                break;
            case "desert":
                DesertGen(field);
                break;
            case "borealForest":
                BorealForestGen(field);
                break;
            case "temperateForest":
                TemperateForestGen(field);
                break;
            case "bog":
                BogGen(field);
                break;
            case "swamp":
                SwampGen(field);
                break;
            case "temperateJungle":
                TemperateJungleGen(field);
                break;
            case "arctic":
                ArcticGen(field);
                break;
            case "tropicalJungle":
                TropicalJungleGen(field);
                break;
            case "grassland":
            default:
                GrasslandGen(field);
                break;
        }

        SaveField(field.Position);
    }

    public void SaveField (Vector2Int pos)
    {
        BoundsInt bounds = new BoundsInt(0, 0, 0, 64, 64, 1);
        TileBase[] objTiles = Objects.GetTilesBlock(bounds);

        string directory = Application.persistentDataPath + "/Fields";
        string file = pos.x + "_" + pos.y + ".json";
        string json = JsonHelper.ToJson(objTiles, true);
        Directory.CreateDirectory(directory);
        File.WriteAllText(directory + "/" + file, json);
    }

    public void LoadTiles ()
    {
        // GROUND

        // grass
        AllTiles.Add("grass1", Resources.Load<TileBase>("Tilesets/ground/grass1") as TileBase);
        AllTiles.Add("grass2", Resources.Load<TileBase>("Tilesets/ground/grass2") as TileBase);
        AllTiles.Add("grass3", Resources.Load<TileBase>("Tilesets/ground/grass3") as TileBase);   // rocky
        // dirt
        AllTiles.Add("dirt1", Resources.Load<TileBase>("Tilesets/ground/dirt1") as TileBase);
        AllTiles.Add("dirt2", Resources.Load<TileBase>("Tilesets/ground/dirt2") as TileBase);
        // stone
        AllTiles.Add("stone1", Resources.Load<TileBase>("Tilesets/ground/stone1") as TileBase);
        AllTiles.Add("stone2", Resources.Load<TileBase>("Tilesets/ground/stone2") as TileBase);
        AllTiles.Add("stone3", Resources.Load<TileBase>("Tilesets/ground/stone3") as TileBase);
        // sand
        AllTiles.Add("sand1", Resources.Load<TileBase>("Tilesets/ground/sand1") as TileBase);
        AllTiles.Add("sand2", Resources.Load<TileBase>("Tilesets/ground/sand2") as TileBase);
        AllTiles.Add("sand3", Resources.Load<TileBase>("Tilesets/ground/sand3") as TileBase);   // rocky
        // snow
        AllTiles.Add("snow1", Resources.Load<TileBase>("Tilesets/ground/snow1") as TileBase);
        AllTiles.Add("snow2", Resources.Load<TileBase>("Tilesets/ground/snow2") as TileBase);
        AllTiles.Add("snow3", Resources.Load<TileBase>("Tilesets/ground/snow3") as TileBase);   // rocky
        // mud
        AllTiles.Add("mud1", Resources.Load<TileBase>("Tilesets/ground/mud1") as TileBase);
        AllTiles.Add("mud2", Resources.Load<TileBase>("Tilesets/ground/mud2") as TileBase);

        // LIQUIDS
        AllTiles.Add("water1", Resources.Load<TerrainTile>("Tilesets/ground/water1") as TerrainTile);   // grass
        AllTiles.Add("water2", Resources.Load<TerrainTile>("Tilesets/ground/water2") as TerrainTile);   // sand
        AllTiles.Add("water3", Resources.Load<TerrainTile>("Tilesets/ground/water3") as TerrainTile);   // dirt
        AllTiles.Add("water4", Resources.Load<TerrainTile>("Tilesets/ground/water4") as TerrainTile);   // mud,  sister ice1
        AllTiles.Add("water5", Resources.Load<TerrainTile>("Tilesets/ground/water5") as TerrainTile);   // snow, sister ice2

        AllTiles.Add("ice1", Resources.Load<TerrainTile>("Tilesets/ground/ice1") as TerrainTile);       // mud,  sister water4
        AllTiles.Add("ice2", Resources.Load<TerrainTile>("Tilesets/ground/ice2") as TerrainTile);       // snow, sister water5

        // PLANTS

        // tall grass
        AllTiles.Add("tallGrass1", Resources.Load<TileBase>("Tilesets/plants/tallGrass1") as TileBase);
        AllTiles.Add("tallGrass2", Resources.Load<TileBase>("Tilesets/plants/tallGrass2") as TileBase);
        // bushes
        AllTiles.Add("bush1", Resources.Load<TileBase>("Tilesets/plants/bush1") as TileBase);       // temp
        AllTiles.Add("bush2", Resources.Load<TileBase>("Tilesets/plants/bush2") as TileBase);       // heat
        AllTiles.Add("bush3", Resources.Load<TileBase>("Tilesets/plants/bush3") as TileBase);       // trop
        AllTiles.Add("bush4", Resources.Load<TileBase>("Tilesets/plants/bush4") as TileBase);       // fruit red
        AllTiles.Add("bush5", Resources.Load<TileBase>("Tilesets/plants/bush5") as TileBase);       // fruit blue
        AllTiles.Add("bush6", Resources.Load<TileBase>("Tilesets/plants/bush6") as TileBase);       // fruit purple
        // short trees
        AllTiles.Add("tree1", Resources.Load<TileBase>("Tilesets/plants/tree1") as TileBase);       // temp.decid
        AllTiles.Add("tree2", Resources.Load<TileBase>("Tilesets/plants/tree2") as TileBase);       // heat.decid
        AllTiles.Add("tree3", Resources.Load<TileBase>("Tilesets/plants/tree3") as TileBase);       // trop.decid
        AllTiles.Add("tree4", Resources.Load<TileBase>("Tilesets/plants/tree4") as TileBase);       // fruit
        AllTiles.Add("tree5", Resources.Load<TileBase>("Tilesets/plants/tree5") as TileBase);       // dead
        AllTiles.Add("tree6", Resources.Load<TileBase>("Tilesets/plants/tree6") as TileBase);       // temp.confi
        AllTiles.Add("tree7", Resources.Load<TileBase>("Tilesets/plants/tree7") as TileBase);       // heat.confi
        AllTiles.Add("tree8", Resources.Load<TileBase>("Tilesets/plants/tree8") as TileBase);       // trop.confi
        // tall trees; objects spanning more than one grid cell require additional gameobjects
        AllTiles.Add("tallTree1", Resources.Load<TileBase>("Tilesets/plants/tallTree1") as TileBase);   // temp.decid
        AllTiles.Add("tallTree2", Resources.Load<TileBase>("Tilesets/plants/tallTree2") as TileBase);
        AllTiles.Add("tallTree3", Resources.Load<TileBase>("Tilesets/plants/tallTree3") as TileBase);   // trop.decid
        AllTiles.Add("tallTree4", Resources.Load<TileBase>("Tilesets/plants/tallTree4") as TileBase);   // fruit
        AllTiles.Add("tallTree5", Resources.Load<TileBase>("Tilesets/plants/tallTree5") as TileBase);   // dead
        AllTiles.Add("tallTree6", Resources.Load<TileBase>("Tilesets/plants/tallTree6") as TileBase);   // temp.conif
        AllTiles.Add("tallTree7", Resources.Load<TileBase>("Tilesets/plants/tallTree7") as TileBase);
        AllTiles.Add("tallTree8", Resources.Load<TileBase>("Tilesets/plants/tallTree8") as TileBase);   // trop.conif
        // cacti
        AllTiles.Add("cactus1", Resources.Load<TileBase>("Tilesets/plants/cactus1") as TileBase);       // classic
        AllTiles.Add("cactus2", Resources.Load<TileBase>("Tilesets/plants/cactus2") as TileBase);
        AllTiles.Add("cactus3", Resources.Load<TileBase>("Tilesets/plants/cactus3") as TileBase);
        AllTiles.Add("cactus4", Resources.Load<TileBase>("Tilesets/plants/cactus4") as TileBase);       // fruit yellow
        AllTiles.Add("cactus5", Resources.Load<TileBase>("Tilesets/plants/cactus5") as TileBase);       // fruit orange
        // lilypads
        AllTiles.Add("lilypad1", Resources.Load<TileBase>("Tilesets/plants/lilypad1") as TileBase);
        AllTiles.Add("lilypad2", Resources.Load<TileBase>("Tilesets/plants/lilypad2") as TileBase);     // flower
        AllTiles.Add("lilypad3", Resources.Load<TileBase>("Tilesets/plants/lilypad3") as TileBase);
        AllTiles.Add("lilypad4", Resources.Load<TileBase>("Tilesets/plants/lilypad4") as TileBase);

        // PROPS

        // rocks
        AllTiles.Add("rock1", Resources.Load<TileBase>("Tilesets/props/rock1") as TileBase);
        AllTiles.Add("rock2", Resources.Load<TileBase>("Tilesets/props/rock2") as TileBase);
        AllTiles.Add("rock3", Resources.Load<TileBase>("Tilesets/props/rock3") as TileBase);
    }

    void GrasslandGen (MapField field)
    {
        // Add basic ground tiles
        TileBase grass1 = AllTiles["grass1"];
        TileBase grass2 = AllTiles["grass2"];
        TileBase[] mainTiles = { grass1, grass2 };
        MainFill(Ground, mainTiles);
        TileBase dirt1 = AllTiles["dirt1"];
        TileBase dirt2 = AllTiles["dirt2"];
        TileBase[] secondaryTiles = { dirt1, dirt2 };
        SecondaryFill(0.3f, 3, 3, 5, Ground, secondaryTiles, null);
        TileBase grass3 = AllTiles["grass3"];
        TileBase[] secondaryTiles2 = { grass3 };
        SecondaryFill(0.2f, 2, 3, 5, Ground, secondaryTiles2, null);
        // Add less common features like water bodies and stone outcrops
        TileBase stone1 = AllTiles["stone1"];
        TileBase stone2 = AllTiles["stone2"];
        TerrainTile water1 = (TerrainTile)AllTiles["water1"];
        if (UnityEngine.Random.value < 0.1f)
        {
            int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int sizeRand = UnityEngine.Random.value > 0.95f ? 4 : 2;
            BlobFill(x, y, sizeRand, sizeRand*20, 3, 0.6f, Ground, new TileBase[] { stone1, stone2 });
            // Add rocks
            float rockRand = 0.05f + UnityEngine.Random.value * 0.1f;           // value 0.05 to 0.15
            TileBase rock1 = AllTiles["rock1"];
            TileBase rock2 = AllTiles["rock2"];
            TileBase rock3 = AllTiles["rock3"];
            TileBase[] rocks = { rock1, rock2, rock3 };
            RequireFill(Objects, rocks, rockRand, Ground, new TileBase[] { stone1, stone2 });
        }
        // Water
        if (UnityEngine.Random.value < (field.Precip * 0.5f))
        {
            int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int sizeRand = UnityEngine.Random.value > 0.9f ? 4 : 2;
            BlobFill(x, y, sizeRand, sizeRand*20, 3, 0.7f, Ground, new TileBase[] { water1 });
        }
        // Add in grass and bushes
        float grassRand = 0.2f + UnityEngine.Random.value * 0.3f;               // value 0.20 to 0.50
        TileBase tallGrass1 = AllTiles["tallGrass1"];
        TileBase tallGrass2 = AllTiles["tallGrass2"];
        TileBase bush1 = AllTiles["bush1"];
        TileBase[] plants1 = { tallGrass1, tallGrass2 };
        TileBase[] plants2 = { bush1 };
        NestedFill(grassRand, 5, 2, 5, Objects, plants1, 0.1f, 6, plants2, Ground, dirt1, dirt2, grass3, stone1, stone2, water1 );
        // Add in 2-tile tall trees
        float treeRand = 0.05f + UnityEngine.Random.value * 0.15f;              // value 0.05 to 0.20
        TileBase tallTree1 = AllTiles["tallTree1"];
        TileBase tallTree4 = AllTiles["tallTree4"];
        NestedFill(treeRand, 2, 2, 6, Objects, new TileBase[] { tallTree1 }, 0.1f, 2, new TileBase[] { tallTree4 }, Ground, grass3, stone1, stone2, water1);
    }

    void TemperateForestGen (MapField field)
    {
        // Add basic ground tiles
        TileBase grass1 = AllTiles["grass1"];
        TileBase grass2 = AllTiles["grass2"];
        TileBase[] tiles = { grass1, grass2 };
        MainFill(Ground, tiles);
        float dirtRand = 0.4f + UnityEngine.Random.value * 0.1f;                // value 0.40 to 0.50
        TileBase dirt1 = AllTiles["dirt1"];
        TileBase dirt2 = AllTiles["dirt2"];
        TileBase[] secondaryTiles = { dirt1, dirt2 };
        SecondaryFill(dirtRand, 3, 3, 5, Ground, secondaryTiles, null);
        // Add rare features like water bodies and stone outcrops
        TileBase stone1 = AllTiles["stone1"];
        TileBase stone2 = AllTiles["stone2"];
        TerrainTile water1 = (TerrainTile)AllTiles["water1"];
        // Chance to add stone
        if (UnityEngine.Random.value < 0.2f)
        {
            int x = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
            int y = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
            int sizeRand = UnityEngine.Random.value > 0.95f ? 5 : 3;
            BlobFill(x, y, sizeRand, sizeRand * 20, 2, 0.8f, Ground, new TileBase[] { stone1, stone2 });
            // Add stones
            float rockRand = 0.05f + UnityEngine.Random.value * 0.1f;           // value 0.05 to 0.15
            TileBase rock1 = AllTiles["rock1"];
            TileBase rock2 = AllTiles["rock2"];
            TileBase rock3 = AllTiles["rock3"];
            RequireFill(Objects, new TileBase[] { rock1, rock2, rock3 }, rockRand, Ground, new TileBase[] { stone1, stone2 });
        }
        // Chance to add a pond
        if (UnityEngine.Random.value < (field.Precip * 0.2f))
        {
            int x = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
            int y = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
            int sizeRand = UnityEngine.Random.value > 0.99f ? 6 : 2;
            BlobFill(x, y, sizeRand, sizeRand * 20, 3, 0.7f, Ground, new TileBase[] { water1 });
        }
        // Add 2-tile trees to all dirt tiles, with small chance of them being fruit trees
        TileBase tallTree1 = AllTiles["tallTree1"];
        TileBase tallTree4 = AllTiles["tallTree4"];
        RequireFill(Objects, new TileBase[] { tallTree1 }, 0.9f, Ground, secondaryTiles);
        float fruitRand = UnityEngine.Random.value > 0.95f ? 0.15f : 0.01f ;    // 5% chance of spawning 15% fruit trees, otherwise 1% fruit trees
        RequireFill(Objects, new TileBase[] { tallTree4 }, fruitRand, Objects, new TileBase[] { tallTree1 });
        // Add in grass and bushes
        float grassRand = 0.2f + UnityEngine.Random.value * 0.2f;               // value 0.20 to 0.40
        TileBase tallGrass1 = AllTiles["tallGrass1"];
        TileBase tallGrass2 = AllTiles["tallGrass2"];
        TileBase bush1 = AllTiles["bush1"];
        TileBase tree1 = AllTiles["tree1"];
        TileBase[] plants1 = { tallGrass1, tallGrass2 };
        TileBase[] plants2 = { bush1, tree1 };
        NestedFill(grassRand, 5, 2, 5, Objects, plants1, 0.2f, 4, plants2, Ground, dirt1, dirt2, water1);
    }

    void DesertGen (MapField field)
    {
        // Add main tiles
        TileBase sand1 = AllTiles["sand1"];
        TileBase sand2 = AllTiles["sand2"];
        TileBase[] tiles = { sand1, sand2 };
        MainFill(Ground, tiles);
        // Chance to have patches of dirt with dead trees
        TileBase dirt1 = AllTiles["dirt1"];
        TileBase dirt2 = AllTiles["dirt2"];
        if (UnityEngine.Random.value < 0.3f)
        {
            int x = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
            int y = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
            int sizeRand = UnityEngine.Random.value > 0.8f ? 3 : 2;
            BlobFill(x, y, sizeRand, sizeRand * 20, 3, 0.6f, Ground, new TileBase[] { dirt1, dirt2 });

            TileBase tree5 = AllTiles["tree5"];
            TileBase tallTree5 = AllTiles["tallTree5"];
            RequireFill(Objects, new TileBase[] { tree5, tallTree5 }, 0.05f, Ground, new TileBase[] { dirt1, dirt2 });
        }
        // Very small chance to have an oasis
        TerrainTile water1 = (TerrainTile)AllTiles["water1"];
        TerrainTile water2 = (TerrainTile)AllTiles["water2"];
        if (UnityEngine.Random.value < field.Precip * 0.04f)                    // chance of oasis up to 1.2%, avg closer to 0.6%
        {
            // Most oases are just water surrounded by sand
            if (field.Temp < 0.975f)
            {
                int x = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
                int y = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
                int sizeRand = UnityEngine.Random.value > 0.95f ? 3 : 2;
                BlobFill(x, y, sizeRand, sizeRand * 20, 2, 0.3f, Ground, new TileBase[] { water2 });
            }
            // extremely rare oases that occur within the highest temperature deserts 
            else
            {
                TileBase grass1 = AllTiles["grass1"];
                TileBase grass2 = AllTiles["grass2"];
                TileBase[] grass = { grass1, grass2 };
                int x = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
                int y = 20 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 40));
                int sizeRand = UnityEngine.Random.value > 0.95f ? 3 : 2;
                BlobFill(x, y, sizeRand, sizeRand * 20, 3, 0.5f, Ground, grass);
                BlobFill(x, y, sizeRand, sizeRand * 20, 2, 0.3f, Ground, new TileBase[] { water1 });
                TileBase tallGrass1 = AllTiles["tallGrass1"];
                TileBase tallGrass2 = AllTiles["tallGrass2"];
                TileBase bush2 = AllTiles["bush2"];
                TileBase bush6 = AllTiles["bush6"];
                TileBase[] plants = { tallGrass1, tallGrass2, tallGrass1, tallGrass2, bush2, bush6 };
                RequireFill(Objects, plants, 0.2f, Ground, grass);
            }
        }
        // Add stones
        float rockRand = UnityEngine.Random.value * 0.01f;                      // value 0.0 to 0.01
        TileBase rock1 = AllTiles["rock1"];
        TileBase rock2 = AllTiles["rock2"];
        TileBase rock3 = AllTiles["rock3"];
        SecondaryFill(rockRand, 0, 0, 0, Objects, new TileBase[] { rock1, rock2, rock3 }, Ground, water1, water2);
        // Add in cacti
        float cactiRand = UnityEngine.Random.value * 0.15f;                      // value 0.0 to 0.15
        TileBase cactus1 = AllTiles["cactus1"];
        TileBase cactus2 = AllTiles["cactus2"];
        TileBase cactus3 = AllTiles["cactus3"];
        TileBase cactus4 = AllTiles["cactus4"];
        TileBase cactus5 = AllTiles["cactus5"];
        TileBase[] cacti1 = { cactus1, cactus2, cactus3 };
        TileBase[] cacti2 = { cactus4, cactus5 };
        NestedFill(cactiRand, 3, 2, 5, Objects, cacti1, 0.05f, 1, cacti2, Ground, water1, water2);
    }

    void BorealForestGen (MapField field)
    {

        TileBase dirt1 = AllTiles["dirt1"];
        TileBase dirt2 = AllTiles["dirt2"];
        TileBase[] mainTiles = { dirt1, dirt2 };
        MainFill(Ground, mainTiles);
        // Boreal forest temp is 0.1 to 0.4 (0.5), precip is 0.3 to 0.7
        // If the temp is above 0.3 there will be grass and not snow, and more water than ice
        TileBase tempTile1;
        TileBase tempTile2;
        TerrainTile water = (TerrainTile)AllTiles["water4"];
        TerrainTile ice = (TerrainTile)AllTiles["ice1"];
        if (field.Temp > 0.3)
        {
            // Grass
            float grassRand = 0.45f + UnityEngine.Random.value * 0.1f;          // value 0.40 to 0.55
            tempTile1 = AllTiles["grass1"];
            tempTile2 = AllTiles["grass2"];
            TileBase[] secondaryTiles = { tempTile1, tempTile2 };
            SecondaryFill(grassRand, 5, 4, 5, Ground, secondaryTiles, null);
            // Water
            if (UnityEngine.Random.value < (field.Precip * field.Precip))
            {
                water = (TerrainTile)AllTiles["water3"];
                int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                int sizeRand = UnityEngine.Random.value > 0.99f ? 4 : 2;
                BlobFill(x, y, sizeRand, sizeRand * 20, 3, 0.7f, Ground, new TileBase[] { water });
            }
        }
        else
        {
            // Snow; coverage depends on both precip and temp
            float snowBase = 0.35f + (field.Precip * 0.5f);                     // value 0.5 to 0.7
            float snowRand = snowBase + (UnityEngine.Random.value * 0.1f) - (field.Temp * 0.1f);    // value 0.35 to 0.77?
            snowRand = Mathf.Clamp(snowRand, 0.2f, 0.7f);
            tempTile1 = AllTiles["snow1"];
            tempTile2 = AllTiles["snow2"];
            TileBase[] secondaryTiles = { tempTile1, tempTile2 };
            SecondaryFill(snowRand, 5, 4, 5, Ground, secondaryTiles, null);
            // Water
            if (UnityEngine.Random.value < (field.Precip * field.Precip))
            {
                int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                int sizeRand = UnityEngine.Random.value > 0.99f ? 4 : 2;
                BlobFill(x, y, sizeRand, sizeRand * 20, 3, 0.7f, Ground, new TileBase[] { water, ice });
            }
        }
        // Add small stone patches
        TileBase stone1 = AllTiles["stone1"];
        TileBase stone2 = AllTiles["stone2"];
        TileBase[] secondaryTiles2 = { stone1, stone2 };
        SecondaryFill(0.2f, 2, 3, 5, Ground, secondaryTiles2, null);
        // Add tall grass; less grass if temp is below 0.25
        float grassFill = field.Temp > 0.25 ? 0.25f: 0.1f;
        TileBase tallGrass1 = AllTiles["tallGrass1"];
        TileBase tallGrass2 = AllTiles["tallGrass2"];
        TileBase bush1 = AllTiles["bush1"];
        TileBase[] plants1 = { tallGrass1, tallGrass2, bush1 };
        RequireFill(Objects, plants1, grassFill, Ground, new TileBase[] { tempTile1, tempTile2 });
        // Add trees with a 5% chance that 5% of trees are fruit trees
        float treeRand = 0.6f + UnityEngine.Random.value * 0.15f;               // value 0.60 to 0.75
        float fruitRand = UnityEngine.Random.value > 0.95f ? 0.03f : 0.0f;
        TileBase tallTree6 = AllTiles["tallTree6"];
        TileBase tree4 = AllTiles["tree4"];
        NestedFill(treeRand, 5, 4, 5, Objects, new TileBase[] { tallTree6 }, fruitRand, 7, new TileBase[] { tree4 }, Ground, tempTile1, tempTile2, water, ice, stone1, stone2);
        // Add rocks
        TileBase rock1 = AllTiles["rock1"];
        TileBase rock2 = AllTiles["rock2"];
        TileBase rock3 = AllTiles["rock3"];
        TileBase[] rocks = { rock1, rock2, rock3 };
        SecondaryFill(0.005f, 0, 0, 0, Objects, rocks, Ground, water, ice);
        RequireFill(Objects, rocks, 0.3f, Ground, secondaryTiles2);
    }

    void TundraGen(MapField field)
    {
        TileBase dirt1 = AllTiles["dirt1"];
        TileBase dirt2 = AllTiles["dirt2"];
        TileBase[] mainTiles = { dirt1, dirt2 };
        MainFill(Ground, mainTiles);
        // Add secondary tiles based on field temperature
        TileBase tempTile1;
        TileBase tempTile2;
        TileBase mud1 = AllTiles["mud1"];
        TileBase mud2 = AllTiles["mud2"];
        TileBase[] mud = { mud1, mud2 };
        SecondaryFill(0.5f, 3, 3, 6, Ground, mud, null);
        TerrainTile water = (TerrainTile)AllTiles["water4"];
        TerrainTile ice = (TerrainTile)AllTiles["ice1"];
        // Plants; also temp dependent (fruit bushes will grow on grass but not snow)
        TileBase tallGrass1 = AllTiles["tallGrass1"];
        TileBase tallGrass2 = AllTiles["tallGrass2"];
        TileBase tree6 = AllTiles["tree6"];
        TileBase bush1 = AllTiles["bush1"];
        TileBase bush5 = AllTiles["bush5"];
        if (field.Temp > 0.3)
        {
            // Grass
            float grassRand = 0.45f + (UnityEngine.Random.value * 0.1f);          // value 0.40 to 0.50 x
            tempTile1 = AllTiles["grass1"];
            tempTile2 = AllTiles["grass2"];
            TileBase[] grass = { tempTile1, tempTile2 };
            SecondaryFill(grassRand, 5, 4, 5, Ground, grass, null);
            // Water; possibility of having multiple seperate ponds
            int pondNo = 0;
            while (pondNo < 3)
            {
                pondNo++;
                if (UnityEngine.Random.value < field.Precip)
                {
                    int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                    int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                    int sizeRand = UnityEngine.Random.value > 0.6f ? 3 : 2;
                    BlobFill(x, y, sizeRand, sizeRand * 30, 4, 0.8f, Ground, mud);
                    BlobFill(x, y, sizeRand, sizeRand * 20, 2, 0.7f, Ground, new TileBase[] { water });
                }
            }
            TileBase[] plants1 = { tallGrass1, tallGrass2 };
            TileBase[] plants2 = { tree6, bush1, bush1, bush5 };
            NestedFill(0.6f, 3, 3, 6, Objects, plants1, 0.1f, 4, plants2, Ground, water, ice);
        }
        else
        {
            // Snow; coverage depends on both precip and temp
            float snowBase = 0.4f + (field.Precip * 0.5f);                     // value 0.5 to 0.7 x
            float snowRand = snowBase + (UnityEngine.Random.value * 0.1f) - (field.Temp * 0.1f);    // value 0.35 to 0.77 x
            snowRand = Mathf.Clamp(snowRand, 0.2f, 0.7f);
            tempTile1 = AllTiles["snow1"];
            tempTile2 = AllTiles["snow2"];
            TileBase[] snow = { tempTile1, tempTile2 };
            SecondaryFill(snowRand, 5, 4, 5, Ground, snow, null);
            // Water; possibility of having multiple seperate ponds
            int pondNo = 0;
            while (pondNo < 3)
            {
                pondNo++;
                if (UnityEngine.Random.value < field.Precip)
                {
                    int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                    int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                    int sizeRand = UnityEngine.Random.value > 0.8f ? 3 : 2;
                    BlobFill(x, y, sizeRand, sizeRand * 30, 4, 0.8f, Ground, mud);
                    BlobFill(x, y, sizeRand, sizeRand * 20, 2, 0.7f, Ground, new TileBase[] { water, ice });
                }
            }
            TileBase[] plants1 = { tallGrass1, tallGrass2 };
            TileBase[] plants2 = { tree6, bush1, bush1 };
            NestedFill(0.6f, 3, 3, 6, Objects, plants1, 0.1f, 4, plants2, Ground, water, ice);
            float fruitRand = UnityEngine.Random.value > 0.99f ? 0.01f : 0.001f;
            SecondaryFill(fruitRand, 0, 0, 0, Objects, new TileBase[] { bush5 }, Ground, water, ice, tempTile1, tempTile2);
        }
        // Add rocks
        TileBase rock1 = AllTiles["rock1"];
        TileBase rock2 = AllTiles["rock2"];
        TileBase rock3 = AllTiles["rock3"];
        TileBase[] rocks = { rock1, rock2, rock3 };
        SecondaryFill(0.005f, 0, 0, 0, Objects, rocks, Ground, water, ice);
    }

    void ArcticGen(MapField field)
    {
        TileBase snow1 = AllTiles["snow1"];
        TileBase snow2 = AllTiles["snow2"];
        TileBase[] snow = { snow1, snow2 };
        MainFill(Ground, snow);
        // Stone surrounded by rocky snow
        float stoneRand = 0.45f + (UnityEngine.Random.value * 0.1f);
        TileBase snow3 = AllTiles["snow3"];
        TileBase stone1 = AllTiles["stone1"];
        TileBase stone2 = AllTiles["stone2"];
        TileBase[] stone = { stone1, stone2 };
        NestedFill(stoneRand, 4, 4, 5, Ground, new TileBase[] { snow3 }, 0.8f, 7, stone, null);
        // Water
        TileBase water = AllTiles["water5"];
        TileBase ice = AllTiles["ice2"];
        // 80% chance of spawning small ponds, 20% chance of larger
        if (UnityEngine.Random.value < 0.8f)
        {
            int pondNo = 0;
            while (pondNo < 6)
            {
                pondNo++;
                if (UnityEngine.Random.value < field.Precip * 0.8f)
                {
                    int x = Mathf.RoundToInt(UnityEngine.Random.value * mapSize);
                    int y = Mathf.RoundToInt(UnityEngine.Random.value * mapSize);
                    int sizeRand = UnityEngine.Random.value > 0.8f ? 3 : 2;
                    BlobFill(x, y, sizeRand, sizeRand * 30, 2, 0.7f, Ground, new TileBase[] { water, ice, ice, ice });
                }
            }
        }
        else
        {
            int pondNo = 0;
            while (pondNo < 3)
            {
                pondNo++;
                if (UnityEngine.Random.value < field.Precip * 0.8f)
                {
                    int x = Mathf.RoundToInt(UnityEngine.Random.value * mapSize);
                    int y = Mathf.RoundToInt(UnityEngine.Random.value * mapSize);
                    int sizeRand = UnityEngine.Random.value > 0.8f ? 5 : 4;
                    BlobFill(x, y, sizeRand, sizeRand * 40, 2, 0.7f, Ground, new TileBase[] { water, ice, ice, ice });
                }
            }
        }
        // Rocks
        TileBase rock1 = AllTiles["rock1"];
        TileBase rock2 = AllTiles["rock2"];
        TileBase rock3 = AllTiles["rock3"];
        TileBase[] rocks = { rock1, rock2, rock3 };
        RequireFill(Objects, rocks, 0.5f, Ground, stone);
        // Dead trees
        TileBase tallTree5 = AllTiles["tallTree5"];
        SecondaryFill(0.01f, 3, 1, 3, Objects, new TileBase[] { tallTree5 }, Ground, stone1, stone2, snow3, water, ice);
    }

    void ArcticWasteGen(MapField field)
    {
        TileBase snow1 = AllTiles["snow1"];
        TileBase snow2 = AllTiles["snow2"];
        TileBase[] snow = { snow1, snow2 };
        MainFill(Ground, snow);
        float stoneRand = 0.6f + (UnityEngine.Random.value * 0.1f);
        TileBase snow3 = AllTiles["snow3"];
        TileBase stone1 = AllTiles["stone1"];
        TileBase stone2 = AllTiles["stone2"];
        TileBase[] stone = { stone1, stone2 };
        NestedFill(stoneRand, 4, 4, 5, Ground, new TileBase[] { snow3 }, 0.9f, 7, stone, null);
        // Rocks
        TileBase rock1 = AllTiles["rock1"];
        TileBase rock2 = AllTiles["rock2"];
        TileBase rock3 = AllTiles["rock3"];
        TileBase[] rocks = { rock1, rock2, rock3 };
        RequireFill(Objects, rocks, 0.2f, Ground, stone);
        RequireFill(Objects, rocks, 0.02f, Ground, new TileBase[] { snow1, snow2, snow3 });
    }

    void BogGen(MapField field)
    {
        TileBase water = AllTiles["water4"];
        TileBase ice = AllTiles["ice1"];
        if (field.Temp > 0.3f)
        {
            MainFill(Ground, new TileBase[] { water });
        }
        else
        {
            MainFill(Ground, new TileBase[] { ice });
            float waterRand = (field.Temp * 2.0f);
            SecondaryFill(waterRand, 4, 3, 5, Ground, new TileBase[] { water }, null);
        }
        // Islands fo mud and dirt
        float mudRand = (UnityEngine.Random.value * 0.2f) + field.Temp + (1.0f - field.Precip);     // value (0.0 to 0.2) + (0.2 to 0.4) + (0.0 to 0.3)
        float dirtRand = 1.0f - field.Precip;
        TileBase mud1 = AllTiles["mud1"];
        TileBase mud2 = AllTiles["mud2"];
        TileBase dirt1 = AllTiles["dirt1"];
        TileBase dirt2 = AllTiles["dirt2"];
        TileBase[] mud = { mud1, mud2 };
        TileBase[] dirt = { dirt1, dirt2 };
        NestedFill(mudRand, 2, 4, 5, Ground, mud, dirtRand, 7, dirt, null);
        // Plants
        TileBase tallGrass1 = AllTiles["tallGrass1"];
        TileBase tallGrass2 = AllTiles["tallGrass2"];
        SecondaryFill(0.4f, 2, 3, 5, Objects, new TileBase[] { tallGrass1, tallGrass2 }, Ground, ice);
        TileBase tree8 = AllTiles["tree8"];
        TileBase bush1 = AllTiles["bush1"];
        TileBase bush3 = AllTiles["bush3"];
        TileBase[] plants = { tree8, bush1, bush3 };
        RequireFill(Objects, plants, 0.7f, Ground, dirt);
        // Dead trees
        TileBase tallTree5 = AllTiles["tallTree5"];
        SecondaryFill(0.01f, 3, 1, 3, Objects, new TileBase[] { tallTree5 }, Ground, ice, dirt1, dirt2);
    }

    void SwampGen(MapField field)
    {
        TileBase water = AllTiles["water4"];
        MainFill(Ground, new TileBase[] { water });
        float mudRand = (UnityEngine.Random.value * 0.2f) + (field.Temp * 0.5f);     // value (0.0 to 0.2) + (0.2 to 0.4)
        float dirtRand = 1.0f - field.Temp;
        TileBase mud1 = AllTiles["mud1"];
        TileBase mud2 = AllTiles["mud2"];
        TileBase dirt1 = AllTiles["dirt1"];
        TileBase dirt2 = AllTiles["dirt2"];
        TileBase[] mud = { mud1, mud2 };
        TileBase[] dirt = { dirt1, dirt2 };
        NestedFill(mudRand, 2, 4, 5, Ground, mud, dirtRand, 7, dirt, null);
        // Plants
        TileBase tallGrass1 = AllTiles["tallGrass1"];
        TileBase tallGrass2 = AllTiles["tallGrass2"];
        SecondaryFill(0.4f, 2, 3, 5, Objects, new TileBase[] { tallGrass1, tallGrass2 }, null);
        float plantRand = 0.4f + (UnityEngine.Random.value * 0.2f);             // value 0.4 to 0.6
        TileBase tree3 = AllTiles["tree3"];
        TileBase bush1 = AllTiles["bush1"];
        TileBase bush3 = AllTiles["bush3"];
        RequireFill(Objects, new TileBase[] { tree3, bush1, bush3 }, (plantRand * 0.8f), Ground, mud);
        // Trees on dirt
        TileBase tallTree3 = AllTiles["tallTree3"];
        RequireFill(Objects, new TileBase[] { tallTree3 }, (plantRand * 1.2f), Ground, dirt);
        // Lilypads
        TileBase lily1 = AllTiles["lilypad1"];
        TileBase lily2 = AllTiles["lilypad2"];
        TileBase lily3 = AllTiles["lilypad3"];
        TileBase lily4 = AllTiles["lilypad4"];
        TileBase[] lilypads = { lily1, lily2, lily3, lily4 };
        SecondaryFill(0.3f, 3, 3, 5, Objects, lilypads, Ground, mud1, mud2, dirt1, dirt2);
        // Dead trees
        TileBase tallTree5 = AllTiles["tallTree5"];
        SecondaryFill(0.01f, 3, 1, 3, Objects, new TileBase[] { tallTree5 }, Ground, dirt1, dirt2);
    }

    void TemperateJungleGen(MapField field)
    {
        // Add basic ground tiles
        TileBase grass1 = AllTiles["grass1"];
        TileBase grass2 = AllTiles["grass2"];
        TileBase[] grass = { grass1, grass2 };
        MainFill(Ground, grass);
        float dirtRand = 0.5f + UnityEngine.Random.value * 0.1f;                // value 0.50 to 0.60
        TileBase dirt1 = AllTiles["dirt1"];
        TileBase dirt2 = AllTiles["dirt2"];
        TileBase mud1 = AllTiles["mud1"];
        TileBase mud2 = AllTiles["mud2"];
        TileBase[] dirt = { dirt1, dirt2 };
        TileBase[] mud = { mud1, mud2 };
        SecondaryFill(dirtRand, 3, 3, 5, Ground, mud, null);
        SecondaryFill(dirtRand, 3, 3, 5, Ground, dirt, null);
        // Add rare features like water bodies and stone outcrops
        TileBase stone1 = AllTiles["stone1"];
        TileBase stone2 = AllTiles["stone2"];
        TerrainTile water = (TerrainTile)AllTiles["water1"];
        // Chance to add stone
        if (UnityEngine.Random.value < 0.2f)
        {
            int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int sizeRand = UnityEngine.Random.value > 0.95f ? 5 : 3;
            BlobFill(x, y, sizeRand, sizeRand * 20, 2, 0.8f, Ground, new TileBase[] { stone1, stone2 });
        }
        // Chance to add a pond
        if (UnityEngine.Random.value < (field.Precip * 0.5f))
        {
            int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int sizeRand = UnityEngine.Random.value > 0.99f ? 6 : 2;
            BlobFill(x, y, sizeRand, sizeRand * 20, 3, 0.7f, Ground, new TileBase[] { water });
        }
        // Add stones
        float rockRand = 0.2f + UnityEngine.Random.value * 0.1f;           // value 0.20 to 0.30
        TileBase rock1 = AllTiles["rock1"];
        TileBase rock2 = AllTiles["rock2"];
        TileBase rock3 = AllTiles["rock3"];
        RequireFill(Objects, new TileBase[] { rock1, rock2, rock3 }, rockRand, Ground, new TileBase[] { stone1, stone2 });
        // Add tall trees
        TileBase tallTree8 = AllTiles["tallTree8"];
        RequireFill(Objects, new TileBase[] { tallTree8 }, 0.8f, Ground, dirt);
        SecondaryFill(0.3f, 3, 3, 4, Objects, new TileBase[] { tallTree8 }, Ground, water, dirt1, dirt2, stone1, stone2);
        // Add in grass, bushes, and smol trees
        float grassRand = 0.3f + UnityEngine.Random.value * 0.2f;               // value 0.30 to 0.50
        TileBase tallGrass1 = AllTiles["tallGrass1"];
        TileBase tallGrass2 = AllTiles["tallGrass2"];
        TileBase bush1 = AllTiles["bush1"];
        TileBase tree1 = AllTiles["tree1"];
        TileBase[] plants1 = { tallGrass1, tallGrass2 };
        TileBase[] plants2 = { bush1, tree1 };
        NestedFill(grassRand, 5, 2, 5, Objects, plants1, 0.5f, 6, plants2, Ground, dirt1, dirt2, water);
    }

    void TropicalJungleGen(MapField field)
    {
        // Add basic ground tiles
        TileBase grass1 = AllTiles["grass1"];
        TileBase grass2 = AllTiles["grass2"];
        TileBase[] grass = { grass1, grass2 };
        MainFill(Ground, grass);
        float dirtRand = 0.5f + UnityEngine.Random.value * 0.1f;                // value 0.50 to 0.60
        TileBase dirt1 = AllTiles["dirt1"];
        TileBase dirt2 = AllTiles["dirt2"];
        TileBase mud1 = AllTiles["mud1"];
        TileBase mud2 = AllTiles["mud2"];
        TileBase[] dirt = { dirt1, dirt2 };
        TileBase[] mud = { mud1, mud2 };
        SecondaryFill(dirtRand, 3, 3, 5, Ground, dirt, null);
        SecondaryFill(dirtRand, 3, 3, 5, Ground, mud, null);
        // Add rare features like water bodies and stone outcrops
        TileBase stone1 = AllTiles["stone1"];
        TileBase stone2 = AllTiles["stone2"];
        TerrainTile water = (TerrainTile)AllTiles["water4"];
        // Chance to add stone
        if (UnityEngine.Random.value < 0.1f)
        {
            int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
            int sizeRand = UnityEngine.Random.value > 0.8f ? 4 : 2;
            BlobFill(x, y, sizeRand, sizeRand * 20, 2, 0.8f, Ground, new TileBase[] { stone1, stone2 });
        }
        // Chance to add ponds
        int pondNum = 0;
        while (pondNum < 4)
        {
            pondNum++;
            if (UnityEngine.Random.value < (field.Precip * 0.7f))
            {
                int x = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                int y = 10 + Mathf.RoundToInt(UnityEngine.Random.value * (mapSize - 20));
                int sizeRand = UnityEngine.Random.value < field.Precip ? 3 : 2;
                BlobFill(x, y, sizeRand, sizeRand * 20, 4, 0.5f, Ground, new TileBase[] { water });
            }
        }
        // Add stones
        float rockRand = 0.3f + UnityEngine.Random.value * 0.1f;           // value 0.30 to 0.40
        TileBase rock1 = AllTiles["rock1"];
        TileBase rock2 = AllTiles["rock2"];
        TileBase rock3 = AllTiles["rock3"];
        RequireFill(Objects, new TileBase[] { rock1, rock2, rock3 }, rockRand, Ground, new TileBase[] { stone1, stone2 });
        // Add tall trees
        TileBase tallTree3 = AllTiles["tallTree3"];
        RequireFill(Objects, new TileBase[] { tallTree3 }, 0.8f, Ground, dirt);
        SecondaryFill(0.5f, 3, 3, 4, Objects, new TileBase[] { tallTree3 }, Ground, water, dirt1, dirt2, stone1, stone2);
        // Add in grass, bushes, and smol trees
        float grassRand = 0.3f + UnityEngine.Random.value * 0.2f;               // value 0.30 to 0.50
        TileBase tallGrass1 = AllTiles["tallGrass1"];
        TileBase tallGrass2 = AllTiles["tallGrass2"];
        TileBase bush1 = AllTiles["bush1"];
        TileBase bush3 = AllTiles["bush3"];
        TileBase tree1 = AllTiles["tree1"];
        TileBase[] plants1 = { tallGrass1, tallGrass2 };
        TileBase[] plants2 = { bush1, tree1 };
        NestedFill(grassRand, 5, 2, 5, Objects, plants1, 0.5f, 6, plants2, Ground, dirt1, dirt2, water);
    }

    // Get a random tile from an array of tiles, if weights are provided the chance of getting each tile is not equal
    TileBase RndFromTiles(TileBase[] tiles)
    {
        int index = Mathf.FloorToInt(UnityEngine.Random.value * tiles.Length);
        if (index >= tiles.Length) index = tiles.Length - 1;
        return tiles[index];
    }

    // Fill the entire tilemap randomly with tiles
    void MainFill (Tilemap map, TileBase[] tiles)
    {
        // Clear old positions and tiles
        positions = new Vector3Int[mapSize * mapSize];
        tileArray = new TileBase[positions.Length];

        for (int index = 0; index < positions.Length; index++)
        {
            positions[index] = new Vector3Int(index % mapSize, index / mapSize, 0);
            tileArray[index] = RndFromTiles(tiles);
        }
        map.SetTiles(positions, tileArray);
    }

    // Functions for adding secondary tiles based on cellular automata which
    // creates more unified sections of tiles instead of just using random placement
    void SecondaryFill(float init, int steps, int deathLim, int birthLim, Tilemap map, TileBase[] tiles, Tilemap exclude, params TileBase[] excludes)
    {
        // Create and initialise nested array for automata
        bool[,] cellMap = new bool[mapSize,mapSize];
        cellMap = InitialiseMap(cellMap, init);
        // Run automata steps
        for (int i = 0; i < steps; i++)
        {
            cellMap = AutomataStep(cellMap, deathLim, birthLim);
        }
        // Use the cellMap to place tiles
        for (int index = 0; index < positions.Length; index++)
        {
            int x = index % mapSize;
            int y = index / mapSize;
            if (cellMap[x,y])
            {
                Vector3Int vect = new Vector3Int(x, y, 0);
                if (exclude != null)
                {
                    // If the ground tile at the current pos is an excluded tile, skip it
                    TileBase under = exclude.GetTile(vect) as TileBase;
                    if (Array.Exists(excludes, element => element == under))
                    {
                        continue;
                    }
                }
                map.SetTile(vect, RndFromTiles(tiles));
            }
        }
    }

    bool[,] InitialiseMap(bool[,] map, float init)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (UnityEngine.Random.value < init)
                {
                    map[x, y] = true;
                }
                else
                {
                    map[x, y] = false; 
                }
            }
        }
        return map;
    }

    bool[,] AutomataStep (bool[,] map, int deathLim, int birthLim)
    {
        bool[,] newMap = map;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                int nbs = CountAliveNeighbours(map, x, y);
                //First, if a cell is alive but has too few neighbours, kill it.
                if (map[x,y])
                {
                    newMap[x,y] = nbs >= deathLim;
                } //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                else
                {
                    newMap[x,y] = nbs > birthLim;
                }
            }
        }
        return newMap;
    }

    int CountAliveNeighbours(bool[,] map, int x, int y)
    {
        int count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int neighbour_x = x + i;
                int neighbour_y = y + j;
                // If we're looking at the middle point
                if (i == 0 && j == 0)
                {
                    //Do nothing, we don't want to add ourselves in!
                }
                //In case the index we're looking at it off the edge of the map
                else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= mapSize || neighbour_y >= mapSize)
                {
                    // nothin
                }
                //Otherwise, a normal check of the neighbour
                else if (map[neighbour_x,neighbour_y])
                {
                    count++;
                }
            }
        }
        return count;
    }

    // Functions for adding clusters of a set of tiles, with a small chance of adding tiles from a second set
    void NestedFill (float init, int steps, int deathLim, int birthLim, Tilemap map, TileBase[] primary, float chance, int nbrs, TileBase[] secondary, Tilemap exclude, params TileBase[] excludes)
    {
        // Create and initialise nested array for automata
        bool[,] cellMap = new bool[mapSize, mapSize];
        cellMap = InitialiseMap(cellMap, init);
        // Run automata steps
        for (int i = 0; i < steps; i++)
        {
            cellMap = AutomataStep(cellMap, deathLim, birthLim);
        }
        // Use the cellMap to place tiles
        for (int index = 0; index < positions.Length; index++)
        {
            int x = index % mapSize;
            int y = index / mapSize;
            if (cellMap[x, y])
            {
                Vector3Int vect = new Vector3Int(x, y, 0);
                // If the ground tile at the current pos is an excluded tile, skip it
                if (exclude != null)
                {
                    TileBase under = exclude.GetTile(vect) as TileBase;
                    if (Array.Exists(excludes, element => element == under))
                    {
                        continue;
                    }
                }
                // Set the primary tiles
                map.SetTile(vect, RndFromTiles(primary));
                // Chance to relpace primary tiles with secondary tiles
                if (CountAliveNeighbours(cellMap, x, y) > nbrs && UnityEngine.Random.value < chance)
                {
                    map.SetTile(vect, RndFromTiles(secondary));
                }
            }
        }
    }

    // Place a tile on a specified tilemap, everywhere that a specified tile is already placed on another tilemap
    void RequireFill(Tilemap map, TileBase[] tiles, float fill, Tilemap mapB, TileBase[] tilesB)
    {
        for (int index = 0; index < positions.Length; index++)
        {
            if (UnityEngine.Random.value > fill) continue;

            int x = index % mapSize;
            int y = index / mapSize;
            Vector3Int vect = new Vector3Int(x, y, 0);

            TileBase under = mapB.GetTile(vect);
            if (Array.Exists(tilesB, element => element == under))
            {
                map.SetTile(vect, RndFromTiles(tiles));
            }
        }
    }

    // Add a single mass of tiles in a roughly circular area
    void BlobFill(int X, int Y, int minSize, int maxSize, int steps, float p, Tilemap map, TileBase[] tiles)
    {
        // Create nested array foor bloob maiking yurr
        bool[,] cellMap = new bool[mapSize, mapSize];
        cellMap = InitialiseBloob(cellMap, X, Y, minSize);

        for (int s = 0; s < steps; s++)
        {
            cellMap = BloobStep(cellMap, X, Y, maxSize, p);
        }
        // Use the cellMap to place tiles
        for (int index = 0; index < positions.Length; index++)
        {
            int x = index % mapSize;
            int y = index / mapSize;
            if (cellMap[x, y])
            {
                Vector3Int vect = new Vector3Int(x, y, 0);
                map.SetTile(vect, RndFromTiles(tiles));
            }
        }
    }

    bool[,] InitialiseBloob (bool[,] map, int X, int Y, int minSize)
    {
        for (int x = X; x < X + minSize; x++)
        {
            for (int y = Y; y < Y + minSize; y++)
            {
                if (x < 0 || y < 0 || x > mapSize - 1 || y > mapSize - 1)
                {
                    // Do nothing if outside map limits
                }
                else
                {
                    map[x, y] = true;
                }
            }
        }
        return map;
    }

    bool[,] BloobStep (bool[,] map, int X, int Y, int maxSize, float p)
    {
        bool[,] newMap = map;
        // Check that maxsize/2 is still an int
        if (maxSize % 2 != 0) maxSize++;

        for (int x = X - maxSize/2; x < X + maxSize/2; x++)
        {
            for (int y = Y - maxSize/2; y < Y + maxSize/2; y++)
            {
                if (x < 0 || y < 0 || x > mapSize - 1 || y > mapSize - 1)
                {
                    // Do nothing if outside map limits
                }
                else
                {
                    int nbrs = CountAliveNeighbours(map, x, y);
                    if (nbrs > 1 && !map[x,y])
                    {
                        newMap[x, y] = UnityEngine.Random.value < p;
                    }
                }
            }
        }
        return newMap;
    }
}
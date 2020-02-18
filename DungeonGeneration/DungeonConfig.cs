using System;
using UnityEngine;

[Serializable]
public class DungeonConfig
{
    public string Path { get; set; }
	public bool SetSeed { get; set; }
	public string SeedString { get; set; }
	public int Seed { get; set; }
	public int BaseLevel { get; set; }
	public int SkillLevel { get; set; }
	public int BuildLevel { get; set; }
	public float ResourceDensity { get; set; }
	public int MaxDepth { get; set; }

	public DungeonConfig(string path)
	{
        Path = path;
		SetSeed = false;
	}

    public void SetResourceDensity()
    {
        float TL = 0.01f * (BaseLevel + SkillLevel + BuildLevel);
        TL = TL < 0.01f ? 0.01f : TL;
        // (0.2+TL) * e ^(-TL * (rand/TL)^2)
        ResourceDensity = Mathf.Clamp01((0.2f + TL) * Mathf.Exp(-1.0f * TL * Mathf.Pow((UnityEngine.Random.value / TL), 2)));
        ResourceDensity = Mathf.RoundToInt(ResourceDensity * 100) / 100.0f;
    }
}

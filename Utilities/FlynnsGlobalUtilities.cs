using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlynnsGlobalUtilities
{
    // Convert an amount of experience to a corresponding level based on a certain scale
    public static int ExperienceToLevel(int scaleType, float experience)
    {
        int level = 0;
        switch (scaleType)
        {
            // Linear scaling, 100 experience per level
            case 1:
                level = Mathf.FloorToInt(experience / 100);
                break;
            // Increasing exponential decay: y=200({1-1.025^{-0.01x}})
            case 2:
                level = Mathf.FloorToInt(200 * (1 - Mathf.Pow(1.025f, -0.01f * experience)));
                break;
            default:
                break;
        }
        return level;
    }
    // Get the amount of experience above the previous level, as a value from 0.0 to 1.0
    public static float ExperienceToLevelProgress(int scaleType, float experience)
    {
        float progress;
        switch (scaleType)
        {
            // Linear scaling, 100 experience per level
            case 1:
                progress = (experience / 100) - ExperienceToLevel(1, experience);
                break;
            // Increasing exponential decay: y=200({1-1.025^{-0.01x}})
            case 2:
                int level = ExperienceToLevel(2, experience);
                float levelFloat = 200 * (1 - Mathf.Pow(1.025f, -0.01f * experience));
                progress = levelFloat - level;
                break;
            default:
                progress = 0;
                break;
        }
        return progress;
    }
    // Get the amount of experience required for a certain level
    public static float LevelToExperience (int scaleType, int level)
    {
        float experience;
        switch (scaleType)
        {
            case 1:
                experience = level * 100;
                break;
            case 2:
                if (level >= 200)
                    experience = float.MaxValue;
                else
                    // x = (100 log(200 - y)) / (3 log(2) + log(5) - log(41)) - (100 (3 log(2) + 2 log(5))) / (3 log(2) + log(5) - log(41)) for y<200
                    experience = ( (100 * Mathf.Log(200 - level)) - (100 * (3 * Mathf.Log(2) + 2 * Mathf.Log(5))) ) / ( 3 * Mathf.Log(2) + Mathf.Log(5) - Mathf.Log(41) );
                break;
            default:
                experience = float.MaxValue;
                break;
        }
        return experience;
    }
}

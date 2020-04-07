using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDBars : MonoBehaviour
{
    [Header("HUD BARS")]
    public RectTransform healthBar;
    public RectTransform staminaBar;
    public RectTransform hungerBar;
    public RectTransform expBar;
    [Header("INVENTORY STATS")]
    public Text levelText;
    public Text healthText;
    public Text staminaText;
    public Text hungerText;
    public Text expText;

    readonly float maxWidth = 43.5f;
    readonly float pad = 7.25f;

    private void Start()
    {
        CharacterManager mngr = CharacterManager.characterManager;
        mngr.OnLevelUp += UpdateLevel;
        mngr.OnExperienceGain += UpdateExperience;
        // Initial update to all displays when the script starts
        UpdateAll();
    }

    void UpdateAll()
    {
        UpdateLevel();
        UpdateHealth();
        UpdateExperience();
    }

    void UpdateLevel()
    {
        Character character = CharacterManager.characterManager.activeChar;
        levelText.text = string.Format("level {0}", character.level);
    }

    void UpdateHealth()
    {
        Character character = CharacterManager.characterManager.activeChar;
        healthBar.offsetMax = new Vector2((character.health/character.maxHealth) * maxWidth + pad, -7.0f);
    }

    void UpdateExperience()
    {
        Character character = CharacterManager.characterManager.activeChar;
        // Adjust the experience HUD display
        expBar.offsetMax = new Vector2((FlynnsGlobalUtilities.ExperienceToLevelProgress(2, character.experience)) * maxWidth + pad, -7.0f);
        // Update text in the inventory stats section
        int roundExp = Mathf.FloorToInt(character.experience);
        int roundNextExp = Mathf.RoundToInt(FlynnsGlobalUtilities.LevelToExperience(2, character.level + 1));

        expText.text = string.Format("experience: {0}/{1}", roundExp.ToString(), roundNextExp.ToString());
    }
}
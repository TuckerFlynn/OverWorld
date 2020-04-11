using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHandler : MonoBehaviour
{
    SkillManager skillMngr;
    CharacterManager charMngr;

    void Start()
    {
        skillMngr = SkillManager.skillManager;
        charMngr = CharacterManager.characterManager;

        // Events!
        skillMngr.OnPassiveSkillChange += ConstitutionPassiveEffect;
        skillMngr.OnPassiveSkillChange += StrengthPassiveEffect;
        // Make sure all values are up-to-date when the game starts
        UpdateAll();
    }

    void UpdateAll()
    {
        ConstitutionPassiveEffect();
        StrengthPassiveEffect();
    }

    void ConstitutionPassiveEffect()
    {
        int lvl = FlynnsGlobalUtilities.ExperienceToLevel(1, skillMngr.activeSkills["Constitution"]);
        // bonus = 0.8 * x ^ 1.1
        float bonus = 0.8f * Mathf.Pow(lvl, 1.1f);
        charMngr.activeChar.maxHealth = 100 + bonus;
    }

    void StrengthPassiveEffect()
    {
        int lvl = FlynnsGlobalUtilities.ExperienceToLevel(1, skillMngr.activeSkills["Strength"]);
        MeleeHandler meleeHandler = charMngr.GetComponentInChildren<MeleeHandler>();
        meleeHandler.powerBonus = 0.1f * lvl;
    }
}

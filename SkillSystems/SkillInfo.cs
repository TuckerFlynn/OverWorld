using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    SkillManager skillMngr;
    CharacterManager charMngr;

    Skill selectedSkill;
    public Text title;
    public Text level;
    public Text description;
    public Button spendPoint;
    public Text spendPointText;

    void Start()
    {
        gameObject.SetActive(false);
        skillMngr = SkillManager.skillManager;
        charMngr = CharacterManager.characterManager;
    }

    public void UpdateInfo (Skill skill)
    {
        selectedSkill = skill;
        title.text = skill.Title;
        description.text = skill.Description;
        if (skillMngr.activeSkills.TryGetValue(skill.Title, out int exp))
            level.text = string.Format("level {0}", FlynnsGlobalUtilities.ExperienceToLevel(1, exp));
        else
            level.text = "level 0";
        if (charMngr.activeChar.skillPoints > 0)
        {
            spendPoint.interactable = true;
            spendPointText.text = "spend skill point";
        }
        else
        {
            spendPoint.interactable = false;
            spendPointText.text = "no skill points";
        }
    }

    public void SpendSkillPoint ()
    {
        if (charMngr.activeChar.skillPoints > 0)
        {
            skillMngr.AddExperience(selectedSkill, 25);
            charMngr.activeChar.skillPoints--;
            skillMngr.UpdateSkillPointCount();
            UpdateInfo(selectedSkill);
        }
    }
}

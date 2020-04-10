using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    public Skill skill;
    public GameObject infoPanel;

    public void DisplaySkillInfo(bool isOn)
    {
        infoPanel.SetActive(isOn);
        infoPanel.GetComponent<SkillInfo>().UpdateInfo(skill);
    }
}

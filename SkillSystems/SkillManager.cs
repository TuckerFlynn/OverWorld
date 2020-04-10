using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SkillManager : MonoBehaviour
{
    public static SkillManager skillManager;
    CharacterManager charManager;

    public Skill[] SkillsDB;
    public Dictionary<string, int> activeSkills = new Dictionary<string, int>();
    // SkillUIElement holds a skill object, plus a vector3 position where that element is placed
    List<SkillUIElement> skillUIElements = new List<SkillUIElement>();

    public ViewportZoom zoom;
    [Header("UI ELEMENTS")]
    public GameObject skillsContent;
    public Text skillPointCount;
    public GameObject connections;
    public ToggleGroup toggleGroup;
    public GameObject infoPanel;

    [Header("PREFABS")]
    public GameObject skillUIPrefab;
    public GameObject UILinePrefab;

    // Events!
    public event Action OnPassiveSkillChange;

    private void Awake()
    {
        if (skillManager == null)
        {
            skillManager = this;
        }
        else if (skillManager != this)
        {
            Destroy(this.gameObject);
        }
        LoadSkills();
    }

    private void Start()
    {
        LoadCharacterSkills();
        UpdateSkillPointCount();
        SkillWindowLayout();
    }

    // Load all skills from skills.json file
    void LoadSkills()
    {
        string JsonIn = Resources.Load<TextAsset>("Json/skills").text;
        // Allows deserializing into multiple classes based on the $type value in the Json file
        TypeNameSerializationBinder binder = new TypeNameSerializationBinder
        {
            KnownTypes = new List<Type> { typeof(BaseSkill), typeof(PassiveSkill), typeof(ActiveSkill) }
        };
        SkillsDB = JsonConvert.DeserializeObject<Skill[]>(JsonIn, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            SerializationBinder = binder
        });
        Debug.Log(string.Format("{0} skills loaded from file", SkillsDB.Length));
    }
    // Get the activeChar from CharacterManager and load the current skill levels;
    // activeSkills is the most up-to-date reference for skill experience during runtime
    void LoadCharacterSkills()
    {
        if (CharacterManager.characterManager == null)
        {
            Debug.Log("SkillManager is unable to load character skills (CharacterManager is null)");
        } else
        {
            charManager = CharacterManager.characterManager;
            activeSkills = charManager.activeChar.Skills;
        }
    }

    public void UpdateSkillPointCount()
    {
        skillPointCount.text = string.Format("skill points: {0}", charManager.activeChar.skillPoints);
    }

    public void AddExperience(Skill skill, int exp)
    {
        // Add the experience
        if (activeSkills.ContainsKey(skill.Title))
        {
            int currentLevel = FlynnsGlobalUtilities.ExperienceToLevel(1, activeSkills[skill.Title]);
            activeSkills[skill.Title] += exp;
            if (currentLevel < FlynnsGlobalUtilities.ExperienceToLevel(1, activeSkills[skill.Title]))
            {
                if ((skill is BaseSkill || skill is PassiveSkill) && OnPassiveSkillChange != null )
                {
                    OnPassiveSkillChange();
                }
            }
        }
        else
        {
            activeSkills.Add(skill.Title, exp);
        }
        // Update the UI display
        foreach (SkillUIElement element in skillUIElements)
        {
            if (skill.ID == element.Skill.ID)
            {
                // Text
                Text level = element.GO.transform.Find("Text").GetComponent<Text>();
                level.text = string.Format("level {0}", FlynnsGlobalUtilities.ExperienceToLevel(1, activeSkills[skill.Title]));
                // Progress bar
                RectTransform rect = element.GO.transform.Find("ProgressBar/ProgressMask/ProgressGraphic").GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(FlynnsGlobalUtilities.ExperienceToLevelProgress(1, activeSkills[skill.Title]) * 30, 0);
                // THIS IS NECESSARY TO FORCE THE DISPLAY TO UPDATE,
                // BUT ONLY IF THE PROGRESS BAR IS AT 0 WHEN THE UI WINDOW IS OPENED ... WHY??
                zoom.Wiggle();
            }
        }
    }

    // ---- UI NONSENSE ----
    void SkillWindowLayout()
    {
        // ---- THIRD VERSION: Skill tree layout ----
        int maxTier = 6;
        float stdSpace = 80.0f;
        int[] tierCounts = new int[maxTier];

        // Get number of skills in each tier
        foreach (Skill skill in SkillsDB)
        {
            if (skill.Tier < tierCounts.Length)
                tierCounts[skill.Tier] += 1;
        }
        // Set radii and find max counts for each tier
        float[] tierRadius = new float[maxTier];
        int[] tierMaxCounts = new int[maxTier];
        tierRadius[0] = 64.0f;
        tierMaxCounts[0] = 6;
        for (int i = 1; i < maxTier; i++)
        {
            tierRadius[i] = tierRadius[i - 1] + stdSpace;
            // UI element is considered as occupying a circular area w/ radius of 64,
            // so the max number of elements arranged in a circle is the circumference over 64
            tierMaxCounts[i] = Mathf.FloorToInt(Mathf.PI * 2 * tierRadius[i] / 64);
            // If the max count is less than the actual count, increase the radius so all elements can fit
            if (tierMaxCounts[i] < tierCounts[i])
            {
                tierRadius[i] += 32.0f;
                tierMaxCounts[i] = Mathf.FloorToInt(Mathf.PI * 2 * tierRadius[i] / 64);
            }
        }
        for (int s = 0; s < maxTier; s++)
        {
            // Get array of possible positions at this tier level
            float thisTierAngle = Mathf.PI * 2 / tierMaxCounts[s];
            Vector3[] thisTierPos = new Vector3[tierMaxCounts[s]];
            for (int p = 0; p < thisTierPos.Length; p++)
            {
                if (s % 2 == 0)
                    thisTierPos[p] = new Vector3(tierRadius[s] * Mathf.Cos(thisTierAngle * p), tierRadius[s] * Mathf.Sin(thisTierAngle * p));
                else
                    thisTierPos[p] = new Vector3(tierRadius[s] * Mathf.Cos(thisTierAngle * p + 0.2f), tierRadius[s] * Mathf.Sin(thisTierAngle * p + 0.2f));
            }
            int P = 0;
            foreach (Skill skill in SkillsDB)
            {
                // Base skills just get the six default positions
                if (s == 0 && skill.Tier == 0)
                {
                    GameObject obj = SetupSkillUIElement(skill, thisTierPos[P], Vector3.zero);
                    skillUIElements.Add(new SkillUIElement(skill, thisTierPos[P], obj));
                    P++;
                    continue;
                }
                // Remaining skill tiers need to find nearest position to root skill
                else if (skill.Tier == s)
                {
                    Vector3 rootPos = GetRootPositionTwo(skill, skillUIElements);
                    int p = GetIndexOfNearestPosition(thisTierPos, rootPos, skillUIElements);
                    GameObject obj = SetupSkillUIElement(skill, thisTierPos[p], rootPos);
                    skillUIElements.Add(new SkillUIElement(skill, thisTierPos[p], obj));
                }
            }

        }
    }

    // ---- UI UTILITIES ----
    GameObject SetupSkillUIElement(Skill skill, Vector3 pos, Vector3 rootPos)
    {
        GameObject element = Instantiate(skillUIPrefab, skillsContent.transform);
        // Save the skill object to the gameobject for easy reference
        if (element.TryGetComponent<SkillUI>(out SkillUI skillUI))
        {
            skillUI.skill = skill;
            skillUI.infoPanel = infoPanel;
        }
        element.transform.localPosition = pos;
        element.name = skill.Title;
        Text[] txts = element.GetComponentsInChildren<Text>();
        txts[0].text = skill.Title.ToLower();
        // Add all skill buttons to one toggle group
        element.GetComponent<Toggle>().group = toggleGroup;
        // Check if the active character has experience in a skill
        if (activeSkills.TryGetValue(skill.Title, out int exp))
        {
            txts[1].text = string.Format("level {0}", FlynnsGlobalUtilities.ExperienceToLevel(1, exp));
            RectTransform rect = element.transform.Find("ProgressBar/ProgressMask/ProgressGraphic").GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(FlynnsGlobalUtilities.ExperienceToLevelProgress(1, activeSkills[skill.Title]) * 30, 0);
        }
        else
        {
            txts[1].text = "level 0";
        }
        // Draw a line connecting the skill ui element to the root skill
        GameObject connection = Instantiate(UILinePrefab, connections.transform);
        connection.name = skill.Title + " UILine";
        UILineRenderer lineRend = connection.GetComponent<UILineRenderer>();
        lineRend.Points[0] = new Vector2(element.transform.localPosition.x, element.transform.localPosition.y);
        lineRend.Points[1] = new Vector2(rootPos.x, rootPos.y);
        return element;
    }

    Vector3 GetRootPositionTwo (Skill skill, List<SkillUIElement> list)
    {
        if (skill is PassiveSkill passiveSkill)
        {
            foreach (SkillUIElement element in list)
            {
                if (element.Skill.ID == passiveSkill.RootID)
                {
                    return element.ElementPosition;
                }
            }
        }
        // If root isn't found, return zero (centre of viewport panel)
        return Vector3.zero;
    }

    int GetIndexOfNearestPosition (Vector3[] positions, Vector3 rootPosition, List<SkillUIElement> list)
    {
        int NearestPosIndex = 0;
        float NearestPosMagnitude = 800.0f;
        for (int i = 0; i < positions.Length; i++)
        {
            if ((positions[i] - rootPosition).magnitude < NearestPosMagnitude && PositionIsEmpty(positions[i], list))
            {
                NearestPosMagnitude = (positions[i] - rootPosition).magnitude;
                NearestPosIndex = i;
            }
        }
        return NearestPosIndex;
    }

    bool PositionIsEmpty (Vector3 position, List<SkillUIElement> list)
    {
        foreach (SkillUIElement element in list)
        {
            if (element.ElementPosition == position)
                return false;
        }
        return true;
    }
}

[Serializable]
public abstract class Skill
{
    public string Title { get; set; }
    public int ID { get; set; }
    public int Tier { get; set; }
    public string Description { get; set; }
    public int[] Children { get; set; }
}
[Serializable]
public class BaseSkill : Skill
{
    public Color color { get; set; }

    public override string ToString()
    {
        return string.Format("{0}: Baseskill with ID:{1}", Title, ID);
    }
}
[Serializable]
public class PassiveSkill : Skill
{
    public int Limit { get; set; }
    public int RootID { get; set; }
    public int RootLvl { get; set; }

    public override string ToString()
    {
        return string.Format("{0}: PassiveSkill with ID:{1}, unlocked by skill {2}", Title, ID, RootID);
    }
}
[Serializable]
public class ActiveSkill : Skill
{
    public int Limit { get; set; }
    public int RootID { get; set; }
    public int RootLvl { get; set; }
}

public class SkillUIElement
{
    public Skill Skill { get; set; }
    public Vector3 ElementPosition { get; set; }
    public GameObject GO { get; set; }

    public SkillUIElement(Skill skill, Vector3 vect, GameObject obj)
    {
        Skill = skill;
        ElementPosition = vect;
        GO = obj;
    }
}
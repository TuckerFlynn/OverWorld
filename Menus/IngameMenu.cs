using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IngameMenu : MonoBehaviour
{
    [Header("IN-GAME MAIN MENU")]
    public GameObject escapeMenu;
    public GameObject[] disableOnQuit;
    public GameObject settingsMenu;
    // HUD objects and related values
    [Header("HUD")]
    public GameObject HUD;
    public Image minimapDisplay;
    public Text coordText;
    Texture2D minimapTex;
    public int zoom = 10;
    // GUI objects and related values
    [Header("UI PANELS")]
    public GameObject UI;
    public GameObject inventory;
    public GameObject crafting;
    public InputField craftSearch;
    public GameObject skills;
    public GameObject mineEntranceUI;
    public GameObject logger;
    public GameObject container;
    [Header("MAIN UI TABS")]
    public GameObject tabs;
    public Toggle invenTab;
    public Toggle craftTab;
    public Toggle skillsTab;

    private void Awake()
    {
        HUD.SetActive(true);
        HideAllUI();
        //if (logger == null)
        //    logger = GameObject.Find("Logger");
    }

    void Update()
    {
        // Prevents normal key actions while the logger input field is active
        if (logger != null)
        {
            if (logger.GetComponentInChildren<InputField>().isFocused)
                return;
        }
        // Prevent key actions when the crafting search bar is focused
        if (craftSearch.isFocused)
            return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventory.activeSelf || crafting.activeSelf || skills.activeSelf || mineEntranceUI.activeSelf || container.activeSelf)
            {
                HideAllUI();
            }
            else
            {
                escapeMenu.SetActive(!escapeMenu.activeSelf);
                if (escapeMenu.activeSelf || settingsMenu.activeSelf)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
                settingsMenu.SetActive(false);
            }
        }
        // If the in-game main menu is open, prevent other key actions
        if (escapeMenu.activeSelf || settingsMenu.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.SetActive(!inventory.activeSelf);
            InvenManager2.invenManager2.RefreshMainInvenUI();
            InvenManager2.invenManager2.RefreshCharacterPreview();
            // Disable other UI windows
            if (crafting.activeSelf || skills.activeSelf || container.activeSelf)
            {
                crafting.SetActive(false);
                skills.SetActive(false);
                if (InvenManager2.invenManager2.activeContainer != null)
                    InvenManager2.invenManager2.activeContainer.CloseContainer();
            }
            if (!inventory.activeSelf && !crafting.activeSelf && !skills.activeSelf)
            {
                tabs.SetActive(false);
            }
            else
            {
                tabs.SetActive(true);
                invenTab.isOn = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            crafting.SetActive(!crafting.activeSelf);
            // Disable other UI windows
            if (inventory.activeSelf || skills.activeSelf || container.activeSelf)
            {
                inventory.SetActive(false);
                skills.SetActive(false);
                if (InvenManager2.invenManager2.activeContainer != null)
                    InvenManager2.invenManager2.activeContainer.CloseContainer();
            }
            if (!inventory.activeSelf && !crafting.activeSelf && !skills.activeSelf)
            {
                tabs.SetActive(false);
            }
            else
            {
                tabs.SetActive(true);
                craftTab.isOn = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            skills.SetActive(!skills.activeSelf);
            // Disable other UI windows
            if (inventory.activeSelf || crafting.activeSelf || container.activeSelf)
            {
                inventory.SetActive(false);
                crafting.SetActive(false);
                if (InvenManager2.invenManager2.activeContainer != null)
                    InvenManager2.invenManager2.activeContainer.CloseContainer();
            }
            if (!inventory.activeSelf && !crafting.activeSelf && !skills.activeSelf)
            {
                tabs.SetActive(false);
            }
            else
            {
                tabs.SetActive(true);
                skillsTab.isOn = true;
            }
        }
    }

    public void InventoryToggle ()
    {
        bool active = invenTab.isOn;
        inventory.SetActive(active);
        crafting.SetActive(false);
        skills.SetActive(false);
    }

    public void CraftingToggle()
    {
        bool active = craftTab.isOn;
        crafting.SetActive(active);
        inventory.SetActive(false);
        skills.SetActive(false);
    }

    public void SkillsToggle()
    {
        bool active = skillsTab.isOn;
        skills.SetActive(active);
        inventory.SetActive(false);
        crafting.SetActive(false);
    }

    // Return to the main menu
    public void QuitToMain()
    {
        Time.timeScale = 1;
        foreach (GameObject obj in disableOnQuit)
        {
            obj.SetActive(false);
        }
        //Destroy(CharacterManager.characterManager.gameObject);
        //Destroy(MapManager.mapManager.gameObject);
        MapManager.mapManager.SaveFieldFile(MapManager.mapManager.worldPos);

        SceneManager.LoadSceneAsync("MainMenu_2");
    }

    public void Settings()
    {
        escapeMenu.SetActive(!escapeMenu.activeSelf);
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
    // Settings options
    public void AdjustHudTransparency (Slider slider)
    {
        Image[] images = HUD.GetComponentsInChildren<Image>();
        Text[] texts = HUD.GetComponentsInChildren<Text>();
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, slider.value);
        }
        foreach (Text text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, slider.value);
        }
    }

    public void HideAllUI()
    {
        escapeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        inventory.SetActive(false);
        crafting.SetActive(false);
        skills.SetActive(false);
        tabs.SetActive(false);
        mineEntranceUI.SetActive(false);
        container.SetActive(false);
        if (InvenManager2.invenManager2.activeContainer != null)
            InvenManager2.invenManager2.activeContainer.CloseContainer();
    }
}
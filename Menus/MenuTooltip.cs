using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuTooltip : MonoBehaviour
{
    public static MenuTooltip menuTooltip;

    public GameObject tooltip;
    [Header("MENU PANELS")]
    public GameObject MainMenu;
    public GameObject LoadGameMenu;
    public GameObject NewWorldMenu;
    public GameObject NewCharacterMenu;
    [Header("RAYCASTERS")]
    public GraphicRaycaster MainRaycaster;
    public GraphicRaycaster LoadRaycaster;
    public GraphicRaycaster WorldRaycaster;
    public GraphicRaycaster CharacterRaycaster;
    private GraphicRaycaster raycaster;

    public EventSystem eventSystem;
    private PointerEventData pointerEventData;

    private void Start()
    {
        eventSystem = EventSystem.current;
        UpdateRaycaster();
    }

    private void Update()
    {
        // Set the tooltip position and text when the mouse is over an image with the InvenTooltip script attached
        tooltip.SetActive(false);
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            bool noMouse = !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2);
            // Display tooltip if an image under the mouse has the tooltip script and no mouse buttons are down
            if (result.gameObject.TryGetComponent<MenuTooltipInfo>(out MenuTooltipInfo tooltipScript) && noMouse)
            {
                tooltip.SetActive(true);
                tooltip.transform.position = result.gameObject.transform.position + tooltipScript.offset;

                StringBuilder builder = new StringBuilder();
                // Ignores blank text fields to allow more flexible tooltips
                if (!string.IsNullOrWhiteSpace(tooltipScript.title))
                    builder.Append("<size=10><color='white'>").Append(tooltipScript.title).Append("</color></size>");
                if (!string.IsNullOrWhiteSpace(tooltipScript.title) && !string.IsNullOrWhiteSpace(tooltipScript.tip))
                    builder.AppendLine();
                if (!string.IsNullOrWhiteSpace(tooltipScript.tip))
                    builder.Append(tooltipScript.tip);
                tooltip.GetComponentInChildren<Text>().text = builder.ToString();
                
            }
        }
    }

    public void UpdateRaycaster ()
    {
        if (MainMenu.activeSelf)
            raycaster = MainRaycaster;
        else if (LoadGameMenu.activeSelf)
            raycaster = LoadRaycaster;
        else if (NewWorldMenu.activeSelf)
            raycaster = WorldRaycaster;
        else if (NewCharacterMenu.activeSelf)
            raycaster = CharacterRaycaster;
    }
}

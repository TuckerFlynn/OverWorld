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
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    private PointerEventData pointerEventData;

    private void Start()
    {
        eventSystem = EventSystem.current;
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
                builder.Append("<size=10><color='white'>").Append(tooltipScript.title).Append("</color></size>").AppendLine();
                builder.Append(tooltipScript.tip);
                tooltip.GetComponentInChildren<Text>().text = builder.ToString();
                
            }
        }
    }
}

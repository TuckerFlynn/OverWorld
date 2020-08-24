using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Inventory;

public class InvenTooltip : MonoBehaviour
{
    public Vector3 offset = new Vector3(0.0f, 10.0f, 0.0f);
    public string panel = "Inventory";
    public int index = 0;

    private void Start()
    {
        if (panel == "Inventory")
        {
            Transform grandParent = transform.parent.parent;
            int childCount = grandParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (this.transform.IsChildOf(grandParent.GetChild(i)))
                {
                    this.index = i;
                }
            }
        }
    }
    // Update the tooltip display
    public void UpdateTooltip(GameObject tooltip, Item item)
    {
        StringBuilder builder = new StringBuilder();

        tooltip.SetActive(true);
        tooltip.transform.position = this.gameObject.transform.position + offset;

        if (string.IsNullOrWhiteSpace(item.Description))
        {
            builder.Append("<size=7><color='white'>");
            builder.Append(item.Title);
            builder.Append("</color></size>");
        }
        else
        {
            builder.Append("<size=7><color='white'>");
            builder.Append(item.Title);
            builder.Append("</color></size>").AppendLine();
            builder.Append(item.Description);

            if (item is Building building)
            {
                builder.AppendLine();
                builder.Append("<size=6><color='green'>");
                builder.Append("Building durability: ").Append(building.Durability);
                builder.Append("</color></size>");
            }

            if (item is Consumable consumable)
            {
                builder.AppendLine();
                builder.Append("<size=6><color='red'>");
                if (consumable.Effects[0].Discrete)
                    builder.Append(consumable.Effects[0].Status).Append(": ").Append(consumable.Effects[0].Effect);
                else
                    builder.Append(consumable.Effects[0].Status).Append(" rate: ").Append(consumable.Effects[0].Effect);
                builder.Append("</color></size>");
            }

            tooltip.GetComponentInChildren<Text>().text = builder.ToString();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Inventory;

public class InvenItem : IComparable<InvenItem>
{
	public Item Item { get; set; }
	public int Quantity { get; set; }

	public InvenItem()
	{
		Item = new Item();
		Quantity = 0;
	}

	public int CompareTo(InvenItem other)
	{
		if (other == null)
			return 1;

		if (string.IsNullOrWhiteSpace(this.Item.Title) && !string.IsNullOrWhiteSpace(other.Item.Title))
		{
			return 1;
		}
		else if (string.IsNullOrWhiteSpace(this.Item.Title) && string.IsNullOrWhiteSpace(other.Item.Title))
		{
			return 0;
		}
		else if (!string.IsNullOrWhiteSpace(this.Item.Title) && string.IsNullOrWhiteSpace(other.Item.Title))
		{
			return -1;
		}

		return string.Compare(this.Item.Title, other.Item.Title, StringComparison.OrdinalIgnoreCase);
	}
}

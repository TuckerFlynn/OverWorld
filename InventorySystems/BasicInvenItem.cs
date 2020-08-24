using System;

/// <summary>
/// Simplified version of InvenItem that only contains ID and Quantity;
/// *note this will have to be changed once unique items are introduced*
/// </summary>
[Serializable]
public class BasicInvenItem
{
    public int ID { get; set; }
    public int Quantity { get; set; }
}

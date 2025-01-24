using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerItemsState
{
    private List<PlatformerItem> _items;
    
    public void AddItem(PlatformerItem item)
    {
        _items.Add(item);
    }

    public List<PlatformerItem> Items => _items;
}

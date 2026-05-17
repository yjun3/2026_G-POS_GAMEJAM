using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [SerializeField] List<ItemData> allItems = new();
    readonly HashSet<ItemData> _broken = new();

    public int TotalItemCount => allItems.Count;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public List<ItemData> GetAvailableItems()
    {
        var result = new List<ItemData>();
        foreach (var item in allItems)
            if (!_broken.Contains(item)) result.Add(item);
        return result;
    }

    public void BreakItem(ItemData item)
    {
        _broken.Add(item);
    }

    public bool IsAvailable(ItemData item) => !_broken.Contains(item);

    public void Reset()
    {
        _broken.Clear();
    }
}

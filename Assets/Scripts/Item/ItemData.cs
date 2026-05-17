using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public bool isBreakable = true;

    [Header("Stat Modifiers")]
    public int hpMod = 0;
    public int luckMod = 0;
    public int immunityMod = 0;
    public int defenseMod = 0;
}

public class EncounterResultData
{
    public CharacterFaceDraggable character;
    public ItemData item;

    public int hpBefore;
    public int hpAfter;

    public bool isInfected;    // 사망 포함 전부 감염 처리
    public bool itemDestroyed;
}

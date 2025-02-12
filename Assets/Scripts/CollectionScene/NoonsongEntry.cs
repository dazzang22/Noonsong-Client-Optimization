using UnityEngine;

[CreateAssetMenu(fileName = "NewNoonsongEntry", menuName = "Noonsong Entry")]
public class NoonsongEntry : ScriptableObject
{
    public string noonsongName;
    public string university;
    public string description;
    public Sprite noonsongSprite;
    public bool isDiscovered;        // 발견 여부
    public GameObject prefab;
    public int requiredNoonsongs;
    public string buildingName;

    [Range(0, 100)]
    public int loveLevel = 0;         // 호감도 (기본값 0)
    public bool isFriend;
    public bool isBestFriend;
}
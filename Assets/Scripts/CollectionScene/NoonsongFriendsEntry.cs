using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNoonsongFriendsEntry", menuName = "Noonsong Friends Entry")]
public class NoonsongFriendsEntry : ScriptableObject
{
    public string noonsongFriendName;
    public string university;
    public string description;
    public Sprite displaySprite;
    public Sprite noonsongSprite;
    public Sprite clickedNoonsongSprite;
    public bool isDiscovered;         // 발견 여부
    public GameObject prefab;

}

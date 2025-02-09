using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewItemEntry", menuName = "Affinity/Item Entry")]
public class ItemEntry : ScriptableObject
{
  public int itemID;                  // 아이템 ID
  public string itemName;                // 아이템 이름
  public string description;             // 아이템 설명
  public int itemPrice;                  // 아이템 가격
  public int itemCount = 0;
  public Sprite itemSprite;              // 아이템 이미지

  [System.Serializable]
  public struct NoonsongPreference
  {
    public List<string> departmentNames;  // 여러 학과 적용 가능
    public PreferenceLevel preference;    // 선호도
  }

  public enum PreferenceLevel
  {
    Love,    // 매우 좋아함 (+10)
    Like,    // 좋아함 (+5)
    Neutral, // 보통 (0)
    Dislike  // 싫어함 (-5)
  }

  public List<NoonsongPreference> preferences; 

  public PreferenceLevel GetPreferenceForDepartment(string department)
  {
    foreach (var pref in preferences)
    {
      if (pref.departmentNames.Contains(department))
      {
        return pref.preference;
      }
    }
    return PreferenceLevel.Neutral; // 기본값: 보통
  }
}


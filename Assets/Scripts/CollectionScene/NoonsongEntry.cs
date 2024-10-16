using UnityEngine;

[CreateAssetMenu(fileName = "NewNoonsongEntry", menuName = "Noonsong Entry")]
public class NoonsongEntry : ScriptableObject
{
  public string noonsongName;      
  public string university;         
  public string description;        
  public Sprite noonsongSprite;     
  public bool isDiscovered;         // 발견 여부
  public GameObject prefab;         
  public int requiredNoonsongs;
  public string buildingName;
}

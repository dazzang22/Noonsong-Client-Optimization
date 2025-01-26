using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System.Text;

public class TuriShopManager : MonoBehaviour
{
    public static TuriShopManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start(){
        DisplayShopItems();
    }

    public void DisplayShopItems()
    {
        if (BackendChart.Instance.shopItems == null || BackendChart.Instance.shopItems.Count == 0)
        {
            Debug.LogError("상점 데이터가 없습니다.");
            return;
        }

        foreach (var item in BackendChart.Instance.shopItems)
        {
            Debug.Log($"아이템 이름: {item.ItemName}, 가격: {item.ItemShopValue}, 설명: {item.ItemInfo}");
        }
    }
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookSlot : MonoBehaviour
{
    [SerializeField] public Image noonsongImage; // 눈송이 이미지
    private NoonsongEntry noonsongData;
    private BookUI bookUI;

    public void Initialize(NoonsongEntry noonsong, BookUI book)
    {
        noonsongData = noonsong;
        bookUI = book;
        noonsongImage.sprite = noonsong.noonsongSprite;

        GetComponent<Button>().onClick.AddListener(OnSlotClicked);
    }

    private void OnSlotClicked()
    {
        bookUI.DisplayNoonsongDetails(noonsongData);
    }
}
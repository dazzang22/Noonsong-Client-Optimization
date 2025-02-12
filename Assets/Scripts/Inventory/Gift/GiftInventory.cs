using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftInventory : MonoBehaviour
{
    public GameObject giftInventoryUI;
    public GameObject giftSlotPrefab;
    public Transform giftSlotContainer;

    private List<ItemEntry> giftItems = new List<ItemEntry>();
    private InventoryManager inventoryManager;
    private EncounterUI encounterUI;
    private ARObjectCatch arObjectCatch;

    private ItemEntry selectedGiftItem;
    public GameObject giftPopup;

    public GameObject collectEffectPrefab;

    public GameObject bestFriendPopup;
    public Button bestFriendConfirmButton;
    public ItemEntry bestFriendRewardItem;

    public void Initialize(InventoryManager inventory, EncounterUI ui, ARObjectCatch arCatch)
    {
        inventoryManager = inventory;
        encounterUI = ui;
        arObjectCatch = arCatch;

        if (bestFriendConfirmButton != null)
        {
            bestFriendConfirmButton.onClick.RemoveAllListeners();
            bestFriendConfirmButton.onClick.AddListener(HandleBestFriendConfirmation);
        }

        SyncWithInventoryManager();
    }

    public void ToggleGiftInventory()
    {
        giftInventoryUI.SetActive(!giftInventoryUI.activeSelf);
        giftPopup.SetActive(false);
    }

    public void ShowGiftPopup(ItemEntry item)
    {
        selectedGiftItem = item;
        encounterUI.ShowGiftPopup(item);
    }

    private IEnumerator DisableEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (collectEffectPrefab != null)
        {
            collectEffectPrefab.SetActive(false);
        }
    }

    public void GiveGift()
    {
        if (selectedGiftItem != null)
        {
            NoonsongEntry currentNoonsong = encounterUI.GetCurrentNoonsongEntry();

            if (currentNoonsong == null)
            {
                Debug.LogError("currentNoonsong이 null입니다! EncounterUI에서 올바른 값이 반환되는지 확인하세요.");
                return;
            }

            if (arObjectCatch == null)
            {
                Debug.LogError("arObjectCatch가 null입니다! GiftInventory.Initialize()에서 올바르게 설정했는지 확인하세요.");
                return;
            }

            if (!currentNoonsong.isDiscovered)
            {
                Debug.Log("눈송이 발견되지 않음. CollectCharacter() 실행");
                if (collectEffectPrefab != null)
                {
                    collectEffectPrefab.SetActive(true);
                }
                arObjectCatch.CollectCharacter();

                StartCoroutine(DisableEffectAfterDelay(5f));
            }

            string university = encounterUI.GetCurrentNoonsongUniversity();
            ItemEntry.PreferenceLevel preference = selectedGiftItem.GetPreferenceForDepartment(university);

            //단과대별 친밀도 증가량 설정
            Dictionary<string, int> affectionValues = new Dictionary<string, int>
            {
                { "LiberalArts", 10},
                { "Science", 4 },
                { "Engineering", 8 },
                { "HumanEcology", 4 },
                { "SocialSciences", 5 },
                { "Law", 1 },
                { "Business", 2 },
                { "Music", 4 },
                { "Pharmacy", 1 },
                { "Art", 5 },
                { "GlobalService", 1 },
                { "GlobalConvergence", 2 },
                { "English", 2 },
                { "Media", 1 }
            };

            int baseAffection = affectionValues.ContainsKey(university) ? affectionValues[university] : 1;
            int affectionChange = baseAffection;
            string giftReaction = "내 생각해서 주는 거야? 고마워.";
            switch (preference)
            {
                case ItemEntry.PreferenceLevel.Love:
                    affectionChange = 5;
                    giftReaction = currentNoonsong.isFriend
                        ? "역시 나를 잘 아는구나? 정말 고마워!"
                        : "와! 나 이거 진짜 좋아하는데, 어떻게 알았어? 정말 고마워~";
                    break;
                case ItemEntry.PreferenceLevel.Like:
                    affectionChange = 3;
                    giftReaction = currentNoonsong.isFriend
                        ? "마음에 든다! 고마워~"
                        : "오, 이거 좋은 걸? 선물해줘서 고마워!";
                    break;
                case ItemEntry.PreferenceLevel.Dislike:
                    affectionChange = 0;
                    giftReaction = currentNoonsong.isFriend
                        ? "고마워~"
                        : "하하, 고마워.";
                    break;
            }

            if (!currentNoonsong.isFriend)
            {
                int newLoveLevel = currentNoonsong.loveLevel + affectionChange;
                if (newLoveLevel > 50)
                {
                    affectionChange = 50 - currentNoonsong.loveLevel;
                }
            }

            int updatedLoveLevel = encounterUI.UpdateNoonsongAffection(affectionChange);

            if (updatedLoveLevel >= 100)
            {
                Debug.Log("베프 팝업");
                ShowBestFriendPopup();
            }

            else if (updatedLoveLevel == 50 && !currentNoonsong.isFriend)
            {
                encounterUI.ShowFriendRequestPopup();
            }

            encounterUI.ShowGiftDialogue(giftReaction);

            selectedGiftItem.itemCount--;
            if (selectedGiftItem.itemCount <= 0)
            {
                giftItems.Remove(selectedGiftItem);
            }

            inventoryManager.UpdateInventory();
            SyncWithInventoryManager();
            encounterUI.GiveGift(selectedGiftItem);
            giftPopup.SetActive(false);
            giftInventoryUI.SetActive(false);
        }
    }

    private void ShowBestFriendPopup()
    {
        if (bestFriendPopup != null)
        {
            bestFriendPopup.SetActive(true);
        }
    }

    private void HandleBestFriendConfirmation()
    {
        if (bestFriendPopup != null)
        {
            bestFriendPopup.SetActive(false);

            // 보상 아이템 지급
            if (bestFriendRewardItem != null)
            {
                bestFriendRewardItem.itemCount++;
                inventoryManager.UpdateInventory();
            }
        }
    }

    public void SyncWithInventoryManager()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager가 연결되지 않았습니다");
            return;
        }

        giftItems = new List<ItemEntry>();
        foreach (var item in inventoryManager.itemEntries)
        {
            if (item.itemCount > 0)
            {
                giftItems.Add(item);
            }
        }
        UpdateGiftInventoryUI();
    }

    private void UpdateGiftInventoryUI()
    {
        foreach (Transform child in giftSlotContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemEntry item in giftItems)
        {
            GameObject slot = Instantiate(giftSlotPrefab, giftSlotContainer);
            GiftSlot slotScript = slot.GetComponent<GiftSlot>();
            slotScript.Setup(item, this);
        }
    }
}
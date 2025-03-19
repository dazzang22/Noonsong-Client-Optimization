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

    public GameObject giftEffectParticle1;
    public GameObject giftEffectParticle2;
    public GameObject giftEffectParticle3;
    public GameObject giftEffectParticle4;

    public GameObject bestFriendPopup;
    public Button bestFriendConfirmButton;
    public ItemEntry bestFriendRewardItem;

    [Header("Audio")]
    public AudioClip gift;
    public AudioClip bestfriend;
    public AudioClip love;
    public AudioClip hate;
    public AudioClip eventUI;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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

    private IEnumerator DisableEffectAfterDelay(float delay, GameObject effect)
    {
        yield return new WaitForSeconds(delay);

        if (effect != null)
        {
            effect.SetActive(false);
        }
    }

        public void GiveGift()
    {
        if (selectedGiftItem != null)
        {
            NoonsongEntry currentNoonsong = encounterUI.GetCurrentNoonsongEntry();

            if (currentNoonsong == null)
            {
                Debug.LogError("NoonsongEntry is null. Cannot proceed with gifting.");
                return;
            }

            if (!currentNoonsong.isDiscovered)
            {
                arObjectCatch.CollectCharacter();
            }

            string university = encounterUI.GetCurrentNoonsongUniversity();

            Dictionary<string, int> affectionValues = new Dictionary<string, int>
            {
                { "LiberalArts", 10},
                { "Science", 4 },
                { "Science(Physical)", 2 },
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

            ItemEntry.PreferenceLevel preference = selectedGiftItem.GetPreferenceForDepartment(university);
            int preferenceMultiplier = 1;
            string giftReaction = "내 생각해서 주는 거야? 고마워.";
            GameObject effectToActivate = null;

            switch (preference)
            {
                case ItemEntry.PreferenceLevel.Love:
                    audioSource.PlayOneShot(love);
                    preferenceMultiplier = 5;
                    giftReaction = currentNoonsong.isFriend
                        ? "역시 나를 잘 아는구나? 정말 고마워!"
                        : "와! 나 이거 진짜 좋아하는데, 어떻게 알았어? 정말 고마워~";
                    effectToActivate = giftEffectParticle4;
                    break;
                case ItemEntry.PreferenceLevel.Like:
                    audioSource.PlayOneShot(love);
                    preferenceMultiplier = 3;
                    giftReaction = currentNoonsong.isFriend
                        ? "마음에 든다! 고마워~"
                        : "오, 이거 좋은 걸? 선물해줘서 고마워!";
                    effectToActivate = giftEffectParticle3;
                    break;
                case ItemEntry.PreferenceLevel.Dislike:
                    audioSource.PlayOneShot(hate);
                    preferenceMultiplier = 0;
                    giftReaction = currentNoonsong.isFriend
                        ? "고마워~"
                        : "하하, 고마워.";
                    effectToActivate = giftEffectParticle1;
                    break;
                default:
                    audioSource.PlayOneShot(love);
                    effectToActivate = giftEffectParticle2;
                    break;
            }

            if (effectToActivate != null)
            {
                effectToActivate.SetActive(true);
                StartCoroutine(DisableEffectAfterDelay(3f, effectToActivate));
            }

            int affectionChange = baseAffection * preferenceMultiplier;
            int friendlevel = DogamChartManager.Instance.friendFavor(currentNoonsong.university);

            if (friendlevel <= 0)
            {
                Debug.LogError($"Invalid friend level for university: {currentNoonsong.university}");
                friendlevel = 0;
            }

            if (!currentNoonsong.isFriend)
            {
                int newLoveLevel = currentNoonsong.loveLevel + affectionChange;
                // 50 -> 대학별 친구가 될 수 있는 호감도 수치로 변경 (DB에서 가져옴)
                if (newLoveLevel > 50)
                {
                    affectionChange = 50 - currentNoonsong.loveLevel;
                }
            }

            int updatedLoveLevel = encounterUI.UpdateNoonsongAffection(affectionChange);
            int bestfriend = DogamChartManager.Instance.maxFavor(currentNoonsong.university);
            // 100 -> 대학별 베프가 될 수 있는 호감도 수치로 변경 (DB에서 가져옴)

            if (updatedLoveLevel >= 100 && !currentNoonsong.isBestFriend)
            {
                audioSource.PlayOneShot(eventUI);
                ShowBestFriendPopup();
                currentNoonsong.isBestFriend = true;
            }

            if (updatedLoveLevel == friendlevel && !currentNoonsong.isFriend)
            {
                encounterUI.ShowFriendRequestPopup();
                audioSource.PlayOneShot(eventUI);
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
            audioSource.PlayOneShot(bestfriend);
            bestFriendPopup.SetActive(false);

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
            return;
        }

        giftItems = new List<ItemEntry>();
        foreach (var item in UserInventoryManager.Instance.userInventoryEntries)
        {
            ItemEntry itemEntry = item.GetItemEntry();
            if (itemEntry != null)
            {
                giftItems.Add(itemEntry);
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
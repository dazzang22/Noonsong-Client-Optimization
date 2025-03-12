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
                Debug.LogError("currentNoonsongï¿½ï¿½ nullï¿½Ô´Ï´ï¿½! EncounterUIï¿½ï¿½ï¿½ï¿½ ï¿½Ã¹Ù¸ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½È¯ï¿½Ç´ï¿½ï¿½ï¿½ È®ï¿½ï¿½ï¿½Ï¼ï¿½ï¿½ï¿½.");
                return;
            }

            if (arObjectCatch == null)
            {
                Debug.LogError("arObjectCatchï¿½ï¿½ nullï¿½Ô´Ï´ï¿½! GiftInventory.Initialize()ï¿½ï¿½ï¿½ï¿½ ï¿½Ã¹Ù¸ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ß´ï¿½ï¿½ï¿½ È®ï¿½ï¿½ï¿½Ï¼ï¿½ï¿½ï¿½.");
                return;
            }

            if (!currentNoonsong.isDiscovered)
            {
                Debug.Log("ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ß°ßµï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½. CollectCharacter() ï¿½ï¿½ï¿½ï¿½");
                arObjectCatch.CollectCharacter();
            }

            string university = encounterUI.GetCurrentNoonsongUniversity();
            ItemEntry.PreferenceLevel preference = selectedGiftItem.GetPreferenceForDepartment(university);

            //ï¿½Ü°ï¿½ï¿½ëº° Ä£ï¿½Ðµï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
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
            int preferenceMultiplier = 1;
            string giftReaction = "ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½Ø¼ï¿½ ï¿½Ö´ï¿½ ï¿½Å¾ï¿½? ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½.";
            GameObject effectToActivate = null;

            switch (preference)
            {
                case ItemEntry.PreferenceLevel.Love:
                    audioSource.PlayOneShot(love);
                    preferenceMultiplier = 5;
                    giftReaction = currentNoonsong.isFriend
                        ? "ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½Æ´Â±ï¿½ï¿½ï¿½? ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½!"
                        : "ï¿½ï¿½! ï¿½ï¿½ ï¿½Ì°ï¿½ ï¿½ï¿½Â¥ ï¿½ï¿½ï¿½ï¿½ï¿½Ï´Âµï¿½, ï¿½î¶»ï¿½ï¿½ ï¿½Ë¾Ò¾ï¿½? ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½~";
                    effectToActivate = giftEffectParticle4;
                    break;
                case ItemEntry.PreferenceLevel.Like:
                    audioSource.PlayOneShot(love);
                    preferenceMultiplier = 3;
                    giftReaction = currentNoonsong.isFriend
                        ? "ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿?! ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½~"
                        : "ï¿½ï¿½, ï¿½Ì°ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½? ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½à¼­ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½!";
                    effectToActivate = giftEffectParticle3;
                    break;
                case ItemEntry.PreferenceLevel.Dislike:
                    audioSource.PlayOneShot(hate);
                    preferenceMultiplier = 0;
                    giftReaction = currentNoonsong.isFriend
                        ? "ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½~"
                        : "ï¿½ï¿½ï¿½ï¿½, ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½.";
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

            if (!currentNoonsong.isFriend)
            {
                int newLoveLevel = currentNoonsong.loveLevel + affectionChange;
                // 50 -> ´ëÇÐº° Ä£±¸°¡ µÉ ¼ö ÀÖ´Â È£°¨µµ ¼öÄ¡·Î º¯°æ (DB¿¡¼­ °¡Á®¿È)
                if (newLoveLevel > 50)
                {
                    affectionChange = 50 - currentNoonsong.loveLevel;
                }
            }

            int updatedLoveLevel = encounterUI.UpdateNoonsongAffection(affectionChange);
            int bestfriend = DogamChartManager.Instance.maxFavor(currentNoonsong.university);
            // 100 -> ´ëÇÐº° º£ÇÁ°¡ µÉ ¼ö ÀÖ´Â È£°¨µµ ¼öÄ¡·Î º¯°æ (DB¿¡¼­ °¡Á®¿È)

            if (updatedLoveLevel >= 100 && !currentNoonsong.isBestFriend)
            {
                Debug.Log("ï¿½ï¿½ï¿½ï¿½ ï¿½Ë¾ï¿½");
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

            // ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
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
            Debug.LogError("InventoryManagerï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿? ï¿½Ê¾Ò½ï¿½ï¿½Ï´ï¿½");
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
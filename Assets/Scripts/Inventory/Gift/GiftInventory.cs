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
                Debug.LogError("currentNoonsong�� null�Դϴ�! EncounterUI���� �ùٸ� ���� ��ȯ�Ǵ��� Ȯ���ϼ���.");
                return;
            }

            if (arObjectCatch == null)
            {
                Debug.LogError("arObjectCatch�� null�Դϴ�! GiftInventory.Initialize()���� �ùٸ��� �����ߴ��� Ȯ���ϼ���.");
                return;
            }

            if (!currentNoonsong.isDiscovered)
            {
                Debug.Log("������ �߰ߵ��� ����. CollectCharacter() ����");
                arObjectCatch.CollectCharacter();
            }

            string university = encounterUI.GetCurrentNoonsongUniversity();
            ItemEntry.PreferenceLevel preference = selectedGiftItem.GetPreferenceForDepartment(university);

            //�ܰ��뺰 ģ�е� ������ ����
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
            string giftReaction = "�� �����ؼ� �ִ� �ž�? ������.";
            GameObject effectToActivate = null;

            switch (preference)
            {
                case ItemEntry.PreferenceLevel.Love:
                    audioSource.PlayOneShot(love);
                    preferenceMultiplier = 5;
                    giftReaction = currentNoonsong.isFriend
                        ? "���� ���� �� �ƴ±���? ���� ������!"
                        : "��! �� �̰� ��¥ �����ϴµ�, ��� �˾Ҿ�? ���� ������~";
                    effectToActivate = giftEffectParticle4;
                    break;
                case ItemEntry.PreferenceLevel.Like:
                    audioSource.PlayOneShot(love);
                    preferenceMultiplier = 3;
                    giftReaction = currentNoonsong.isFriend
                        ? "������ ���! ������~"
                        : "��, �̰� ���� ��? �������༭ ������!";
                    effectToActivate = giftEffectParticle3;
                    break;
                case ItemEntry.PreferenceLevel.Dislike:
                    audioSource.PlayOneShot(hate);
                    preferenceMultiplier = 0;
                    giftReaction = currentNoonsong.isFriend
                        ? "������~"
                        : "����, ������.";
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

            if (!currentNoonsong.isFriend)
            {
                int newLoveLevel = currentNoonsong.loveLevel + affectionChange;
                if (newLoveLevel > 50)
                {
                    affectionChange = 50 - currentNoonsong.loveLevel;
                }
            }

            int updatedLoveLevel = encounterUI.UpdateNoonsongAffection(affectionChange);

            if (updatedLoveLevel >= 100 && !currentNoonsong.isBestFriend)
            {
                Debug.Log("���� �˾�");
                audioSource.PlayOneShot(eventUI);
                ShowBestFriendPopup();
                currentNoonsong.isBestFriend = true;
            }

            if (updatedLoveLevel == 50 && !currentNoonsong.isFriend)
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

            // ���� ������ ����
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
            Debug.LogError("InventoryManager�� ������� �ʾҽ��ϴ�");
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
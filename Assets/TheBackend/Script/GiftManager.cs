using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 뒤끝 SDK namespace 추가
using BackEnd;

public class GiftManager {
    private static GiftManager _instance = null;

    public static GiftManager Instance {
        get {
            if(_instance == null) {
                _instance = new GiftManager();
            }

            return _instance;
        }
    }
    
    //아이템 소모
    public void sendGiftItem(int itemIdToDelete)
    {
        string userId = UserDataManager.Instance.getUserID();
        UserInventoryManager.Instance.RemoveItem(userId, itemIdToDelete);
    }

    //친밀도 상승
    public void getGiftFavor(int favor)
    {
        
    }



}
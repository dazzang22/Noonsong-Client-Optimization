using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;
using TMPro;
using System.Collections.Generic;


public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance {get; private set;}
    [SerializeField] private List<TextMeshProUGUI> nicknameTexts = new List<TextMeshProUGUI>(); // UI 텍스트 리스트
    [SerializeField] private List<TextMeshProUGUI> idTexts = new List<TextMeshProUGUI>(); // UI 텍스트 리스트

    private string userId = "";
    private string userNickname = "";

    



    private void Awake()
    {
        //userId=UserDataManager.Instance.getUserID();
        //userNickname=UserDataManager.Instance.getUserNickname();
        setID();
        setNickname();
         if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void setID()
    {
        userId=UserDataManager.Instance.getUserID();
        UpdateIdUI();
    }
    public void setNickname()
    {
        userNickname=UserDataManager.Instance.getUserNickname();
        UpdateNickUI();
    }

    private void UpdateNickUI()
    {
        foreach(var text in nicknameTexts)
        {
            if(text != null)
            {
                text.text=$"{userNickname}";
            }
        }

    }
    private void UpdateIdUI()
    {
        foreach(var text in idTexts)
        {
            if(text != null)
            {
                text.text=$"{userId}";
            }
            Debug.Log($"idtext: {text.text}");

        }
    }
}

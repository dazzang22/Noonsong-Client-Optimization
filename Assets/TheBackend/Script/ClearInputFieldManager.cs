using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClearInputFieldManager : MonoBehaviour
{
    [SerializeField] private List<TMP_InputField> inputFields;
    [SerializeField] private Button clearButton;

    void Start()
    {
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearAllInputFields);
        }
    }

    public void ClearAllInputFields()
    {
        foreach (var field in inputFields)
        {
            if (field != null)
            {
                field.text = "";
            }
        }
    }
}

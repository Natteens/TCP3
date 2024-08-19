using UnityEngine;
using TMPro;
using System;

public class SendMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ToText();
        }
    }

    private void ToText()
    {
        transform.SetAsLastSibling();
        inputField.Select();
    }
}

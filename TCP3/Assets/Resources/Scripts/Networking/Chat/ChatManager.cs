using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;

    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] GameObject chatContent;
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] GameObject chat;
    [SerializeField] StarterAssetsInputs input;
    public string playerName;

     [SerializeField] private bool isChatActive = false;
     [SerializeField] private float chatDeactivateTime = 5f;
     [SerializeField] private float chatTimer = 0f;
     [SerializeField] private bool inputWasPressed = false;

    void Awake()
    {
        ChatManager.Singleton = this;
        chat = GameObject.Find("ChatBox");
        chatContent = GameObject.Find("Chat Content");
        chatInput = GameObject.Find("inputField").GetComponent<TMP_InputField>();
        input = GetComponent<StarterAssetsInputs>();
    }

    void Start()
    {
        ActiveChat(false);
    }

    void Update()
    {
        if (input.chat && !inputWasPressed)
        {
            ToggleChat();
            inputWasPressed = true;
        }

        if (!input.chat)
        {
            inputWasPressed = false;
        }

        if (isChatActive)
        {
            chatTimer += Time.deltaTime;
            if (chatTimer >= chatDeactivateTime)
            {
                ActiveChat(false);
                chatTimer = 0f;
            }

            if (input.chat)
            {
                SendMessage();
                chatTimer = 0f;
            }
        }
    }

    private void ToggleChat()
    {
        isChatActive = !isChatActive;
        ActiveChat(isChatActive);

        if (isChatActive)
        {
            chatInput.ActivateInputField();
        }
    }

    private void ActiveChat(bool state)
    {
        chat.SetActive(state);
        isChatActive = state;

        if (state)
        {
            chatInput.ActivateInputField();
        }
        else
        {
            chatInput.text = "";
        }
    }

    private void SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(chatInput.text))
        {
            string playerName = LobbyManager.Instance.GetName();
            SendChatMessage(chatInput.text, playerName);
            chatInput.text = "";
        }
    }

    public void SendChatMessage(string _message, string _fromWho = null)
    {
        if (string.IsNullOrWhiteSpace(_message)) return;

        string S = _fromWho + " > " + _message;
        SendChatMessageServerRpc(S);
    }

    void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }

    [ServerRpc(RequireOwnership = false)]
    void SendChatMessageServerRpc(string message)
    {
        ReceiveChatMessageClientRpc(message);
    }

    [ClientRpc]
    void ReceiveChatMessageClientRpc(string message)
    {
        ChatManager.Singleton.AddMessage(message);
    }
}

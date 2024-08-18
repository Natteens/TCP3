using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Instance;

    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] float messageCooldown = 1f;
    private float cooldownTimer = 0f;

    void Awake() 
    { ChatManager.Instance = this; }

    void Update() 
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
    }

    public void SendMessage()
    {
        if (CanSendMessage())
        {
            string message = chatInput.text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                SendChatMessageInLobby(message);
                chatInput.text = "";
                cooldownTimer = messageCooldown;
            }
        }
    }

    public void SendChatMessageInLobby(string _message)
    {
        if (string.IsNullOrWhiteSpace(_message)) return;
        LobbyManager.Instance.SendLobbyChatMessage(_message);
    }

    public void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }

    public void ReceiveChatMessage(string message)
    {
        ChatManager.Instance.AddMessage(message);
    }

    private bool CanSendMessage()
    {
        return cooldownTimer <= 0f;
    }
}
using gameServer.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{

    private Text uiChat;
    private InputField uiInputField;
    private string chat = "";

    // Start is called before the first frame update
    void Start()
    {
        GameClient.Instanse.OnReceiveChatMessage += OnReceiveChatMessage;
        GameClient.Instanse.GetRecentChatMessage();
        uiChat = gameObject.GetComponentInChildren<Text>();
        uiInputField = gameObject.GetComponentInChildren<InputField>();
        uiInputField.onEndEdit.AddListener(onEndEdit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onEndEdit(string text)
    {
        GameClient.Instanse.SendChatMessage(text);
        uiInputField.text = "";
    }

    private void OnReceiveChatMessage(object sender, ChatMessageEventArgs chatMessageEventArgs)
    {
        chat += "\n" + chatMessageEventArgs.Message;
        uiChat.text = chat;
    }

    void onDestroy()
    {
        GameClient.Instanse.OnReceiveChatMessage -= OnReceiveChatMessage;
    }
}

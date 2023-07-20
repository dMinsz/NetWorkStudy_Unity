using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public static Chat instance { get; private set; }

    public InputField inputField;
    public RectTransform ChatContent;
    public Text ChatText;
    public ScrollRect ChatScrollRect;

    private void Awake()
    {
        instance = this;
        inputField.onSubmit.AddListener(AddMessage);
    }

    public void AddMessage(string data)
    {
        inputField.text = "";
        inputField.ActivateInputField();
        Text newMessage = Instantiate(ChatText);
        newMessage.text = data;
        newMessage.transform.SetParent(ChatContent.transform);
        ChatScrollRect.verticalScrollbar.value = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI content;


    public void SetMessage(string content, string title, Color color)
    {
        this.title.text = title;
        this.title.color = color;
        this.content.text = content;
    }
}

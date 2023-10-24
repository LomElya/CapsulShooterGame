using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AlertWindow : WindowCore
{
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _OKButton;

    private void Awake()
    {
        _OKButton.onClick.AddListener(() => { Hide(); });
    }

    public void Init(string text, UnityAction onOKButtonClicked)
    {
        _messageText.text = text;
        _OKButton.onClick.AddListener(onOKButtonClicked);
    }
}
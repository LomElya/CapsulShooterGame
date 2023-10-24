using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmWindow : WindowCore
{
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _buttonOK;
    [SerializeField] private Button _buttonCancel;

    private void Awake()
    {
        _buttonOK.onClick.AddListener(() => { Hide(); });
        _buttonCancel.onClick.AddListener(() => { Hide(); });
    }

    public void Init(string text, UnityAction onOKButtonClicked)
    {
        _messageText.text = text;

        _buttonOK.onClick.AddListener(onOKButtonClicked);

    }
}

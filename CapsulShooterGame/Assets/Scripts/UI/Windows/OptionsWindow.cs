using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsWindow : WindowCore
{
    [Header("Options")]
    [SerializeField] private TMP_Dropdown _dropdownQuality;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Button _buttonСontrols;

    [Header("Buttons")]
    [SerializeField] private Button _buttonSave;
    [SerializeField] private Button _buttonDefault;
    [SerializeField] private Button _buttonCancel;

    private bool IsActive => this.gameObject.activeSelf;
    private bool IsControlsActive;

    public void SetActive() => this.gameObject.SetActive(!IsActive);

    private void Init()
    {
        SetActive();

        if (!IsActive)
            return;

        LoadSettings();
    }

    private void Awake()
    {
        AddListener();
        LoadSettings();
    }

    private void OpenСontrols()
    {
        if (IsControlsActive)
            return;

        var control = WindowsManagers.ShowDialog<ControlsWindow>();
        control.Init(() =>
        {
            IsControlsActive = false;
        });

        IsControlsActive = true;
    }

    private void AddListener()
    {
        _buttonСontrols.onClick.AddListener(OpenСontrols);

        _dropdownQuality.onValueChanged.AddListener(SetQuality);

        _volumeSlider.onValueChanged.AddListener(SetVolume);

        _buttonSave.onClick.AddListener(SaveOptions);
        _buttonDefault.onClick.AddListener(DefaultOptions);
        _buttonCancel.onClick.AddListener(Init);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
            _dropdownQuality.value = PlayerPrefs.GetInt("QualitySettingPreference");
        else
            _dropdownQuality.value = 3;

        if (PlayerPrefs.HasKey("VolumePreference"))
        {
            Debug.Log(PlayerPrefs.GetFloat("VolumePreference"));
            Debug.Log(_volumeSlider.value);
            AudioUtility.SetMasterVolume(PlayerPrefs.GetFloat("VolumePreference"));
            _volumeSlider.value = PlayerPrefs.GetFloat("VolumePreference");
        }
        else
        {
            AudioUtility.SetMasterVolume(1f);
            _volumeSlider.value = 1f;
        }
    }

    private void SaveOptions()
    {
        var confirm = WindowsManagers.ShowDialog<ConfirmWindow>();

        confirm.Init("Save all changes?", () =>
        {
            PlayerPrefs.SetInt("QualitySettingPreference", _dropdownQuality.value);
            PlayerPrefs.SetFloat("VolumePreference", _volumeSlider.value);

            SetActive();
            Destroy(confirm);
        });
    }

    private void DefaultOptions()
    {
        _dropdownQuality.value = 3;
        _volumeSlider.value = 1f;
    }

    private void SetQuality(int qualityIndex) => QualitySettings.SetQualityLevel(qualityIndex);

    private void SetVolume(float volume) => AudioUtility.SetMasterVolume(volume);
}

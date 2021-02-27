using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsView : MonoBehaviour
{
    [SerializeField] private string masterParam;
    [SerializeField] private string musicParam;
    [SerializeField] private string sfxParam;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        masterSlider.onValueChanged.AddListener(value =>
        {
            HandleSliderChange(masterParam, value);
        });
        
        musicSlider.onValueChanged.AddListener(value =>
        {
            HandleSliderChange(musicParam, value);
        });
        
        sfxSlider.onValueChanged.AddListener(value =>
        {
            HandleSliderChange(sfxParam, value);
        });
    }

    private void OnEnable()
    {
        var audioManager = AudioManager.Instance;

        masterSlider.SetValueWithoutNotify(audioManager.GetParamFromMixer(masterParam));
        musicSlider.SetValueWithoutNotify(audioManager.GetParamFromMixer(musicParam));
        sfxSlider.SetValueWithoutNotify(audioManager.GetParamFromMixer(sfxParam));
    }

    private void HandleSliderChange(string paramName, float value)
    {
        AudioManager.Instance.SetParamOnMixer(paramName, value);
    }
}

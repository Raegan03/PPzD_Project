using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private bool hoverAudio = true;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnPointerClick);
    }

    public void OnPointerClick()
    {
        AudioManager.Instance.PlayClickUIClip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!hoverAudio) return;
        AudioManager.Instance.PlayHoverUIClip();
    }
}

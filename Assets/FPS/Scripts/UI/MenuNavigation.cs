using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private CanvasGroup fader;
    [SerializeField] private float transitionTime = .75f;
    
    public Selectable defaultSelection;

    IEnumerator Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
        
        fader.gameObject.SetActive(true);
        
        AudioManager.Instance.ChangeAudioState(AudioState.UI, transitionTime);
        for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / transitionTime)
        {
            fader.alpha = Mathf.SmoothStep(1f, 0f, t);
            yield return null;
        }
        
        fader.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Input.GetButtonDown(GameConstants.k_ButtonNameSubmit)
                || Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal) != 0
                || Input.GetAxisRaw(GameConstants.k_AxisNameVertical) != 0)
            {
                EventSystem.current.SetSelectedGameObject(defaultSelection.gameObject);
            }
        }
    }

    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(LoadTargetScene(sceneName));
    }
    
    private IEnumerator LoadTargetScene(string sceneName)
    {
        fader.gameObject.SetActive(true);
        
        AudioManager.Instance.ChangeAudioState(AudioState.Silent, transitionTime);
        for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / transitionTime)
        {
            fader.alpha = Mathf.SmoothStep(0f, 1f, t);
            yield return null;
        }
        
        yield return null;
        SceneManager.LoadScene(sceneName);
    }
}

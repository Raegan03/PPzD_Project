using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneName = "";

    private MenuNavigation _menuNavigation;
    
    private void Awake()
    {
        _menuNavigation = FindObjectOfType<MenuNavigation>();
    }

    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == gameObject 
            && Input.GetButtonDown(GameConstants.k_ButtonNameSubmit))
        {
            LoadTargetScene();
        }
    }

    public void LoadTargetScene()
    {
        if(_menuNavigation) _menuNavigation.LoadNextScene(sceneName);
        else SceneManager.LoadScene(sceneName);
    }
}

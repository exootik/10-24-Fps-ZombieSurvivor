using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button buttonStart;
    public Button buttonQuit;
    public TMP_Dropdown dropdownLevelSelector;

    private void Start()
    {
        buttonStart.onClick.AddListener(OnStartButtonClicked);
        buttonQuit.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnDestroy()
    {
        if (buttonStart != null)
            buttonStart.onClick.RemoveListener(OnStartButtonClicked);

        if (buttonQuit != null)
            buttonQuit.onClick.RemoveListener(OnQuitButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        var idx = dropdownLevelSelector.value;

        if (idx == 0)
        {
            Debug.Log("Scene LEVEL lancé");
            SceneManager.LoadScene("LEVEL");
        }
        else if (idx == 1)
        {
            Debug.Log("Scene LEVEL2 lancé");
            SceneManager.LoadScene("LEVEL2");
        }
        else
        {
            Debug.LogWarning($"MainMenuManager: option du dropdown non gérée (index = {idx}).");
        }
    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("Quitter l'application...");
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
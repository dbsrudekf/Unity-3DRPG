using System;
using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    SavingWrapper savingWrapper;

    [SerializeField] TMP_InputField newGameNameField;


    private void Start()
    {
        savingWrapper = GetSavingWrapper();
    }

    private SavingWrapper GetSavingWrapper()
    {
        return FindObjectOfType<SavingWrapper>();
    }

    public void ContinueGame()
    {
        savingWrapper.ContinueGame();
    }

    public void NewGame()
    {
        savingWrapper.NewGame(newGameNameField.text);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}

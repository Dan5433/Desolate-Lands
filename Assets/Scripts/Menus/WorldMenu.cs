using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldMenu : MonoBehaviour
{
    [SerializeField] Button loadWorld;

    private void OnEnable()
    {
        if (Directory.GetDirectories(MainMenuManager.SavesDirPath).Length > 0)
            EnableLoadButton();
        else
            DisableLoadButton();
    }

    void DisableLoadButton()
    {
        loadWorld.interactable = false;
        loadWorld.GetComponentInChildren<TMP_Text>().color = loadWorld.colors.disabledColor;
    }

    void EnableLoadButton()
    {
        loadWorld.interactable = true;
        loadWorld.GetComponentInChildren<TMP_Text>().color = Color.white;
    }
}

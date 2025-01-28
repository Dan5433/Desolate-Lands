using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WorldMenu : MonoBehaviour
{
    [SerializeField] Button loadWorld;

    private void Awake()
    {
        if (Directory.GetDirectories(MainMenuManager.SavesDirPath).Length > 0)
            loadWorld.interactable = true;
        else
            loadWorld.interactable = false;
    }
}

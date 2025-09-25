using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonCallingPauseMenuMap : MonoBehaviour
{
    GameObject settings;
    public void OpenSettings()
    {
        settings.SetActive(true);
    }
    public void CloseSettings()
    {
        settings.SetActive(false);
    }
    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

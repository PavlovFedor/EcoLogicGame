using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonCallingLevelScene : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject SettingsGame;
    public void ClickOpenMenu()
    {
        Time.timeScale = 0;
        PauseMenu.SetActive(true);
    }
    public void ClickMenuContinue()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
    }
    public void ClickMenuSettings()
    {
        SettingsGame.SetActive(true);
    }
    public void ClickCloseSettings()
    {
        SettingsGame.SetActive(false);
    }
    public void ClickStartAgain(string scName)
    {
        SceneManager.LoadScene(scName);
    }
    public void ClickSelectLevels()
    {
        SceneManager.LoadScene("MainMapV1.8Animated");
    }
}

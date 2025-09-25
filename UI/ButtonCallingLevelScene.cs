using UnityEngine;

public class ButtonCallingLevelScene : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject SettingsGame;
    public void ClickOpenMenu()
    {
        PauseMenu.SetActive(true);
    }
    public void ClickMenuContinue()
    {
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
    public void ClickStartAgain()
    {
        Debug.Log("StartAgain");
    }
    public void ClickSelectLevels()
    {
        Debug.Log("SelectLevels");
    }
}

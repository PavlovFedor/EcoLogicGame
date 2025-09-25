using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCalling : MonoBehaviour
{
    public GameObject SaveGame;
    public GameObject LoadGame;
    public GameObject SettingsMenu;
    public void ClickContinue()
    {
        Debug.Log("Continue");
    }
    public void ClickNewGame()
    {
        SaveGame.SetActive(true);
    }
    public void ClickLoadGame()
    {
        LoadGame.SetActive(true);
    }
    public void ClickSettings()
    {
        SettingsMenu.SetActive(true);
    }
    public void ClickLeaveGame()
    {
        Application.Quit();
    }
    public void CLickOpenMenu()
    {
        Debug.Log("Open menu");
    }
    public void ClickStartCode()
    {
        Debug.Log("Start code");
    }
    public void ClickStopCode()
    {
        Debug.Log("Stop code");
    }
    public void CLickButtonReturnSettings()
    {
        SettingsMenu.SetActive(false);
    }
    public void CLickButtonReturnSaveGame()
    {
        SaveGame.SetActive(false);
    }
    public void CLickButtonReturnLoadGame()
    {
        LoadGame.SetActive(false);
    }
}

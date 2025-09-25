using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGame : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;
    private void Start()
    {
        // Заполняем в Dropdown все возможные разрешения
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0; // индекс нынешнего разрешения на девайсе
        for(int i = 0; i<resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            // Находим индекс нынешнего разрешения
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        // Автозаполнение разрешения под девайс
        resolutionDropdown.value = currentResolutionIndex; // Устанавливаем значение Dropdown с помощью индекса нынешнего разрешения
        resolutionDropdown.RefreshShownValue(); // Обновляем значение Dropdown
    }
    public void SetResolution(int resolutionIndex)
    {
        // Функция установки разрешения
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetSFXVolume(float sfxvolume)
    {
        // Функция установки громкости музыки
        Debug.Log("SFXVolume: "+sfxvolume);
    }
    public void SetMusicVolume(float musicvolume)
    {
        // Функция установки громкости музыки
        Debug.Log("MusicVolume: "+musicvolume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        // Функция установки полноэкранного режима
        Screen.fullScreen = isFullscreen;
    }
}

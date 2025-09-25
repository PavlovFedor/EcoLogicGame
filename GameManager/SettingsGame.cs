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
        // ��������� � Dropdown ��� ��������� ����������
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0; // ������ ��������� ���������� �� �������
        for(int i = 0; i<resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            // ������� ������ ��������� ����������
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        // �������������� ���������� ��� ������
        resolutionDropdown.value = currentResolutionIndex; // ������������� �������� Dropdown � ������� ������� ��������� ����������
        resolutionDropdown.RefreshShownValue(); // ��������� �������� Dropdown
    }
    public void SetResolution(int resolutionIndex)
    {
        // ������� ��������� ����������
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetSFXVolume(float sfxvolume)
    {
        // ������� ��������� ��������� ������
        Debug.Log("SFXVolume: "+sfxvolume);
    }
    public void SetMusicVolume(float musicvolume)
    {
        // ������� ��������� ��������� ������
        Debug.Log("MusicVolume: "+musicvolume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        // ������� ��������� �������������� ������
        Screen.fullScreen = isFullscreen;
    }
}

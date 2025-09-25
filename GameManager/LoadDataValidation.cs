using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class LoadDataValidation : SaveCellProcessing
{
    public Button btn;
    void Update()
    {
        Validation();
    }
    private void Validation()
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(savePath))
        {
            btn.interactable = false;
        }
        else
        {
            btn.interactable = true;
        }
    }
}

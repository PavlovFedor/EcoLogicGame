using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonCallingSaveSystem : MonoBehaviour
{
    public void ClickSaveSlot(int CellId)
    {
        SaveSystem saveSys = FindAnyObjectByType<SaveSystem>();
        saveSys.SaveNewGame(CellId);
        saveSys.LoadGameData(CellId);
        SceneManager.LoadScene("PanelSelectLevelPrototype");
    }
    public void ClickLoadSlot(int CellId)
    {
        SaveSystem saveSys = FindAnyObjectByType<SaveSystem>();
        saveSys.LoadGameData(CellId);
        SceneManager.LoadScene("PanelSelectLevelPrototype");
    }
}

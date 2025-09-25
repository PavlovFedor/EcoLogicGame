using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelButtonProcessing : MonoBehaviour
{
    public TextMeshProUGUI titleLevelText;
    private Level levelDataset;
    public Button lvlbtn;

    private Biome biomeDataset;
    private void Start()
    {
        lvlbtn.onClick.AddListener(RunLevel);
    }
    public void lvlDataProcessing(Level data)
    {
        levelDataset = data;
        titleLevelText.text = data.nameLevel;
    }
    public void RunLevel()
    {
        SceneManager.LoadScene(levelDataset.sceneName);
    }
    public void SetBiomeDatset(Biome biome)
    {
        biomeDataset = biome;
    }
}

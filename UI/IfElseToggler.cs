using UnityEngine;
using TMPro;
public class IfElseToggler : ButtonToggle
{
    [SerializeField] private GameObject containerElse;
    [SerializeField] private GameObject IfElseBlock;
    [SerializeField] private float multiblockHeightDiff;

    public override void ToggleOn()
    {
        base.ToggleOn();
        if (!containerElse.activeInHierarchy)
        {
            IfElseBlock.GetComponent<ForMultiblock>().multiblockHeightOffset += multiblockHeightDiff;
        }
        containerElse.SetActive(true);
    }

    public override void ToggleOff()
    {
        base.ToggleOff();
        if (containerElse.activeInHierarchy)
        {
            IfElseBlock.GetComponent<ForMultiblock>().multiblockHeightOffset -= multiblockHeightDiff;
        }
        containerElse.SetActive(false);


        /*
        //Удаляем блоки кода контейнера иначе
        foreach (var go in containerElse)
        {
            CodeBlock codeBlock = go.GetComponent<CodeBlock>();
            codeBlockManager.DeleteCodeBlock(transform.GetSiblingIndex());
            
            Destroy(clone);
        }
        */
    }

    public override void ApplyInitialState()
    {
    }
}

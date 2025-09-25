using UnityEngine;

public class CB_Foreach : CBCycles
{

    public override void Execute()
    {
        foreach (var block in codeBlocks)
        {
            block.Execute();
        }
        Debug.Log($"Блок {blockName} выполнен.");
    }

    public override void InitializationCB()
    {
         blockName = "CB_Foreach";
    }
}

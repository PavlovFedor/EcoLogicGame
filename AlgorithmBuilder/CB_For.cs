using UnityEngine;

public class CB_For : CBCycles
{
    public string[] conditions = {"Less", "More", "LessEqual", "MoreEqual"};
    public string condition = "Less";
    public int startValue = 0;

    public override void Execute()
    {
        if (condition == "Less")
            for (int i = startValue; conditionFor(i); edit(i)){
                codeBlocks[i].Execute();
            }
    }

    public override void InitializationCB()
    {
         blockName = "CB_Foreach";
    }

    private int edit(int number){
        return number++;
    }

    private bool conditionFor(int number){
        if(true)   
        {
            return true;   
        }
    }
}

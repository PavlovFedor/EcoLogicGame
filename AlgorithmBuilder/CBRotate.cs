using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CBRotate: CodeBlock
{
// Блок Вращения
// Поворачивает игрока на указанное значение 
	
   public int angleOfRotation = 90;

   private void RotatePlayer(int rotate)
   {
      player.transform.Rotate(0, rotate, 0);
   }
	
   public override void Execute()
   {
      // Здесь будет логика выполнения блока
      RotatePlayer(angleOfRotation);
      Debug.Log($"Блок {blockName} выполнен.");
   }

   public override void InitializationCB()
   {
         blockName = "CBRotate";
   }
}
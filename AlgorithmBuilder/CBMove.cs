using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// Блок Передвижения
public class CBMove: CodeBlock
{
   public int direction = 1;
   public int distance = 1;

   public Dropdown dropdown;

    //for move
    public float moveDuration = 0.2f;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float moveTimer = 0.0f;

   void Update()
   {
      MovePlayer();
   }

   void OnDropdownValueChanged(int index)
   {
      Debug.Log("Выбран элемент с индексом: " + index);
   }

   public override void Execute()
   {
      // Здесь будет логика выполнения блока
      StartMovePlayer();
      Debug.Log($"Блок {blockName} выполнен.");
   }

   private void StartMovePlayer(){

      //Выбор направления
      int selectedIndex = dropdown.value;
      //Текущие координаты игрока
      Vector3 playerPosition = player.transform.position; 

      //Задаем координаты и обнуляем флаг с таймером
      startPosition = playerPosition;
      moveTimer = 0.0f;
      isMoving = true;
 
      //Передвигаем персонажа по направлениям
      //Влево
      if (selectedIndex == 0) {
         endPosition = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z + distance);      
      }
      //Вправо
      else if (selectedIndex == 1) {
         endPosition = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z - distance);
      }
   }

   //Осуществяет передвижение игрока
   public void MovePlayer(){
     
      if (isMoving)
      {
         moveTimer += Time.deltaTime;
         player.transform.position = Vector3.Lerp(startPosition, endPosition, moveTimer / moveDuration);
         if (moveTimer >= moveDuration)
         {
            isMoving = false;
            player.transform.position = endPosition;
         }
         return;
      }
   }

   public override void InitializationCB()
   {
      blockName = "CBWalk";

      if (dropdown == null){
         Debug.LogError("Dropdown не задан у " + blockName);
      }

      // Добавляем обработчик события
      dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
   }

   private void OnDestroy()
   {
      dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
   }

}
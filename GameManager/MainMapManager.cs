using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System; 
using System.Collections;

public class MainMapManager : MonoBehaviour
{
    [Header("Камеры")]
    public Camera camToFactory;
    public Camera camToPark;
    public Camera camToForest;
    public Camera camToDesert;
    public Camera camToFinally;

    [Header("Баррикады")]
    public MovableObject[] movObjFactory;
    public MovableObject[] movObjPark;
    public MovableObject[] movObjForest;
    public MovableObject[] movObjDesert;

    [Header("Задержки после анимаций")]
    public int timerToFactory;
    public int timerToPark;
    public int timerToForest;
    public int timerToDesert;
    public int timerToFinally;

    [Header("VFX баррикад")]
    public GameObject[] vfxToFactory;
    public GameObject[] vfxToPark;
    public GameObject[] vfxToForest;
    public GameObject[] vfxToDesert;
    public GameObject[] vfxToFinally;

    [Header("Good Object")]
    public GameObject goodToLandscape;
    public GameObject goodToFactory;
    public GameObject goodToPark;
    public GameObject goodToForest;
    public GameObject goodToShore;
    public GameObject goodToDesert;

    [Header("Bad Object")]
    public GameObject badToLandscape;
    public GameObject badToFactory;
    public GameObject badToPark;
    public GameObject badToForest;
    public GameObject badToShore;
    public GameObject badToDesert;

    [Header("List Object")]
    public List<GameObject> goodToObject = new List<GameObject>();
    public List<GameObject> badToObject = new List<GameObject>();

    [Header("Other")]
    public int objectsPerFrame = 10;
    public Jump jumpScript;
    public GameObject CanvasGO;
    public PlayerInteraction scriptPlayerInteraction;
    public FirstPersonMovement fpMovementScript;
    public Crouch crouchScript;

    private bool isProcessing;
    private List<GameObject> objectsToProcess;

    private MovableObject[] movableObjects; // Кешированные скрипты
    private bool isExecuting = false;

    private MainMapCameraSwitcher camSwitch;
    protected TaskCompletionSource<bool> moveObjCompletionSource;
    private Canvas canvas;
    public GameObject congratulationsPanel;
    void Start()
    {
        canvas = CanvasGO.GetComponent<Canvas>();
        // Заполняем списки объектов из дочерних элементов
        PopulateObjectLists();
        
        UpdateScriptsCache(); // Первоначальное кеширование

        //Находим скрипт для переключения камер
        camSwitch = GetComponent<MainMapCameraSwitcher>();
        if (camSwitch == null)
            Debug.LogWarning("Not found MainMapCameraSwitcher!");

        if (camToFactory == null || camToPark == null || camToForest == null || camToDesert == null)
            Debug.LogWarning("Not found camera in inspector");
        
        StartProcessingAsync(goodToObject, false);
        if (SaveSystem.Instance.CheckBiomeCompleted(1))
        {
            LoadBiome(badToLandscape, goodToLandscape, movObjFactory);
        }
        if (SaveSystem.Instance.CheckBiomeCompleted(2))
        {
            LoadBiome(badToFactory, goodToFactory, movObjPark);
        }
        if (SaveSystem.Instance.CheckBiomeCompleted(3))
        {
            LoadBiome(badToPark, goodToPark, movObjForest);
        }
        if (SaveSystem.Instance.CheckBiomeCompleted(4))
        {
            LoadBiome(badToForest, goodToForest, null);
        }
        if (SaveSystem.Instance.CheckBiomeCompleted(5))
        {
            LoadBiome(badToShore, goodToShore, movObjDesert);
        }
        if (SaveSystem.Instance.CheckBiomeCompleted(6))
        {
            LoadBiome(badToDesert, goodToDesert, null);
        }
    }
    private async void LoadBiome(GameObject bad, GameObject good, MovableObject[] movObj)
    {
        await StartProcessingAsync(GetAllChildren(bad), false);
        await StartProcessingAsync(GetAllChildren(good), true);
        await ActivateMovableObject(movObj);
    }
    // Метод для заполнения списков объектов из дочерних элементов
    private void PopulateObjectLists()
    {
        goodToObject.Clear();
        badToObject.Clear();

        // Добавляем все дочерние объекты (включая вложенные) для Good Objects
        if (goodToLandscape != null) goodToObject.AddRange(GetAllChildren(goodToLandscape));
        if (goodToFactory != null) goodToObject.AddRange(GetAllChildren(goodToFactory));
        if (goodToPark != null) goodToObject.AddRange(GetAllChildren(goodToPark));
        if (goodToForest != null) goodToObject.AddRange(GetAllChildren(goodToForest));
        if (goodToShore != null) goodToObject.AddRange(GetAllChildren(goodToShore));
        if (goodToDesert != null) goodToObject.AddRange(GetAllChildren(goodToDesert));

        // Добавляем все дочерние объекты (включая вложенные) для Bad Objects
        if (badToLandscape != null) badToObject.AddRange(GetAllChildren(badToLandscape));
        if (badToFactory != null) badToObject.AddRange(GetAllChildren(badToFactory));
        if (badToPark != null) badToObject.AddRange(GetAllChildren(badToPark));
        if (badToForest != null) badToObject.AddRange(GetAllChildren(badToForest));
        if (badToShore != null) badToObject.AddRange(GetAllChildren(badToShore));
        if (badToDesert != null) badToObject.AddRange(GetAllChildren(badToDesert));
    }

    // Рекурсивно получаем все дочерние объекты
    private List<GameObject> GetAllChildren(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();
        
        if (parent == null)
            return children;

        // Добавляем все дочерние объекты рекурсивно
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
            children.AddRange(GetAllChildren(child.gameObject));
        }

        return children;
    }

    public async Task StartProcessingAsync(List<GameObject> targetObj, bool makeVisible)
    {
        if (isProcessing) 
        {
            // Можно добавить ожидание, пока предыдущая обработка завершится
            while (isProcessing)
                await Task.Yield();
        }

        objectsToProcess = new List<GameObject>(targetObj);
        isProcessing = true;
        await ProcessObjectAsync(makeVisible);
    }

    private async Task ProcessObjectAsync(bool makeVisible)
    {
        int processedCount = 0;

        while (objectsToProcess.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(objectsPerFrame, objectsToProcess.Count); i++)
            {
                GameObject obj = objectsToProcess[i];
                if (obj == null) continue;

                ToggleVisibility(obj, makeVisible);
            }

            objectsToProcess.RemoveRange(0, Mathf.Min(objectsPerFrame, objectsToProcess.Count));
            processedCount += objectsPerFrame;

            await Task.Yield();
        }

        isProcessing = false;
        Debug.Log($"Обработка завершена. Изменено объектов: {processedCount}");
    }

    private void ToggleVisibility(GameObject obj, bool visible)
    {
        var renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null) renderer.enabled = visible;

        var collider = obj.GetComponent<Collider>();
        if (collider != null) collider.enabled = visible;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y) && !isExecuting)
        {
            ActivateAllMovableObject();
        }

        //if (Input.GetKeyDown(KeyCode.Z) && !isExecuting)
        //{
        //    GoToFactory();
        //}

        //if (Input.GetKeyDown(KeyCode.X) && !isExecuting)
        //{
        //    GoToPark();
        //}

        //if (Input.GetKeyDown(KeyCode.C) && !isExecuting)
        //{
        //    GoToForest();
        //}

        //if (Input.GetKeyDown(KeyCode.V) && !isExecuting)
        //{
        //    GoToDesert();
        //}

        //if (Input.GetKeyDown(KeyCode.B) && !isExecuting)
        //{
        //    GoToFinally();
        //}
        if(SaveSystem.Instance.MarkPlayCutscene(1) && !isExecuting)
        {
            GoToFactory();
            SaveSystem.Instance.AutoSave();
        }
        if(SaveSystem.Instance.MarkPlayCutscene(2) && !isExecuting)
        {
            GoToPark();
            SaveSystem.Instance.AutoSave();
        }
        if (SaveSystem.Instance.MarkPlayCutscene(3) && !isExecuting)
        {
            GoToForest();
            SaveSystem.Instance.AutoSave();
        }
        if (SaveSystem.Instance.MarkPlayCutscene(4) && !isExecuting)
        {
            SaveSystem.Instance.AutoSave();
        }
        if (SaveSystem.Instance.MarkPlayCutscene(5) && !isExecuting)
        {
            GoToDesert();
            SaveSystem.Instance.AutoSave();
        }
        if (SaveSystem.Instance.MarkPlayCutscene(6) && !isExecuting)
        {
            GoToFinally();
            SaveSystem.Instance.AutoSave();
        }
    }

    public async void GoToFactory()
    {       
        isExecuting = true;

        try
        {
            //Отключаем UI для избежания ошибок
            await SwitchUI(false);
            //Делает основной камеру с видом на ворота завода
            await camSwitch.SwitchToStaticCamera(camToFactory);
            //Активация спецэффектов
            await ActivateParticleSystem(vfxToFactory);
            //Вызов метода скрипта, осуществяющего анимацию
            await ActivateMovableObject(movObjFactory);
            await Task.Delay(timerToFactory);
            //Делает основной камеру игрока
            await camSwitch.SwitchToPlayerCamera();
            //Отключение объектов Bad
            await StartProcessingAsync(GetAllChildren(badToLandscape), false);
            //Включение обектов Good
            await StartProcessingAsync(GetAllChildren(goodToLandscape), true);
            //Включаем UI
            await SwitchUI(true);
        }
        finally
        {
            isExecuting = false;
            Debug.Log("GoToFactory completed!");
        }
    }

    public async void GoToPark()
    {
        isExecuting = true;
        
        try
        {
            //Отключаем UI для избежания ошибок
            await SwitchUI(false);
            //Делает основной камеру с видом на калитки
            await camSwitch.SwitchToStaticCamera(camToPark);
            //Активация спецэффектов
            await ActivateParticleSystem(vfxToPark);
            //Вызов метода скрипта, осуществяющего анимацию
            await ActivateMovableObject(movObjPark);
            await Task.Delay(timerToPark);
            //Делает основной камеру игрока
            await camSwitch.SwitchToPlayerCamera();
            //Отключение объектов Bad
            await StartProcessingAsync(GetAllChildren(badToFactory), false);
            //Включение обектов Good
            await StartProcessingAsync(GetAllChildren(goodToFactory), true);
            //Включаем UI
            await SwitchUI(true);
        }
        finally
        {
            isExecuting = false;
            Debug.Log("GoToPark completed!");
        }
    }
    
    public async void GoToForest()
    {
        isExecuting = true;
        
        try
        {
            //Отключаем UI для избежания ошибок
            await SwitchUI(false);
            //Делает основной камеру с видом на фонарный столб
            await camSwitch.SwitchToStaticCamera(camToForest);
            //Активация спецэффектов
            await ActivateParticleSystem(vfxToForest);
            //Вызов метода скрипта, осуществяющего анимацию
            await ActivateMovableObject(movObjForest);
            await Task.Delay(timerToForest);
            //Делает основной камеру игрока
            await camSwitch.SwitchToPlayerCamera();
            //Отключение объектов Bad
            await StartProcessingAsync(GetAllChildren(badToPark), false);
            //Включение обектов Good
            await StartProcessingAsync(GetAllChildren(goodToPark), true);
            //Включаем UI
            await SwitchUI(true);
        }
        finally
        {
            isExecuting = false;
            Debug.Log("GoToForest completed!");
        }
    }
    
    public async void GoToDesert()
    {
        isExecuting = true;
        
        try
        {
            //Отключаем UI для избежания ошибок
            await SwitchUI(false);
            //Делает основной камеру с видом на ворота завода
            await camSwitch.SwitchToStaticCamera(camToDesert);
            //Активация спецэффектов
            await ActivateParticleSystem(vfxToDesert);
            //Вызов метода скрипта, осуществяющего анимацию
            await ActivateMovableObject(movObjDesert);
            await Task.Delay(timerToDesert);
            //Делает основной камеру игрока
            await camSwitch.SwitchToPlayerCamera();
            //Отключение объектов Bad
            List<GameObject> badToForestShore = new List<GameObject>();
            if (badToShore != null) badToForestShore.AddRange(GetAllChildren(badToShore));
            if (badToForest != null) badToForestShore.AddRange(GetAllChildren(badToForest));  
            await StartProcessingAsync(badToForestShore, false); 
            //Включение обектов Good
            List<GameObject> goodToForestShore = new List<GameObject>();
            if (goodToShore != null) goodToForestShore.AddRange(GetAllChildren(goodToShore));
            if (goodToForest != null) goodToForestShore.AddRange(GetAllChildren(goodToForest));
            await StartProcessingAsync(goodToForestShore, true);
            //Включаем UI
            await SwitchUI(true);
        }
        finally
        {
            isExecuting = false;
            Debug.Log("GoToDesert completed!");
        }
    }

    public async void GoToFinally()
    {
        isExecuting = true;

        try
        {
            //Отключаем UI для избежания ошибок
            await SwitchUI(false);
            await camSwitch.SwitchToStaticCamera(camToFinally);
            await ActivateParticleSystem(vfxToFinally);
            await StartProcessingAsync(GetAllChildren(badToDesert), false);
            await StartProcessingAsync(GetAllChildren(goodToDesert), true);
            await Task.Delay(timerToDesert);
            await camSwitch.SwitchToPlayerCamera();
            await ActivateTemporarily();
            jumpScript.jumpStrength = 1000f;
            //Включаем UI
            await SwitchUI(true);
        }
        finally
        {
            isExecuting = false;
            Debug.Log("GoToFinally completed!");
        }
    }

    // Обновляет кеш скриптов (можно вызывать при изменении объектов)
    public void UpdateScriptsCache()
    {
        movableObjects = FindObjectsByType<MovableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

    private async Task SwitchUI(bool isActivate)
    {
        canvas.enabled = isActivate;
        scriptPlayerInteraction.enabled = isActivate;
        fpMovementScript.enabled = isActivate;
        jumpScript.enabled = isActivate;
        crouchScript.enabled = isActivate;
    }

    private async Task ActivateParticleSystem(GameObject[] partSys)
    {
        if (partSys == null || partSys.Length == 0)
        {
            Debug.LogWarning("Particle System array is null or empty!");
            return;
        }

        try
        {
            var tasks = partSys
                .Where(sys => sys != null)
                .Select(sys => 
                {
                    sys.gameObject.SetActive(true);
                    return Task.CompletedTask;
                })
                .ToArray();

            await Task.WhenAll(tasks);
            Debug.Log("All movements completed!");
        }
        catch( Exception ex)
        {
            Debug.LogError($"VFX failed: {ex.Message}");
        }
    }

    private async Task DisableBadObject(GameObject badObj)
    {
        if (badObj == null || !badObj.activeInHierarchy)
        {
            Debug.LogWarning($"Bad object({badObj}) is null or already inactive");
            return;
        }

        try
        {
            badObj.gameObject.SetActive(false);
            await Task.CompletedTask;
            Debug.Log($"Disabled {badObj}");
        }
        catch(Exception ex)
        {
            Debug.LogError($"Disabled failed: {ex.Message}");
        }
    }

    private async Task EnabledGoodObject(GameObject goodObj)
    {
        if (goodObj == null || goodObj.activeInHierarchy)
        {
            Debug.LogWarning($"Good object({goodObj}) is null or already inactive");
            return;
        }

        try
        {
            goodObj.gameObject.SetActive(true);
            await Task.CompletedTask;
            Debug.Log($"Activated {goodObj}");

        }
        catch(Exception ex)
        {
            Debug.LogError($"Activated failed: {ex.Message}");
        }
    }

    private async Task ActivateMovableObject(MovableObject[] movObj)
    {
        if (movObj == null || movObj.Length == 0)
        {
            Debug.LogWarning("Movable objects array is null or empty!");
            return;
        }

        try
        {   
            var tasks = movObj
                .Where(obj => obj != null && obj.gameObject.activeInHierarchy)
                .Select(obj => obj.ExecuteFullSequenceAsync())
                .ToArray();

            await Task.WhenAll(tasks);
            Debug.Log("All movements completed!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Movement failed: {ex.Message}");
        }
    }
    
    // Вызывает метод на всех закешированных скриптах
    private async void ActivateAllMovableObject()
    {
        if (movableObjects == null || movableObjects.Length == 0)
        {
            Debug.LogWarning("No scripts found in cache in MovementManager. Updating cache...");
            UpdateScriptsCache(); // Автоматическое обновление, если кеш пуст
           
            if (movableObjects.Length == 0)
            {
                Debug.LogError("No movable objects found in scene!");
                return;
            }
        }

        isExecuting = true;

        var tasks = new List<Task>();
        foreach (var obj in movableObjects)
        {
            // Проверка на случай уничтоженных или неактивных объектов
            if (obj != null  && obj.gameObject.activeInHierarchy)  
                tasks.Add(obj.ExecuteFullSequenceAsync());
        }

        await Task.WhenAll(tasks);
        isExecuting = false;
        Debug.Log("All movements completed!");
    }

    // Метод для временной активации объекта
    public async Task ActivateTemporarily()
    {
        if (congratulationsPanel != null)
        {
            congratulationsPanel.SetActive(true); // Активируем объект
            StartCoroutine(DeactivateAfterDelay()); // Запускаем корутин для деактивации
        }
    }

    // Корутин для деактивации через заданное время
    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(2); // Ждём указанное время
        congratulationsPanel.SetActive(false); // Деактивируем объект
    }

    [ContextMenu("Force Update Cache")]
    public void ForceUpdateCache()
    {
        UpdateScriptsCache();
    }

    [ContextMenu("Update Object Lists")]
    public void ForceUpdateObjectLists()
    {
        PopulateObjectLists();
    }
}
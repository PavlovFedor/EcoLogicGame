using UnityEngine;

public class ObjectID : MonoBehaviour
{
    public int biomeId;
    public void ObjSelector()
    {
        SaveSystem.Instance.OnObjectSelected(biomeId);
    }
}

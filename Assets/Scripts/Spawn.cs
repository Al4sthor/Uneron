using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float xFull = 1777f;
    public ObjectSpawn[] objects;
    public float timeSpawn = 2f;
    private float contador;
    void Update()
    {
        contador += Time.deltaTime;
        if(contador >= timeSpawn)
        {
            SpawnObject();
            contador = 0;
        }

    }

    void SpawnObject()
    {
        int index = Random.Range(0, objects.Length);
        ObjectSpawn obj = objects[index];

        float yRandom = Random.Range(obj.yMin, obj.yMax);
        Vector2 posicion = new Vector2(xFull, yRandom);

        GameObject nuevo = Instantiate(obj.prefab, transform);

        RectTransform rect = nuevo.GetComponent<RectTransform>();
        rect.anchoredPosition = posicion;
    }

    [System.Serializable]
    public class ObjectSpawn
    {
        public GameObject prefab;
        public float yMin;
        public float yMax;
    }
}

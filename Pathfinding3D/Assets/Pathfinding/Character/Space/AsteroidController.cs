using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(AsteroidController))]
public class AsteroidControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Asteroids"))
        {
            AsteroidController ac = (AsteroidController)target;
            ac.Generate();
        }
    }
}
#endif

public class AsteroidController : MonoBehaviour
{

    public Vector3Int Bounds;
    [SerializeField] int MinCount;
    [SerializeField] int MaxCount;
    [SerializeField] float MinSize;
    [SerializeField] float MaxSize;
    [SerializeField] List<GameObject> _AsteroidPrefabs;
    public List<GameObject> Asteroids;

    public void Generate()
    {
        if (Asteroids == null)
        {
            Asteroids = new List<GameObject>();
        }
        foreach(var i in Asteroids)
        {
            DestroyImmediate(i);
        }
        Asteroids.Clear();
        int count = Random.Range(MinCount, MaxCount);
        for(int i = 0; i < count; i++)
        {
            int randIndex = Random.Range(0, _AsteroidPrefabs.Count);
            GameObject asteroid = Instantiate(_AsteroidPrefabs[randIndex], transform);
            Asteroids.Add(asteroid);
            asteroid.transform.localScale = Vector3.one * Random.Range(MinSize, MaxSize);
            asteroid.transform.rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
            float x = Random.Range(-Bounds.x, Bounds.x);
            float y = Random.Range(-Bounds.y, Bounds.y);
            float z = Random.Range(-Bounds.z, Bounds.z);
            asteroid.transform.position = transform.position + new Vector3(x,y,z) / 2f;
        }


        
    }
}

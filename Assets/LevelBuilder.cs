using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    // pick random prefab

    public List<Transform> spawnPoints;
    public GameObject defaultPrefab;

    // Start is called before the first frame update
    void Start()
    {
        SpawnCards();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnCards()
    {
        int totalSpawns = spawnPoints.Count;

        for (int i = 0; i < totalSpawns; i++)
        {
            GameObject prefabToSpawn;

            if (i < GameManager.Instance.recipeCards.Count)
            {
                prefabToSpawn = GameManager.Instance.recipeCards[i].prefabToSpawn;
            }
            else
            {
                prefabToSpawn = defaultPrefab;
            }

            GameObject obj = Instantiate(prefabToSpawn, spawnPoints[i].position, spawnPoints[i].rotation);

            // Fix Z position
            Vector3 fixedPos = obj.transform.position;
            fixedPos.z = 0f;
            obj.transform.position = fixedPos;
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to manage various meteor settings.
/// </summary>
[Serializable]
public class MeteorManager : MonoBehaviour
{
    /// <summary>
    ///
    /// </summary>
    public GameObject meteorPrefab;

    /// <summary>
    /// The spawn points for the meteors.
    /// </summary>
    public GameObject[] spawnPoints;

    /// <summary>
    /// A collection of the meteors that have been spawned.
    /// </summary>
    private List<GameObject> meteors = new List<GameObject>();

    /// <summary>
    /// A reference to the ship GameObject used to check if meteors should
    /// be despawned.
    /// </summary>
    private GameObject ship;

    /// <summary>
    /// When the MeteorManager starts we want to get our reference to the
    /// ship and start the coroutine that spawns meteors.
    /// </summary>
    void Start()
    {
        ship = GameObject.Find("Ship");
        StartCoroutine(SpawnMeteors());
    }

    /// <summary>
    /// Every frame we go through the meteors that have been spawned and
    /// check to see if they are past the ship and if so we can destory
    /// them and remove them from the meteors list.
    /// </summary>
    void Update()
    {
        for (int i = meteors.Count - 1; i >= 0; i--)
        {
            if (meteors[i].transform.position.z > ship.transform.position.z + 10.0)
            {
                Destroy(meteors[i]);
                meteors.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Creates an instance of a meteor every x seconds. The meteor will
    /// spawn at a point around the spawn point.
    /// </summary>
    private IEnumerator SpawnMeteors()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);

            int spawnPoint = UnityEngine.Random.Range(0, spawnPoints.Length);

            Vector3 position = spawnPoints[spawnPoint].transform.position;
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-5.0f, 5.0f), 0.0f);

            meteors.Add(Instantiate(meteorPrefab, position + offset, spawnPoints[spawnPoint].transform.rotation) as GameObject);
        }
    }
}

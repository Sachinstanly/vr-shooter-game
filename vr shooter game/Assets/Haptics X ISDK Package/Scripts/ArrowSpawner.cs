using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform spawnPoint;

    private GameObject spawnedArrow;

    //private void Start()
    //{
    //    SpawnArrow();
    //}

    private void SpawnArrow()
    {
        spawnedArrow = Instantiate(arrowPrefab, spawnPoint);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<ArrowInteraction>())
        {
            if (spawnedArrow)
            {
                spawnedArrow.transform.parent = null;
                spawnedArrow = null;
            }

            SpawnArrow();
        }
    }
}

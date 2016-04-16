using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> GroundChunkPrefabs;

    float _leftMostChunkPos = 0;

    void Awake()
    {

    }

    void Update()
    {
        if (CameraFollow.RightEdge +25 >= _leftMostChunkPos)
        {
            SpawnChunk(GroundChunkPrefabs[Random.Range(0, GroundChunkPrefabs.Count - 1)]);
        }
    }

    void SpawnChunk(GameObject chunkPrefab)
    {
        GameObject newChunk = ObjectPool.Pop(chunkPrefab);
        newChunk.SetActive(true);
        newChunk.transform.position = new Vector3(_leftMostChunkPos, -10);
        _leftMostChunkPos += 40;
    }
}
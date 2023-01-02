using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    
    public Transform [] playerSpawnPoints;
    private readonly List<int> takenSpawnPoints = new List<int>();
    void Start()
    {
        int spawnPointIndex = ChooseRandomSpawnPoint();
        Transform spawnPointPlayer = playerSpawnPoints[spawnPointIndex];
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPointPlayer.position, Quaternion.identity);
    }
    private int ChooseRandomSpawnPoint()
    {
        int randomPoint = Random.Range(0, playerSpawnPoints.Length);
        // If the chosen spawn point has already been taken, try again
        if (takenSpawnPoints.Contains(randomPoint))
        {
            return ChooseRandomSpawnPoint();
        }
        // Mark the chosen spawn point as taken
        takenSpawnPoints.Add(randomPoint);
        return randomPoint;
    }
    
}

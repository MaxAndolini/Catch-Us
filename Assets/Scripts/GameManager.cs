using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject imposterPanel;
    public Text imposterName;
    public Image imposterImage; 

    public Transform[] playerSpawnPoints;
    private readonly List<int> takenSpawnPoints = new List<int>();

    private void Start()
    {
        var spawnPointIndex = ChooseRandomSpawnPoint();
        var spawnPointPlayer = playerSpawnPoints[spawnPointIndex];
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPointPlayer.position, Quaternion.identity);


        if (PhotonNetwork.IsMasterClient)
        {
            GameObject imposter = AssignImposters();
            Debug.Log(imposter);
            if (imposter != null)
            {
                Debug.Log("GEL VATANDAŞ");
                Debug.Log(imposter.GetComponent<PlayerMovement>().playerName);
                Debug.Log(imposter.GetComponent<ColorChanger>().selectedColor);
                imposterName.text = imposter.GetComponent<PlayerMovement>().playerName;
                imposterImage.color = imposter.GetComponent<ColorChanger>().selectedColor;
            }

            imposterPanel.SetActive(true);
        }
    }

    private int ChooseRandomSpawnPoint()
    {
        var randomPoint = Random.Range(0, playerSpawnPoints.Length);
        // If the chosen spawn point has already been taken, try again
        if (takenSpawnPoints.Contains(randomPoint)) return ChooseRandomSpawnPoint();
        // Mark the chosen spawn point as taken
        takenSpawnPoints.Add(randomPoint);
        return randomPoint;
    }

    public GameObject AssignImposters()
    {
        var threshold = 1;

        if (PhotonNetwork.CurrentRoom.PlayerCount < threshold)
            return null;

        var pvs = FindObjectsOfType<PhotonView>();
        var plyPV = new List<PhotonView>();
        GameObject found = null;

        foreach (var p in pvs)
        {
            var pc = p.GetComponent<PlayerMovement>();
            if (pc)
            {
                if (pc.isImposter)
                {
                    found = pc.gameObject;
                    break;
                }
                
                p.RPC("RPC_NeutralizeImposter", RpcTarget.All, p.ViewID);
                plyPV.Add(p);
            }
        }

        if (found != null) return found;

        var rand = Random.Range(0, plyPV.Count);
        var pv = plyPV[rand];
        pv.RPC("RPC_MakeImposter", RpcTarget.All, pv.ViewID);
        plyPV.Remove(plyPV[rand]);
        return pv.gameObject;
    }
}
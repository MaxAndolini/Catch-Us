using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject imposterPanel;
    public Text imposterName;
    public Image imposterImage;
    public Text countDown;
    public GameObject winPanel;
    public Text winText;
    public Text time;
    public double timer = 10;

    public Transform[] playerSpawnPoints;
    private readonly List<int> takenSpawnPoints = new List<int>();
    private double _startTime;
    private bool _startTimer;
    private float _timeRemaining;
    private double _timerIncrementValue;
    private bool gameOver;

    private void Start()
    {
        var spawnPointIndex = ChooseRandomSpawnPoint();
        var spawnPointPlayer = playerSpawnPoints[spawnPointIndex];
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPointPlayer.position, Quaternion.identity);


        if (PhotonNetwork.IsMasterClient)
        {
            var imposter = AssignImposters();
            Debug.Log(imposter);
            if (imposter != null)
            {
                Debug.Log("GEL VATANDAŞ");
                Debug.Log(imposter.GetComponent<PlayerMovement>().playerName);
                Debug.Log(imposter.GetComponent<ColorChanger>().selectedColor);
                imposterName.text = imposter.GetComponent<PlayerMovement>().playerName;
                imposterImage.color = imposter.GetComponent<ColorChanger>().selectedColor;
            }

            //imposterPanel.SetActive(true);

            photonView.RPC("DeleteWaiting", RpcTarget.All, null);
        }
    }

    private void Update()
    {
        if (_startTimer && !gameOver)
        {
            _timerIncrementValue = PhotonNetwork.Time - _startTime;

            DisplayTime(timer - _timerIncrementValue);

            if (_timerIncrementValue >= timer || EveryoneDeadCheck())
            {
                _startTimer = false;
                gameOver = true;

                Win();
            }
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

    [PunRPC]
    public void DeleteWaiting()
    {
        // Find the player's PhotonView by its Photon ID
        var imposter = false;
        PlayerMovement imposterObject = null;
        if (photonView.IsMine)
        {
            imposterObject = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            imposter = imposterObject.isImposter;
            Debug.Log(GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().isImposter);
        }

        imposterPanel.SetActive(true);
        if (imposter)
        {
            imposterObject.photonView.RPC("Freeze", RpcTarget.All, true);
            StartCoroutine(ImposterCountdownCoroutine());
            imposterObject.photonView.RPC("Freeze", RpcTarget.All, false);
        }
        else
        {
            StartCoroutine(CrewmateCountdownCoroutine());
        }
    }

    private IEnumerator ImposterCountdownCoroutine()
    {
        countDown.text = "5";
        yield return new WaitForSeconds(1.0f);
        countDown.text = "4";
        yield return new WaitForSeconds(1.0f);
        countDown.text = "3";
        yield return new WaitForSeconds(1.0f);
        countDown.text = "2";
        yield return new WaitForSeconds(1.0f);
        countDown.text = "1";
        yield return new WaitForSeconds(1.0f);
        countDown.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        imposterPanel.SetActive(false);

        _startTime = PhotonNetwork.Time;
        _startTimer = true;

        yield return null;
    }

    private IEnumerator CrewmateCountdownCoroutine()
    {
        countDown.text = "2";
        yield return new WaitForSeconds(1.0f);
        countDown.text = "1";
        yield return new WaitForSeconds(1.0f);
        countDown.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        imposterPanel.SetActive(false);

        yield return null;
    }

    private void DisplayTime(double timeToDisplay)
    {
        var t = TimeSpan.FromSeconds(timeToDisplay);
        time.text = $"{t.Minutes:D2}:{t.Seconds:D2}";
    }

    public void Win()
    {
        var winner = "Crewmates";
        if (EveryoneDeadCheck()) winner = "Imposter";
        winner += " Win!";
        winPanel.SetActive(true);
        winText.text = winner;

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public bool EveryoneDeadCheck()
    {
        var pvs = FindObjectsOfType<PhotonView>();
        var total = 0;
        var dead = 0;

        foreach (var p in pvs)
        {
            var pc = p.GetComponent<PlayerMovement>();
            if (pc)
            {
                if (!pc.isImposter && pc.isDead) dead++;

                total++;
            }
        }

        return total - 1 == dead;
    }
}
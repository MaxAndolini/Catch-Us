using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public float speed = 3f;
    public float smoothTime = 0.3f; // The smoothing factor for the camera's movement
    public Vector2 minPos, maxPos; // The minimum and maximum positions that the camera can have
    public SpriteRenderer playerSpriteRenderer;
    public SpriteRenderer playerOutlineSpriteRenderer;
    public string playerName;
    public bool isImposter;
    public bool isDead;

    private Animator animator;
    private Camera mainCamera; // A reference to the main camera
    private Tilemap tilemap; // A reference to the Tilemap object
    private List<PlayerMovement> targets = new List<PlayerMovement>();
    public GameObject deadBodyPrefab;
    public bool freeze;

    private void Start()
    {
        playerName = PlayerPrefs.GetString("Username");
        Debug.Log(playerName);
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        // Find the Tilemap object in the scene
        tilemap = GameObject.Find("Tilemap_Walls").GetComponent<Tilemap>();

        // Get the bounding rectangle of the tilemap in grid coordinates
        var cellBounds = tilemap.cellBounds;

        // Convert the bounding rectangle to world coordinates
        var minCellPos = tilemap.CellToWorld(cellBounds.min);
        var maxCellPos = tilemap.CellToWorld(cellBounds.max);

        // Get the size of the camera's viewport in world units
        var cameraViewportWidth = mainCamera.orthographicSize * mainCamera.aspect;
        var cameraViewportHeight = mainCamera.orthographicSize;

        // Calculate the minimum and maximum positions
        minPos = new Vector2(minCellPos.x + cameraViewportWidth / 2, minCellPos.y + cameraViewportHeight / 2);
        maxPos = new Vector2(maxCellPos.x - cameraViewportWidth / 2, maxCellPos.y - cameraViewportHeight / 2);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        if (freeze) return;
        
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            animator.Play("Walk");
            photonView.RPC("SetAnimation", RpcTarget.Others, "Walk");
            
            if (horizontal > 0 || horizontal < 0)
            {
                playerSpriteRenderer.flipX = horizontal < 0;
                playerOutlineSpriteRenderer.flipX = horizontal < 0;
                photonView.RPC("FlipSprite", RpcTarget.Others, horizontal < 0);
            }

            transform.position += new Vector3(horizontal, vertical).normalized * (speed * Time.deltaTime);
            photonView.RPC("Move", RpcTarget.Others, transform.position);

            // Follow the character with the camera
            var targetPos = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, smoothTime);
        }
        else
        {
            animator.Play("Idle");
            photonView.RPC("SetAnimation", RpcTarget.Others, "Idle");
        }

        // Clamp the camera's position to the minimum and maximum positions
        mainCamera.transform.position = new Vector3(
            Mathf.Clamp(mainCamera.transform.position.x, minPos.x, maxPos.x),
            Mathf.Clamp(mainCamera.transform.position.y, minPos.y, maxPos.y),
            mainCamera.transform.position.z);
        
        if (!isImposter)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (targets.Count == 0)
                return;
            else
            {
                if (targets[targets.Count - 1].isDead)
                    return;
                transform.position = targets[targets.Count - 1].transform.position;
                //targets[targets.Count - 1].Die();
                targets[targets.Count - 1].photonView.RPC("RPC_Kill", RpcTarget.All);
                targets.RemoveAt(targets.Count - 1);
            }
        }
    }

    [PunRPC]
    private void Move(Vector3 position)
    {
        transform.position = position;
    }

    [PunRPC]
    private void FlipSprite(bool flipX)
    {
        playerSpriteRenderer.flipX = flipX;
        playerOutlineSpriteRenderer.flipX = flipX;
    }

    [PunRPC]
    private void SetAnimation(string animationName)
    {
        if(animator != null) animator.Play(animationName);
    }

    [PunRPC]
    private void RPC_NeutralizeImposter(int viewID)
    {
        PhotonViewByID(viewID).GetComponent<PlayerMovement>().isImposter = false;
        Debug.Log("Crewmate");
    }

    [PunRPC]
    private void RPC_MakeImposter(int viewID)
    {
        PhotonViewByID(viewID).GetComponent<PlayerMovement>().isImposter = true;
        Debug.Log("Imposter");
    }

    public PhotonView PhotonViewByID(int viewID)
    {
        var pvs = FindObjectsOfType<PhotonView>();
        foreach (var pview in pvs)
            if (pview.ViewID == viewID)
                return pview;
        return null;
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerMovement tempTarget = col.GetComponent<PlayerMovement>();
            if (isImposter)
            {
                if (tempTarget.isImposter)
                    return;
                else
                {
                    targets.Add(tempTarget);
                    
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerMovement tempTarget = col.GetComponent<PlayerMovement>();
            if (targets.Contains(tempTarget))
            {
                targets.Remove(tempTarget);
            }
        }
    }
    
    [PunRPC]
    void RPC_Kill()
    {
        Die();
    }
    
    public void Die()
    {
        if (!photonView.IsMine)
            return;
        
        //AU_Body tempBody = Instantiate(bodyPrefab, transform.position, transform.rotation).GetComponent<AU_Body>();
        Dead tempBody = PhotonNetwork.Instantiate(deadBodyPrefab.name, transform.position, transform.rotation).GetComponent<Dead>();
        tempBody.SetColor(gameObject.GetComponent<ColorChanger>().selectedColor);
        PhotonNetwork.Destroy(transform.gameObject);
        isDead = true;
    }

    [PunRPC]
    public void Freeze(bool condition)
    {
        freeze = condition;
        Debug.Log(condition);
    }
}
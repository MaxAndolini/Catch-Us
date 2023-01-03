using Photon.Pun;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public float speed = 3f;
    public float smoothTime = 0.3f; // The smoothing factor for the camera's movement
    public Vector2 minPos, maxPos; // The minimum and maximum positions that the camera can have
    public Tilemap tilemap; // A reference to the Tilemap object

    private Animator animator;
    public SpriteRenderer playerSpriteRenderer;
    public SpriteRenderer playerOutlineSpriteRenderer;
    private Camera mainCamera; // A reference to the main camera

    private void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        // Find the Tilemap object in the scene
        tilemap = GameObject.Find("Tilemap_Walls").GetComponent<Tilemap>();

        // Get the bounding rectangle of the tilemap in grid coordinates
        BoundsInt cellBounds = tilemap.cellBounds;

        // Convert the bounding rectangle to world coordinates
        Vector3 minCellPos = tilemap.CellToWorld(cellBounds.min);
        Vector3 maxCellPos = tilemap.CellToWorld(cellBounds.max);

        // Get the size of the camera's viewport in world units
        float cameraViewportWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraViewportHeight = mainCamera.orthographicSize;

        // Calculate the minimum and maximum positions
        minPos = new Vector2(minCellPos.x + cameraViewportWidth / 2, minCellPos.y + cameraViewportHeight / 2);
        maxPos = new Vector2(maxCellPos.x - cameraViewportWidth / 2, maxCellPos.y - cameraViewportHeight / 2);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            animator.Play("Walk");
            photonView.RPC("SetAnimation", RpcTarget.Others, "Walk");

            // Eğer y eksenine göre hareket ediliyorsa, sprite'ı y eksenine göre aynala
            if (horizontal > 0 || horizontal < 0)
            {
                playerSpriteRenderer.flipX = horizontal < 0;
                playerOutlineSpriteRenderer.flipX = horizontal < 0;
                photonView.RPC("FlipSprite", RpcTarget.Others, horizontal < 0);
            }

            transform.position += new Vector3(horizontal, vertical).normalized * (speed * Time.deltaTime);
            photonView.RPC("Move", RpcTarget.Others, transform.position);
            
            // Follow the character with the camera
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
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
        animator.Play(animationName);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public float speed = 3f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
            
        if (horizontal != 0 || vertical != 0)
        {
            animator.Play("Walk");
            photonView.RPC("SetAnimation", RpcTarget.Others,  "Walk");
            
            // Eğer y eksenine göre hareket ediliyorsa, sprite'ı y eksenine göre aynala
            if (horizontal > 0 || horizontal < 0)
            {
                spriteRenderer.flipX = horizontal < 0;
                photonView.RPC("FlipSprite", RpcTarget.Others, horizontal < 0);
                
            }
            transform.position += new Vector3(horizontal, vertical).normalized * (speed * Time.deltaTime);
            photonView.RPC("Move", RpcTarget.Others, transform.position);
        }
        else
        {
            animator.Play("Idle");
            photonView.RPC("SetAnimation", RpcTarget.Others, "Idle");
        }
    }
    
        [PunRPC]
        void Move(Vector3 position)
        {
            transform.position = position;
        }
        
        [PunRPC]
        void FlipSprite(bool flipX)
        {
            spriteRenderer.flipX = flipX;
            
        }
        
        [PunRPC]
        void SetAnimation(string animationName)
        {
            animator.Play(animationName);
        }
}


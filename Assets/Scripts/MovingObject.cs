﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;
using Photon.Pun;

public abstract class MovingObject : MonoBehaviour
{
    private float moveTime = 0.15f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    private bool isMoving;

    private AudioSource audioSource;

    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        isMoving = false;
        audioSource = gameObject.GetComponent<AudioSource> ();
    }

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit, AudioClip sfx)
    {
       
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        //Debug.Log(start);
        //Debug.Log(end);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
            
        if (hit.transform == null)
        {
            if (!isMoving)
            {
                isMoving = true;
                // Debug.Log("isMoving");
                //transform.position = end;
                //return true;
                audioSource.clip = sfx;
                audioSource.Play ();

                StartCoroutine(SmoothMovement(end));
                return true;
            }
            else{
                Debug.Log("cant move");
            }


            return false;
        }
        // Debug.Log("----");
        return false;
    


    }

    protected virtual void AttemptMove (int xDir, int yDir, AudioClip sfx)
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit, sfx);

        if (hit.transform == null)
        {
            return;
        }

        //T hitComponent = hit.transform.GetComponent<T>();

        //if (!canMove && hitComponent != null)
        //    OnCantMove(hitComponent);
    }
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = end;
        
        
        // float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // while (sqrRemainingDistance > float.Epsilon)
        // {
        //     Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
        //     rb2D.MovePosition(newPosition);
        //     sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        //     yield return null;
        // }
        isMoving = false;
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component; 

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15;
    private AudioSource playerAudio;

    public ParticleSystem fireParticle;
    public AudioClip swipeSound;
    private bool isTravelling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPositon;
    public int minSwipeRecognition = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 swipePosLastSwipe;
    private Vector2 currentSwipe;
    private Color solveColor;
    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;
        playerAudio = GetComponent<AudioSource>();
    }


    private void FixedUpdate()
    {
        if (isTravelling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.1f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }


        if (nextCollisionPositon != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, nextCollisionPositon) < 1)
            {
                isTravelling = false;
                travelDirection = Vector3.zero;
                nextCollisionPositon = Vector3.zero;
                fireParticle.Stop();
            }
        }
        if (isTravelling)
            return;

        if (Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if (currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();

                //up/Down
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }
            swipePosLastFrame = swipePosCurrentFrame;
            playerAudio.PlayOneShot(swipeSound, 1.0f);
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPositon = hit.point;
        }
        isTravelling = true;
        fireParticle.Play();
    }

}

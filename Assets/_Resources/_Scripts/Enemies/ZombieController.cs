using System;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float rotationSpeed;

    private Rigidbody2D rigidBody;
    private PlayerDetector playerDetector;
    private Vector2 targetDirection;
    bool dead = true;

    private float changeDirectionTimer;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerDetector = GetComponent<PlayerDetector>();
        targetDirection = transform.up;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead)
        {
            return;
        }

        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }
    private void UpdateTargetDirection()
    {
        ChangeToRandomDir();
        HandlePlayerLocation();

    }

    private void RotateTowardsTarget()
    {

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        rigidBody.SetRotation(rotation);
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        SoundPlayer player = other.gameObject.GetComponent<SoundPlayer>();
        SpinTrap spinTrap = other.gameObject.GetComponent<SpinTrap>();
        if (player != null)
        {
            //player.ChangeHealth(-1);
        }
        if (spinTrap != null)
        {
            Dead();
        }
    }

    private void ChangeToRandomDir()
    {
        changeDirectionTimer -= Time.deltaTime;
        if (changeDirectionTimer <= 0)
        {
            //float angle = Random.Range(-90f, 90f);
            //Quaternion quaternionRotate = Quaternion.AngleAxis(angle, transform.forward);
            //targetDirection = quaternionRotate * targetDirection;

           // changeDirectionTimer = Random.Range(1f, 5f);
        }
    }

    private void HandlePlayerLocation()
    {
        if (playerDetector.AwareOfPlayer)
        {
            targetDirection = playerDetector.DirectionToPlayer;
        }
    }

    private void SetVelocity()
    {
        rigidBody.velocity = transform.up * speed;

    }

    public void Dead()
    {
        dead = false;
        rigidBody.simulated = false;
        //optional if you added the fixed animation
        Destroy(gameObject);
    }
}
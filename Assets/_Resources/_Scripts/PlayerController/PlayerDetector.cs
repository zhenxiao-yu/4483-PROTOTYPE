using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{

    public bool AwareOfPlayer { get; private set; }

    public Vector2 DirectionToPlayer { get; private set; }

    private Transform playerTransform;

    [SerializeField]
    private float rubyAwarenessDistance;

    private void Awake()
    {
        playerTransform = FindAnyObjectByType<PlayerDetector>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 enemy2RubyPosition = playerTransform.position - transform.position;
        DirectionToPlayer = enemy2RubyPosition.normalized;

        if (enemy2RubyPosition.magnitude < rubyAwarenessDistance)
        {
            AwareOfPlayer = true;
        }
        else
        {
            AwareOfPlayer = false;
        }
    }
}
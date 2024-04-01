using System.Collections;
using System.Collections.Generic;
using System.Media;
using UnityEngine;

public class SpinTrap : MonoBehaviour
{
    public float rotationSpeed;
    private int damageValue = 1;
    public bool clockwise;

    void Update()
    {
        //rotate along Z axis
        if (clockwise == true)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        SoundPlayer player = other.gameObject.GetComponent<SoundPlayer>();

        if (player != null)
        {
           // player.ChangeHealth(damageValue);
        }
    }
}
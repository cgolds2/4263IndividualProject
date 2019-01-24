using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleScript : MonoBehaviour {

    public float smoothing = 1f;
    public Transform target;


    void Start()
    {
     
        PlayPlacement();
    }

    public void PlayPlacement()
    {
        GetComponent<AudioSource>().Play();
    }

    float timer = 2f;
    bool isRotating = false;
    bool rotLeft;
    Vector3 point;
    private void Update()
    {
        {



            if (timer <= 1)
            {
                // Time.deltaTime*100 will make sure we are moving at a constant speed of 100 per second
                transform.RotateAround(point, rotLeft ? Vector3.down : Vector3.up, 90 * Time.deltaTime);


                //transform.Rotate(0f, 0f, Time.deltaTime * 90);
                // Increment the timer so we know when to stop
                timer += Time.deltaTime;
            }
            else if (isRotating)
            {
                //transform.rotation = new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f);
                isRotating = false;
            }
        }
    }
    public void Rotate(bool rotLeft, Vector3 position)
    {
        point = position;
        timer = 0;
        isRotating = true;
        this.rotLeft = rotLeft;
  
    }
}



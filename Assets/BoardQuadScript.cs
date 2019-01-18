using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardQuadScript : MonoBehaviour
{
    public float smoothing = 1f;
    public Transform target;
    public List<GameObject> marbles = new List<GameObject>();

    void Start()
    {
    }

    float timer = 2f;
    bool isRotating = false;
    bool rotLeft;
    private void Update()
    {
        {



            if (timer <= 1)
            {
                // Time.deltaTime*100 will make sure we are moving at a constant speed of 100 per second
                transform.RotateAround(this.transform.position, rotLeft?Vector3.down:Vector3.up,  90 * Time.deltaTime);
             

                //transform.Rotate(0f, 0f, Time.deltaTime * 90);
                // Increment the timer so we know when to stop
                timer += Time.deltaTime;
            }
            else if(isRotating)
            {
                transform.rotation = new Quaternion(-0.5f,0.5f,0.5f,0.5f);
                isRotating = false;
                //maybe change game state here?
                GameMindScript.AdvanceGameState();
            }
        }
    }
    public void Rotate(bool rotLeft)
    {
        timer = 0;
        isRotating = true;
        this.rotLeft = rotLeft;
        foreach (var item in marbles)
        {
            var ScriptThatYouWant = item.GetComponent<MarbleScript>();
            ScriptThatYouWant.Rotate(rotLeft, this.transform.position); 
            item.transform.RotateAround(this.transform.position, rotLeft ? Vector3.down : Vector3.up, 90 * Time.deltaTime);


        }
    }
}



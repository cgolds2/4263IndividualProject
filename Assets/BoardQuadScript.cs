using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardQuadScript : MonoBehaviour {
    public float smoothing = 1f;
    public Transform target;


    void Start()
    {
        target = this.transform;

        float x = target.position.x;
        x += 5;

        var x2 = Vector3.Distance(transform.position, target.position);
        StartCoroutine(MyCoroutine(target));
    }


    IEnumerator MyCoroutine(Transform target)
    {
        var x = Vector3.Distance(transform.position, target.position);
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, smoothing * Time.deltaTime);

            yield return null;
        }

        print("Reached the target.");

        yield return new WaitForSeconds(3f);

        print("MyCoroutine is now finished.");
    }

    //IEnumerator moveCoroutine()
    //{
    //    int maxIterations = 10;
    //    if (!isCollided && canRotate)
    //    {
    //        for (float i = 0; i < maxIterations; i++)
    //        {
    //            transform.RotateAround(pivot, direction, 9.0f * Time.deltaTime);
    //            yield return new WaitForSeconds(Metronome.manager.delay / maxIterations);
    //        }
    //    }
    //}
}



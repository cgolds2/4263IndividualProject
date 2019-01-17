using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnMouseEnter()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse entered " + this.name);
        var cursorObj = GameObject.Find("CursorObject");
        cursorObj.transform.position = this.transform.position;
        cursorObj.transform.AddPos(y: 1);

    }
    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        //Debug.Log("Mouse is over " + this.name);
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on " + this.name);
    }

    void OnMouseDown()
    {
        // load a new scene
        Debug.Log("Mouse clicked " + this.name);
        var marble = GameObject.Find("Marble");
        GameObject newBox = Instantiate(marble);
        newBox.transform.position = this.transform.position;
        newBox.transform.AddPos(y: 1);
    }



}


public static class Helper
{
    public static void SetPos(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 position = transform.position;

        if (x.HasValue) position.x = x.Value;
        if (y.HasValue) position.y = y.Value;
        if (z.HasValue) position.z = z.Value;
        transform.position = position;
    }

    public static void AddPos(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 position = transform.position;

        if (x.HasValue) position.x += x.Value;
        if (y.HasValue) position.y += y.Value;
        if (z.HasValue) position.z += z.Value;
        transform.position = position;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour {
    public static Camera main;
    public static Camera options;
    public static Camera credits;
    // Use this for initialization
    void Start()
    {
      main = GameObject.Find("mainCamera").GetComponent<Camera>();
    options = GameObject.Find("optionsCamera").GetComponent<Camera>();
   credits = GameObject.Find("creditsCamera").GetComponent<Camera>();
    main.enabled = true;
        options.enabled = false;
        credits.enabled = false;
    }

  

    // Update is called once per frame
    void Update()
    {

    }
}

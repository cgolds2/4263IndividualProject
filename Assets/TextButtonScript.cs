using System.IO;
using UnityEditor;
using UnityEngine;

public class TextButtonScript : MonoBehaviour
{

    void Start()
    {

    }

  

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        GetComponent<Renderer>().enabled = true;



    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().enabled = false;



    }

    void OnMouseDown()
    {
        

        switch (name)
        {
            case "2Player":
                GameMindScript.SetGameType(GameMindScript.GameType.TwoPlayer);
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
                break;
            case "Heu":
                GameMindScript.SetGameType(GameMindScript.GameType.UseHeu);
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

                break;
            case "NN":
                if(PlayerPrefs.HasKey("file") && PlayerPrefs.HasKey("dir")){
                    GameMindScript.SetGameType(GameMindScript.GameType.UseAI);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
                }
                else{
                    MenuScript.main.enabled = false;
                    MenuScript.options.enabled = true;
                    MenuScript.credits.enabled = false;
                    MenuScript.rules.enabled = false;
                }
    
                break;
            case "Options":
                MenuScript.main.enabled = false;
                MenuScript.options.enabled = true;
                MenuScript.credits.enabled = false;
                MenuScript.rules.enabled = false;

                break;
            case "Credits":
                MenuScript.main.enabled = false;
                MenuScript.options.enabled = false;
                MenuScript.credits.enabled = true;
                MenuScript.rules.enabled = false;

                break;
            case "Back":
                MenuScript.main.enabled = true;
                MenuScript.options.enabled = false;
                MenuScript.credits.enabled = false;
                MenuScript.rules.enabled = false;

                break;
            case "Rules":
                MenuScript.main.enabled = false;
                MenuScript.options.enabled = false;
                MenuScript.credits.enabled = false;
                MenuScript.rules.enabled = true;
                break;
            case "File":
                string path = EditorUtility.OpenFilePanel("Select Python File", "", "py");
                if (path.Length != 0)
                {
                    //TODO: set this to stuff
                    bool fileContent = File.Exists(path);
                    var dir = Path.GetDirectoryName(path);
                    var file = Path.GetFileName(path);


                    PlayerPrefs.SetString("dir", dir);
                    PlayerPrefs.SetString("file", file);


                }
                break;
            default:
                break;
        }

    }




}

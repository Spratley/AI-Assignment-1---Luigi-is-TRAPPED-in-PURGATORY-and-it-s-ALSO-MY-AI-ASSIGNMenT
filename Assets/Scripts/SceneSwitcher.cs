using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Non-static version of the function is here so that UI buttons can still call it (as they need an object)
    public void SwitchScene(int index)
    {
        SwitchSceneStatic(index);
    }
    
    // Static version of the function is included so an instance of the object is not required to switch scenes
    public static void SwitchSceneStatic(int index)
    {
        SceneManager.LoadScene(index);
    }
}
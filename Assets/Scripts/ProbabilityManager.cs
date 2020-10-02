using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum DoorProperties
{
    Hot,
    Noisy,
    Safe,
    Count
};

// Struct that holds data for door properties
[System.Serializable]
public struct DoorData
{
    public DoorData(bool hot, bool noisy, bool safe)
    {
        isHot = hot;
        isNoisy = noisy;
        isSafe = safe;
    }

    public bool isHot;
    public bool isNoisy;
    public bool isSafe;
}

// Struct that holds the probability for a specific door type to appear
[System.Serializable]
public struct Door
{
    public DoorData door;

    public float probability;

    public bool this[DoorProperties index]
    {
        get
        {
            switch(index)
            {
                case DoorProperties.Hot:
                    return door.isHot;
                case DoorProperties.Noisy:
                    return door.isNoisy;
                case DoorProperties.Safe:
                    return door.isSafe;
                default:
                    Debug.LogError("Attempting to access DoorData structure at a non-enumerated value. Please don't :(");
                    return false;
            }
        }

        set
        {
            switch (index)
            {
                case DoorProperties.Hot:
                    door.isHot = value;
                    break;
                case DoorProperties.Noisy:
                    door.isNoisy = value;
                    break;
                case DoorProperties.Safe:
                    door.isSafe = value;
                    break;
                default:
                    Debug.LogError("Attempting to access DoorData structure at a non-enumerated value. Please don't :(");
                    return;
            }
        }
    }
}

public class ProbabilityManager : MonoBehaviour
{
    public static ProbabilityManager instance;
    public List<Door> probabilityTable;

    public InputField inputField;
    public GameObject errorMessage;

    private void Awake()
    {
        // Singleton stuff
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);

        probabilityTable = new List<Door>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        GrabReferences();
    }

    // This has to be done because the Probability manager loses references to the input field and error message when the scene switches
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GrabReferences();
    }

    // As said above this just grabs the references to the input field and error message
    private void GrabReferences()
    {
        //Note: Don't do this :( It's bad lol
        inputField = FindObjectOfType<InputField>();
        errorMessage = GameObject.FindWithTag("ErrorMessage");

        if (errorMessage != null)
        {
            errorMessage.SetActive(false);
        }
    }

    // Only loads game scene if the file path is valid
    public void LoadDataAndLaunchGame()
    {
        if(LoadTextFromInputField())
        {
            SceneSwitcher.SwitchSceneStatic(1);
        }
    }

    // Self explanitory :\
    public bool LoadTextFromInputField()
    {
        return LoadTextAsset(inputField.text);
    }

    // Checks if the file exists at a given path, then reads and interprets it
    private bool LoadTextAsset(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            errorMessage.SetActive(true);
            Debug.LogError("File not found at path " + path + "!");
            return false;
        }

        string text = System.IO.File.ReadAllText(path);

        if (text != "")
        {
            if (!TranslateProbabilityText(text))
            {
                Debug.LogError("TextAsset not found at provided path");
                return false;
            }
        }
        
        return true;
    }

    // This funciton splits the string that was read at new lines, then removes all whitespace from each given string and interprets the contents as three bools followed by a float
    private bool TranslateProbabilityText(string text)
    {
        /********************************************************************************************************************************************
         * The format is expected to be:                                                                                                            *
         * Hot   Noisy Safe Door  Percentage of Doors                                                                                               *
         * Y     Y     Y     0.05                                                                                                                   *
         * Y     Y     N     0.30                                                                                                                   *
         * Y     N     Y     0.03                                                                                                                   *
         * Y     N     N     0.21                                                                                                                   *
         * N     Y     Y     0.06                                                                                                                   *
         * N     Y     N     0.11                                                                                                                   *
         * N     N     Y     0.20                                                                                                                   *
         * N     N     N     0.04                                                                                                                   *
         * --NOTE: This example is the provided one on canvas. Tabs were sadly converted to spaces--                                                *
         *                                                                                                                                          *
         * Since the first row of the file is expected to be text for a human to read and help understand, it will be ignored.                      *
         * The order of these data sets are not expected to be changed (Hot, Noisy, Safe, Percentage, in that order).                               *
         * These are technical limitations of this implementation and would not be present in a commercial or professional version of this program. *
         ********************************************************************************************************************************************/

        List<string> probabilityDataStrings = new List<string>(text.Split('\n'));
        probabilityDataStrings.RemoveAt(0); // Trash the header line, the computer does not need it

        foreach (var item in probabilityDataStrings)
        {
            string trimmed = item.Replace(" ", "");
            trimmed = trimmed.Replace("\t", "");

            Door newDoor = new Door();

            for (int i = 0; i < (int)DoorProperties.Count; i++)
            {
                newDoor[(DoorProperties)i] = char.ToLower(trimmed[i]) == 'y';
            }

            string probability = trimmed.Substring(3);
            newDoor.probability = float.Parse(probability);

            //Debug.Log("Hot: " + newDoor.isHot + " Noisy: " + newDoor.isNoisy + " Safe: " + newDoor.isSafe + " Probability: " + newDoor.probability);
            probabilityTable.Add(newDoor);
        }

        return true;
    }

    // Since probability is in percentage, we can incrementally step through our probability table to find what properties our new random door should have
    // This implementation can be compared to walking through a pie chart, finding which slice of the pie the given probability lies within.
    // Note, the order of probabilities in the table does not affect the overall outcome of this function, just which slice is chosen for any specific number
    // Please ask me (Jacob Pratley) if you have any questions about why the code works because it's not a lucky guess, it's just not the mathematician's answer to the problem.
    public Door GetDoorByProbability(float probability)
    {
        float sum = 0;
        for (int i = 0; i < probabilityTable.Count; i++)
        {
            if(probability <= probabilityTable[i].probability + sum)
            {
                return probabilityTable[i];
            }
            sum += probabilityTable[i].probability;
        }

        Debug.LogError("Door not found at probability " + probability + ", did you enter a number higher than 1?");
        return probabilityTable[0];
    }
}

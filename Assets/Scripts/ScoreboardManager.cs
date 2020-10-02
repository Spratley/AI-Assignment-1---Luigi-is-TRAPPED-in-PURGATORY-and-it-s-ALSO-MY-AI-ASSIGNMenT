using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{
    public static ScoreboardManager instance;

    private Dictionary<DoorData, int> openedDoors;
    private int totalDoorsOpened = 0;

    public GameObject uiScoreboardItemPrefab;
    public GameObject uiScoreboardItemParent;

    public GameObject scoreboardRoot;

    private List<GameObject> scoreboardObjects;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        scoreboardObjects = new List<GameObject>();
        openedDoors = new Dictionary<DoorData, int>();

        Init();
    }

    // Hide or display scoreboard on tab
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboardRoot.SetActive(true);
        }

        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboardRoot.SetActive(false);
        }
    }

    // Encapsulated method for instantiating new UI scoreboard items
    private GameObject AddScoreboardItem()
    {
        GameObject item = Instantiate(uiScoreboardItemPrefab);
        item.transform.SetParent(uiScoreboardItemParent.transform);
        return item;
    }

    // Creates all required scoreboard items for the game and fills them with their proper contents
    private void Init()
    {
        scoreboardObjects.Add(AddScoreboardItem());
        SetContents(scoreboardObjects[0], "Hot", "Noisy", "Safe", "Number Opened", "Percent Encountered");

        for (int h = 0; h < 2; h++)
        {
            for (int n = 0; n < 2; n++)
            {
                for (int s = 0; s < 2; s++)
                {
                    openedDoors.Add(new DoorData(h == 0, n == 0, s == 0), 0);
                    scoreboardObjects.Add(AddScoreboardItem());
                }
            }
        }

        scoreboardObjects.Add(AddScoreboardItem());
        SetContents(scoreboardObjects[scoreboardObjects.Count - 1], "Total");

        UpdateScoreboard();

        scoreboardRoot.SetActive(false);
    }

    // Loops through and assigns proper values to all text boxes in scoreboard item
    public void SetContents(GameObject scoreboardItem, params string[] values)
    {
        var texts = scoreboardItem.GetComponentsInChildren<Text>();

        // Loop for the shorter one so we don't go out of bounds
        for (int i = 0; i < (values.Length < texts.Length ? values.Length : texts.Length); i++)
        {
            texts[i].text = values[i];
        }
    }

    // Adds the door you just opened to the scoreboard's tally
    // This is used to calculate the percentage of doors encountered so you don't have to do as much work
    public void AddOpenedDoor(DoorData data)
    {
        openedDoors[data]++;
        totalDoorsOpened++;
        UpdateScoreboard();
    }

    // Refreshes the text that is subject to change on the scoreboard
    private void UpdateScoreboard()
    {
        // I'd prefer if I only updated the score elements, but it is what it is
        int index = 1;
        foreach (var item in openedDoors)
        {
            if(scoreboardObjects.Count <= index)
            {
                // Prevents accessing object not in array
                break;
            }
            SetContents(scoreboardObjects[index], item.Key.isHot.ToString(), item.Key.isNoisy.ToString(), item.Key.isSafe.ToString(), item.Value.ToString(), ((float)item.Value / (float)totalDoorsOpened).ToString());
            index++;
        }


        SetContents(scoreboardObjects[scoreboardObjects.Count - 1], "Total", "", "", totalDoorsOpened.ToString());
    }
}

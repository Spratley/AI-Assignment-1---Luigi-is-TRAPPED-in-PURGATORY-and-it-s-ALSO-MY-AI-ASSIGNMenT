using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TieButtonToFunction : MonoBehaviour
{
    Button button;

    // I haven't worked with buttons in Unity very much and
    // sadly this is the only way I could think to fix an issue
    // The only reason this made it into my final build is because
    // of the lack of time I have on my hands
    private void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(delegate { ProbabilityManager.instance.LoadDataAndLaunchGame(); });
    }
}

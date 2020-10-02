using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class DoorComponent : MonoBehaviour
{
    public DoorData data;
    public Ghost ghost;
    private Animator animator;
    private AudioSource creekSound;
    private bool isOpen = false;

    public GameObject soundParticles;
    public GameObject heatParticles;

    private void Start()
    {
        animator = GetComponent<Animator>();
        creekSound = GetComponent<AudioSource>();

        ResetDoor();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Open();
        }
    }

    public void Open()
    {
        if (isOpen)
        {
            return;
        }

        // Animate the door opening
        isOpen = true;
        animator.SetBool("Open", isOpen);
        creekSound.Play();

        // Sp00k the player if the door is not safe
        if (!data.isSafe)
        {
            ghost.Jumpscare();
        }

        // Add this door to the tally
        ScoreboardManager.instance.AddOpenedDoor(data);
    }

    // Creates dummy door data in the same way opening a door would
    // This can be used to populate the data in the scoreboard to see if the program is working
    public void SimulateDoor()
    {
        data = ProbabilityManager.instance.GetDoorByProbability(Random.value).door;
        ScoreboardManager.instance.AddOpenedDoor(data);
    }

    // Regenerate all properties and close the door again
    public void ResetDoor()
    {
        isOpen = false;
        animator.SetBool("Open", isOpen);
        animator.Play("Closed");

        ghost.ResetGhost();

        data = ProbabilityManager.instance.GetDoorByProbability(Random.value).door;

        soundParticles.SetActive(data.isNoisy);
        heatParticles.SetActive(data.isHot);
    }
}

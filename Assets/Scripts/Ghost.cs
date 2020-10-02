using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private Animator animator;
    public Material ghostMaterial;

    public List<Texture2D> ghostTextures;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Pick a random color and animate
    public void Jumpscare()
    {
        ghostMaterial.SetTexture("_EmissionMap", ghostTextures[Random.Range(0, ghostTextures.Count)]);
        animator.Play("Jumpscare");
    }

    public void ResetGhost()
    {
        animator.Play("Idle");
    }
}

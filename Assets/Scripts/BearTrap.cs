using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    Animator animator;
    AudioSource src;
    AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && animator.GetBool("Triggered") == false)
        {
            animator.SetBool("Triggered", true);
            src.PlayOneShot(clip);
            PlayerLives.Instance.PlayerCaught();
        }
    }
}

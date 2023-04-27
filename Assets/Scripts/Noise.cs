using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    [SerializeField] private float noiseLevel = 0f;
    public float noiseVolume;

    private AudioSource src;
    public AudioClip clip;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    //If player steps on noise hazard, play sound and set noise level
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //play sound
            if (clip != null)
            {
                src.PlayOneShot(clip);
            }
            SetNoiseLevel(noiseVolume);
            NoiseMeter.Instance.onDistraction = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (PlayerNoise.Instance.GetNoiseLevel() == 0f)
            {
                SetNoiseLevel(0f);
            }
            else
            {
                SetNoiseLevel(noiseVolume);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SetNoiseLevel(0f);
            NoiseMeter.Instance.onDistraction = false;
        }
    }


    public float GetNoiseLevel()
    {
        return noiseLevel;
    }

    public void SetNoiseLevel(float newNoise)
    {
        if (noiseLevel != newNoise)
        {
            if (newNoise > 10f)
            {
                newNoise = 10f;
            }

            if (newNoise < 0)
            {
                newNoise = 0;
            }

            noiseLevel = newNoise;
        }

        NoiseMeter.Instance.UpdateNoiseMeter(noiseLevel);
    }
}

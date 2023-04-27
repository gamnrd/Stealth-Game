using UnityEngine;


public class PlayerNoise : MonoBehaviour
{
    public static PlayerNoise Instance;
    [SerializeField]private float noiseLevel = 0f;

    private void Awake()
    {
        Instance = this;
    }

    //Return current noiseLevel
    public float GetNoiseLevel()
    {
        return noiseLevel;
    }

    //Set new noiseLevel
    public void SetNoiseLevel(float newNoise)
    {
        if (newNoise > 10f)
        {
            newNoise = 10f;
        }

        if (newNoise < 0)
        {
            newNoise = 0;
        }

        //Update noise meter
        noiseLevel = newNoise;
        NoiseMeter.Instance.UpdateNoiseMeter(noiseLevel);
    }
}

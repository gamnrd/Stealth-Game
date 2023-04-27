using UnityEngine;
using UnityEngine.UI;

public class NoiseMeter : MonoBehaviour
{
    public static NoiseMeter Instance;
    private Image noiseMeter;
    public bool onDistraction = false;

    private void Awake()
    {
        Instance = this;
        noiseMeter = GetComponent<Image>();
    }


    //Update UI Noise meter
    public void UpdateNoiseMeter(float newFill)
    {
        noiseMeter.fillAmount = newFill / 10;
    }
}

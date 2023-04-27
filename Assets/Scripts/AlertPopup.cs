using UnityEngine;

public class AlertPopup : MonoBehaviour
{
    private Transform cameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        cameraPosition = Camera.main.transform;

        //Destroy after 5 sec
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        //Get text to face the camera (also has to be rotated or it will be backwards
        transform.LookAt(cameraPosition.position);
        transform.localRotation *= Quaternion.Euler(0, 180, 0);
    }
}

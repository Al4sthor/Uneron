using UnityEngine;

public class ControlLuz : MonoBehaviour
{
    private Light luz;

    void Start()
    {
        luz = GetComponent<Light>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            luz.enabled = !luz.enabled;
        }
    }
}
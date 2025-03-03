using UnityEngine;

public class CameraColorCycler : MonoBehaviour
{
    public float cycleSpeed = 0.5f; // Speed of color change

    private float hue, saturation, value;
    private Camera cam;

    void Start()
    {
        cam = Camera.main; // Get the main camera

        // Convert the initial color (Hex: 856B8F) to HSV
        Color.RGBToHSV(new Color(0x85 / 255f, 0x6B / 255f, 0x8F / 255f), out hue, out saturation, out value);
    }

    void Update()
    {
        // Increment hue over time and loop it within 0-1
        hue += cycleSpeed * Time.deltaTime;
        if (hue > 1f) hue -= 1f;

        // Convert back to RGB
        Color newColor = Color.HSVToRGB(hue, saturation, value);

        // Apply to Camera background color
        cam.backgroundColor = newColor;
    }
}

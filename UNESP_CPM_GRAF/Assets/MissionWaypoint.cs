using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionWaypoint : MonoBehaviour
{
    // Indicator icon
    public Image img;
    // The target (location, enemy, etc..)
    public Transform target;
    // UI Text to display the distance
    public TextMeshProUGUI meter;
    // To adjust the position of the icon
    public Vector3 offset;

    // Distance threshold for the fade effect
    public float fadeDistance = 50f;

    private void Update()
    {
        // Giving limits to the icon so it sticks on the screen
        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        // Convert 3D world position to 2D screen position
        Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

        // Check if the target is behind us
        if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }

        // Clamp X and Y positions to stay on screen
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // Update marker position
        img.transform.position = pos;

        // Calculate distance to the target
        float distance = Vector3.Distance(target.position, transform.position);
        meter.text = ((int)distance).ToString() + "m";

        // Adjust transparency based on distance
        if (distance < fadeDistance)
        {
            float alpha = Mathf.Lerp(0f, 1f, distance / fadeDistance); // Fade from 0 (invisible) to 1 (fully visible)
            SetAlpha(img, alpha);
            SetAlpha(meter, alpha);
        }
        else
        {
            // Fully visible if distance is greater than fadeDistance
            SetAlpha(img, 1f);
            SetAlpha(meter, 1f);
        }
    }

    // Function to set the alpha of an image or text
    void SetAlpha(Graphic uiElement, float alpha)
    {
        Color color = uiElement.color;
        color.a = alpha;
        uiElement.color = color;
    }
}

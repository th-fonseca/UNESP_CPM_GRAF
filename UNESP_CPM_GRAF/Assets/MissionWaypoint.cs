using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionWaypoint : MonoBehaviour
{
    public Image img;
    public Transform target;
    public TextMeshProUGUI meter;

    public Vector3 offset;

    public float fadeDistance = 50f;

    private void Update()
    {
        if (!Camera.main) return;
        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;


        Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

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

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        img.transform.position = pos;

        float distance = Vector3.Distance(target.position, transform.position);
        meter.text = ((int)distance).ToString() + "m";

        if (distance < fadeDistance)
        {
            float alpha = Mathf.Lerp(0f, 1f, distance / fadeDistance);
            SetAlpha(img, alpha);
            SetAlpha(meter, alpha);
        }
        else
        {
            SetAlpha(img, 1f);
            SetAlpha(meter, 1f);
        }
    }

    void SetAlpha(Graphic uiElement, float alpha)
    {
        Color color = uiElement.color;
        color.a = alpha;
        uiElement.color = color;
    }
}

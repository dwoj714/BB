using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvasScaler : MonoBehaviour
{
    // Sets the bounds of the attached rect transform to match the camera viewport
    void Update()
    {
		RectTransform rtf = (RectTransform)transform;
		float vert = Camera.main.orthographicSize * 2;
		float horiz = Camera.main.aspect * vert;
		rtf.sizeDelta = Vector2.up * vert + Vector2.right * horiz;
    }
}

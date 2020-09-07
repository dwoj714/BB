using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Text))]
public class FadeText : MonoBehaviour
{
    [HideInInspector] public Text text;
    private RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }



}

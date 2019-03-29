using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UI;

public class HealthReader : MonoBehaviour
{
	Text text;
	public HealthBar hb;

	private void Start()
	{
		text = GetComponent<Text>();
	}

	// Update is called once per frame
	void Update()
    {
		text.text = "" + Mathf.Ceil(hb.getHealth());
    }
}

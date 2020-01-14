using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
	public Material mat;
	public SpriteRenderer spr;
	public float timeScale = 1;
	public float duration = 1;
	private float pTime;
	private float timer = 0;
	private Color pColor;

	public Transform targetTransform;
	public bool parentScaleCompensation = true;


	private int _flipID = Shader.PropertyToID("_flip");
	private int _timeScaleID = Shader.PropertyToID("_timeScale");
	private int _colorID = Shader.PropertyToID("_color");
	private int _startTimeID = Shader.PropertyToID("_startTime");

	// Use this for initialization
	void Awake()
	{
		spr = GetComponent<SpriteRenderer>();
		mat = spr.material;
	}

	// Update is called once per frame
	void Update()
	{
		if(timer < duration)
		{
			timer += Time.deltaTime;

			if(timer >= duration)
			{
				Visible = false;
				mat.SetFloat(_flipID, 0);
			}
		}

		if (pTime != timeScale)
		{
			pTime = timeScale;
			mat.SetFloat(_timeScaleID, timeScale);
		}

		if (spr.color != pColor)
		{
			pColor = spr.color;
			mat.SetColor(_colorID, pColor);
		}
	}

	public void ActivateFX()
	{
		Visible = true;
		mat.SetFloat(_startTimeID, Time.time);
		mat.SetFloat(_flipID, 1);
		timer = 0;
	}



	public void SetRadius(float radius)
	{
		if (transform.parent)
			transform.localScale = (Vector2.up + Vector2.right) * radius * 2 * (1 / transform.parent.lossyScale.x);
		else
			transform.localScale = (Vector2.up + Vector2.right) * radius * 2;
	}

	public bool Visible
	{
		get
		{
			return spr.enabled;
		}

		set
		{
			spr.enabled = value;
		}
	}

}

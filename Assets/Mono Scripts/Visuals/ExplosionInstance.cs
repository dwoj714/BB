using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionInstance : MonoBehaviour
{
	[Header("References")]
	public Material mat;
	public SpriteRenderer spr;
	public bool selfDestruct = true;
	[SerializeField]private bool trigger = false;

	[SerializeField] private AudioClip[] clipPool;

	[Header("Material Properties")]
	public float timeScale = 1;
	public float outerEdge = 0;
	public float bgScale = 0.25f;
	private float pTime;
	private Color pColor;

	new public AudioSource audio;

	// Use this for initialization
	void Awake()
	{
		spr = GetComponent<SpriteRenderer>();
		mat = spr.material;

		mat.SetFloat("_bgScale", bgScale);
		mat.SetFloat("_outer", outerEdge);

		if (spr.color != pColor)
		{
			pColor = spr.color;
			mat.SetColor("_color", pColor);
		}

		audio = GetComponent<AudioSource>();

	}

	// Update is called once per frame
	void Update()
	{

		if (trigger)
		{
			Detonate();
			trigger = false; 
		}

		if (pTime != timeScale)
		{
			pTime = timeScale;
			mat.SetFloat("_timeScale", timeScale);
		}
	}

	public void SetRadius(float radius)
	{
		transform.localScale = Vector2.up * radius * 2 + Vector2.right * radius * 2;
	}

	public void Detonate()
	{

		if (audio)
		{
			audio.clip = GetClip();
			audio.pitch = Random.Range(0.25f, 2f);
			audio.Play();
		}
		mat.SetFloat("_startTime", Time.time);
		if (selfDestruct)
		{
			float duration = Mathf.Max(1 / timeScale, audio ? audio.clip.length : 0);
			Destroy(gameObject, duration);
		}
	}

	private AudioClip GetClip()
	{
		int clips = clipPool.Length;
		float rng = Random.Range(0, clips);
		if(rng == clips)
		{
			rng --;
		}

		return clipPool[Mathf.FloorToInt(rng)];
	}

}

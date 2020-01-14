using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionInstance : MonoBehaviour
{
	[Header("References")]
	public Material mat;
	public SpriteRenderer spr;
	public bool selfDestruct = true;

	private static float minRadAudio = 1;
	private static float maxRadAudio = 4.5f;
	private static float minPitch = 0.2f;
	private static float maxPitch = 1.8f;	

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
	void Start()
	{
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

			//set the pitch of the clip based on the size of the explosion
			float radius = transform.localScale.x / 2;
			if (radius >= maxRadAudio)
				audio.pitch = minPitch;
			else if (radius <= minRadAudio)
				audio.pitch = maxPitch;
			else
			{
				float radRange = maxRadAudio - minRadAudio;
				float scaledRadius = radius - minRadAudio;
				audio.pitch = (1 - (scaledRadius / radRange)) * (maxPitch - minPitch) + minPitch;
			}

			/*/set the pitch of the clip based on the size of the explosion
			// Modified to use radius squares
			float sqrRad = Mathf.Pow(transform.localScale.x / 2, 2);
			float sqrMinRad = minRadAudio * minRadAudio;
			float sqrMaxRad = maxRadAudio * maxRadAudio;

			if (sqrRad >= sqrMaxRad)
				audio.pitch = minPitch;
			else if (sqrRad <= sqrMinRad)
				audio.pitch = maxPitch;
			else
			{
				float radRange = sqrMaxRad - sqrMinRad;
				float scaledRadius = sqrRad - sqrMinRad;
				audio.pitch = (1 - (scaledRadius / radRange)) * (maxPitch - minPitch) + minPitch;
			}*/

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
		return clipPool[Mathf.FloorToInt(rng)];
	}

}

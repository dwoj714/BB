using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public abstract class PointAbility : MonoBehaviour {

	[HideInInspector]
	public bool enableMouseInput;

	Camera cam;

	public float radius;
	private CircleCollider2D col;
	private Collider2D[] hits;

	protected virtual void Start()
	{
		cam = Camera.main;
	}

	protected virtual void Update()
	{
		if (enableMouseInput)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Activate(cam.ScreenToWorldPoint(Input.mousePosition));
			}
		}
	}

	protected virtual void Activate(Vector3 position)
	{
		hits = Physics2D.OverlapCircleAll(position, radius);
	}
}

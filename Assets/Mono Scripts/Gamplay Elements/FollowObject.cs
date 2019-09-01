using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public Rigidbody2D followRB;
	public Vector2 offset;
	private Rigidbody2D rb;

    void Start()
    {
		rb = GetComponent<Rigidbody2D>();
    }

	private void FixedUpdate()
	{
		//if the rigidbody to be followed stops existing, destroy this gameObject
		if (!followRB)
		{
			Destroy(gameObject);
		}
		if (followRB) rb.position = followRB.position + offset;
	}


}

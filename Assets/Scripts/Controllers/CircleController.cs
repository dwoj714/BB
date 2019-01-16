using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleController : MonoBehaviour {

	public float fallSpeed = 3;
	public float speedingDrag = 2;

	private float gravity;

	Rigidbody2D rb;

	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		gravity = rb.gravityScale;
	}
	
	void FixedUpdate ()
	{
		if(rb.velocity.y < -fallSpeed)
		{
			rb.gravityScale = 0;
		}
		else
		{
			rb.gravityScale = gravity;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.name.Equals("Projectile(Clone)"))
		{
			Destroy(gameObject);
		}
	}

}

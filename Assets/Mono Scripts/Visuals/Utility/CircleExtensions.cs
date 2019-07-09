using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CircleExtensions
{
	//Returns 1 if other is in contact with center,
	//and 0 if the distance between center and other is >= outerRadius
	public static float CircleCloseness(CircleCollider2D center, float outerRadius, Collider2D other)
	{
		/*if(other == null)
		{
			Debug.Log("What");
			other = new CircleCollider2D();
			other.radius = 0.0001f;
		}*/

		float dist = Mathf.Clamp(center.Distance((CircleCollider2D)other).distance, 0, Mathf.Infinity);
		return 1 - (Mathf.Clamp01(dist / outerRadius));
	}

	public static float PhysicalRadius(CircleCollider2D col)
	{
		if(col.transform.lossyScale.x > col.transform.lossyScale.y)
		{
			return col.radius * col.transform.lossyScale.x;
		}
		else
		{
			return col.radius * col.transform.lossyScale.y;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
	[SerializeField] Vector2 offset;
	[SerializeField] GameObject shieldPrefab;

    // Start is called before the first frame update
    void Start()
    {
		FollowObject shield = Instantiate(shieldPrefab).GetComponent<FollowObject>();
		shield.followRB = GetComponent<Rigidbody2D>();
		shield.offset = offset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for use with spawner formations. The attached gameObject will have children that have no need to be
//part of the same object, outside from being spawned together. This script unlinks them from this GameObject,
//and deletes itself.
public class FormationParent : MonoBehaviour
{
	[SerializeField] private int difficultyRating = 0;

	private bool done = true;

	private void Update()
	{
		if (done) StartCoroutine("WaitDelete");
	}

	private IEnumerable WaitDelete()
	{
		done = false;

		float timer = 5;

		//wait 5 seconds
		while(timer > 0)
		{
			timer -= Time.unscaledDeltaTime;
			yield return null;
		}

		//check if all child objects are inactive. Bombs with formationMember == true are
		//made inactive on death rather than being destroyed.
		int c = 0;
		for(int i = 0; i < transform.childCount; i++)
		{
			if (!transform.GetChild(i).gameObject.activeSelf) c++;
		}

		if (c == transform.childCount) Destroy(gameObject);

		done = true;
	}

}

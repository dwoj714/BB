using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionDisplayController : MonoBehaviour
{
	[SerializeField] private RotorController rotor;
	[SerializeField] private SpriteRenderer baseSpr;
	[SerializeField] private GameObject[] objectList;

	private Material baseMat;
	private float initial, previous;    //values for FX (transparency) radius
	private int objIdx;
	private int nameID;

    // Start is called before the first frame update
    void Start()
    {
		baseMat = baseSpr.material;
		nameID = Shader.PropertyToID("_depth");
		initial = previous = baseMat.GetFloat(nameID);
		//objIdx = rotor.MedianIdx;
    }

    public void OnWeaponCycle()
	{
		StartCoroutine("SwapStep");
	}

	IEnumerator SwapStep()
	{
		float timer = 0;
		float rad;
		int current = rotor.MedianIdx;

		while(timer <= rotor.swapTime / 2)
		{
			rad = initial * (1 - (timer / (rotor.swapTime / 2)));
			baseMat.SetFloat(nameID, rad);
			timer += Time.deltaTime;
			yield return null;
		}

		//wait until the index is actually swapped
		while(current == rotor.MedianIdx)
		{
			yield return null;
		}

		objectList[current].SetActive(false);
		objectList[rotor.MedianIdx].SetActive(true);

		while(timer < rotor.swapTime)
		{
			rad = initial * ((timer - rotor.swapTime / 2) / (rotor.swapTime / 2));
			baseMat.SetFloat(nameID, rad);
			timer += Time.deltaTime;
			yield return null;
		}

		baseMat.SetFloat(nameID, initial);

	}

}

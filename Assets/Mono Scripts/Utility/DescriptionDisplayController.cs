using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionDisplayController : MonoBehaviour
{
	[SerializeField] private RotorController rotor;
	[SerializeField] private SpriteRenderer baseSpr;
	[SerializeField] private GameObject[] objectList;

	private Material baseMat;
	private float initial, timer;    //values for FX (transparency) radius
	private int objIdx;
	private int radID, blurID;

    // Start is called before the first frame update
    void Start()
    {
		baseMat = baseSpr.material;
		radID = Shader.PropertyToID("_depth");
		blurID = Shader.PropertyToID("_blur");
		initial = baseMat.GetFloat(radID);
    }

    public void OnWeaponCycle()
	{
		if(rotor.CanSwap) StartCoroutine("SwapStep");
	}

	IEnumerator SwapStep()
	{
		timer = 0;
		int current = rotor.MedianIdx;

		float progress;

		while(timer <= rotor.swapTime / 2)
		{
			progress = 1 - (timer / (rotor.swapTime / 2));

			baseMat.SetFloat(radID, initial * progress);
			baseMat.SetFloat(blurID, 1 - progress);

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
			progress = (timer - rotor.swapTime / 2) / (rotor.swapTime / 2);

			baseMat.SetFloat(radID, initial * progress);
			baseMat.SetFloat(blurID, 1 - progress);

			timer += Time.deltaTime;
			yield return null;
		}

		baseMat.SetFloat(radID, initial);
		baseMat.SetFloat(blurID, 0);
	}

}

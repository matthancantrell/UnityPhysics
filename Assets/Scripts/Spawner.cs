using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] GameObject prefab;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			Instantiate(prefab, transform.position, transform.rotation);
		}
	}
}

using UnityEngine;
using System.Collections;

class AnimateTiledTexture : MonoBehaviour
{
	public int columns = 10;
	public int rows = 1;
	public float framesPerSecond = 1f;

	//the current frame to display
	private int index = 0;
	float timer = 0.0f;

	void Start()
	{
		Vector2 size = new Vector2(1f / columns, 1f / rows);
		GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", size);
	}

	void Update()
	{
		timer += Time.deltaTime;

		if( timer >= 1f / framesPerSecond )
		{
			timer = 0.0f;
			index++;
			if (index >= rows * columns)
				index = 0;

			//split into x and y indexes
			Vector2 offset = new Vector2((float)index / columns - (index / columns), //x index
				(index / columns) / (float)rows);          //y index

			GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
		}
	}
}
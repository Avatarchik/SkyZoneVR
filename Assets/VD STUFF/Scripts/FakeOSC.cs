using UnityEngine;
using System.Collections;

public class FakeOSC : MonoBehaviour {
	
	GameManager gameManager;

	int[] colors;

	int curColor = 0;

	void Start () {
		gameManager = GetComponent<GameManager>();
	
		colors = new int[] {0, 3};
	}

	void Update () {
		if(Input.GetButtonDown("Fire1")) {
			Vector3 pos = Input.mousePosition;
			pos.x /= Screen.width;
			pos.y /= Screen.height;
//			pos.y = 1 - pos.y;
			pos.x = 1 - pos.x;
			BallHit(pos, colors[curColor]);
		}

		if(Input.GetKeyDown(KeyCode.Q)) {
			curColor--;
			if(curColor < 0)
				curColor = colors.Length - 1;
		}

		if(Input.GetKeyDown(KeyCode.E)) {
			curColor++;
			if(curColor > colors.Length - 1)
				curColor = 0;
		}
	}

	void BallHit(Vector2 pos, int color) {
		ArrayList list = new ArrayList();
		list.Add(pos.x);
		list.Add(pos.y);
		list.Add(color);
		gameManager.BallHit(list);
	}
}
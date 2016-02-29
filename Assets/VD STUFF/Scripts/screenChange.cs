using UnityEngine;
using System.Collections;

public class screenChange : MonoBehaviour {
	public bool screenChanged;

	int counter = 8;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnPostRender(){
		if(screenChanged){
			counter--;
			if(counter < 0) {
				screenChanged = false;
				//StartCoroutine("Send");
				counter = 8;
				OSCSender.SendEmptyMessage("/readyCheck");
			}
		}
	}
}

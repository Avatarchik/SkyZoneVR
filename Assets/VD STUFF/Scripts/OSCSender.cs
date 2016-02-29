using UnityEngine;
using System.Collections;
using OSC.NET;

public class OSCSender : MonoBehaviour {
	// Use this for initialization
	public OSCTransmitter transmit;
	private float intensity;
	public bool networkE;

	public static OSCSender s_instance;

	void Start () {
		if(networkE){
			transmit = new OSCTransmitter("localhost", 7600);
		}
		s_instance = this;
	} 

	public void SendFloat(string path, params float[] data){
		OSCMessage packet = new OSCMessage(path);
		foreach(float val in data){
			packet.Append(val);
		}
		transmit.Send(packet);
		Debug.Log("Sent data! -" + data);
		
	}

	public static void SendEmptyMessage(string address) {
		print ("Sent " + address);
		OSCMessage packet = new OSCMessage (address);
		s_instance.transmit.Send (packet);
	}
}
 using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
 
public class SerialManager : MonoBehaviour 
{
	bool configured;

  	public string port;
  	private Thread thread;
  	SerialPort stream;// = new SerialPort("/dev/tty.usbmodem1411", 115200); 

  	public int baudRate = 9600;
 	int readTimeout = 10; //20

  	private List<string> packetQueue = new List<string>();
	private List<string> availableCOMPorts = new List<string>();

	float timeSinceLastConfig, delay = 2.0f;

	void Start () 
	{
		availableCOMPorts = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
		foreach (string port in availableCOMPorts) 
		{
			print (port);
		}
	}

	public void Update ()
	{
		if (!configured) 
		{
			if (timeSinceLastConfig < Time.time - delay) 
			{
				print ("not configured");
				NewComPort (availableCOMPorts);
				timeSinceLastConfig = Time.time;
				return;
			} else if (stream != null) {
				stream.Write ("?");
				if (stream.ReadLine () == "!") {
					configured = true;
					print ("Stream connected to port: " + stream.PortName);
					port = stream.PortName;
					thread = new Thread (new ThreadStart (readSerial));
					thread.Start ();
					print ("update: configured");
				} 
			}
		} 
		else 
		{
			//print ("update: configured");
		}
			

		lock (packetQueue) 
		{
			foreach (string message in packetQueue) 
			{
				print ("CHECKING MESSAGES");
				print ("packet queue: " + message);
				if (configured) 
				{
					//BroadcastMessage ("SerialInputRecieved", message, SendMessageOptions.DontRequireReceiver);
					SendMessage("SerialInputRecieved", message, SendMessageOptions.DontRequireReceiver);
				} 
				else if(message == "!")
				{
					configured = true;
				}
		  	}
		  packetQueue.Clear ();
		}
	}

	private void readSerial()
	{
		print ("READING SERIAL");

		while(stream.IsOpen) 
		{
	  		try
			{
		    string lineToRead = stream.ReadLine(); 
		    print("reading serial: " + lineToRead);

		    if (lineToRead != null) 
			{
		      	lock (packetQueue) 
				{
		        packetQueue.Add(lineToRead);
		      	}
		    }

	    	stream.BaseStream.Flush(); 
			} 
			catch (Exception e) 
			{ 
			Debug.Log (e.Message);
			Console.WriteLine (e.Message); 
			}
		}
	}

	void NewComPort(List<string> COMportList)
	{
		stream = new SerialPort (COMportList[0], baudRate);
		//stream.ReadBufferSize = 9024;
		stream.Open ();
		stream.ReadTimeout = 1; //readTimeout;
		stream.Write ("?");
		COMportList.RemoveAt (0);

		/*

			print (newPort.WriteTimeout.ToString ());


			if (newPort.ReadLine () == "!") 
			{
				print ("configured");
				configured = true;

				if (configured) 
					return newPort;
			}
		}
		
		return null;	
	}*/
	}

	void OnApplicationQuit()
	{
		thread.Abort ();
		stream.Close ();
	}

	public void WriteToStream(string send)
	{
		stream.Write (send);
	}

	public void ClearPacketQueueAndBuffer()
	{
        if (this.isActiveAndEnabled == false)
            return;

		//stream.BreakState = true;
		packetQueue.Clear ();
		stream.DiscardInBuffer ();
		stream.DiscardOutBuffer ();
//		stream.ReadBufferSize = 0;
//		stream.WriteBufferSize = 0;
		stream.BaseStream.Flush();

		//print ("Thread state: " + thread.ThreadState);
	}

	public void OpenOrCloseStream(bool open)
	{
		if (open)
			stream.Open ();
		else
			stream.Close ();
	}
}

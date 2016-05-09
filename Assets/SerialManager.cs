using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
 
public class SerialManager : MonoBehaviour {
  public string port;
  private Thread thread;
  SerialPort stream;// = new SerialPort("/dev/tty.usbmodem1411", 115200); 

  int baudRate = 57600;
  int readTimeout = 20;

  private List<string> packetQueue = new List<string>();
	
  void Start () {
    try {
      stream = new SerialPort(port, baudRate);
      stream.Open(); //Open the Serial Stream.
      stream.ReadTimeout = readTimeout;

      thread = new Thread (new ThreadStart (readSerial));
      thread.Start();
    } catch (Exception e) {
      Debug.Log (e.Message);
    }
  }
 
  public void Update (){
    lock (packetQueue) {
      foreach (string message in packetQueue) {
        BroadcastMessage ("SerialInputRecieved", message, SendMessageOptions.DontRequireReceiver);
	  }
      packetQueue.Clear ();
    }
  }

  private void readSerial(){
    while(stream.IsOpen) {
      try{
        string lineToRead = stream.ReadLine(); 
        if (lineToRead != null) {
          lock (packetQueue) {
            packetQueue.Add(lineToRead);
          }
        }
        stream.BaseStream.Flush(); 
      } catch (Exception e) { 
        Debug.Log (e.Message);
        Console.WriteLine (e.Message); 
      }
    }
  }
}

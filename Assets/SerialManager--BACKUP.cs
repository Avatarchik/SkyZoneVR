//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Ports;
//using System.Threading;
// 
//public class SerialManager : MonoBehaviour {
//	bool configured = false;
//
//  public string port;
//  private Thread thread;
//  SerialPort stream;// = new SerialPort("/dev/tty.usbmodem1411", 115200); 
//
//  public int baudRate = 9600;
//  int readTimeout = 20; //20
//
//  private List<string> packetQueue = new List<string>();
//
//  void Start () {
//		
//
//  }
// 
//  public void Update (){
//
//		if(!configured){
//			stream.Write ("?");
//		}
//    lock (packetQueue) {
//      foreach (string message in packetQueue) {
//        BroadcastMessage ("SerialInputRecieved", message, SendMessageOptions.DontRequireReceiver);
//	  }
//      packetQueue.Clear ();
//    }
//  }
//
//
//
//	/*
//	 *     try {
//	  stream = new SerialPort("COM6", baudRate);
//      stream.Open(); //Open the Serial Stream.
//      stream.ReadTimeout = readTimeout;
//
//      thread = new Thread (new ThreadStart (readSerial));
//      thread.Start();
//    } catch (Exception e) {
//      Debug.Log (e.Message);
//    }
//    
//	 * /
//
//
//
//  private void readSerial(){
//    while(stream.IsOpen) {
//      try{
//        string lineToRead = stream.ReadLine(); 
//        print(lineToRead);
//        if (lineToRead != null) {
//          lock (packetQueue) {
//            packetQueue.Add(lineToRead);
//          }
//        }
//        stream.BaseStream.Flush(); 
//      } catch (Exception e) { 
//        Debug.Log (e.Message);
//        Console.WriteLine (e.Message); 
//      }
//    }
//  }
//}
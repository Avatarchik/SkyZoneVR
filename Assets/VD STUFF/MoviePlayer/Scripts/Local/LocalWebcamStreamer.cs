//--------------------------------------------
// Movie Player
// Copyright © 2014 SHUU Games
//--------------------------------------------
using UnityEngine;
using System;

#if (UNITY_4_3 || UNITY_4_4 || UNITY_4_5) && (UNITY_STANDALONE_OSX || UNITY_IPHONE || UNITY_DASHBOARD_WIDGET)

// Starting from Unity 4.3.2 webcam support is dropped on OSX platforms
// see http://answers.unity3d.com/questions/632130/cant-build-for-osx-when-using-webcamtexture-or-web.html


#else

namespace MP.Local
{
	/// <summary>
	/// Http mjpeg streamer.
	/// </summary>
	public class LocalWebcamStreamer : Streamer
	{
		#region ----- public members -----

		public const string URL_PREFIX = "webcam://";

		public override void Connect (string url, LoadOptions loadOptions = null)
		{
			if (!Application.HasUserAuthorization (UserAuthorization.WebCam)) { // | UserAuthorization.Microphone
				throw new MpException ("Not authorized to use webcam. Use Application.RequestUserAuthorization before calling this");
			}

			if (loadOptions != null && loadOptions.videoStreamInfo != null) {
				videoStreamInfo = loadOptions.videoStreamInfo;
				webcam = new WebCamTexture (url.Substring (9),
				                           loadOptions.videoStreamInfo.width,
				                           loadOptions.videoStreamInfo.height,
				                           (int)loadOptions.videoStreamInfo.framerate);
			} else {
				webcam = new WebCamTexture (url.Substring (9));
			}
			webcam.Play ();

			videoStreamInfo = new VideoStreamInfo ();
			videoStreamInfo.codecFourCC = MP.Decoder.VideoDecoderRGB.FOURCC_NULL;
			videoStreamInfo.width = webcam.width;
			videoStreamInfo.height = webcam.height;
			videoStreamInfo.bitsPerPixel = 24;
			videoStreamInfo.framerate = webcam.requestedFPS;

			colorBuffer = new Color32[webcam.width * webcam.height];
			rawBuffer = new byte[colorBuffer.Length * 3];
		}

		public override void Shutdown (bool force = false)
		{
			webcam.Stop ();
			webcam = null;
			colorBuffer = null;
			rawBuffer = null;
		}
		
		public override bool IsConnected {
			get {
				return webcam != null && webcam.isPlaying;
			}
		}

		public override int VideoPosition {
			get {
				return receivedFrameCount;
			}
			set {
				throw new NotSupportedException ("Can't seek a live stream");
			}
		}

		/// <summary>
		/// Reads last received frame into buffer. Returns bytes read.
		/// </summary>
		public override int ReadVideoFrame (out byte[] targetBuf)
		{
			// this check is not the best, but can't do it better here
			if (webcam.didUpdateThisFrame) {
				webcam.GetPixels32 (colorBuffer);

				// Performance: 0.75ms for 352x288 video on Intel i5
				int len = colorBuffer.Length;
				for (int i = 0; i < len; i++) {
					Color32 col = colorBuffer [i];
					rawBuffer [i * 3] = col.b;
					rawBuffer [i * 3 + 1] = col.g;
					rawBuffer [i * 3 + 2] = col.r;
				}
				// would like to use direct memory copy, but can't in managed C# :(
				//System.Buffer.BlockCopy(colorBuffer, 0, rawBuffer, 0, rawBuffer.Length);

				receivedFrameCount++;
			}
			targetBuf = rawBuffer;
			return rawBuffer.Length;
		}

		public override int AudioPosition {
			get {
				throw new NotSupportedException ("There's no webcam audio support for now");
			}
			set {
				throw new NotSupportedException ("Can't seek a live stream");
			}
		}

		public override int ReadAudioSamples (out byte[] targetBuf, int sampleCount)
		{
			throw new NotSupportedException ("There's no webcam audio support for now");
		}

		#endregion

		#region ----- private members -----

		private int receivedFrameCount = 0;
		private WebCamTexture webcam;
		private Color32[] colorBuffer;
		private byte[] rawBuffer;

		#endregion
	}
}

#endif

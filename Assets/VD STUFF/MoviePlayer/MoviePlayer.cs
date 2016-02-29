//--------------------------------------------
// Movie Player
// Copyright Â© 2014 SHUU Games
//--------------------------------------------

using UnityEngine;
using System;
using System.IO;
using MP;

/// <summary>
/// Movie player
/// </summary>
public class MoviePlayer : MoviePlayerBase
{
	/// <summary>
	/// Package version
	/// </summary>
	public const string PACKAGE_VERSION = "v0.8";

	#region ----- Public properties ------
	
	/// <summary>
	/// RAW MJPEG AVI asset that is loaded at Start, can be NULL.
	/// 
	/// If Load() is called on this component, this is not relevant any more.
	/// </summary>
	public TextAsset source;

	/// <summary>
	/// The audio clip to play, can be NULL.
	/// 
	/// If the source already contains audio, then audioClip will override the audio in source.
	/// </summary>
	public AudioClip audioSource;

	/// <summary>
	/// Movie load options. The Load() methods on this component will use
	/// this unless you're provinding your own.
	/// </summary>
	public LoadOptions loadOptions = LoadOptions.Default;

	/// <summary>
	/// The current playhead time. Use this for seeking.
	/// </summary>
	public float videoTime;
	
	/// <summary>
	/// The current playhead frame index. Use this for seeking.
	/// </summary>
	public int videoFrame;

	/// <summary>
	/// If TRUE, the movie will automatically loop.
	/// </summary>
	public bool loop;
	
	/// <summary>
	/// Called when the movie reaches an end, right after
	/// it is rewinded back to the beginning.
	/// </summary>
	public event MovieEvent OnLoop;

	#endregion ------ /public properties ------

	#region ------ public methods ------

	/// <summary>
	/// Loads the movie from byte array.
	/// </summary>
	public bool Load (byte[] bytes, LoadOptions loadOptions = null)
	{
		return Load (new MemoryStream (bytes), loadOptions);
	}
	
	/// <summary>
	/// Loads the movie from TextAsset
	/// </summary>
	public bool Load (TextAsset textAsset, LoadOptions loadOptions = null)
	{
		this.source = textAsset;
		return Load (new MemoryStream (textAsset.bytes), loadOptions);
	}

#if !UNITY_WINRT
	/// <summary>
	/// Loads the movie from file path.
	/// </summary>
	public bool Load (string path, LoadOptions loadOptions = null)
	{
		return Load (File.OpenRead (path), loadOptions);
	}
#endif

	public bool Load(Stream srcStream, LoadOptions loadOptions = null)
	{
		if(loadOptions == null) {
			loadOptions = this.loadOptions;
		} else {
			this.loadOptions = loadOptions;
		}

		// if we have audioSource set here to override audio in the source stream
		// don't load the audio in the demux.
		bool overrideAudio = audioSource != null && !loadOptions.skipAudio;
		if(overrideAudio) loadOptions.skipAudio = true;

		bool success = false;
		try {
			if(overrideAudio) audiobuffer = audioSource;

			base.Load (new MovieSource() { stream = srcStream }, loadOptions);
			if(movie.videoDecoder != null) {
				movie.videoDecoder.Decode(videoFrame);
			}

			success = true;
		}
		catch (Exception e) {
			Debug.LogError (e);
			//throw e;
		}
		return success;
	}

	/// <summary>
	/// Reloads the movie from "source".
	/// </summary>
	[ContextMenu("Reload")]
	public bool Reload ()
	{
		bool success = true;
		if (source != null)
		{
			success = Load (source.bytes, loadOptions);
			lastVideoFrame = -1; // will make HandleFrameDecode decode one frame even if not play=true
		}
		return success;
	}

	void Start ()
	{
		Reload ();
	}

	#endregion ------ / public methods ------

	protected float lastVideoTime;
	protected int lastVideoFrame;

	void OnGUI ()
	{
		if (movie == null || movie.demux == null || movie.demux.videoStreamInfo == null)
			return;

		// if we're playing the movie directly to screen
		if (drawToScreen && framebuffer != null) {
			DrawFramebufferToScreen ();
		}
	}

	void Update ()
	{
		// if this.play changed, Play or Stop the movie
		HandlePlayStop ();

		// advance playhead time or handle seeking
		bool wasSeeked = HandlePlayheadMove ();

		// decode a frame when necessary
		HandleFrameDecode (wasSeeked);

		if (play) {
			// synchronize audio and video
			HandleAudioSync ();

			// movie has been played back. should we restart it or loop
			HandleLoop ();
		}
	}

	protected bool HandlePlayheadMove ()
	{
		// let the videoTime advance normally, but in case
		// frameIndex has changed, use it to find new videoTime
		bool seekedByVideoFrame = videoFrame != lastVideoFrame;
		bool seekedByVideoTime = videoTime != lastVideoTime;

		if (seekedByVideoFrame) {
			videoTime = videoFrame / framerate;
		} else if (play) {
			videoTime += Time.deltaTime;
		}
		return seekedByVideoFrame || seekedByVideoTime;
	}

	protected void HandleFrameDecode (bool wasSeeked)
	{
		if (movie == null)
			return;
		
		// now when videoTime is known, find the corresponding
		// frameIndex and decode it if was not decoded last time
		videoFrame = Mathf.FloorToInt (videoTime * framerate);
		if (lastVideoFrame != videoFrame)
		{
			// Decode a video frame only if there is a decoder.
			if(movie.videoDecoder != null) {
				movie.videoDecoder.Decode (videoFrame);
				// we could compensate for loading frame decode time here,
				// but it seems to not make timing better for some reason
				//videoTime += movie.videoDecoder.lastFrameDecodeTime;
			}
			
			if (!wasSeeked && lastVideoFrame != videoFrame - 1) {
				int dropCnt = videoFrame - lastVideoFrame - 1;
				#if MP_DEBUG
				Debug.Log ("Frame drop. offset=" + (lastVideoFrame + 1) + ", count=" + dropCnt + " @ " + videoTime);
				#endif
				_framesDropped += dropCnt;
			}
		}
		lastVideoFrame = videoFrame;
		lastVideoTime = videoTime;
	}

	protected void HandleAudioSync ()
	{
		if (GetComponent<AudioSource>() == null || !GetComponent<AudioSource>().enabled || GetComponent<AudioSource>().clip == null)
			return;
		
		if (videoTime <= GetComponent<AudioSource>().clip.length && (Mathf.Abs (videoTime - GetComponent<AudioSource>().time) > (float)maxSyncErrorFrames / framerate))
		{
			#if MP_DEBUG
			Debug.Log ("Synchronizing audio and video. Drift: " + (videoTime - audio.time));
			#endif
			GetComponent<AudioSource>().Stop ();
			GetComponent<AudioSource>().time = videoTime;
			GetComponent<AudioSource>().Play ();
			_syncEvents++;
		}
	}

	protected void HandleLoop ()
	{
		if (movie == null || movie.demux == null || movie.demux.videoStreamInfo == null)
			return;
		
		if (videoTime >= movie.demux.videoStreamInfo.lengthSeconds) {
			if (loop) {
				// seek to the beginning
				videoTime = 0;
				#if MP_DEBUG
				Debug.Log ("LOOP");
				#endif
				if (OnLoop != null)
					OnLoop (this);
			} else {
				// stop the playback
				play = false;
			}
		}
	}
}

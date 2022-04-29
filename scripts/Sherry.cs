using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sherry : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
	[SerializeField] AudioSource audioWord;
	[SerializeField] AudioSource audioBreath;
	[SerializeField] int level = 0;


	VideoConfig currentVideoConfig;

	private void Update()
	{
		if (currentVideoConfig == null) return;

		PlayVideoClip(currentVideoConfig.GetVideoClip(), currentVideoConfig.IsClipLoopable());
		PlayAudioClip(GetAudioClipByExcitment(level), currentVideoConfig.IsAudioLoopable());
	}

	private void EndReached(VideoPlayer source)
	{
		GetComponent<Animator>().SetTrigger("EndReachTrigger");
	}

	//For animation events
	public void PlayVideo(VideoConfig videoConfig)
	{
		if(!ReferenceEquals(videoConfig, currentVideoConfig))
		{
			Debug.Log(string.Format("Switch to VideoConfig {0}", videoConfig.name));
			StopAudioWord();
			currentVideoConfig = videoConfig;
		}
		
	}

	private void StopAudioWord()
	{
		audioWord.Stop();
	}

	//For Input events
	public void PlayAudioWord()
	{
		AudioClip word_clip = null ;
		if (word_clip == null) return;

		PlayAudioWord(word_clip);

		//use pausing feature?
		//PauseAudioBreath();
		//StartCoroutine(WaitForAudioWordFinished(ResumeAudioBreath)); //Start Coroutine
	}

	IEnumerator WaitForAudioWordFinished(Action resume_action)
	{
		//Wait Until Sound has finished playing
		while (audioWord.isPlaying)
		{
			yield return null;
		}

		//Auidio has finished playing, resume
		resume_action();
	}

	private void ResumeAudioBreath()
	{
		if (audioBreath == null) return;
		if (audioBreath.clip == null) return;

		Debug.Log("ResumeAudioBreath");
		audioBreath.Play();
	}

	private void PauseAudioBreath()
	{
		if (audioBreath == null) return;
		if (audioBreath.clip == null) return;

		Debug.Log("PauseAudioBreath");
		audioBreath.Pause();
	}

	private void PlayAudioWord(AudioClip audioClip)
	{
		Debug.Log(string.Format("PlayAudioWord {0}", audioClip));
		//audioWord.Stop();
		audioWord.clip = audioClip;
		audioWord.Play();
	}

	private void PlayAudioClip(AudioClip audioClip, bool loopable)
	{
		if (audioBreath == null) return;

		if(ReferenceEquals(audioBreath.clip, audioClip))
		{
			return;
			//Debug.Log(string.Format("Detect Same AudioClip"));
		}
		else
		{
			Debug.Log(string.Format("Detect Different AudioClip, Start Playing {0}", audioClip.name));
			//audioBreath.Stop();
			audioBreath.loop = loopable;
			audioBreath.clip = audioClip;
			audioBreath.Play();
		}
		
	}

	private void PlayVideoClip(VideoClip videoClip, bool loopable)
	{
		if (videoClip == null) return;

		if (ReferenceEquals(videoPlayer.clip, videoClip))
		{
			return;
			//Debug.Log(string.Format("Detect Same VideoClip"));
		}
		else
		{
			Debug.Log(string.Format("Detect Different VideoClip {0}, Loopable {1}", videoClip.name, loopable));
			videoPlayer.Stop();
			videoPlayer.loopPointReached -= EndReached;
			if (!loopable)
			{
				videoPlayer.loopPointReached += EndReached;
			}
			videoPlayer.isLooping = loopable;
			videoPlayer.clip = videoClip;
			videoPlayer.Play();
		}
		
	}

	private AudioClip GetRandomClip(AudioClip[] list)
	{
		if (list.Length <= 0) return null;

		int index = UnityEngine.Random.Range(0, list.Length - 1);
		return list[index];
	}

	public void setExcitmentLevel(int lv)
	{
		level = Mathf.Clamp(lv, 0, 2);
	}

	private AudioClip GetAudioClipByExcitment(int lv)
	{
		if (lv >= 2)
		{
			AudioClip curr_audioclip2 = currentVideoConfig.GetAudioClipLevel2();
			if (curr_audioclip2 != null) return curr_audioclip2;
		}

		if (lv >= 1)
		{
			AudioClip curr_audioclip1 = currentVideoConfig.GetAudioClipLevel1();
			if (curr_audioclip1 != null) return curr_audioclip1;
		}

		return currentVideoConfig.GetAudioClip();
	}
}

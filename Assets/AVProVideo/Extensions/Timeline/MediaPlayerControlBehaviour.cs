// You need to define AVPRO_PACKAGE_TIMELINE manually to use this script
// We could set up the asmdef to reference the package, but the package doesn't 
// existing in Unity 2017 etc, and it throws an error due to missing reference
//#define AVPRO_PACKAGE_TIMELINE
#if (UNITY_2018_1_OR_NEWER && AVPRO_PACKAGE_TIMELINE)
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
// Copyright 2020-2020 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Playables
{
	public class MediaPlayerControlBehaviour : PlayableBehaviour
	{
		public MediaPlayer mediaPlayer = null;
		public float audioVolume = 1f;
		public string videoPath = null;

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (mediaPlayer != null)
			{
				if (Application.isPlaying)
				{
					if (!string.IsNullOrEmpty(videoPath) && videoPath != mediaPlayer.VideoPath)
					{
						mediaPlayer.OpenVideoFromFile(MediaLocation.RelativeToStreamingAssetsFolder, videoPath, true);
					}
					else
					{
						mediaPlayer.Play();
					}
				}
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.Pause();
			}
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
		}
	}
}
#endif
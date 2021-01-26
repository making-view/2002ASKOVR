using UnityEngine;
using UnityEditor;

//-----------------------------------------------------------------------------
// Copyright 2015-2020 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Editor
{
	/// <summary>
	/// Editor for the MediaPlayer component
	/// </summary>
	public partial class MediaPlayerEditor : UnityEditor.Editor
	{
		private void OnInspectorGUI_GlobalSettings()
		{
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			EditorGUILayout.LabelField("Current Platform", EditorUserBuildSettings.selectedBuildTargetGroup.ToString());

			EditorGUILayout.BeginVertical(GUI.skin.box);
			GUILayout.Label("Video Capture", EditorStyles.boldLabel);

			// TimeScale
			{
				const string TimeScaleDefine = "AVPROVIDEO_BETA_SUPPORT_TIMESCALE";
				if (EditorHelper.IMGUI.ToggleScriptDefine("TimeScale Support", TimeScaleDefine))
				{
					EditorHelper.IMGUI.NoticeBox(MessageType.Warning, "This will affect performance if you change Time.timeScale or Time.captureFramerate.  This feature is useful for supporting video capture system that adjust time scale during capturing.");
				}
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(GUI.skin.box);
			GUILayout.Label("Other", EditorStyles.boldLabel);

			// Disable Debug GUI
			{
				const string DisableDebugGUIDefine = "AVPROVIDEO_DISABLE_DEBUG_GUI";
				if (!EditorHelper.IMGUI.ToggleScriptDefine("Disable Debug GUI", DisableDebugGUIDefine))
				{
					EditorHelper.IMGUI.NoticeBox(MessageType.Info, "The Debug GUI can be disabled globally for builds to help reduce garbage generation each frame.");
				}
			}

			// Disable Logging
			{
				const string DisableLogging = "AVPROVIDEO_DISABLE_LOGGING";
				EditorHelper.IMGUI.ToggleScriptDefine("Disable Logging", DisableLogging);
			}

			_allowDeveloperMode = EditorGUILayout.Toggle("Developer Mode", _allowDeveloperMode);

			EditorGUILayout.EndVertical();

			EditorGUI.EndDisabledGroup();
		}
	}
}
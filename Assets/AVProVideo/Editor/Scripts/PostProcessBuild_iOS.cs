#if (UNITY_IOS || UNITY_TVOS) && UNITY_2017_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

//-----------------------------------------------------------------------------
// Copyright 2012-2020 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Editor
{
	public class PostProcessBuild_iOS
	{
		const string PluginName = "AVProVideo.framework";

		[PostProcessBuild]
		public static void ModifyProject(BuildTarget target, string path)
		{
			Debug.Log("[AVProVideo] Post-processing Xcode project.");
			string platform = null;
			switch (target)
			{
				case BuildTarget.iOS:
					platform = "iOS";
					break;
				case BuildTarget.tvOS:
					platform = "tvOS";
					break;
				default:
					Debug.LogWarningFormat("Unknown build target: {0}", target.ToString());
					break;
			}

			string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject project = new PBXProject();
			project.ReadFromFile(projectPath);

			// Attempt to find the plugin path
			string pluginPath = null;
			string[] guids = AssetDatabase.FindAssets(PluginName);
			if (guids != null && guids.Length > 0)
			{
				foreach (string guid in guids)
				{
					string assetPath = AssetDatabase.GUIDToAssetPath(guid);
					if (assetPath.Contains("AVProVideo/Runtime/Plugins/" + platform))
					{
						List<string> components = new List<string>(assetPath.Split(new char[] { '/' }));
						components[0] = "Frameworks";
						#if UNITY_2019_1_OR_NEWER
						pluginPath = string.Join("/", components);
						#else
						pluginPath = string.Join("/", components.ToArray());
						#endif
					}
				}
			}

			if (pluginPath != null)
			{
#if UNITY_2019_3_OR_NEWER
				string targetGuid = project.GetUnityMainTargetGuid();
#else
				string targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

				string fileGuid = project.FindFileGuidByProjectPath(pluginPath);
				if (fileGuid != null)
				{
					PBXProjectExtensions.AddFileToEmbedFrameworks(project, targetGuid, fileGuid);
				}
				else
				{
					Debug.LogWarningFormat("Failed to find {0} in the generated project. You will need to manually set {0} to 'Embed & Sign' in the Xcode project's framework list.", PluginName);
				}
				project.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
				project.WriteToFile(projectPath);
			}
			else
			{
				Debug.LogErrorFormat("Failed to find '{0}' for '{1}' in the Unity project. Something is horribly wrong, please reinstall AVPro Video.", PluginName, platform);
			}
		}
	}
}

#endif // (UNITY_IOS || UNITY_TVOS) && UNITY_2017_1_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RenderHeads.Media.AVProVideo.Editor
{

	[CustomPropertyDrawer(typeof(MediaPath))]
	public class MediaPathDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return 0f; }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, GUIContent.none, property);

			SerializedProperty propPath = property.FindPropertyRelative("_path");
			SerializedProperty propPathType = property.FindPropertyRelative("_pathType");

			EditorGUILayout.PropertyField(propPathType, GUIContent.none);

			//GUI.color = HttpHeader.IsValid(valueProp.stringValue)?Color.white:Color.red;
			string newUrl = EditorGUILayout.TextArea(propPath.stringValue, EditorHelper.IMGUI.GetWordWrappedTextAreaStyle());
			//GUI.color = Color.white;
			newUrl = newUrl.Trim();
			if (EditorHelper.SafeSetPathProperty(newUrl, propPath))
			{
				// TODO: shouldn't we set all targets?
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
			MediaPlayerEditor.ShowFileWarningMessages(propPath.stringValue, (MediaPathType)propPathType.enumValueIndex, null, MediaSource.Path, false, Platform.Unknown);
			GUI.color = Color.white;
			
			EditorGUI.EndProperty();
		}

		public static void ShowBrowseButton(SerializedProperty propMediaPath)
		{
			GUIContent buttonText = new GUIContent("Browse", EditorGUIUtility.IconContent("d_Project").image);
			if (GUILayout.Button(buttonText, GUILayout.ExpandWidth(true)))
			{
				RecentMenu.Create(propMediaPath, null, MediaPlayerEditor.MediaFileExtensions, false);
			}
		}

		public static void ShowBrowseButtonIcon(SerializedProperty propMediaPath, SerializedProperty propMediaSource)
		{
			if (GUILayout.Button(EditorGUIUtility.IconContent("d_Project"), GUILayout.ExpandWidth(false)))
			{
				RecentMenu.Create(propMediaPath, propMediaSource, MediaPlayerEditor.MediaFileExtensions, false, 100);
			}
		}

		public static void ShowBrowseSubtitlesButtonIcon(SerializedProperty propMediaPath)
		{
			if (GUILayout.Button(EditorGUIUtility.IconContent("d_Project"), GUILayout.ExpandWidth(false)))// GUILayout.Height(EditorGUIUtility.singleLineHeight)))
			{
				RecentMenu.Create(propMediaPath, null, MediaPlayerEditor.SubtitleFileExtensions, false);
			}
		}
	}

	[CustomPropertyDrawer(typeof(MediaHints))]
	public class MediaHintsDrawer : PropertyDrawer
	{
		private readonly static GUIContent[] StereoPackingOptions =
		{
			// NOTE: must be in the same order as enum StereoPacking
			new GUIContent("None"),
			new GUIContent("Top Bottom"),
			new GUIContent("Left Right"),
			new GUIContent("Custom UV"),
		};

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return 0f; }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, GUIContent.none, property);

			SerializedProperty propHintsTransparency = property.FindPropertyRelative("transparency");
			SerializedProperty propHintsAlphaPacking = property.FindPropertyRelative("alphaPacking");
			SerializedProperty propHintsStereoPacking = property.FindPropertyRelative("stereoPacking");

			EditorGUILayout.PropertyField(propHintsTransparency);
			if ((TransparencyMode)propHintsTransparency.enumValueIndex == TransparencyMode.Transparent)
			{
				EditorGUILayout.PropertyField(propHintsAlphaPacking);
			}

			{
				// NOTE: We don't allow selection of 'Two Textures' as this mode is only produced by the Players as it is platform specific
				propHintsStereoPacking.enumValueIndex = EditorGUILayout.Popup(new GUIContent("Stereo Packing"), propHintsStereoPacking.enumValueIndex, StereoPackingOptions);
			}

			EditorGUI.EndProperty();
		}
	}

	[CanEditMultipleObjects()]
	[CustomEditor(typeof(MediaReference))]
	public class MediaReferenceEditor : UnityEditor.Editor
	{
		internal const string SettingsPrefix = "AVProVideo-MediaReferenceEditor-";

		private SerializedProperty _propMediaPath;
		private SerializedProperty _propHints;

		private SerializedProperty _propPlatformMacOS;
		private SerializedProperty _propPlatformWindows;
		private SerializedProperty _propPlatformAndroid;
		private SerializedProperty _propPlatformIOS;
		private SerializedProperty _propPlatformTvOS;
		private SerializedProperty _propPlatformWindowsUWP;
		private SerializedProperty _propPlatformWebGL;
		//private SerializedProperty _propAlias;

		void OnEnable()
		{
			_propMediaPath = this.CheckFindProperty("_mediaPath");
			_propHints = this.CheckFindProperty("_hints");

			_propPlatformMacOS = this.CheckFindProperty("_macOS");
			_propPlatformWindows = this.CheckFindProperty("_windows");
			_propPlatformAndroid = this.CheckFindProperty("_android");
			_propPlatformIOS = this.CheckFindProperty("_iOS");
			_propPlatformTvOS = this.CheckFindProperty("_tvOS");
			_propPlatformWindowsUWP = this.CheckFindProperty("_windowsUWP");
			_propPlatformWebGL = this.CheckFindProperty("_webGL");

			//_propAlias = CheckFindProperty("_alias");
			_zoomToFill = EditorPrefs.GetBool(SettingsPrefix + "ZoomToFill", _zoomToFill);
			_thumbnailTime = EditorPrefs.GetFloat(SettingsPrefix + "ThumbnailTime", _thumbnailTime);
		}

		void OnDisable()
		{
			EndGenerateThumbnails(false);
			RemoveProgress();

			EditorPrefs.SetBool(SettingsPrefix + "ZoomToFill", _zoomToFill);
			EditorPrefs.SetFloat(SettingsPrefix + "ThumbnailTime", _thumbnailTime);
		}

		public override void OnInspectorGUI()
		{
			//MediaPlayer media = (this.target) as MediaPlayer;

			//this.DrawDefaultInspector();

			serializedObject.Update();

			GUILayout.Label("Media Reference");
			EditorGUILayout.Space();
			//EditorGUILayout.PropertyField(_propAlias);
			//EditorGUILayout.Space();

			{
				string mediaName = _propMediaPath.FindPropertyRelative("_path").stringValue;	
				GUILayout.BeginVertical(GUI.skin.box);
				MediaPlayerEditor.OnInspectorGUI_CopyableFilename(mediaName);
				GUILayout.EndVertical();
			}

			EditorGUILayout.PropertyField(_propMediaPath);

			MediaPathDrawer.ShowBrowseButton(_propMediaPath);

			EditorGUILayout.Space();

			//GUILayout.Label("Media Hints", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_propHints);

			EditorGUILayout.PropertyField(_propPlatformMacOS, new GUIContent("macOS"));
			EditorGUILayout.PropertyField(_propPlatformWindows, new GUIContent("Windows"));
			EditorGUILayout.PropertyField(_propPlatformAndroid, new GUIContent("Android"));
			EditorGUILayout.PropertyField(_propPlatformIOS, new GUIContent("iOS"));
			EditorGUILayout.PropertyField(_propPlatformTvOS, new GUIContent("tvOS"));
			EditorGUILayout.PropertyField(_propPlatformWindowsUWP, new GUIContent("UWP"));
			EditorGUILayout.PropertyField(_propPlatformWebGL, new GUIContent("WebGL"));
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			serializedObject.ApplyModifiedProperties();

			bool beginGenerateThumbnails = false;
			
			GUILayout.FlexibleSpace();
			EditorGUI.BeginDisabledGroup(IsGeneratingThumbnails());

			GUILayout.BeginHorizontal();
			_thumbnailTime = GUILayout.HorizontalSlider(_thumbnailTime, 0f, 1f, GUILayout.ExpandWidth(true));
			_zoomToFill = GUILayout.Toggle(_zoomToFill, "Zoom And Crop", GUI.skin.button, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Generate Thumbnail"))
			{
				beginGenerateThumbnails = true;
			}
			EditorGUI.EndDisabledGroup();

			if (beginGenerateThumbnails)
			{
				BeginGenerateThumbnails();
			}

			if (IsGeneratingThumbnails())
			{
				ShowProgress();
			}
			if (!IsGeneratingThumbnails())
			{
				RemoveProgress();
			}
		}

		private void ShowProgress()
		{
			// Show cancellable progress
			float t = (float)_targetIndex / (float)this.targets.Length;
			t = 0.25f + t * 0.75f;
			MediaReference media = (this.targets[_targetIndex]) as MediaReference;
			
			#if UNITY_2020_1_OR_NEWER
			if (_progressId < 0)
			{
				//Progress.RegisterCancelCallback(_progressId...)
				_progressId = Progress.Start("[AVProVideo] Generating Thumbnails...", null, Progress.Options.Managed);
			}
			Progress.Report(_progressId, t, media.MediaPath.Path);
			#else
			if (EditorUtility.DisplayCancelableProgressBar("[AVProVideo] Generating Thumbnails...", media.MediaPath.Path, t))
			{
				EndGenerateThumbnails(false);
			}
			#endif
		}

		private void RemoveProgress()
		{
			#if UNITY_2020_1_OR_NEWER
			if (_progressId >= 0)
			{
				Progress.Remove(_progressId);
				_progressId = -1;
			}
			#else
			EditorUtility.ClearProgressBar();
			#endif
		}

		#if UNITY_2020_1_OR_NEWER
		private int _progressId = -1;
		#endif
		private float _thumbnailTime;
		private bool _zoomToFill = false;
		private int _lastFrame;
		private BaseMediaPlayer _thumbnailPlayer;
		private int _mediaFrame = -1;
		private int _targetIndex = 0;
		private float _timeoutTimer = 0f;

		private bool IsGeneratingThumbnails()
		{
			return (_thumbnailPlayer != null);
		}

		private void BeginGenerateThumbnails()
		{
			EditorApplication.update -= UpdateGenerateThumbnail;

			Debug.Assert(_thumbnailPlayer == null);
			#if UNITY_EDITOR_WIN
			if (WindowsMediaPlayer.InitialisePlatform())
			{
				MediaPlayer.OptionsWindows options = new MediaPlayer.OptionsWindows();
				_thumbnailPlayer = new WindowsMediaPlayer(options);
			}
			#elif UNITY_EDITOR_OSX
			{
				MediaPlayer.OptionsApple options = new MediaPlayer.OptionsApple(MediaPlayer.OptionsApple.TextureFormat.BGRA, MediaPlayer.OptionsApple.Flags.None);
				_thumbnailPlayer = new AppleMediaPlayer(options);
			}
			#endif

			if (_thumbnailPlayer != null)
			{
				_targetIndex = 0;
				BeginNextThumbnail(0);
			}
			else
			{
				EndGenerateThumbnails(false);
			}
		}

		private void BeginNextThumbnail(int index)
		{
			EditorApplication.update -= UpdateGenerateThumbnail;
			_mediaFrame = -1;
			_timeoutTimer = 0f;

			if (_thumbnailPlayer != null)
			{
				if (index < this.targets.Length)
				{
					_targetIndex = index;
					MediaReference media = (this.targets[_targetIndex]) as MediaReference;
					string path = media.MediaPath.GetResolvedFullPath();
					bool openedMedia = false;
					if (!string.IsNullOrEmpty(path))
					{
						if (_thumbnailPlayer.OpenMedia(path, 0, string.Empty, media.Hints, 0, false))
						{
							openedMedia = true;
							EditorApplication.update += UpdateGenerateThumbnail;
						}
					}

					if (!openedMedia)
					{
						// If the media failed to open, continue to the next one
						BeginNextThumbnail(_targetIndex + 1);
					}
				}
				else
				{
					EndGenerateThumbnails(true);
				}
			}
		}

		private void EndGenerateThumbnails(bool updateAssets)
		{
			EditorApplication.update -= UpdateGenerateThumbnail;
			if (_thumbnailPlayer != null)
			{
				_thumbnailPlayer.CloseMedia();
				_thumbnailPlayer.Dispose();
				_thumbnailPlayer = null;
			}
			_mediaFrame = -1;

			if (updateAssets)
			{
				// This forces the static preview to refresh
				foreach (Object o in this.targets)
				{
					EditorUtility.SetDirty(o);
					AssetPreview.GetAssetPreview(o);
				}
				AssetDatabase.SaveAssets();
			}
		}

		private void UpdateGenerateThumbnail()
		{
			if (Time.renderedFrameCount == _lastFrame)
			{
				// In at least Unity 5.6 we have to force refresh of the UI otherwise the render thread doesn't run to update the textures
				this.Repaint();
				UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
				return;
			}

			// Wait for a frame to be rendered
			Debug.Assert(_thumbnailPlayer != null);
			if (_thumbnailPlayer != null)
			{
				_timeoutTimer += Time.unscaledDeltaTime;
				bool nextVideo = false;
				_thumbnailPlayer.Update();
				_thumbnailPlayer.Render();

				if (_mediaFrame < 0 && _thumbnailPlayer.CanPlay())
				{
					_thumbnailPlayer.MuteAudio(true);
					_thumbnailPlayer.Play();
					_thumbnailPlayer.Seek(_thumbnailPlayer.GetDuration() * _thumbnailTime);
					_mediaFrame = _thumbnailPlayer.GetTextureFrameCount();
				}
				if (_thumbnailPlayer.GetTexture() != null)
				{
					if (_mediaFrame != _thumbnailPlayer.GetTextureFrameCount() && _thumbnailPlayer.GetTextureFrameCount() > 3)
					{
						bool prevSRGB = GL.sRGBWrite;
						GL.sRGBWrite = false;

						RenderTexture rt2 = null;
						// TODO: move this all into VideoRender as a resolve method
						{
							Material materialResolve = new Material(Shader.Find(VideoRender.Shader_Resolve));
							VideoRender.SetupVerticalFlipMaterial(materialResolve, _thumbnailPlayer.RequiresVerticalFlip());
							VideoRender.SetupAlphaPackedMaterial(materialResolve, _thumbnailPlayer.GetTextureAlphaPacking());
							VideoRender.SetupGammaMaterial(materialResolve, !_thumbnailPlayer.PlayerSupportsLinearColorSpace());

							RenderTexture prev = RenderTexture.active;

							// Scale to fit and downsample
							rt2 = RenderTexture.GetTemporary(128, 128, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

							RenderTexture.active = rt2;
							GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
							ScaleMode scaleMode = ScaleMode.ScaleToFit;
							if (_zoomToFill)
							{
								scaleMode = ScaleMode.ScaleAndCrop;
							}
							VideoRender.DrawTexture(new Rect(0f, 0f, 128f, 128f), _thumbnailPlayer.GetTexture(), scaleMode, _thumbnailPlayer.GetTextureAlphaPacking(), materialResolve);
							RenderTexture.active = prev;

							Material.DestroyImmediate(materialResolve); materialResolve = null;
						}
				
						Texture2D readTexture = new Texture2D(128, 128, TextureFormat.RGBA32, true, false);
						Helper.GetReadableTexture(rt2, readTexture);
						MediaReference mediaRef = (this.targets[_targetIndex]) as MediaReference;
						mediaRef.GeneratePreview(readTexture);
						DestroyImmediate(readTexture); readTexture = null;

						RenderTexture.ReleaseTemporary(rt2);

						GL.sRGBWrite = prevSRGB;
						nextVideo = true;
						Debug.Log("Thumbnail Written");
					}
				}
				if (!nextVideo)
				{
					// If there is an error or it times out, then skip this media
					if (_timeoutTimer > 10f || _thumbnailPlayer.GetLastError() != ErrorCode.None)
					{
						MediaReference mediaRef = (this.targets[_targetIndex]) as MediaReference;
						mediaRef.GeneratePreview(null);
						nextVideo = true;
					}
				}

				if (nextVideo)
				{
					BeginNextThumbnail(_targetIndex + 1);
				}
			}
			_lastFrame = Time.renderedFrameCount;
		}

		public override bool HasPreviewGUI()
		{
			return true;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			Texture texture = RenderStaticPreview(string.Empty, null, 128, 128);
			if (texture)
			{
				GUI.DrawTexture(r, texture, ScaleMode.ScaleToFit, true);
			}
		}

		public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
		{
			MediaReference media = this.target as MediaReference;
			if (media)
			{
				bool isLinear = false;
				#if !UNITY_2018_1_OR_NEWER
				// NOTE: These older versions of Unity don't handle sRGB in the editor correctly so a workaround is to create texture as linear
				isLinear = true;
				#endif
				Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, true, isLinear);
				if (!media.GetPreview(result))
				{
					DestroyImmediate(result); result = null;
				}
				return result;
			}
			return null;
		}
	}
}
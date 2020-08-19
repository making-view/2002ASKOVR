/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

See SampleFramework license.txt for license terms.  Unless required by applicable law
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific
language governing permissions and limitations under the license.

************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using OVRTouchSample;

namespace OculusSampleFramework
{
	public class HandsManager : MonoBehaviour
	{
		private const string SKELETON_VISUALIZER_NAME = "SkeletonRenderer";

		[SerializeField] GameObject _leftHand = null;
		[SerializeField] GameObject _rightHand = null;
        [SerializeField] GameObject _leftController = null;
        [SerializeField] GameObject _rightController = null;

        public HandsVisualMode VisualMode = HandsVisualMode.Mesh;
		private OVRHand[] _hand = new OVRHand[(int)OVRHand.Hand.HandRight + 1];
		private OVRSkeleton[] _handSkeleton = new OVRSkeleton[(int)OVRHand.Hand.HandRight + 1];
		private OVRSkeletonRenderer[] _handSkeletonRenderer = new OVRSkeletonRenderer[(int)OVRHand.Hand.HandRight + 1];
		private OVRMesh[] _handMesh = new OVRMesh[(int)OVRHand.Hand.HandRight + 1];
		private OVRMeshRenderer[] _handMeshRenderer = new OVRMeshRenderer[(int)OVRHand.Hand.HandRight + 1];

        private Hand[] _controller = new Hand[2];
		private SkinnedMeshRenderer[] _skinnedMeshRenderer = new SkinnedMeshRenderer[2];
        private GestureDetector[] _gestureDetector = new GestureDetector[2];
		private GestureTeleporter[] _gestureTeleporter = new GestureTeleporter[2];
        private GameObject[] _skeletonVisual = new GameObject[2];
        private StockGrabber[] _stockGrabber = new StockGrabber[4];
		private float _currentHandAlpha = 1.0f;
		private int HandAlphaId = Shader.PropertyToID("_HandAlpha");

		public enum HandsVisualMode
		{
			Mesh = 0, Skeleton = 1, Both = 2
		}

		public OVRHand RightHand
		{
			get
			{
				return _hand[(int)OVRHand.Hand.HandRight];
			}
			private set
			{
				_hand[(int)OVRHand.Hand.HandRight] = value;
			}
		}

        public Hand RightController
        {
            get
            {
                return _controller[1];
            }
            private set
            {
                _controller[1] = value;
            }
        }

        public OVRSkeleton RightHandSkeleton
		{
			get
			{
				return _handSkeleton[(int)OVRHand.Hand.HandRight];
			}
			private set
			{
				_handSkeleton[(int)OVRHand.Hand.HandRight] = value;
			}
		}

		public OVRSkeletonRenderer RightHandSkeletonRenderer
		{
			get
			{
				return _handSkeletonRenderer[(int)OVRHand.Hand.HandRight];
			}
			private set
			{
				_handSkeletonRenderer[(int)OVRHand.Hand.HandRight] = value;
			}
		}

		public OVRMesh RightHandMesh
		{
			get
			{
				return _handMesh[(int)OVRHand.Hand.HandRight];
			}
			private set
			{
				_handMesh[(int)OVRHand.Hand.HandRight] = value;
			}
		}

		public OVRMeshRenderer RightHandMeshRenderer
		{
			get
			{
				return _handMeshRenderer[(int)OVRHand.Hand.HandRight];
			}
			private set
			{
				_handMeshRenderer[(int)OVRHand.Hand.HandRight] = value;
			}
		}

		public OVRHand LeftHand
		{
			get
			{
				return _hand[(int)OVRHand.Hand.HandLeft];
			}
			private set
			{
				_hand[(int)OVRHand.Hand.HandLeft] = value;
			}
		}

        public Hand LeftController
        {
            get
            {
                return _controller[0];
            }
            private set
            {
                _controller[0] = value;
            }
        }

        public OVRSkeleton LeftHandSkeleton
		{
			get
			{
				return _handSkeleton[(int)OVRHand.Hand.HandLeft];
			}
			private set
			{
				_handSkeleton[(int)OVRHand.Hand.HandLeft] = value;
			}
		}

		public OVRSkeletonRenderer LeftHandSkeletonRenderer
		{
			get
			{
				return _handSkeletonRenderer[(int)OVRHand.Hand.HandLeft];
			}
			private set
			{
				_handSkeletonRenderer[(int)OVRHand.Hand.HandLeft] = value;
			}
		}

		public OVRMesh LeftHandMesh
		{
			get
			{
				return _handMesh[(int)OVRHand.Hand.HandLeft];
			}
			private set
			{
				_handMesh[(int)OVRHand.Hand.HandLeft] = value;
			}
		}

		public OVRMeshRenderer LeftHandMeshRenderer
		{
			get
			{
				return _handMeshRenderer[(int)OVRHand.Hand.HandLeft];
			}
			private set
			{
				_handMeshRenderer[(int)OVRHand.Hand.HandLeft] = value;
			}
		}

        public bool IsPinchEnabled
        {
            get
            {
                return !_leftController.activeSelf && !_rightController.activeSelf 
                    &&!(_gestureDetector[0] && _gestureDetector[1]
                    && _gestureTeleporter[0] && _gestureTeleporter[1]
					&& _stockGrabber[0] && _stockGrabber[1]);
            }
        }

        public bool IsHandNeutral(OVRHand.Hand handType)
        {
            var result = true;

            if (_gestureDetector[0] && _gestureDetector[1])
            {
                var gestureDector = handType == OVRHand.Hand.HandLeft ? _gestureDetector[0] : _gestureDetector[1];
                var gestureTeleporter = handType == OVRHand.Hand.HandLeft ? _gestureTeleporter[0] : _gestureTeleporter[1];

                result = !gestureDector.IsAnyGestureActive && !gestureTeleporter.IsTargetMarkerActive;
            }

            return result;
        }

		public static HandsManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance && Instance != this)
			{
				Destroy(this);
				return;
			}
			Instance = this;

			Assert.IsNotNull(_leftHand);
			Assert.IsNotNull(_rightHand);

			LeftHand = _leftHand.GetComponent<OVRHand>();
			LeftController = _leftController.GetComponent<Hand>();
			LeftHandSkeleton = _leftHand.GetComponent<OVRSkeleton>();
			LeftHandSkeletonRenderer = _leftHand.GetComponent<OVRSkeletonRenderer>();
			LeftHandMesh = _leftHand.GetComponent<OVRMesh>();
			LeftHandMeshRenderer = _leftHand.GetComponent<OVRMeshRenderer>();

			RightHand = _rightHand.GetComponent<OVRHand>();
			RightController = _rightController.GetComponent<Hand>();
			RightHandSkeleton = _rightHand.GetComponent<OVRSkeleton>();
			RightHandSkeletonRenderer = _rightHand.GetComponent<OVRSkeletonRenderer>();
			RightHandMesh = _rightHand.GetComponent<OVRMesh>();
			RightHandMeshRenderer = _rightHand.GetComponent<OVRMeshRenderer>();
			_skinnedMeshRenderer[0] = LeftHand.GetComponent<SkinnedMeshRenderer>();
			_skinnedMeshRenderer[1] = RightHand.GetComponent<SkinnedMeshRenderer>();
			StartCoroutine(FindSkeletonVisualGameObjects());

			var detectors = FindObjectsOfType<GestureDetector>().ToList();
			var teleporters = FindObjectsOfType<GestureTeleporter>().ToList();
			var gGrabbers = FindObjectsOfType<GestureStockGrabber>().ToList();
			var cGrabbers = FindObjectsOfType<ControllerStockGrabber>().ToList();
			var leftSkelType = OVRSkeleton.SkeletonType.HandLeft;
			var leftHandType = OVRHand.Hand.HandLeft;
			var rightSkelType = OVRSkeleton.SkeletonType.HandRight;
			var rightHandType = OVRHand.Hand.HandRight;

			foreach (var teleporter in teleporters)
			{
				teleporter.Initialize();
			}

			_gestureDetector[0] = detectors.FirstOrDefault(gd => gd.skeleton.GetSkeletonType() == leftSkelType);
			_gestureDetector[1] = detectors.FirstOrDefault(gd => gd.skeleton.GetSkeletonType() == rightSkelType);

			_gestureTeleporter[0] = teleporters.FirstOrDefault(gd => gd.Skeleton.GetSkeletonType() == leftSkelType);
			_gestureTeleporter[1] = teleporters.FirstOrDefault(gd => gd.Skeleton.GetSkeletonType() == rightSkelType);

			_stockGrabber[0] = gGrabbers.FirstOrDefault(sg => sg.SkeletonType == leftSkelType);
			_stockGrabber[1] = gGrabbers.FirstOrDefault(sg => sg.SkeletonType == rightSkelType);
			_stockGrabber[2] = cGrabbers.FirstOrDefault(sg => sg.HandType == leftHandType);
			_stockGrabber[3] = cGrabbers.FirstOrDefault(sg => sg.HandType == rightHandType);
		}

		private void Update()
		{
			switch (VisualMode)
			{
				case HandsVisualMode.Mesh:
				case HandsVisualMode.Skeleton:
					_currentHandAlpha = 1.0f;
					break;
				case HandsVisualMode.Both:
					_currentHandAlpha = 0.6f;
					break;
				default:
					_currentHandAlpha = 1.0f;
					break;
			}

			_skinnedMeshRenderer[0].sharedMaterial.SetFloat(HandAlphaId, _currentHandAlpha);
			_skinnedMeshRenderer[1].sharedMaterial.SetFloat(HandAlphaId, _currentHandAlpha);
		}

        public void SetFocusOnStock(Stock stock, bool isRightHand, bool isHand)
        {
			if (isRightHand)
            {
				if (isHand)
					_stockGrabber[1].SetFocusOnStock(stock);
				else
					_stockGrabber[3].SetFocusOnStock(stock);
            }
			else
            {
				if (isHand)
					_stockGrabber[0].SetFocusOnStock(stock);
				else
					_stockGrabber[2].SetFocusOnStock(stock);
			}
        }

        public void DeFocusStock(bool isRightHand, bool isHand)
        {
			if (isRightHand)
			{
				if (isHand)
					_stockGrabber[1].DeFocus();
				else
					_stockGrabber[3].DeFocus();
			}
			else
			{
				if (isHand)
					_stockGrabber[0].DeFocus();
				else
					_stockGrabber[2].DeFocus();
			}
		}

		private IEnumerator FindSkeletonVisualGameObjects()
		{
			while (!_skeletonVisual[0] || !_skeletonVisual[1])
			{
				if (!_skeletonVisual[0])
				{
					Transform leftSkeletonVisualTransform = LeftHand.transform.Find(SKELETON_VISUALIZER_NAME);
					if (leftSkeletonVisualTransform)
					{
						_skeletonVisual[0] = leftSkeletonVisualTransform.gameObject;
					}
				}

				if (!_skeletonVisual[1])
				{
					Transform rightSkeletonVisualTransform = RightHand.transform.Find(SKELETON_VISUALIZER_NAME);
					if (rightSkeletonVisualTransform)
					{
						_skeletonVisual[1] = rightSkeletonVisualTransform.gameObject;
					}
				}
				yield return null;
			}
			SetToCurrentVisualMode();
		}

		public void SwitchVisualization()
		{
			if (!_skeletonVisual[0] || !_skeletonVisual[1])
			{
				return;
			}
			VisualMode = (HandsVisualMode)(((int)VisualMode + 1) % ((int)HandsVisualMode.Both + 1));
			SetToCurrentVisualMode();
		}

		private void SetToCurrentVisualMode()
		{
			switch (VisualMode)
			{
				case HandsVisualMode.Mesh:
					RightHandMeshRenderer.enabled = true;
					_skinnedMeshRenderer[1].enabled = true;
					_skeletonVisual[1].gameObject.SetActive(false);
					LeftHandMeshRenderer.enabled = true;
					_skinnedMeshRenderer[0].enabled = true;
					_skeletonVisual[0].gameObject.SetActive(false);
					break;
				case HandsVisualMode.Skeleton:
					RightHandMeshRenderer.enabled = false;
					_skinnedMeshRenderer[1].enabled = false;
					_skeletonVisual[1].gameObject.SetActive(true);
					LeftHandMeshRenderer.enabled = false;
					_skinnedMeshRenderer[0].enabled = false;
					_skeletonVisual[0].gameObject.SetActive(true);
					break;
				case HandsVisualMode.Both:
					RightHandMeshRenderer.enabled = true;
					_skinnedMeshRenderer[1].enabled = true;
					_skeletonVisual[1].gameObject.SetActive(true);
					LeftHandMeshRenderer.enabled = true;
					_skinnedMeshRenderer[0].enabled = true;
					_skeletonVisual[0].gameObject.SetActive(true);
					break;
				default:
					break;
			}
		}

		public static List<OVRBoneCapsule> GetCapsulesPerBone(OVRSkeleton skeleton, OVRSkeleton.BoneId boneId)
		{
			List<OVRBoneCapsule> boneCapsules = new List<OVRBoneCapsule>();
			var capsules = skeleton.Capsules;
			for (int i = 0; i < capsules.Count; ++i)
			{
				if (capsules[i].BoneIndex == (short)boneId)
				{
					boneCapsules.Add(capsules[i]);
				}
			}
			return boneCapsules;
		}

		public bool IsInitialized()
		{
			return LeftHandSkeleton && LeftHandSkeleton.IsInitialized &&
				RightHandSkeleton && RightHandSkeleton.IsInitialized &&
				LeftHandMesh && LeftHandMesh.IsInitialized &&
				RightHandMesh && RightHandMesh.IsInitialized;
		}
	}
}

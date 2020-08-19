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
        private Hand[] _controller = new Hand[2];
		private OVRSkeleton[] _handSkeleton = new OVRSkeleton[(int)OVRHand.Hand.HandRight + 1];
		private OVRSkeletonRenderer[] _handSkeletonRenderer = new OVRSkeletonRenderer[(int)OVRHand.Hand.HandRight + 1];
		private OVRMesh[] _handMesh = new OVRMesh[(int)OVRHand.Hand.HandRight + 1];
		private OVRMeshRenderer[] _handMeshRenderer = new OVRMeshRenderer[(int)OVRHand.Hand.HandRight + 1];
		private SkinnedMeshRenderer _leftMeshRenderer = null;
		private SkinnedMeshRenderer _rightMeshRenderer = null;
        private GestureDetector _leftGestureDetector = null;
        private GestureDetector _rightGestureDetector = null;
        private GestureTeleporter _leftGestureTeleporter = null;
        private GestureTeleporter _rightGestureTeleporter = null;
        private StockGrabber _leftStockGrabber = null;
        private StockGrabber _rightStockGrabber = null;
        private GameObject _leftSkeletonVisual = null;
		private GameObject _rightSkeletonVisual = null;
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
                    &&!(_leftGestureDetector && _rightGestureDetector
                    && _leftGestureTeleporter && _rightGestureTeleporter
                    && _leftStockGrabber && _rightStockGrabber);
            }
        }

        public bool IsHandNeutral(OVRHand.Hand handType)
        {
            var result = true;

            if (_leftGestureDetector && _rightGestureDetector)
            {
                var gestureDector = handType == OVRHand.Hand.HandLeft ? _leftGestureDetector : _rightGestureDetector;
                var gestureTeleporter = handType == OVRHand.Hand.HandLeft ? _leftGestureTeleporter : _rightGestureTeleporter;

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
			_leftMeshRenderer = LeftHand.GetComponent<SkinnedMeshRenderer>();
			_rightMeshRenderer = RightHand.GetComponent<SkinnedMeshRenderer>();
			StartCoroutine(FindSkeletonVisualGameObjects());

            var detectors = FindObjectsOfType<GestureDetector>().ToList();
            var teleporters = FindObjectsOfType<GestureTeleporter>().ToList();
            var grabbers = FindObjectsOfType<GestureStockGrabber>().ToList();
            var leftSkelType = OVRSkeleton.SkeletonType.HandLeft;
            var rightSkelType = OVRSkeleton.SkeletonType.HandRight;

            foreach (var teleporter in teleporters)
            {
                teleporter.Initialize();
            }

            _leftGestureDetector = detectors.FirstOrDefault(gd => gd.skeleton.GetSkeletonType() == leftSkelType);
            _rightGestureDetector = detectors.FirstOrDefault(gd => gd.skeleton.GetSkeletonType() == rightSkelType);

            _leftGestureTeleporter = teleporters.FirstOrDefault(gd => gd.Skeleton.GetSkeletonType() == leftSkelType);
            _rightGestureTeleporter = teleporters.FirstOrDefault(gd => gd.Skeleton.GetSkeletonType() == rightSkelType);

            _leftStockGrabber = grabbers.FirstOrDefault(sg => sg.SkeletonType == leftSkelType);
            _rightStockGrabber = grabbers.FirstOrDefault(sg => sg.SkeletonType == rightSkelType);
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
			_rightMeshRenderer.sharedMaterial.SetFloat(HandAlphaId, _currentHandAlpha);
			_leftMeshRenderer.sharedMaterial.SetFloat(HandAlphaId, _currentHandAlpha);
		}

        public void SetFocusOnStock(OVRHand.Hand handType, Stock stock)
        {
            if (_leftStockGrabber && _rightStockGrabber)
            {
                if (handType == OVRHand.Hand.HandLeft)
                    _leftStockGrabber.SetFocusOnStock(stock);
                else
                    _rightStockGrabber.SetFocusOnStock(stock);
            }
        }

        public void DeFocusStock(OVRHand.Hand handType)
        {
            if (_leftStockGrabber && _rightStockGrabber)
            {
                if (handType == OVRHand.Hand.HandLeft)
                    _leftStockGrabber.DeFocus();
                else
                    _rightStockGrabber.DeFocus();
            }
        }

		private IEnumerator FindSkeletonVisualGameObjects()
		{
			while (!_leftSkeletonVisual || !_rightSkeletonVisual)
			{
				if (!_leftSkeletonVisual)
				{
					Transform leftSkeletonVisualTransform = LeftHand.transform.Find(SKELETON_VISUALIZER_NAME);
					if (leftSkeletonVisualTransform)
					{
						_leftSkeletonVisual = leftSkeletonVisualTransform.gameObject;
					}
				}

				if (!_rightSkeletonVisual)
				{
					Transform rightSkeletonVisualTransform = RightHand.transform.Find(SKELETON_VISUALIZER_NAME);
					if (rightSkeletonVisualTransform)
					{
						_rightSkeletonVisual = rightSkeletonVisualTransform.gameObject;
					}
				}
				yield return null;
			}
			SetToCurrentVisualMode();
		}

		public void SwitchVisualization()
		{
			if (!_leftSkeletonVisual || !_rightSkeletonVisual)
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
					_rightMeshRenderer.enabled = true;
					_rightSkeletonVisual.gameObject.SetActive(false);
					LeftHandMeshRenderer.enabled = true;
					_leftMeshRenderer.enabled = true;
					_leftSkeletonVisual.gameObject.SetActive(false);
					break;
				case HandsVisualMode.Skeleton:
					RightHandMeshRenderer.enabled = false;
					_rightMeshRenderer.enabled = false;
					_rightSkeletonVisual.gameObject.SetActive(true);
					LeftHandMeshRenderer.enabled = false;
					_leftMeshRenderer.enabled = false;
					_leftSkeletonVisual.gameObject.SetActive(true);
					break;
				case HandsVisualMode.Both:
					RightHandMeshRenderer.enabled = true;
					_rightMeshRenderer.enabled = true;
					_rightSkeletonVisual.gameObject.SetActive(true);
					LeftHandMeshRenderer.enabled = true;
					_leftMeshRenderer.enabled = true;
					_leftSkeletonVisual.gameObject.SetActive(true);
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

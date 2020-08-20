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
        private const int LEFT = 0;
        private const int RIGHT = 1;
        private const int LEFT_C = 2;
        private const int RIGHT_C = 3;

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
        private GameObject _leftSkeletonVisual;
        private GameObject _rightSkeletonVisual;

        private Dictionary<int, Hand> _controller = new Dictionary<int, Hand>();
		private Dictionary<int, SkinnedMeshRenderer> _skinnedMeshRenderer = new Dictionary<int, SkinnedMeshRenderer>();
        private Dictionary<int, GestureDetector> _gestureDetector = new Dictionary<int, GestureDetector>();
		private Dictionary<int, GestureTeleporter> _gestureTeleporter = new Dictionary<int, GestureTeleporter>();
        private Dictionary<int, StockGrabber> _stockGrabber = new Dictionary<int, StockGrabber>();

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
			_skinnedMeshRenderer[LEFT] = LeftHand.GetComponent<SkinnedMeshRenderer>();
			_skinnedMeshRenderer[RIGHT] = RightHand.GetComponent<SkinnedMeshRenderer>();
			StartCoroutine(FindSkeletonVisualGameObjects());
		}

        private void Start()
        {
            var detectors = Resources.FindObjectsOfTypeAll<GestureDetector>().ToList();
            var teleporters = Resources.FindObjectsOfTypeAll<GestureTeleporter>().ToList();
            var gGrabbers = Resources.FindObjectsOfTypeAll<GestureStockGrabber>().ToList();
            var cGrabbers = Resources.FindObjectsOfTypeAll<ControllerStockGrabber>().ToList();
            var leftHandType = OVRSkeleton.SkeletonType.HandLeft;
            var leftControllerType = OVRHand.Hand.HandLeft;
            var rightHandType = OVRSkeleton.SkeletonType.HandRight;
            var rightControllerType = OVRHand.Hand.HandRight;

            foreach (var teleporter in teleporters)
            {
                teleporter.Initialize();
            }

            _gestureDetector.Add(LEFT, detectors.FirstOrDefault(gd => gd.skeleton != null
                && gd.skeleton.GetSkeletonType() == leftHandType));
            _gestureDetector.Add(RIGHT, detectors.FirstOrDefault(gd => gd.skeleton != null
                && gd.skeleton.GetSkeletonType() == rightHandType));

            _gestureTeleporter.Add(LEFT, teleporters.FirstOrDefault(gd => gd.Skeleton.GetSkeletonType() == leftHandType));
            _gestureTeleporter.Add(RIGHT, teleporters.FirstOrDefault(gd => gd.Skeleton.GetSkeletonType() == rightHandType));

            _stockGrabber.Add(LEFT, gGrabbers.FirstOrDefault(sg => sg.SkeletonType == leftHandType));
            _stockGrabber.Add(RIGHT, gGrabbers.FirstOrDefault(sg => sg.SkeletonType == rightHandType));
            _stockGrabber.Add(LEFT_C, cGrabbers.FirstOrDefault(sg => sg.HandType == leftControllerType));
            _stockGrabber.Add(RIGHT_C, cGrabbers.FirstOrDefault(sg => sg.HandType == rightControllerType));
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

			_skinnedMeshRenderer[LEFT].sharedMaterial.SetFloat(HandAlphaId, _currentHandAlpha);
			_skinnedMeshRenderer[RIGHT].sharedMaterial.SetFloat(HandAlphaId, _currentHandAlpha);
		}

        public void SetFocusOnStock(Stock stock, bool isRightHand, bool isHand)
        {
			if (isRightHand)
            {
				if (isHand)
					_stockGrabber[RIGHT].SetFocusOnStock(stock);
				else
					_stockGrabber[RIGHT_C].SetFocusOnStock(stock);
            }
			else
            {
				if (isHand)
					_stockGrabber[LEFT].SetFocusOnStock(stock);
				else
					_stockGrabber[LEFT_C].SetFocusOnStock(stock);
			}
        }

        public void DeFocusStock(bool isRightHand, bool isHand)
        {
			if (isRightHand)
			{
				if (isHand)
					_stockGrabber[RIGHT].DeFocus();
				else
					_stockGrabber[RIGHT_C].DeFocus();
			}
			else
			{
				if (isHand)
					_stockGrabber[LEFT].DeFocus();
				else
					_stockGrabber[LEFT_C].DeFocus();
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
					_skinnedMeshRenderer[RIGHT].enabled = true;
                    _rightSkeletonVisual.gameObject.SetActive(false);
					LeftHandMeshRenderer.enabled = true;
					_skinnedMeshRenderer[LEFT].enabled = true;
                    _leftSkeletonVisual.gameObject.SetActive(false);
					break;
				case HandsVisualMode.Skeleton:
					RightHandMeshRenderer.enabled = false;
					_skinnedMeshRenderer[RIGHT].enabled = false;
                    _rightSkeletonVisual.gameObject.SetActive(true);
					LeftHandMeshRenderer.enabled = false;
					_skinnedMeshRenderer[LEFT].enabled = false;
                    _leftSkeletonVisual.gameObject.SetActive(true);
					break;
				case HandsVisualMode.Both:
					RightHandMeshRenderer.enabled = true;
					_skinnedMeshRenderer[RIGHT].enabled = true;
                    _rightSkeletonVisual.gameObject.SetActive(true);
					LeftHandMeshRenderer.enabled = true;
					_skinnedMeshRenderer[LEFT].enabled = true;
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

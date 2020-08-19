/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	/// <summary>
	/// Spawns all interactable tools that are specified for a scene.
	/// </summary>
	public class InteractableToolsCreator : MonoBehaviour
	{
		[SerializeField] private Transform[] LeftHandTools = null;
		[SerializeField] private Transform[] RightHandTools = null; 

		private void Awake()
		{
			if (LeftHandTools != null && LeftHandTools.Length > 0)
			{
                StartCoroutine(AttachToolsToHands(LeftHandTools, false, false));
                StartCoroutine(AttachToolsToHands(LeftHandTools, false, true));
			}

			if (RightHandTools != null && RightHandTools.Length > 0)
			{
                StartCoroutine(AttachToolsToHands(RightHandTools, true, false));
                StartCoroutine(AttachToolsToHands(RightHandTools, true, true));
			}
		}

		private IEnumerator AttachToolsToHands(Transform[] toolObjects, bool isRightHand, bool isHand)
		{
			HandsManager handsManagerObj = null;
			while ((handsManagerObj = HandsManager.Instance) == null || !handsManagerObj.IsInitialized())
			{
				yield return null;
			}

			// create set of tools per hand to be safe
			HashSet<Transform> toolObjectSet = new HashSet<Transform>();
			foreach (Transform toolTransform in toolObjects)
			{
				toolObjectSet.Add(toolTransform.transform);
			}

			foreach (Transform toolObject in toolObjectSet)
			{
                if (isHand)
                {
                    OVRSkeleton handSkeletonToAttachTo =
                        isRightHand ? handsManagerObj.RightHandSkeleton : handsManagerObj.LeftHandSkeleton;
                    while (handSkeletonToAttachTo == null || handSkeletonToAttachTo.Bones == null)
                    {
                        yield return null;
                    }
                }

				AttachToolToHandTransform(toolObject, isRightHand, isHand);
			}
		}

		private void AttachToolToHandTransform(Transform tool, bool isRightHanded, bool isHand)
		{
			var newTool = Instantiate(tool).transform;
			newTool.localPosition = Vector3.zero; 
			var toolComp = newTool.GetComponent<InteractableTool>();
			toolComp.IsRightHandedTool = isRightHanded;
            toolComp.IsHandTool = isHand;
			// Initialize only AFTER settings have been applied!
			Debug.Log(isRightHanded);
			toolComp.Initialize();
		}
	}
}

﻿#region Copyright Information
// Sentience Lab Unity Framework
// (C) Sentience Lab (sentiencelab@aut.ac.nz), Auckland University of Technology, Auckland, New Zealand 
#endregion Copyright Information

using UnityEngine;
using UnityEngine.EventSystems;

namespace SentienceLab
{
	/// <summary>
	/// Component for an object that can be aimed at for teleporting.
	/// This component uses the event system.
	/// </summary>

	[AddComponentMenu("Locomotion/Teleport Target")]
	[DisallowMultipleComponent]

	public class TeleportTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		public Transform groundMarker;


		void Start()
		{
			raycaster     = null;
			raycastResult = new RaycastResult();

			FindCameraTeleporter();
			groundMarker.gameObject.SetActive(false);
		}


		void Update()
		{
			if (raycaster != null)
			{
				groundMarker.gameObject.SetActive((teleporter != null) && teleporter.IsReady());

				// If this object is still "hit" by the raycast source, update ground marker position and orientation
				raycastResult.Clear();
				BaseInputModule bim = EventSystem.current.currentInputModule;
				if (bim is GvrPointerInputModule)
				{
					raycastResult = GvrPointerInputModule.CurrentRaycastResult;
				}

				if (raycastResult.gameObject != null)
				{
					Transform hit = raycastResult.gameObject.transform;
					if ((hit.transform == this.transform) || (hit.parent == this.transform))
					{
						float yaw = raycaster.transform.rotation.eulerAngles.y;
						groundMarker.position = raycastResult.worldPosition;
						groundMarker.rotation = Quaternion.Euler(0, yaw, 0);
					}
				}
			}
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			if (teleporter != null)
			{
				groundMarker.gameObject.SetActive(false);
				teleporter.TeleportMainCamera(eventData.pointerPressRaycast.worldPosition);
			}
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			raycaster = eventData.enterEventCamera.transform;
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			raycaster = null;
			groundMarker.gameObject.SetActive(false);
		}


		protected void FindCameraTeleporter()
		{
			teleporter = null;
			Teleporter[] teleporters = FindObjectsOfType<Teleporter>();
			foreach (Teleporter t in teleporters)
			{
				Camera c = t.GetComponentInChildren<Camera>();
				if (c != null && c.gameObject.activeInHierarchy)
				{
					teleporter = t;
					break;
				}
			}
		}

		private Transform     raycaster;
		private RaycastResult raycastResult;
		private Teleporter    teleporter;
	}
}

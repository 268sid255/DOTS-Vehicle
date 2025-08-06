using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTSVehicle.Authoring.Vehicles;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DOTSVehicle.DOTS.Utils
{
	public class VehicleCamera : MonoBehaviour
	{
		private float dist = 15.0f;
		public float zoomCoef = 1.2f;
		public float currentX = 60.0f;
		public float currentY = 40.0f;
		public float currentZ = 0.0f;
		public float distanceSnapTime;
		private float zVelocity = 0.0f;
		private Vector3 zVecVelocity;
		private float usedDistance = 15.0f;
		float speed = 6;

		private EntityManager entityManager;
		protected Entity targetEntity;
		private bool isTargetFound = false;

		void Start()
		{
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		}

		void LateUpdate()
		{
			if (!isTargetFound)
			{
				FindTargetEntity();
				return;
			}

			if (isTargetFound && entityManager.Exists(targetEntity))
			{
				var targetPosition = entityManager.GetComponentData<LocalToWorld>(targetEntity).Position;
				Vector3 targetPositionVec = targetPosition;
				usedDistance = Mathf.SmoothDampAngle(usedDistance, dist + (speed * zoomCoef), ref zVelocity, distanceSnapTime);
				var dir = new Vector3(0, 0, -dist);
				var rot = Quaternion.Euler(currentX, currentY, currentZ);

				var desiredPosition = targetPositionVec + rot * dir;
				transform.position = transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref zVecVelocity, 0f);
				transform.rotation = rot;
			}
			else
			{
				isTargetFound = false;
			}

		}

		void FindTargetEntity()
		{
			var vehicleQuery = entityManager.CreateEntityQuery(typeof(CameraTag), typeof(LocalToWorld));

			if (!vehicleQuery.IsEmpty)
			{
				targetEntity = vehicleQuery.GetSingletonEntity();
				isTargetFound = true;
			}
		}
	}
}
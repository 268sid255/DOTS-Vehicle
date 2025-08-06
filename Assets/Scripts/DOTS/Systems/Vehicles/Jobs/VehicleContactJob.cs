using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTSVehicle.Components.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityRandom = UnityEngine.Random;

namespace DOTSVehicle.Systems.Vehicles.Jobs
{
	[BurstCompile]
	public unsafe partial struct VehicleContactJob : IJobEntity
	{
		[ReadOnly] public CollisionWorld collisionWorld;

		public void Execute(
			ref WheelContact contact,
			in Wheel wheel,
			in WheelSuspension suspension,
			in WheelInput input)
		{
			var colliderCastInput = new ColliderCastInput
			{
				Collider = (Collider*)wheel.collider.GetUnsafePtr(),
				Start = input.worldTransform.pos,
				End = input.worldTransform.pos - input.up * suspension.suspensionLength,
				Orientation = input.worldTransform.rot
			};

			if (!collisionWorld.CastCollider(colliderCastInput, out var hit))
			{
				contact.isHitContact = false;
				contact.hitDistance = suspension.suspensionLength;
				return;
			}

			contact.isHitContact = true;
			contact.hitPoint = hit.Position;
			contact.hitNormal = hit.SurfaceNormal;
			contact.hitDistance = hit.Fraction * suspension.suspensionLength;
			contact.entity = hit.Entity;
		}
	}
}
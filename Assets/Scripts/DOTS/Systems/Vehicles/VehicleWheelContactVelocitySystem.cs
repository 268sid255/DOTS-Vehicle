using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTSVehicle.Components.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DOTSVehicle.DOTS.Systems.Vehicles
{
	[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
	// [UpdateAfter(typeof(PhysicsSimulationGroup))]
	public partial struct VehicleWheelContactVelocitySystem : ISystem
	{
		private ComponentLookup<WheelContactVelocity> velocityLookup;
		private ComponentLookup<WheelContact> contactLookup;
		public void OnCreate(ref SystemState state)
		{
			velocityLookup = state.GetComponentLookup<WheelContactVelocity>(false);
			contactLookup = state.GetComponentLookup<WheelContact>(true);
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			velocityLookup.Update(ref state);
			contactLookup.Update(ref state);
			ExecuteWheelContactVelocitySystem(ref state);
		}

		void ExecuteWheelContactVelocitySystem(ref SystemState state)
		{
			var vehicleWheelContactVelocityJob = new VehicleWheelContactVelocityJob
			{
				contactLookup = contactLookup,
				contactVeloLookup = velocityLookup
			};

			state.Dependency = vehicleWheelContactVelocityJob.Schedule(state.Dependency);
		}
	}

	[BurstCompile]
	public partial struct VehicleWheelContactVelocityJob : IJobEntity
	{
		public ComponentLookup<WheelContactVelocity> contactVeloLookup;
		[ReadOnly] public ComponentLookup<WheelContact> contactLookup;

		public void Execute(
			in DynamicBuffer<VehicleWheelElement> wheels,
			in PhysicsVelocity velocity,
			in PhysicsMass mass,
			in LocalTransform transform)
		{
			foreach (var wheel in wheels)
			{
				var wheelEntity = wheel.wheelEntity;

				if (!contactLookup.HasComponent(wheelEntity)) continue;

				contactLookup.TryGetComponent(wheelEntity, out var contact);
				contactVeloLookup.TryGetComponent(wheelEntity, out var componentData);

				if (contact.isHitContact && contactVeloLookup.HasComponent(wheelEntity))
				{
					var velo = velocity.GetLinearVelocity(mass, transform.Position, transform.Rotation, contact.hitPoint);
					componentData.value = velo;
					contactVeloLookup[wheelEntity] = componentData;
				}

			}
		}
	}
}
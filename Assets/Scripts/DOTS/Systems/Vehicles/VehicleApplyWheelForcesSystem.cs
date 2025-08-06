using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTSVehicle.Components.Vehicles;
using DOTSVehicle.Systems.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DOTSVehicle.DOTS.Systems.Vehicles
{
	[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
	[UpdateAfter(typeof(VehicleWheelSimulationSystem))]
	public partial struct VehicleApplyWheelForcesSystem : ISystem
	{
		private ComponentLookup<WheelOutput> wheelOutputLookup;
		private ComponentLookup<WheelControllable> wheelControlLookup;
		private ComponentLookup<WheelContact> wheelContactLookup;
		public void OnCreate(ref SystemState state)
		{
			wheelOutputLookup = state.GetComponentLookup<WheelOutput>(true);
			wheelContactLookup = state.GetComponentLookup<WheelContact>(true);
			wheelControlLookup = state.GetComponentLookup<WheelControllable>(true);
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			wheelOutputLookup.Update(ref state);
			wheelContactLookup.Update(ref state);
			wheelControlLookup.Update(ref state);
			ExecuteVehicleApplyWheelForcesSystem(ref state);
		}

		void ExecuteVehicleApplyWheelForcesSystem(ref SystemState state)
		{
			var applyWheelForcesJob = new ApplyWheelForceJob
			{
				wheelContactLookup = wheelContactLookup,
				wheelOutputLookup = wheelOutputLookup,
				wheelControllableLookup = wheelControlLookup
			};

			state.Dependency = applyWheelForcesJob.Schedule(state.Dependency);
		}
	}

	[BurstCompile]
	public partial struct ApplyWheelForceJob : IJobEntity
	{
		[ReadOnly] public ComponentLookup<WheelOutput> wheelOutputLookup;
		[ReadOnly] public ComponentLookup<WheelContact> wheelContactLookup;
		[ReadOnly] public ComponentLookup<WheelControllable> wheelControllableLookup;

		public void Execute(
			ref PhysicsVelocity velocity,
			ref VehicleOutput output,
			in DynamicBuffer<VehicleWheelElement> wheels,
			in PhysicsMass mass,
			in LocalTransform transform)
		{
			output.maxWheelRotationSpeed = 0.0f;
			output.averageWheelRotationSpeed = 0.0f;

			var driveCount = 0;

			for (int i = 0; i < wheels.Length; i++)
			{
				var wheelEntity = wheels[i].wheelEntity;

				if (!wheelOutputLookup.HasComponent(wheelEntity) || !wheelContactLookup.HasComponent(wheelEntity))
					continue;

				var wheelOutput = wheelOutputLookup[wheelEntity];
				var wheelContact = wheelContactLookup[wheelEntity];

				if (wheelContact.isHitContact)
				{
					velocity.ApplyImpulse(mass, transform.Position, transform.Rotation, wheelOutput.suspensionForce + wheelOutput.frictionForce, wheelContact.hitPoint);
				}

				if (wheelControllableLookup.HasComponent(wheelEntity))
				{
					var wc = wheelControllableLookup[wheelEntity];
					if (wc.driveRate > 0f)
					{
						output.maxWheelRotationSpeed = math.max(output.maxWheelRotationSpeed, math.abs(wheelOutput.rotationSpeed));
						driveCount++;
						output.averageWheelRotationSpeed += wheelOutput.rotationSpeed;
					}
				}

				output.localVelocity = math.rotate(math.inverse(transform.Rotation.value), velocity.Linear);
			}

			output.averageWheelRotationSpeed /= driveCount;
		}
	}
}
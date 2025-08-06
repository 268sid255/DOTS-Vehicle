using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTSVehicle.Components.Vehicles;
using DOTSVehicle.DOTS.Authoring.Vehicles;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DOTSVehicle.DOTS.Systems.Vehicles
{
	[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
	public partial struct VehicleSteeringSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			ExecuteVehicleSteeringSystem(ref state);
		}	

		void ExecuteVehicleSteeringSystem(ref SystemState state)
		{
			var vehicleSteerJob = new VehicleSteeringJob
			{
				deltaTime = SystemAPI.Time.DeltaTime
			};
			state.Dependency = vehicleSteerJob.Schedule(state.Dependency);
		}
	}


	[BurstCompile]
	public partial struct VehicleSteeringJob : IJobEntity
	{
		public float deltaTime;
		public void Execute(
			ref PhysicsVelocity velocity,
			in VehicleInput input,
			in VehicleSteering steer,
			in LocalTransform localTransform)
		{
			var steeringAbs = math.abs(input.steering);

			var rotationSpeed = velocity.Angular.y;
			if (rotationSpeed * input.steering < 0)
			{
				var counter = steeringAbs * math.clamp(input.throttle, 0, 1) * steer.counterSteeringRate;
				velocity.Angular.y = math.lerp(velocity.Angular.y, 0, deltaTime * counter);
			}

			var forwardMove = math.abs(math.dot(math.forward(localTransform.Rotation.value), math.normalizesafe(velocity.Linear)));
			var steerRate = math.saturate((0.1f - steeringAbs) * 10f);
			var forwardStab = steerRate * forwardMove * steer.forwardStabilizationRate;
			velocity.Angular.y = math.lerp(velocity.Angular.y, 0, deltaTime * forwardStab);
		}
	}
}
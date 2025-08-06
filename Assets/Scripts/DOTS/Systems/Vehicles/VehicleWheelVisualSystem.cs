using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTSVehicle.Components.Vehicles;
using DOTSVehicle.Systems.Vehicles.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DOTSVehicle.Systems.Vehicles
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
	[UpdateBefore(typeof(TransformSystemGroup))]
	public partial struct VehicleWheelVisualSystem : ISystem
	{
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<WheelInput>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			ExecuteVehicleWheelVisualSystem(ref state);
		}

		void ExecuteVehicleWheelVisualSystem(ref SystemState state)
		{
			var vehicleWheelVisualJob = new VehicleWheelVisualJob() { };

			state.Dependency = vehicleWheelVisualJob.Schedule(state.Dependency);
		}
	}
}
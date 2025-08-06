using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTSVehicle.Components.Vehicles;
using DOTSVehicle.DOTS.Systems.Vehicles;
using DOTSVehicle.Systems.Vehicles.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using static Unity.Entities.SystemAPI;


namespace DOTSVehicle.Systems.Vehicles
{
	[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
	[UpdateAfter(typeof(VehicleWheelContactVelocitySystem))]
	public partial struct VehicleWheelSimulationSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			ExecuteVehicleWheelSimulationSystem(ref state);
		}

		void ExecuteVehicleWheelSimulationSystem(ref SystemState state)
		{
			var dt = Time.DeltaTime;
			var vehicleWheelSimulationJob = new VehicleWheelSimulationJob
			{
				deltaTime = dt
			};

			state.Dependency = vehicleWheelSimulationJob.Schedule(state.Dependency);
		}
	}
}
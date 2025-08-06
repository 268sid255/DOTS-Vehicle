using DOTSVehicle.Components.Vehicles;
using DOTSVehicle.Systems.Vehicles.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using static Unity.Entities.SystemAPI;

namespace DOTSVehicle.Systems.Vehicles
{
	[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
	[UpdateAfter(typeof(VehicleWheelsSystem))]
	// [UpdateAfter(typeof(PhysicsSimulationGroup))]
	public partial struct VehicleContactSystem : ISystem
	{
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PhysicsWorldSingleton>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			ExecuteVehicleContactSystem(ref state);
		}

		void ExecuteVehicleContactSystem(ref SystemState state)
		{
			var collisionWorld = GetSingleton<PhysicsWorldSingleton>().PhysicsWorld.CollisionWorld;
			var vehicleContactJob = new VehicleContactJob
			{
				collisionWorld = collisionWorld
			};

			state.Dependency = vehicleContactJob.Schedule(state.Dependency);
		}
	}
}
using DOTSVehicle.Systems.Vehicles.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using static Unity.Entities.SystemAPI;

namespace DOTSVehicle.Systems.Vehicles
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
	public partial struct VehicleInputSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			ExecuteVehicleInputSystem(ref state);
		}

		void ExecuteVehicleInputSystem(ref SystemState state)
		{
			var movementInput = UnityEngine.Input.GetAxis("Vertical");
        	var steeringInput = UnityEngine.Input.GetAxis("Horizontal");
        	var handbrakeInput = UnityEngine.Input.GetKey(UnityEngine.KeyCode.Space) ? 1 : 0;
        	var deltaTime = Time.DeltaTime;

			var vehicleInputJob = new VehicleInputJob
			{
				movementInput = movementInput,
				steeringInput = steeringInput,
				handbrakeInput = handbrakeInput,
				deltaTime = deltaTime
			};

			state.Dependency = vehicleInputJob.Schedule(state.Dependency);
		}
	}
}
using DOTSVehicle.Authoring.Vehicles;
using DOTSVehicle.Components.Vehicles;
using DOTSVehicle.Systems.Vehicles.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using static Unity.Entities.SystemAPI;


namespace DOTSVehicle.Systems.Vehicles
{
	[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
	public partial struct VehicleWheelsSystem : ISystem
	{
		private ComponentLookup<WheelOrigin> originLookup;
		private ComponentLookup<WheelControllable> controlLookup;
		private ComponentLookup<WheelInput> wheelInputLookup;
		private ComponentLookup<WheelOutput> wheelOutputLookup;
		private ComponentLookup<EntityName> nameLookup;

		public void OnCreate(ref SystemState state)
		{
			originLookup = state.GetComponentLookup<WheelOrigin>(true);
			controlLookup = state.GetComponentLookup<WheelControllable>(true);
			wheelInputLookup = state.GetComponentLookup<WheelInput>(false);
			wheelOutputLookup = state.GetComponentLookup<WheelOutput>(false);
			nameLookup = state.GetComponentLookup<EntityName>(true);
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			originLookup.Update(ref state);
			controlLookup.Update(ref state);
			wheelInputLookup.Update(ref state);
			wheelOutputLookup.Update(ref state);
			nameLookup.Update(ref state);
			ExecuteVehicleWheelSystem(ref state);
		}

		void ExecuteVehicleWheelSystem(ref SystemState state)
		{
			var vehicleWheelJob = new VehicleWheelJob
			{
				originLookup = originLookup,
				controlLookup = controlLookup,
				wheelInputLookup = wheelInputLookup,
				wheelOutputLookup = wheelOutputLookup,
				nameLookup = nameLookup
			};

			state.Dependency = vehicleWheelJob.Schedule(state.Dependency);
		}
	}
}
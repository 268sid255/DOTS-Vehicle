using Unity.Entities;
using Unity.Mathematics;

namespace DOTSVehicle.Components.Vehicles
{
	public struct VehicleOutput : IComponentData
	{
		public float engineRotationRate;
		public float maxWheelRotationSpeed;
		public float averageWheelRotationSpeed;
		public float3 localVelocity;
	}
}
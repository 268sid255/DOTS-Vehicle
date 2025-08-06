using Unity.Entities;
using Unity.Mathematics;

namespace DOTSVehicle.Components.Vehicles
{
	public struct WheelContactVelocity : IComponentData
	{
		public float3 value;
	}
}
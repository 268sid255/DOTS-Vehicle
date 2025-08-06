using Unity.Entities;
using Unity.Mathematics;

namespace DOTSVehicle.Components.Vehicles
{
	public struct WheelOutput : IComponentData
	{
		public float3 suspensionForce;
		public float3 frictionForce;
		public float rotation;
		public float rotationSpeed;
		public float slip;
	}
}
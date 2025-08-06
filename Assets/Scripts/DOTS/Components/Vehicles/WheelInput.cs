using Unity.Entities;
using Unity.Mathematics;

namespace DOTSVehicle.Components.Vehicles
{
	public struct WheelInput : IComponentData
	{
		public float3 up;
		public RigidTransform localTransform;
		public RigidTransform worldTransform;
		public float massMultiplier;
		public float torque;
		public float brake;
		public float handbrake;
	}
}
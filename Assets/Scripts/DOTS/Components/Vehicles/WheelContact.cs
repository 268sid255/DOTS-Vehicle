using Unity.Entities;
using Unity.Mathematics;

namespace DOTSVehicle.Components.Vehicles
{
	public struct WheelContact : IComponentData
	{
		public float3 hitPoint;
		public float3 hitNormal;
		public float hitDistance;
		public bool isHitContact;
		public Entity entity;
	}
}
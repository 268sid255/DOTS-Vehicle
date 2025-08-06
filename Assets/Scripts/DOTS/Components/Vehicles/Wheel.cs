using Unity.Entities;
using Unity.Physics;

namespace DOTSVehicle.Components.Vehicles
{
	public struct Wheel : IComponentData
	{
		public float wheelRadius;
		public float wheelWidth;
		public float inertia;
		public BlobAssetReference<Collider> collider;
	}
}
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSVehicle.Components.Vehicles
{
	public struct WheelOrigin : IComponentData
	{
		public RigidTransform value;
	}
}
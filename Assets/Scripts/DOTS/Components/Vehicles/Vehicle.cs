using Unity.Entities;
using Unity.Collections;

namespace DOTSVehicle.Components.Vehicles
{
	public struct Vehicle : IComponentData
	{
		//public FixedList128Bytes<Entity> wheels;
	}

	public struct VehicleWheelElement : IBufferElementData
	{
		public Entity wheelEntity;
	}
}

using DOTSVehicle.Enums;
using Unity.Entities;

namespace DOTSVehicle.Components.Vehicles
{
	public struct VehicleInput : IComponentData
	{
		public float steering;
		public float throttle;
		public float brake;
		public float handbrake;
		public float load;
		public ThrottleMode throttleMode;
	}
}
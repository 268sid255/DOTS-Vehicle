using Unity.Entities;

namespace DOTSVehicle.Components.Vehicles
{
	public struct WheelBrakes : IComponentData
	{
		public float brakeTorque;
		public float handbrakeTorque;
	}
}
using Unity.Entities;

namespace DOTSVehicle.Components.Vehicles
{
	public struct WheelControllable : IComponentData
	{
		public float maxSteeringAngle;
		public float driveRate;
		public float brakeRate;
		public float handbrakeRate;
	}
}
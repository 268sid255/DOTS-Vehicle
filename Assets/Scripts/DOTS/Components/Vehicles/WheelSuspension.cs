using Unity.Entities;

namespace DOTSVehicle.Components.Vehicles
{
	public struct WheelSuspension : IComponentData
	{
		public float suspensionLength;
		public float suspensionStiffness;
		public float suspensionDamping;
	}
}
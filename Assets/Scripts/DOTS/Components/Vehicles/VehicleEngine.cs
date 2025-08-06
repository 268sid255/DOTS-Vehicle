using Unity.Entities;
using DOTSVehicle.Utils;

namespace DOTSVehicle.Components.Vehicles
{
	public struct VehicleEngine : IComponentData
	{
		public BlobAssetReference<AnimationCurveBlob> torque;
		public float transmissionRate;
	}
}
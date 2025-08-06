using DOTSVehicle.Utils;
using Unity.Entities;

namespace DOTSVehicle.Components.Vehicles
{
	public struct WheelFriction : IComponentData
	{
		public BlobAssetReference<AnimationCurveBlob> longitudinal;
		public BlobAssetReference<AnimationCurveBlob> lateral;
	}
}
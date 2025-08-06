using DOTSVehicle.Components.Vehicles;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSVehicle.Systems.Vehicles.Jobs
{
	[BurstCompile]
	public partial struct VehicleWheelVisualJob : IJobEntity
	{
		public void Execute(
			ref LocalTransform transform,
			in WheelInput input,
			in WheelContact contact,
			in WheelOutput output)
		{
			var origin = input.localTransform;

			transform.Position = origin.pos - math.rotate(origin.rot, math.up()) * contact.hitDistance;

			var wheelRotation = quaternion.AxisAngle(math.right(), output.rotation);
			transform.Rotation = math.mul(origin.rot, wheelRotation);
		}
	}
}
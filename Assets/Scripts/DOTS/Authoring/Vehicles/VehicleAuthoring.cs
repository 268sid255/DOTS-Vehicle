using System.Collections.Generic;
using DOTSVehicle.Components.Vehicles;
using DOTSVehicle.Utils;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSVehicle.Authoring.Vehicles
{
	public struct CameraTag : IComponentData { }

	public class VehicleAuthoring : MonoBehaviour
	{
		public List<GameObject> wheels;

		[Header("Engine")]
		public AnimationCurve torque;
		public float transmissionRate;

		private class Baker : Baker<VehicleAuthoring>
		{
			public override void Bake(VehicleAuthoring authoring)
			{
				var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

				AddComponent<CameraTag>(entity);

				var vehicle = new Vehicle();
				//Add Vehicle
				AddComponent(entity, vehicle);

				var wheelBuffer = AddBuffer<VehicleWheelElement>(entity);

				foreach (var wheel in authoring.wheels)
				{
					if (wheel != null)
					{
						wheelBuffer.Add(new VehicleWheelElement
						{
							wheelEntity = GetEntity(wheel, TransformUsageFlags.Dynamic)
						});
					}
				}

				//Add Vehicle Input
				AddComponent<VehicleInput>(entity);
				//Add Vehicle Output
				AddComponent<VehicleOutput>(entity);

				var torqueBlob = AnimationCurveBlob.Build(authoring.torque, 128, Allocator.Persistent);
				AddBlobAsset(ref torqueBlob, out _);

				var vehicleEngine = new VehicleEngine
				{
					torque = torqueBlob,
					transmissionRate = authoring.transmissionRate
				};

				//Add Vehicle Engine
				AddComponent(entity, vehicleEngine);
			}
		}
	}
}
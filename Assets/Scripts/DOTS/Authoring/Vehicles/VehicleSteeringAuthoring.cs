using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DOTSVehicle.DOTS.Authoring.Vehicles
{
	public class VehicleSteeringAuthoring : MonoBehaviour
	{
		public float counterSteeringRate = 5;
		public float forwardStabilizationRate = 5;

		private class Baker : Baker<VehicleSteeringAuthoring>
		{
			public override void Bake(VehicleSteeringAuthoring authoring)
			{
				var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

				var vehicleSteer = new VehicleSteering
				{
					counterSteeringRate = authoring.counterSteeringRate,
					forwardStabilizationRate = authoring.forwardStabilizationRate
				};

				AddComponent(entity, vehicleSteer);
			}
		}
	}

	public struct VehicleSteering : IComponentData
	{
		public float counterSteeringRate;
		public float forwardStabilizationRate;
	}
}
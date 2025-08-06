using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTSVehicle.Components.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using DOTSVehicle.Enums;
using Unity.Mathematics;

namespace DOTSVehicle.Systems.Vehicles.Jobs
{
	[BurstCompile]
	public partial struct VehicleInputJob : IJobEntity
	{
		public float movementInput;
		public float steeringInput;
		public float handbrakeInput;
		public float deltaTime;
		
		public void Execute(
			ref VehicleInput vehicleInput,
			in VehicleOutput output)
		{
			var throttleInput = movementInput;
			var brakeInput = 0.0f;

			switch (vehicleInput.throttleMode)
			{
				case ThrottleMode.AccelerationForward:
					if (throttleInput < 0)
					{
						vehicleInput.throttleMode = ThrottleMode.Braking;
					}
					break;
				case ThrottleMode.AccelerationBackward:
					if (throttleInput > 0)
					{
						vehicleInput.throttleMode = ThrottleMode.Braking;
					}
					break;
				case ThrottleMode.Braking:
					if (output.localVelocity.z * movementInput > 0 || math.abs(output.localVelocity.z) < 0.1f)
					{
						vehicleInput.throttleMode = movementInput > 0 ? ThrottleMode.AccelerationForward : ThrottleMode.AccelerationBackward;
						break;
					}

					throttleInput = 0.0f;
					brakeInput = math.abs(movementInput);
					break;
			}

			vehicleInput.load = math.abs(throttleInput);
			vehicleInput.steering = Mathf.MoveTowards(vehicleInput.steering, steeringInput, deltaTime * 4f);
			vehicleInput.throttle = Mathf.MoveTowards(vehicleInput.throttle, throttleInput, deltaTime * 4f);
			vehicleInput.brake = Mathf.MoveTowards(vehicleInput.brake, brakeInput, deltaTime * 4f);
			vehicleInput.handbrake = Mathf.MoveTowards(vehicleInput.handbrake, handbrakeInput, deltaTime * 10);
		}
	}
}
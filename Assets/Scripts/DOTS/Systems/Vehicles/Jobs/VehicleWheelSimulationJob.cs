using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTSVehicle.Components.Vehicles;
using DOTSVehicle.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DOTSVehicle.Systems.Vehicles.Jobs
{
	[BurstCompile]
	public partial struct VehicleWheelSimulationJob : IJobEntity
	{
		public float deltaTime;
		public void Execute(
			ref WheelOutput output,
			in Wheel wheel,
			in WheelContact contact,
			in WheelInput input,
			in WheelSuspension suspension,
			in WheelContactVelocity cv,
			in WheelFriction friction,
			in WheelBrakes brakes)
		{
			output.frictionForce = float3.zero;
			output.suspensionForce = float3.zero;

			var brakeTorque = (brakes.brakeTorque * input.brake + brakes.handbrakeTorque * input.handbrake) * input.massMultiplier;
			var engineTorque = input.torque;

			if (brakeTorque > 0)
			{
				var torqueAbs = math.abs(input.torque);
				if (torqueAbs < brakeTorque)
				{
					brakeTorque -= torqueAbs;
					engineTorque = 0.0f;
				}
				else
				{
					engineTorque -= brakeTorque * math.sign(engineTorque);
					brakeTorque = 0.0f;
				}
			}

			output.rotationSpeed += (engineTorque / wheel.inertia) * deltaTime;

			if (!contact.isHitContact)
			{
				ApplyBrakeTorque(ref output, wheel.inertia, brakeTorque, deltaTime);
				output.rotation += output.rotationSpeed * deltaTime;
				return;
			}

			//suspension
			var suspensionDelta = suspension.suspensionLength - contact.hitDistance;
			var suspensionForceValue = suspensionDelta * suspension.suspensionStiffness * input.massMultiplier;
			var suspensionDampingSpeed = math.dot(input.up, cv.value);
			var suspensionDampingForce = suspensionDampingSpeed * suspension.suspensionDamping * input.massMultiplier;

			suspensionForceValue -= suspensionDampingForce;
			if (suspensionForceValue < 0) suspensionForceValue = 0;

			var suspensionForce = suspensionForceValue * contact.hitNormal;

			output.suspensionForce = suspensionForce * deltaTime;

			//friction

			float combinedSlip = 0.0f;
			if (suspensionForceValue > 0)
			{
				var lateralDirection = math.rotate(input.worldTransform.rot, math.right());
				var longitudinalDirection = math.normalizesafe(math.cross(lateralDirection, contact.hitNormal));

				var lateralSpeed = math.dot(lateralDirection, cv.value);
				var longitudinalSpeed = math.dot(longitudinalDirection, cv.value);

				var wheelDeltaSpeed = longitudinalSpeed - output.rotationSpeed.RotationToLinearSpeed(wheel.wheelRadius);
				var wheelDeltaSpeedSign = math.sign(wheelDeltaSpeed);
				var wheelDeltaSpeedAbs = wheelDeltaSpeedSign * wheelDeltaSpeed;

				var lateralSpeedSign = math.sign(lateralSpeed);
				var lateralSpeedAbs = lateralSpeedSign * lateralSpeed;

				var longitudinalTimeRange = friction.longitudinal.Value.TimeRange;
				var longitudinalSlip = math.saturate(math.unlerp(longitudinalTimeRange.x, longitudinalTimeRange.y, wheelDeltaSpeedAbs));
				var lateralTimeRange = friction.lateral.Value.TimeRange;
				var lateralSlip = math.saturate(math.unlerp(lateralTimeRange.x, lateralTimeRange.y, lateralSpeedAbs));

				combinedSlip = math.max(longitudinalSlip, lateralSlip);
				var fakeSlip = math.lerp(combinedSlip, 1, input.handbrake);

				var lateralFrictionRate = friction.lateral.Value.Evaluate(math.lerp(lateralTimeRange.x, lateralTimeRange.y, fakeSlip));

				var longitudinalFrictionRate = friction.lateral.Value.Evaluate(math.lerp(longitudinalTimeRange.x, longitudinalTimeRange.y, fakeSlip));

				var lateralForce = (-lateralSpeedSign * lateralFrictionRate * suspensionForceValue * Bias(math.saturate(lateralSpeedAbs), -1)) * lateralDirection;
				var longitudinalBias = Bias(math.saturate(wheelDeltaSpeedAbs), -1);

				var longitudinalFrictionForceValue = -wheelDeltaSpeedSign * longitudinalFrictionRate * longitudinalBias * suspensionForceValue;

				var brakeForceValue = brakeTorque.TorqueToForce(wheel.wheelRadius);
				var usedForceValue = brakeForceValue > math.abs(longitudinalFrictionForceValue) ? longitudinalFrictionForceValue : brakeForceValue * math.sign(longitudinalFrictionForceValue);

				brakeForceValue -= math.abs(usedForceValue);

				longitudinalFrictionForceValue -= usedForceValue;

				var neutralForce = (-wheelDeltaSpeed.LinearToRotationSpeed(wheel.wheelRadius) * wheel.inertia / deltaTime).TorqueToForce(wheel.wheelRadius);

				var usedForceValue2 = math.abs(neutralForce) > math.abs(longitudinalFrictionForceValue) ? longitudinalFrictionForceValue : neutralForce;

				output.rotationSpeed -= usedForceValue2.ForceToTorque(wheel.wheelRadius) / wheel.inertia * deltaTime;
				usedForceValue += usedForceValue2;

				brakeTorque = brakeForceValue > 0 ? brakeForceValue.ForceToTorque(wheel.wheelRadius) : 0;

				var longitudinalForce = usedForceValue * longitudinalDirection;

				output.frictionForce = (lateralForce + longitudinalForce) * deltaTime;
			}

			if (brakeTorque > 0)
			{
				ApplyBrakeTorque(ref output, wheel.inertia, brakeTorque, deltaTime);
			}

			output.rotation += output.rotationSpeed * deltaTime;
			output.slip = combinedSlip;			
		}

		private static void ApplyBrakeTorque(ref WheelOutput output, float wheelInertia, float brakeTorque, float deltaTime)
		{
			if (brakeTorque <= 0) return;
			var zeroTorque = -output.rotationSpeed * wheelInertia / deltaTime;
			var zeroTorqueAbs = math.abs(zeroTorque);

			var usedBrakeTorque = zeroTorqueAbs < brakeTorque ? zeroTorqueAbs : brakeTorque;
			output.rotationSpeed += math.sign(zeroTorque) * usedBrakeTorque / wheelInertia * deltaTime;
		}

		private static float Bias(float x, float bias)
		{
			var k = math.pow(1 - bias, 3);
			return (x * k) / (x * k - x + 1);
		}
	}
}
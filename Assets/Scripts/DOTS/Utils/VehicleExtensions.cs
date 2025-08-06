using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DOTSVehicle.Components.Vehicles;
using Unity.Mathematics;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DOTSVehicle.Utils
{
	public static class VehicleExtensions
	{
		private static readonly float wheelRotationSpeedToEngineRpm = 60 / (2 * math.PI);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float LinearToRotationSpeed(this float speed, float radius)
		{
			return speed / radius;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float RotationToLinearSpeed(this float speed, float radius)
		{
			return speed * radius;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float TorqueToForce(this float torque, float radius)
		{
			return torque / radius;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ForceToTorque(this float speed, float radius)
		{
			return speed * radius;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EvaluateTorque(this VehicleEngine engine, float rpm)
		{
			return engine.torque.Value.Evaluate(rpm);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float TransmitWheelRotationSpeedToEngineRpm(this VehicleEngine engine, float speed)
		{
			return speed * engine.transmissionRate * wheelRotationSpeedToEngineRpm;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float TransmitEngineTorqueToWheel(this VehicleEngine engine, float torque)
		{
			return torque * engine.transmissionRate;
		}
	}
}
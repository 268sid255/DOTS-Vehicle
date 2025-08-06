using DOTSVehicle.Components.Vehicles;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using DOTSVehicle.Utils;
using Unity.Collections;
using UnityEngine;
using System.Diagnostics;
using DOTSVehicle.Authoring.Vehicles;


namespace DOTSVehicle.Systems.Vehicles.Jobs
{
	[BurstCompile]
	public partial struct VehicleWheelJob : IJobEntity
	{
		[ReadOnly] public ComponentLookup<WheelOrigin> originLookup;
		[ReadOnly] public ComponentLookup<WheelControllable> controlLookup;
		[NativeDisableParallelForRestriction]
		public ComponentLookup<WheelInput> wheelInputLookup;
		public ComponentLookup<WheelOutput> wheelOutputLookup;
		[ReadOnly] public ComponentLookup<EntityName> nameLookup;

		public void Execute(
			ref VehicleOutput output,
			in DynamicBuffer<VehicleWheelElement> wheels,
			in PhysicsMass mass,
			in VehicleInput input,
			in VehicleEngine engine,
			in LocalTransform transform)
		{
			var engineToRPM = engine.TransmitWheelRotationSpeedToEngineRpm(math.abs(output.maxWheelRotationSpeed));
			output.engineRotationRate = engineToRPM / engine.torque.Value.TimeRange.y;
			var engineTorque = engine.EvaluateTorque(engineToRPM) * input.throttle;
			var wheelsTorque = engine.TransmitEngineTorqueToWheel(engineTorque);

			var chassisTransform = new RigidTransform(transform.Rotation, transform.Position);

			for (int i = 0; i < wheels.Length; i++)
			{
				var wheelEntity = wheels[i].wheelEntity;

				if (!originLookup.HasComponent(wheelEntity) || !controlLookup.HasComponent(wheelEntity))
					continue;

				originLookup.TryGetComponent(wheelEntity, out var origin);
				controlLookup.TryGetComponent(wheelEntity, out var control);

				var localTransform = origin.value;
	
				localTransform.rot = math.mul(localTransform.rot, quaternion.AxisAngle(math.up(), input.steering * control.maxSteeringAngle));

				var wheelTransform = math.mul(chassisTransform, localTransform);

				if (wheelInputLookup.HasComponent(wheelEntity))
				{
					var wheelInput = new WheelInput
					{
						localTransform = localTransform,
						worldTransform = wheelTransform,
						up = math.rotate(wheelTransform.rot, math.up()),
						massMultiplier = 1.0f / mass.InverseMass,
						torque = wheelsTorque * control.driveRate,
						brake = control.brakeRate * input.brake,
						handbrake = control.handbrakeRate * input.handbrake
					};

					wheelInputLookup[wheelEntity] = wheelInput;
				}

				if (control.driveRate > 0 && wheelOutputLookup.HasComponent(wheelEntity))
				{
					var wheelOutput = wheelOutputLookup[wheelEntity];
					wheelOutput.rotationSpeed = output.averageWheelRotationSpeed;
					wheelOutputLookup[wheelEntity] = wheelOutput;
				}
			}
		}
	}
}
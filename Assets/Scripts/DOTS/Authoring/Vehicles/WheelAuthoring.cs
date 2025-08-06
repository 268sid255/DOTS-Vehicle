using DOTSVehicle.Components.Vehicles;
using DOTSVehicle.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace DOTSVehicle.Authoring.Vehicles
{
	public class WheelAuthoring : MonoBehaviour
	{
		[Header("Wheel Setting")]
		public float wheelRadius = 0.3f;
		public float wheelWidth = 0.2f;
		public float wheelMass = 20;
		public PhysicsCategoryTags belongsTo;
		public PhysicsCategoryTags collidesWith;

		[Header("Suspension Setting")]
		public float suspensionLenght = 0.15f;
		public float suspensionStiffness;
		public float suspensionDamping;

		[Header("Friction")]
		public AnimationCurve longitudinal;
		public AnimationCurve lateral;

		[Header("Brakes")]
		public float brakeTorque;
		public float handbrakeTorque;

		[Header("Controls")]
		public float maxSteeringAngle;
		public float driveRate;
		public float brakeRate;
		public float handbrakeRate;

		private class Baker : Baker<WheelAuthoring>
		{
			public override void Bake(WheelAuthoring authoring)
			{
				var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

				AddComponent(entity, new EntityName { Value = authoring.name });
				
				var wheelCollider = CylinderCollider.Create(new CylinderGeometry
				{
					Center = float3.zero,
					Height = authoring.wheelWidth,
					Radius = authoring.wheelRadius,
					BevelRadius = 0.1f,
					SideCount = 12,
					Orientation = quaternion.AxisAngle(math.up(), math.PI * 0.5f)
				}, new CollisionFilter
				{
					BelongsTo = authoring.belongsTo.Value,
					CollidesWith = authoring.collidesWith.Value
				});
				AddBlobAsset(ref wheelCollider, out var hash);

				var wheel = new Wheel
				{
					wheelRadius = authoring.wheelRadius,
					wheelWidth = authoring.wheelWidth,
					inertia = authoring.wheelMass * authoring.wheelRadius * authoring.wheelRadius * 0.5f,
					collider = wheelCollider
				};

				//Add Wheel
				AddComponent(entity, wheel);

				var wheelOrigin = new WheelOrigin
				{
					value = new RigidTransform(authoring.transform.localRotation, authoring.transform.localPosition)
				};

				//Add Wheel Origin
				AddComponent(entity, wheelOrigin);
				//Add Wheel Input
				AddComponent<WheelInput>(entity);

				//Add Wheel Contact
				AddComponent<WheelContact>(entity);

				var wheelSuspension = new WheelSuspension
				{
					suspensionLength = authoring.suspensionLenght,
					suspensionStiffness = authoring.suspensionStiffness,
					suspensionDamping = authoring.suspensionDamping
				};

				//Add Wheel Suspension
				AddComponent(entity, wheelSuspension);
				//Add Wheel Contact Velocity
				AddComponent<WheelContactVelocity>(entity);
				//Add Wheel Output
				AddComponent<WheelOutput>(entity);

				var wheelControl = new WheelControllable
				{
					maxSteeringAngle = math.radians(authoring.maxSteeringAngle),
					driveRate = authoring.driveRate,
					brakeRate = authoring.brakeRate,
					handbrakeRate = authoring.handbrakeRate
				};
				//Add Wheel Controllable
				AddComponent(entity, wheelControl);

				var longitudinal = AnimationCurveBlob.Build(authoring.longitudinal, 128, Allocator.Persistent);
				var lateral = AnimationCurveBlob.Build(authoring.lateral, 128, Allocator.Persistent);
				AddBlobAsset(ref longitudinal, out _);
				AddBlobAsset(ref lateral, out _);

				var wheelFriction = new WheelFriction
				{
					longitudinal = longitudinal,
					lateral = lateral
				};
				//Add Wheel Friction
				AddComponent(entity, wheelFriction);

				var wheelBrakes = new WheelBrakes
				{
					brakeTorque = authoring.brakeTorque,
					handbrakeTorque = authoring.handbrakeTorque
				};
				//Add Wheel Brakes
				AddComponent(entity, wheelBrakes);
			}
		}
	}
	
	public struct EntityName : IComponentData
	{
		public FixedString64Bytes Value;
	}
}
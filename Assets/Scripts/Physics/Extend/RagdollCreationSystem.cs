using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using static Unity.Physics.Math;

namespace Anvil.Physics
{
    public partial class RagdollCreationSystem : SystemBase
    {
        public NativeList<BlobAssetReference<Collider>> CreatedColliders;

        protected override void OnCreate()
        {
            CreatedColliders = new NativeList<BlobAssetReference<Collider>>(Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            foreach (var collider in CreatedColliders)
            {
                if (collider.IsCreated)
                    collider.Dispose();
            }

            CreatedColliders.Dispose();
        }

        private Entity CreateRagdollByBoneData(RagdollBoneData data, CollisionFilter filter)
        {
            BlobAssetReference<Collider> collider = CapsuleCollider.Create(new CapsuleGeometry
            {
                Vertex0 = data.Vertex0,
                Vertex1 = data.Vertex1,
                Radius = data.Radius
            }, filter);
            CreatedColliders.Add(collider);
            return CreateDynamicBody(data.position, data.rotation, collider, float3.zero, float3.zero, 5.0f);
        }

        public void CreateRagdoll(RagdollConfigData ragdollConfigData, float3 positionOffset, quaternion rotationOffset, CollisionFilter filter, float rangeGain = 1.0f)
        {
            var entities = new NativeList<Entity>(Allocator.Temp);
            var rangeModifier = new float2(math.max(0, math.min(rangeGain, 1)));

            Entity head = CreateRagdollByBoneData(ragdollConfigData.Head, filter);
            entities.Add(head);
            Entity torso = CreateRagdollByBoneData(ragdollConfigData.Torso, filter);
            entities.Add(torso);

            float headRadius = 0.1f;
            float3 headPosition = ragdollConfigData.Head.position;
            float3 torsoSize = new float3(1.0f, 1.0f, 1.0f);
            float3 torsoPosition = ragdollConfigData.Torso.position;
            // Neck
            {
                float3 pivotHead = new float3(0, -headRadius, 0);
                float3 pivotTorso = math.transform(math.inverse(GetBodyTransform(torso)), math.transform(GetBodyTransform(head), pivotHead));
                float3 axisHead = new float3(0, 0, 1);
                float3 perpendicular = new float3(1, 0, 0);
                FloatRange coneAngle = new FloatRange(math.radians(0), math.radians(45)) * rangeModifier;
                FloatRange perpendicularAngle = new FloatRange(math.radians(-30), math.radians(+30)) * rangeModifier;
                FloatRange twistAngle = new FloatRange(math.radians(-5), math.radians(5)) * rangeModifier;

                var axisTorso = math.rotate(math.inverse(GetBodyTransform(torso).rot), math.rotate(GetBodyTransform(head).rot, axisHead));
                axisTorso = math.rotate(quaternion.AxisAngle(perpendicular, math.radians(10)), axisTorso);

                var headFrame = new BodyFrame { Axis = axisHead, PerpendicularAxis = perpendicular, Position = pivotHead };
                var torsoFrame = new BodyFrame { Axis = axisTorso, PerpendicularAxis = perpendicular, Position = pivotTorso };

                PhysicsJoint.CreateRagdoll(headFrame, torsoFrame, coneAngle.Max, perpendicularAngle, twistAngle, out var ragdoll0, out var ragdoll1);
                CreateJoint(ragdoll0, head, torso);
                CreateJoint(ragdoll1, head, torso);
            }

            // Arms
            {
                float armLength = 0.25f;
                float armRadius = 0.05f;

                BlobAssetReference<Collider> upperArmCollider = CapsuleCollider.Create(new CapsuleGeometry
                {
                    Vertex0 = new float3(-armLength / 2, 0, 0),
                    Vertex1 = new float3(armLength / 2, 0, 0),
                    Radius = armRadius
                }, filter);
                BlobAssetReference<Collider> foreArmCollider = CapsuleCollider.Create(new CapsuleGeometry
                {
                    Vertex0 = new float3(-armLength / 2, 0, 0),
                    Vertex1 = new float3(armLength / 2, 0, 0),
                    Radius = armRadius
                }, filter);

                float handLength = 0.025f;
                float handRadius = 0.055f;

                BlobAssetReference<Collider> handCollider = CapsuleCollider.Create(new CapsuleGeometry
                {
                    Vertex0 = new float3(-handLength / 2, 0, 0),
                    Vertex1 = new float3(handLength / 2, 0, 0),
                    Radius = handRadius
                }, filter);

                CreatedColliders.Add(upperArmCollider);
                CreatedColliders.Add(foreArmCollider);
                CreatedColliders.Add(handCollider);


                for (int i = 0; i < 2; i++)
                {
                    float s = i * 2 - 1.0f;

                    float3 upperArmPosition = torsoPosition + new float3(s * (torsoSize.x + armLength) / 2.0f, 0.9f * torsoSize.y - armRadius, 0.0f);
                    Entity upperArm = CreateDynamicBody(upperArmPosition, quaternion.identity, upperArmCollider, float3.zero, float3.zero, 10.0f);
                    float3 foreArmPosition = upperArmPosition + new float3(armLength * s, 0, 0);
                    Entity foreArm = CreateDynamicBody(foreArmPosition, quaternion.identity, foreArmCollider, float3.zero, float3.zero, 5.0f);
                    float3 handPosition = foreArmPosition + new float3((armLength + handLength) / 2.0f * s, 0, 0);
                    Entity hand = CreateDynamicBody(handPosition, quaternion.identity, handCollider, float3.zero, float3.zero, 2.0f);

                    entities.Add(upperArm);
                    entities.Add(foreArm);
                    entities.Add(hand);

                    // shoulder
                    {
                        float3 pivotArm = new float3(-s * armLength / 2.0f, 0, 0);
                        float3 pivotTorso = math.transform(math.inverse(GetBodyTransform(torso)), math.transform(GetBodyTransform(upperArm), pivotArm));
                        float3 axisArm = new float3(-s, 0, 0);
                        float3 perpendicularArm = new float3(0, 1, 0);
                        FloatRange coneAngle = new FloatRange(math.radians(0), math.radians(80)) * rangeModifier;
                        FloatRange perpendicularAngle = new FloatRange(math.radians(-70), math.radians(20)) * rangeModifier;
                        FloatRange twistAngle = new FloatRange(math.radians(-5), math.radians(5)) * rangeModifier;

                        var axisTorso = math.rotate(math.inverse(GetBodyTransform(torso).rot), math.rotate(GetBodyTransform(upperArm).rot, axisArm));
                        axisTorso = math.rotate(quaternion.AxisAngle(perpendicularArm, math.radians(-s * 45.0f)), axisTorso);

                        var armFrame = new BodyFrame { Axis = axisArm, PerpendicularAxis = perpendicularArm, Position = pivotArm };
                        var bodyFrame = new BodyFrame { Axis = axisTorso, PerpendicularAxis = perpendicularArm, Position = pivotTorso };

                        PhysicsJoint.CreateRagdoll(armFrame, bodyFrame, coneAngle.Max, perpendicularAngle, twistAngle, out var ragdoll0, out var ragdoll1);
                        CreateJoint(ragdoll0, upperArm, torso);
                        CreateJoint(ragdoll1, upperArm, torso);
                    }

                    // elbow
                    {
                        float3 pivotUpper = new float3(s * armLength / 2.0f, 0, 0);
                        float3 pivotFore = -pivotUpper;
                        float3 axis = new float3(0, -s, 0);
                        float3 perpendicular = new float3(-s, 0, 0);

                        var lowerArmFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotFore };
                        var upperArmFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotUpper };
                        var hingeRange = new FloatRange(math.radians(0), math.radians(100));
                        hingeRange = (hingeRange - new float2(hingeRange.Mid)) * rangeModifier + hingeRange.Mid;
                        PhysicsJoint hinge = PhysicsJoint.CreateLimitedHinge(lowerArmFrame, upperArmFrame, hingeRange);
                        CreateJoint(hinge, foreArm, upperArm);
                    }

                    // wrist
                    {
                        float3 pivotFore = new float3(s * armLength / 2.0f, 0, 0);
                        float3 pivotHand = new float3(-s * handLength / 2.0f, 0, 0);
                        float3 axis = new float3(0, 0, -s);
                        float3 perpendicular = new float3(0, 0, 1);

                        var handFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotHand };
                        var forearmFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotFore };
                        var hingeRange = new FloatRange(math.radians(0), math.radians(135)) * rangeModifier;
                        PhysicsJoint hinge = PhysicsJoint.CreateLimitedHinge(handFrame, forearmFrame, hingeRange);
                        CreateJoint(hinge, hand, foreArm);
                    }
                }
            }

            // Pelvis
            float pelvisRadius = 0.08f;
            float pelvisLength = 0.22f;
            float3 pelvisPosition = torsoPosition - new float3(0, pelvisRadius * 0.75f, 0.0f);
            Entity pelvis;
            {
                BlobAssetReference<Collider> collider = CapsuleCollider.Create(new CapsuleGeometry
                {
                    Vertex0 = new float3(-pelvisLength / 2, 0, 0),
                    Vertex1 = new float3(pelvisLength / 2, 0, 0),
                    Radius = pelvisRadius
                }, filter);
                pelvis = CreateDynamicBody(pelvisPosition, quaternion.identity, collider, float3.zero, float3.zero, 15.0f);
                CreatedColliders.Add(collider);
            }
            entities.Add(pelvis);

            // Waist
            {
                float3 pivotTorso = float3.zero;
                float3 pivotPelvis = math.transform(math.inverse(GetBodyTransform(pelvis)), math.transform(GetBodyTransform(torso), pivotTorso));
                float3 axis = new float3(0, 1, 0);
                float3 perpendicular = new float3(0, 0, 1);
                FloatRange coneAngle = new FloatRange(math.radians(0), math.radians(5)) * rangeModifier;
                FloatRange perpendicularAngle = new FloatRange(math.radians(-5), math.radians(90)) * rangeModifier;
                FloatRange twistAngle = new FloatRange(-math.radians(-5), math.radians(5)) * rangeModifier;

                var pelvisFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotPelvis };
                var torsoFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotTorso };
                PhysicsJoint.CreateRagdoll(pelvisFrame, torsoFrame, coneAngle.Max, perpendicularAngle, twistAngle, out var ragdoll0, out var ragdoll1);
                CreateJoint(ragdoll0, pelvis, torso);
                CreateJoint(ragdoll1, pelvis, torso);
            }

            // Legs
            {
                float thighLength = 0.32f;
                float thighRadius = 0.08f;
                BlobAssetReference<Collider> thighCollider = CapsuleCollider.Create(new CapsuleGeometry
                {
                    Vertex0 = new float3(0, -thighLength / 2, 0),
                    Vertex1 = new float3(0, thighLength / 2, 0),
                    Radius = thighRadius
                }, filter);

                float calfLength = 0.32f;
                float calfRadius = 0.06f;
                BlobAssetReference<Collider> calfCollider = CapsuleCollider.Create(new CapsuleGeometry
                {
                    Vertex0 = new float3(0, -calfLength / 2, 0),
                    Vertex1 = new float3(0, calfLength / 2, 0),
                    Radius = calfRadius
                }, filter);

                float footLength = 0.08f;
                float footRadius = 0.06f;
                BlobAssetReference<Collider> footCollider = CapsuleCollider.Create(new CapsuleGeometry
                {
                    Vertex0 = new float3(0),
                    Vertex1 = new float3(0, 0, footLength),
                    Radius = footRadius
                }, filter);

                CreatedColliders.Add(thighCollider);
                CreatedColliders.Add(calfCollider);
                CreatedColliders.Add(footCollider);

                for (int i = 0; i < 2; i++)
                {
                    float s = i * 2 - 1.0f;

                    float3 thighPosition = pelvisPosition + new float3(s * pelvisLength / 2.0f, -thighLength / 2.0f, 0.0f);
                    Entity thigh = CreateDynamicBody(thighPosition, quaternion.identity, thighCollider, float3.zero, float3.zero, 10.0f);
                    float3 calfPosition = thighPosition + new float3(0, -(thighLength + calfLength) / 2.0f, 0);
                    Entity calf = CreateDynamicBody(calfPosition, quaternion.identity, calfCollider, float3.zero, float3.zero, 5.0f);
                    float3 footPosition = calfPosition + new float3(0, -calfLength / 2.0f, 0);
                    Entity foot = CreateDynamicBody(footPosition, quaternion.identity, footCollider, float3.zero, float3.zero, 2.0f);

                    entities.Add(thigh);
                    entities.Add(calf);
                    entities.Add(foot);

                    // hip
                    {
                        float3 pivotThigh = new float3(0, thighLength / 2.0f, 0);
                        float3 pivotPelvis = math.transform(math.inverse(GetBodyTransform(pelvis)), math.transform(GetBodyTransform(thigh), pivotThigh));
                        float3 axisLeg = new float3(0, -1, 0);
                        float3 perpendicularLeg = new float3(-s, 0, 0);
                        FloatRange coneAngle = new FloatRange(math.radians(0), math.radians(60)) * rangeModifier;
                        FloatRange perpendicularAngle = new FloatRange(math.radians(-10), math.radians(40)) * rangeModifier;
                        FloatRange twistAngle = new FloatRange(-math.radians(5), math.radians(5)) * rangeModifier;

                        var axisPelvis = math.rotate(math.inverse(GetBodyTransform(pelvis).rot), math.rotate(GetBodyTransform(thigh).rot, axisLeg));
                        axisPelvis = math.rotate(quaternion.AxisAngle(perpendicularLeg, math.radians(s * 45.0f)), axisPelvis);

                        var upperLegFrame = new BodyFrame { Axis = axisLeg, PerpendicularAxis = perpendicularLeg, Position = pivotThigh };
                        var pelvisFrame = new BodyFrame { Axis = axisPelvis, PerpendicularAxis = perpendicularLeg, Position = pivotPelvis };

                        PhysicsJoint.CreateRagdoll(upperLegFrame, pelvisFrame, coneAngle.Max, perpendicularAngle, twistAngle, out var ragdoll0, out var ragdoll1);
                        CreateJoint(ragdoll0, thigh, pelvis);
                        CreateJoint(ragdoll1, thigh, pelvis);
                    }

                    // knee
                    {
                        float3 pivotThigh = new float3(0, -thighLength / 2.0f, 0);
                        float3 pivotCalf = math.transform(math.inverse(GetBodyTransform(calf)), math.transform(GetBodyTransform(thigh), pivotThigh));
                        float3 axis = new float3(-1, 0, 0);
                        float3 perpendicular = new float3(0, 0, 1);

                        var lowerLegFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotCalf };
                        var upperLegFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotThigh };
                        var hingeRange = new FloatRange(math.radians(-90), math.radians(0));
                        hingeRange = (hingeRange - new float2(hingeRange.Mid)) * rangeModifier + hingeRange.Mid;
                        PhysicsJoint hinge = PhysicsJoint.CreateLimitedHinge(lowerLegFrame, upperLegFrame, hingeRange);
                        CreateJoint(hinge, calf, thigh);
                    }

                    // ankle
                    {
                        float3 pivotCalf = new float3(0, -calfLength / 2.0f, 0);
                        float3 pivotFoot = float3.zero;
                        float3 axis = new float3(-1, 0, 0);
                        float3 perpendicular = new float3(0, 0, 1);

                        var footFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotFoot };
                        var lowerLegFrame = new BodyFrame { Axis = axis, PerpendicularAxis = perpendicular, Position = pivotCalf };
                        var hingeRange = new FloatRange(math.radians(-5), math.radians(5)) * rangeModifier;
                        PhysicsJoint hinge = PhysicsJoint.CreateLimitedHinge(footFrame, lowerLegFrame, hingeRange);
                        CreateJoint(hinge, foot, calf);
                    }
                }
            }

            // reposition with offset information
            if (entities.Length > 0)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    var e = entities[i];

                    Translation positionComponent = EntityManager.GetComponentData<Translation>(e);
                    Rotation rotationComponent = EntityManager.GetComponentData<Rotation>(e);
                    PhysicsVelocity velocityComponent = EntityManager.GetComponentData<PhysicsVelocity>(e);

                    float3 position = positionComponent.Value;
                    quaternion rotation = rotationComponent.Value;

                    float3 localPosition = position - pelvisPosition;
                    localPosition = math.rotate(rotationOffset, localPosition);

                    position = localPosition + pelvisPosition + positionOffset;
                    rotation = math.mul(rotation, rotationOffset);

                    positionComponent.Value = position;
                    rotationComponent.Value = rotation;

                    EntityManager.SetComponentData<PhysicsVelocity>(e, velocityComponent);
                    EntityManager.SetComponentData<Translation>(e, positionComponent);
                    EntityManager.SetComponentData<Rotation>(e, rotationComponent);
                }
            }
        }

        #region Utilities

        public Entity CreateBody(float3 position, quaternion orientation, BlobAssetReference<Collider> collider,
            float3 linearVelocity, float3 angularVelocity, float mass, bool isDynamic)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            Entity entity = entityManager.CreateEntity(new ComponentType[] { });

            entityManager.AddComponentData(entity, new LocalToWorld { });
            entityManager.AddComponentData(entity, new Translation { Value = position });
            entityManager.AddComponentData(entity, new Rotation { Value = orientation });

            var colliderComponent = new PhysicsCollider { Value = collider };
            entityManager.AddComponentData(entity, colliderComponent);

            EntityManager.AddSharedComponentData(entity, new PhysicsWorldIndex());

            if (isDynamic)
            {
                entityManager.AddComponentData(entity, PhysicsMass.CreateDynamic(colliderComponent.MassProperties, mass));

                float3 angularVelocityLocal = math.mul(math.inverse(colliderComponent.MassProperties.MassDistribution.Transform.rot), angularVelocity);
                entityManager.AddComponentData(entity, new PhysicsVelocity
                {
                    Linear = linearVelocity,
                    Angular = angularVelocityLocal
                });
                entityManager.AddComponentData(entity, new PhysicsDamping
                {
                    Linear = 0.01f,
                    Angular = 0.05f
                });
            }

            return entity;
        }

        public Entity CreateStaticBody(float3 position, quaternion orientation, BlobAssetReference<Collider> collider)
        {
            return CreateBody(position, orientation, collider, float3.zero, float3.zero, 0.0f, false);
        }

        public Entity CreateDynamicBody(float3 position, quaternion orientation, BlobAssetReference<Collider> collider,
            float3 linearVelocity, float3 angularVelocity, float mass)
        {
            return CreateBody(position, orientation, collider, linearVelocity, angularVelocity, mass, true);
        }

        public Entity CreateJoint(PhysicsJoint joint, Entity entityA, Entity entityB, bool enableCollision = false)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            ComponentType[] componentTypes =
            {
            typeof(PhysicsConstrainedBodyPair),
            typeof(PhysicsJoint)
        };
            Entity jointEntity = entityManager.CreateEntity(componentTypes);

            entityManager.SetComponentData(jointEntity, new PhysicsConstrainedBodyPair(entityA, entityB, enableCollision));
            entityManager.SetComponentData(jointEntity, joint);

            EntityManager.AddSharedComponentData(jointEntity, new PhysicsWorldIndex());

            return jointEntity;
        }

        public static RigidTransform GetBodyTransform(Entity entity)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            return new RigidTransform(
                entityManager.GetComponentData<Rotation>(entity).Value,
                entityManager.GetComponentData<Translation>(entity).Value);
        }

        #endregion
    }
}

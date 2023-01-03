using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;

namespace Anvil.Editor
{
    public class RagdollConfigTool : OdinEditorWindow
    {
        private static string ms_RagdollFilePath = "Assets/Resources/Scriptable/Ragdoll";

        [LabelText("根节点/Root")]
        public Transform RootTransform;
        [LabelText("骨盆/Pelvis")]
        public Transform PelvisTransform;
        [LabelText("左臀部/LeftHips")]
        public Transform LeftHipsTransform;
        [LabelText("左膝盖/LeftKnee")]
        public Transform LeftKneeTransform;
        [LabelText("左脚踝/LeftAnkle")]
        public Transform LeftAnkleTransform;
        [LabelText("右臀部/RightHips")]
        public Transform RightHipsTransform;
        [LabelText("右膝盖/RightKnee")]
        public Transform RightKneeTransform;
        [LabelText("右脚踝/RightAnkle")]
        public Transform RightAnkleTransform;
        [LabelText("躯干/Torso")]
        public Transform TorsoTransform;
        [LabelText("头部/Head")]
        public Transform HeadTransform;
        [LabelText("左肩膀/LeftShoulder")]
        public Transform LeftShoulderTransform;
        [LabelText("左臂肘/LeftElbow")]
        public Transform LeftElbowTransform;
        [LabelText("左手腕/LeftWrist")]
        public Transform LeftWristTransform;
        [LabelText("右肩膀/RightShoulder")]
        public Transform RightShoulderTransform;
        [LabelText("右臂肘/RightElbow")]
        public Transform RightElbowTransform;
        [LabelText("右手腕/RightWrist")]
        public Transform RightWristTransform;

        [LabelText("布娃娃配置文件名称")]
        public string RagdollConfigName;

        //Root
        //Pelvis
        //LeftHips
        //LeftKnee
        //LeftAnkle
        //RightHips
        //RightKnee
        //RightAnkle
        //Torso
        //Head
        //LeftShoulder
        //LeftElbow
        //LeftWrist
        //RightShoulder
        //RightElbow
        //RightWrist

        private GameObject RagdollGroup;

        private GameObject Pelvis;
        private GameObject LeftHips;
        private GameObject LeftKnee;
        private GameObject LeftAnkle;
        private GameObject RightHips;
        private GameObject RightKnee;
        private GameObject RightAnkle;
        private GameObject Torso;
        private GameObject Head;
        private GameObject LeftShoulder;
        private GameObject LeftElbow;
        private GameObject LeftWrist;
        private GameObject RightShoulder;
        private GameObject RightElbow;
        private GameObject RightWrist;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearTempRagdoll();
        }

        [MenuItem("Anvil/RagdollConfig")]
        private static void OpenWindow()
        {
            var window = GetWindow<RagdollConfigTool>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
            window.titleContent = new GUIContent("Ragdoll Config Tool");
        }

        [Button("创建布娃娃")]
        private void CreateRagdollSkeleton()
        {
            ClearTempRagdoll();

            RagdollGroup = new GameObject("Ragdoll Config GameObject");
            RagdollGroup.transform.position = RootTransform.position;
            RagdollGroup.transform.rotation = RootTransform.rotation;
            RagdollGroup.transform.localScale = RootTransform.localScale;

            Pelvis = GetRagdollConfigObject("Pelvis", PelvisTransform);
            LeftHips = GetRagdollConfigObject("LeftHips", LeftHipsTransform);
            LeftKnee = GetRagdollConfigObject("LeftKnee", LeftKneeTransform);
            LeftAnkle = GetRagdollConfigObject("LeftAnkle", LeftAnkleTransform);
            RightHips = GetRagdollConfigObject("RightHips", RightHipsTransform);
            RightKnee = GetRagdollConfigObject("RightKnee", RightKneeTransform);
            RightAnkle = GetRagdollConfigObject("RightAnkle", RightAnkleTransform);
            Torso = GetRagdollConfigObject("Torso", TorsoTransform);
            Head = GetRagdollConfigObject("Head", HeadTransform);
            LeftShoulder = GetRagdollConfigObject("LeftShoulder", LeftShoulderTransform);
            LeftElbow = GetRagdollConfigObject("LeftElbow", LeftElbowTransform);
            LeftWrist = GetRagdollConfigObject("LeftWrist", LeftWristTransform);
            RightShoulder = GetRagdollConfigObject("RightShoulder", RightShoulderTransform);
            RightElbow = GetRagdollConfigObject("RightElbow", RightElbowTransform);
            RightWrist = GetRagdollConfigObject("RightWrist", RightWristTransform);

            SetHumanoidRagdoll();
        }

        [Button("通过数据创建布娃娃")]
        private void CreateRagdollSkeletonFromData()
        {
            if (RagdollConfigName.IsNullOrWhitespace())
            {
                Debug.Log("config path is empty or null or only whitespace");
                return;
            }

            var data = GetRagdollConfigData(false);
            if (data == null)
            {
                Debug.Log("config data doesn't exist");
                return;
            }

            if (RootTransform == null)
            {
                Debug.Log("you need to set root transform first");
                return;
            }

            RagdollGroup = new GameObject("Ragdoll Config GameObject");
            RagdollGroup.transform.position = RootTransform.position;
            RagdollGroup.transform.rotation = RootTransform.rotation;
            RagdollGroup.transform.localScale = RootTransform.localScale;

            Pelvis = GetRagdollConfigObject("Pelvis", data.Pelvis);
            LeftHips = GetRagdollConfigObject("LeftHips", data.LeftHips);
            LeftKnee = GetRagdollConfigObject("LeftKnee", data.LeftKnee);
            LeftAnkle = GetRagdollConfigObject("LeftAnkle", data.LeftAnkle);
            RightHips = GetRagdollConfigObject("RightHips", data.RightHips);
            RightKnee = GetRagdollConfigObject("RightKnee", data.RightKnee);
            RightAnkle = GetRagdollConfigObject("RightAnkle", data.RightAnkle);
            Torso = GetRagdollConfigObject("Torso", data.Torso);
            Head = GetRagdollConfigObject("Head", data.Head);
            LeftShoulder = GetRagdollConfigObject("LeftShoulder", data.LeftShoulder);
            LeftElbow = GetRagdollConfigObject("LeftElbow", data.LeftElbow);
            LeftWrist = GetRagdollConfigObject("LeftWrist", data.LeftWrist);
            RightShoulder = GetRagdollConfigObject("RightShoulder", data.RightShoulder);
            RightElbow = GetRagdollConfigObject("RightElbow", data.RightElbow);
            RightWrist = GetRagdollConfigObject("RightWrist", data.RightWrist);

            SetHumanoidRagdoll();
        }

        private GameObject GetRagdollConfigObject(string name, Transform transform)
        {
            Debug.Assert(transform != null);
            GameObject gameObject = new GameObject(name);
            gameObject.transform.position = transform.position;
            gameObject.transform.rotation = transform.rotation;
            gameObject.transform.localScale = transform.localScale;
            {
                var comp = gameObject.AddComponent<ConvertToEntity>();
                comp.ConversionMode = ConvertToEntity.Mode.ConvertAndDestroy;
            }
            {
                var comp = gameObject.AddComponent<PhysicsShapeAuthoring>();
                Debug.Assert(comp != null);
                comp.SetCapsule(new CapsuleGeometryAuthoring { });
            }
            {
                var comp = gameObject.AddComponent<PhysicsBodyAuthoring>();
                Debug.Assert(comp != null);
                comp.MotionType = BodyMotionType.Dynamic;
            }

            gameObject.transform.SetParent(RagdollGroup.transform);

            return gameObject;
        }

        private GameObject GetRagdollConfigObject(string name, Physics.RagdollBoneData data)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.position = data.position;
            gameObject.transform.rotation = data.rotation;
            gameObject.transform.localScale = Vector3.one;
            {
                var comp = gameObject.AddComponent<ConvertToEntity>();
                comp.ConversionMode = ConvertToEntity.Mode.ConvertAndDestroy;
            }
            {
                var comp = gameObject.AddComponent<PhysicsShapeAuthoring>();
                Debug.Assert(comp != null);
                comp.SetCapsule(CapsuleGeometryAuthoringExtensions.ToAuthoring(new CapsuleGeometry
                {
                    Vertex0 = data.Vertex0,
                    Vertex1 = data.Vertex1,
                    Radius = data.Radius,
                }));
            }
            {
                var comp = gameObject.AddComponent<PhysicsBodyAuthoring>();
                Debug.Assert(comp != null);
                comp.MotionType = BodyMotionType.Dynamic;
            }

            gameObject.transform.SetParent(RagdollGroup.transform);

            return gameObject;
        }

        private RagdollJoint AddRagdollJoint(GameObject tarObject, GameObject connectedObject, float maxConeAngle, float minPerpendicularAngle, float maxPerpendicularAngle, float minTwistAngle, float maxTwistAngle)
        {
            var comp = tarObject.AddComponent<RagdollJoint>();
            comp.ConnectedBody = connectedObject.GetComponent<PhysicsBodyAuthoring>();
            comp.EnableCollision = true;
            comp.AutoSetConnected = true;

            comp.PositionLocal = float3.zero;
            comp.PositionInConnectedEntity = float3.zero;
            comp.TwistAxisLocal = float3.zero;
            comp.TwistAxisInConnectedEntity = float3.zero;
            comp.PerpendicularAxisLocal = float3.zero;
            comp.PerpendicularAxisInConnectedEntity = float3.zero;

            comp.MaxConeAngle = maxConeAngle;
            comp.MinPerpendicularAngle = minPerpendicularAngle;
            comp.MaxPerpendicularAngle = maxPerpendicularAngle;
            comp.MinTwistAngle = minTwistAngle;
            comp.MaxTwistAngle = maxTwistAngle;
            return comp;
        }

        private LimitedHingeJoint AddLimitedHinge(GameObject tarObject, GameObject connectedObject, float minAngle, float maxAngle)
        {
            var comp = tarObject.AddComponent<LimitedHingeJoint>();
            comp.ConnectedBody = connectedObject.GetComponent<PhysicsBodyAuthoring>();
            comp.EnableCollision = true;
            comp.AutoSetConnected = true;

            comp.PositionLocal = float3.zero;
            comp.PositionInConnectedEntity = float3.zero;
            comp.HingeAxisLocal = float3.zero;
            comp.HingeAxisInConnectedEntity = float3.zero;
            comp.PerpendicularAxisLocal = float3.zero;
            comp.PerpendicularAxisInConnectedEntity = float3.zero;

            comp.MinAngle = minAngle;
            comp.MaxAngle = maxAngle;
            return comp;
        }

        private void SetHumanoidRagdoll()
        {
            //Neck RagdollJoint
            AddRagdollJoint(Head, Torso, 45f, -30f, 30f, -5f, 5f);

            //shoulder RagdollJoint
            AddRagdollJoint(LeftShoulder, Torso, 80f, -70f, 20f, -5f, 5f);
            AddRagdollJoint(RightShoulder, Torso, 80f, -70f, 20f, -5f, 5f);

            //elbow LimitedHinge
            AddLimitedHinge(LeftElbow, LeftShoulder, 0, 100f);
            AddLimitedHinge(RightElbow, RightShoulder, 0, 100f);

            //wrist LimitedHinge
            AddLimitedHinge(LeftWrist, LeftElbow, 0, 135f);
            AddLimitedHinge(RightWrist, RightElbow, 0, 135f);

            //Waist RagdollJoint
            AddRagdollJoint(Pelvis, Torso, 5f, -5f, 90f, -5f, 5f);

            //hip RagdollJoint
            AddRagdollJoint(LeftHips, Pelvis, 60f, -10f, 40f, -5f, 5f);
            AddRagdollJoint(RightHips, Pelvis, 60f, -10f, 40f, -5f, 5f);

            //knee LimitedHinge
            AddLimitedHinge(LeftKnee, LeftHips, -90f, 0f);
            AddLimitedHinge(RightKnee, RightHips, -90f, 0f);

            //ankle LimitedHinge
            AddLimitedHinge(LeftAnkle, LeftKnee, -5f, 5f);
            AddLimitedHinge(RightAnkle, RightKnee, -5f, 5f);
        }

        [Button("清理布娃娃配置游戏对象")]
        private void ClearTempRagdoll()
        {
            DestroyImmediate(Pelvis);
            DestroyImmediate(LeftHips);
            DestroyImmediate(LeftKnee);
            DestroyImmediate(LeftAnkle);
            DestroyImmediate(RightHips);
            DestroyImmediate(RightKnee);
            DestroyImmediate(RightAnkle);
            DestroyImmediate(Torso);
            DestroyImmediate(Head);
            DestroyImmediate(LeftShoulder);
            DestroyImmediate(LeftElbow);
            DestroyImmediate(LeftWrist);
            DestroyImmediate(RightShoulder);
            DestroyImmediate(RightElbow);
            DestroyImmediate(RightWrist);

            DestroyImmediate(RagdollGroup);
        }

        [Button("保存布娃娃数据")]
        private void SaveRagdollConfig()
        {
            var data = GetRagdollConfigData(true);

            SaveRagdollBoneData(Pelvis, ref data.Pelvis);
            SaveRagdollBoneData(LeftHips, ref data.LeftHips);
            SaveRagdollBoneData(LeftKnee, ref data.LeftKnee);
            SaveRagdollBoneData(LeftAnkle, ref data.LeftAnkle);
            SaveRagdollBoneData(RightHips, ref data.RightHips);
            SaveRagdollBoneData(RightKnee, ref data.RightKnee);
            SaveRagdollBoneData(RightAnkle, ref data.RightAnkle);
            SaveRagdollBoneData(Torso, ref data.Torso);
            SaveRagdollBoneData(Head, ref data.Head);
            SaveRagdollBoneData(LeftShoulder, ref data.LeftShoulder);
            SaveRagdollBoneData(LeftElbow, ref data.LeftElbow);
            SaveRagdollBoneData(LeftWrist, ref data.LeftWrist);
            SaveRagdollBoneData(RightShoulder, ref data.RightShoulder);
            SaveRagdollBoneData(RightElbow, ref data.RightElbow);
            SaveRagdollBoneData(RightWrist, ref data.RightWrist);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();

            ClearTempRagdoll();
        }

        private void SaveRagdollBoneData(GameObject gameObject, ref Physics.RagdollBoneData data)
        {
            var comp = gameObject.GetComponent<PhysicsShapeAuthoring>();
            Debug.Assert(comp != null && comp.ShapeType == ShapeType.Capsule);
            
            data.position = gameObject.transform.position;
            data.rotation = gameObject.transform.rotation;

            var input = comp.GetCapsuleProperties();
            var capsule = CapsuleGeometryAuthoringExtensions.ToRuntime(input);
            data.Vertex0 = capsule.Vertex0;
            data.Vertex1 = capsule.Vertex1;
            data.Radius = capsule.Radius;
        }

        private Physics.RagdollConfigData GetRagdollConfigData(bool allowCreate)
        {
            string path = Galaxy.Utility.Text.Format("{0}/{1}.asset", ms_RagdollFilePath, RagdollConfigName);
            var data = AssetDatabase.LoadAssetAtPath(path, typeof(Physics.RagdollConfigData)) as Physics.RagdollConfigData;
            if (allowCreate && data == null)
            {
                data = CreateInstance<Physics.RagdollConfigData>();
                AssetDatabase.CreateAsset(data, path);
                Debug.Log("Create new ragdoll config data file");
            }
            return data;
        }
    }
}

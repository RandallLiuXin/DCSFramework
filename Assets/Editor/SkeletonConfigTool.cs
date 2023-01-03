using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Physics.Authoring;

namespace Anvil.Editor
{
    public class SkeletonConfigTool : OdinEditorWindow
    {
        [LabelText("ѡ��Ǽܵĸ��ڵ�")]
        public Transform rootTransform;

        [MenuItem("Anvil/SkeletonConfig")]
        private static void OpenWindow()
        {
            var window = GetWindow<SkeletonConfigTool>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
            window.titleContent = new GUIContent("Skeleton Config Tool");
        }

        [Button("Dots�����Ǽ�����")]
        private void ConfigSkeletionBone()
        {
            if (rootTransform == null)
                return;

            Transform rootTF = rootTransform;
#if UNITY_EDITOR
            Debug.Assert(rootTF.name.Equals("Root"));
#endif
            int index = 0;
            List<Transform> searchList = new List<Transform> { rootTF };
            while (searchList.Count != 0)
            {
                Transform currentTF = searchList[0];
                if (currentTF.gameObject.GetComponent<Animation.SkeletonBoneAuthoring>() == null)
                {
                    currentTF.gameObject.AddComponent<Animation.SkeletonBoneAuthoring>();
                }
                bool hasPhysicsShape = currentTF.gameObject.GetComponent<PhysicsShapeAuthoring>() != null;
                var skeletonBone = currentTF.gameObject.GetComponent<Animation.SkeletonBoneAuthoring>();
                skeletonBone.BoneIndex = index;
                skeletonBone.SkeletonVid = 0;
                skeletonBone.NeedCache = false;
                skeletonBone.CacheName = currentTF.gameObject.name;
                skeletonBone.HasHitBox = hasPhysicsShape;
                index++;

                searchList.RemoveAt(0);
                IEnumerator iter = currentTF.GetEnumerator();
                iter.Reset();
                while (iter.MoveNext())
                {
                    Transform childTF = iter.Current as Transform;
                    searchList.Add(childTF.transform);
                }
            }
            Debug.Log("��ǰ�Ǽܹ�����Ϊ: " + index);
        }

        //[Button("Dots�Ǽ��ܻ���Ԥ��")]
        private void PreviewSkeletonHitBox()
        {
            Debug.LogError("�ݲ�֧��...");
            if (rootTransform == null)
                return;

            Transform rootTF = rootTransform;
#if UNITY_EDITOR
            Debug.Assert(rootTF.name.Equals("Root"));
#endif
            List<Object> hitBoxObjects = new List<Object>();

            List<Transform> searchList = new List<Transform> { rootTF };
            while (searchList.Count != 0)
            {
                Transform currentTF = searchList[0];
                bool hasPhysicsShape = currentTF.gameObject.GetComponent<PhysicsShapeAuthoring>() != null;
                bool hasHitBox = currentTF.gameObject.GetComponent<Combat.HitBoxAuthoring>() != null;
                if (!hasPhysicsShape && hasHitBox)
                {
                    Debug.LogError("���������з�����hitbox,���Զ��Ƴ�: " + currentTF.name);
                    DestroyImmediate(currentTF.gameObject.AddComponent<Combat.HitBoxAuthoring>());
                }
                if (hasPhysicsShape)
                {
                    hitBoxObjects.Add(currentTF);
                }

                searchList.RemoveAt(0);
                IEnumerator iter = currentTF.GetEnumerator();
                iter.Reset();
                while (iter.MoveNext())
                {
                    Transform childTF = iter.Current as Transform;
                    searchList.Add(childTF.transform);
                }
            }

            Selection.objects = hitBoxObjects.ToArray();
        }

        //[Button("Dots�Ǽ��ܻ�������")]
        private void SetSkeletonHitBox()
        {
            if (rootTransform == null)
                return;

            Transform rootTF = rootTransform;
#if UNITY_EDITOR
            Debug.Assert(rootTF.name.Equals("Root"));
#endif
            List<Transform> searchList = new List<Transform> { rootTF };
            while (searchList.Count != 0)
            {
                Transform currentTF = searchList[0];
                bool hasPhysicsShape = currentTF.gameObject.GetComponent<PhysicsShapeAuthoring>() != null;
                bool hasHitBox = currentTF.gameObject.GetComponent<Combat.HitBoxAuthoring>() != null;
                if (!hasPhysicsShape && hasHitBox)
                {
                    Debug.LogError("���������з�����hitbox,���Զ��Ƴ�: " + currentTF.name);
                    DestroyImmediate(currentTF.gameObject.AddComponent<Combat.HitBoxAuthoring>());
                }
                if (hasPhysicsShape)
                {
                    if (!hasHitBox)
                    {
                        currentTF.gameObject.AddComponent<Combat.HitBoxAuthoring>();
                    }
                    var physicsShape = currentTF.gameObject.GetComponent<PhysicsShapeAuthoring>();
                    var hitbox = currentTF.gameObject.GetComponent<Combat.HitBoxAuthoring>();

                    var boxProperties = physicsShape.GetBoxProperties();

                    physicsShape.enabled = false;
                    hitbox.ColliderOffset = boxProperties.Center;
                    hitbox.ColliderShape = boxProperties.Size;
                    hitbox.ColliderBelongsTo = physicsShape.BelongsTo;
                    hitbox.ColliderCollidesWith = physicsShape.CollidesWith;
                }

                searchList.RemoveAt(0);
                IEnumerator iter = currentTF.GetEnumerator();
                iter.Reset();
                while (iter.MoveNext())
                {
                    Transform childTF = iter.Current as Transform;
                    searchList.Add(childTF.transform);
                }
            }
        }
    }
}

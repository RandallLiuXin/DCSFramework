using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

namespace Anvil.Physics
{
    [Serializable]
    public struct RagdollBoneData
    {
        public float3 position;
        public quaternion rotation;
        public float3 Vertex0;
        public float3 Vertex1;
        public float Radius;
    }

    [CreateAssetMenu(fileName = "RagdollConfig", menuName = "ScriptableObjects/RagdollConfigData")]
    public class RagdollConfigData : ScriptableObject
    {
        public RagdollBoneData Pelvis;
        public RagdollBoneData LeftHips;
        public RagdollBoneData LeftKnee;
        public RagdollBoneData LeftAnkle;
        public RagdollBoneData RightHips;
        public RagdollBoneData RightKnee;
        public RagdollBoneData RightAnkle;
        public RagdollBoneData Torso;
        public RagdollBoneData Head;
        public RagdollBoneData LeftShoulder;
        public RagdollBoneData LeftElbow;
        public RagdollBoneData LeftWrist;
        public RagdollBoneData RightShoulder;
        public RagdollBoneData RightElbow;
        public RagdollBoneData RightWrist;
    }
}

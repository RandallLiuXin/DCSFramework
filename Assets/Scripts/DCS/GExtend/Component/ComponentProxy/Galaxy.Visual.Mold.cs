// auto generate file (mold)
using Galaxy.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Visual
{
	[Serializable]
	public class AnimationComponent : ComponentBase
	{
		public AnimationComponent()
		{
		}
		public AnimationComponent(AnimationComponent component)
		{
			AnimatorType = component.AnimatorType;
		}
		public override CompType GetCompType()
		{
			return Entities.CompType.Animation;
		}
		private System.Int32 m_AnimatorType = 0;
		public System.Int32 AnimatorType
		{
			get
			{
				return m_AnimatorType;
			}
			set
			{
				m_AnimatorType = value;
			}
		}
	}
	public class AnimationComponentProxy : ComponentProxy
	{
		private AnimationComponent m_Data;
		public AnimationComponentProxy(AnimationComponent data, AccessType accessType) : base(data, accessType)
		{
			m_Data = data;
		}
		public override ComponentBase GetComponentBase()
		{
			return m_Data;
		}
		public CompType GetCompType()
		{
			return m_Data.GetCompType();
		}
		public System.Int32 AnimatorType
		{
			get
			{
				return m_Data.AnimatorType;
			}
			set
			{
				if (m_AccessType != AccessType.ReadWrite)
				{
					throw new GalaxyException("Only can modify readwrite component!");
				}
				m_Data.AnimatorType = value;
			}
		}
	}
	[Serializable]
	public class VisualComponent : ComponentBase
	{
		public VisualComponent()
		{
		}
		public VisualComponent(VisualComponent component)
		{
			ModelPath = component.ModelPath;
			VisualPid = component.VisualPid;
			VisualPos = component.VisualPos;
			VisualRot = component.VisualRot;
			VisualType = component.VisualType;
		}
		public override CompType GetCompType()
		{
			return Entities.CompType.Visual;
		}
		private System.String m_ModelPath = "";
		public System.String ModelPath
		{
			get
			{
				return m_ModelPath;
			}
			set
			{
				m_ModelPath = value;
			}
		}
		private System.UInt32 m_VisualPid = 0;
		public System.UInt32 VisualPid
		{
			get
			{
				return m_VisualPid;
			}
			set
			{
				m_VisualPid = value;
			}
		}
		private UnityEngine.Vector3 m_VisualPos = Vector3.zero;
		public UnityEngine.Vector3 VisualPos
		{
			get
			{
				return m_VisualPos;
			}
			set
			{
				m_VisualPos = value;
			}
		}
		private UnityEngine.Quaternion m_VisualRot = Quaternion.identity;
		public UnityEngine.Quaternion VisualRot
		{
			get
			{
				return m_VisualRot;
			}
			set
			{
				m_VisualRot = value;
			}
		}
		private System.UInt32 m_VisualType = 0;
		public System.UInt32 VisualType
		{
			get
			{
				return m_VisualType;
			}
			set
			{
				m_VisualType = value;
			}
		}
	}
	public class VisualComponentProxy : ComponentProxy
	{
		private VisualComponent m_Data;
		public VisualComponentProxy(VisualComponent data, AccessType accessType) : base(data, accessType)
		{
			m_Data = data;
		}
		public override ComponentBase GetComponentBase()
		{
			return m_Data;
		}
		public CompType GetCompType()
		{
			return m_Data.GetCompType();
		}
		public System.String ModelPath
		{
			get
			{
				return m_Data.ModelPath;
			}
			set
			{
				if (m_AccessType != AccessType.ReadWrite)
				{
					throw new GalaxyException("Only can modify readwrite component!");
				}
				m_Data.ModelPath = value;
			}
		}
		public System.UInt32 VisualPid
		{
			get
			{
				return m_Data.VisualPid;
			}
			set
			{
				if (m_AccessType != AccessType.ReadWrite)
				{
					throw new GalaxyException("Only can modify readwrite component!");
				}
				m_Data.VisualPid = value;
			}
		}
		public UnityEngine.Vector3 VisualPos
		{
			get
			{
				return m_Data.VisualPos;
			}
			set
			{
				if (m_AccessType != AccessType.ReadWrite)
				{
					throw new GalaxyException("Only can modify readwrite component!");
				}
				m_Data.VisualPos = value;
			}
		}
		public UnityEngine.Quaternion VisualRot
		{
			get
			{
				return m_Data.VisualRot;
			}
			set
			{
				if (m_AccessType != AccessType.ReadWrite)
				{
					throw new GalaxyException("Only can modify readwrite component!");
				}
				m_Data.VisualRot = value;
			}
		}
		public System.UInt32 VisualType
		{
			get
			{
				return m_Data.VisualType;
			}
			set
			{
				if (m_AccessType != AccessType.ReadWrite)
				{
					throw new GalaxyException("Only can modify readwrite component!");
				}
				m_Data.VisualType = value;
			}
		}
	}
}

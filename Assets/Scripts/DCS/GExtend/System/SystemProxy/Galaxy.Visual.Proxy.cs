// auto generate file (mold)
using Galaxy.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Visual
{
	public class AnimationSystemProxy : GalaxySystemProxy
	{
		protected Galaxy.Visual.AnimationSystem m_System;
		public AnimationSystemProxy(GalaxySystem system)
		{
			m_System = system as Galaxy.Visual.AnimationSystem;
			Debug.Assert(m_System != null);
		}
		
		protected override GalaxySystem GetGalaxySystem()
		{
			return m_System;
		}
		
	}
	public class VisualSystemProxy : GalaxySystemProxy
	{
		protected Galaxy.Visual.VisualSystem m_System;
		public VisualSystemProxy(GalaxySystem system)
		{
			m_System = system as Galaxy.Visual.VisualSystem;
			Debug.Assert(m_System != null);
		}
		
		protected override GalaxySystem GetGalaxySystem()
		{
			return m_System;
		}
		
		public void AttachToBone(uint uid, System.UInt32 targetPid, System.String bindTargetName, UnityEngine.Vector3 localPosition, UnityEngine.Quaternion localRotaion, UnityEngine.Vector3 localScale)
		{
			m_System.AttachToBone(GetHolder(uid), targetPid, bindTargetName, localPosition, localRotaion, localScale);
		}
		public void AttachToBone(Galaxy.Data.HolderProxy holder, System.UInt32 targetPid, System.String bindTargetName, UnityEngine.Vector3 localPosition, UnityEngine.Quaternion localRotaion, UnityEngine.Vector3 localScale)
		{
			m_System.AttachToBone(GetHolder(holder), targetPid, bindTargetName, localPosition, localRotaion, localScale);
		}
		public void UnAttach(uint uid)
		{
			m_System.UnAttach(GetHolder(uid));
		}
		public void UnAttach(Galaxy.Data.HolderProxy holder)
		{
			m_System.UnAttach(GetHolder(holder));
		}
	}
}

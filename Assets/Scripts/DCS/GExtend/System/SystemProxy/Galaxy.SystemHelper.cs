// auto generate file (mold)
using Galaxy.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Entities
{
	public class SystemHelper
	{
		public static SystemType GetSystemEnumByType(Type type)
		{
			if (type == typeof(Galaxy.Visual.AnimationSystem) || type == typeof(Galaxy.Visual.AnimationSystemProxy))
				return SystemType.Animation;
			if (type == typeof(Galaxy.Visual.VisualSystem) || type == typeof(Galaxy.Visual.VisualSystemProxy))
				return SystemType.Visual;
			throw new NotImplementedException();
		}
		public static GalaxySystem CreateInstance(SystemType systemType)
		{
			switch (systemType)
			{
				case SystemType.Animation:
					return new Galaxy.Visual.AnimationSystem();
				case SystemType.Visual:
					return new Galaxy.Visual.VisualSystem();
				default:
					throw new NotImplementedException();
			}
		}
		public static GalaxySystemProxy CreateProxyInstance(SystemType systemType, GalaxySystem system)
		{
			switch (systemType)
			{
				case SystemType.Animation:
					return new Galaxy.Visual.AnimationSystemProxy(system);
				case SystemType.Visual:
					return new Galaxy.Visual.VisualSystemProxy(system);
				default:
					throw new NotImplementedException();
			}
		}
	}
}

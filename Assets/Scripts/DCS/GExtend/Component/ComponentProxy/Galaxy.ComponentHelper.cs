// auto generate file (mold)
using Galaxy.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Entities
{
	public class ComponentHelper
	{
		public static CompType GetCompEnumByType(Type type)
		{
			if (type == typeof(Galaxy.Visual.AnimationComponent) || type == typeof(Galaxy.Visual.AnimationComponentProxy))
				return CompType.Animation;
			if (type == typeof(Galaxy.Visual.VisualComponent) || type == typeof(Galaxy.Visual.VisualComponentProxy))
				return CompType.Visual;
			throw new NotImplementedException();
		}
		public static ComponentBase CreateInstance(CompType compType)
		{
			switch (compType)
			{
				case CompType.Animation:
					return new Galaxy.Visual.AnimationComponent();
				case CompType.Visual:
					return new Galaxy.Visual.VisualComponent();
				default:
					throw new NotImplementedException();
			}
		}
		public static ComponentBase CreateInstance(CompType compType, ComponentBase component)
		{
			switch (compType)
			{
				case CompType.Animation:
				{
					Galaxy.Visual.AnimationComponent data = component as Galaxy.Visual.AnimationComponent;
					Debug.Assert(data != null);
					return new Galaxy.Visual.AnimationComponent(data);
				}
				case CompType.Visual:
				{
					Galaxy.Visual.VisualComponent data = component as Galaxy.Visual.VisualComponent;
					Debug.Assert(data != null);
					return new Galaxy.Visual.VisualComponent(data);
				}
				default:
					throw new NotImplementedException();
			}
		}
		public static ComponentProxy CreateProxyInstance(CompType compType, ComponentBase component, AccessType accessType)
		{
			switch (compType)
			{
				case CompType.Animation:
				{
					Galaxy.Visual.AnimationComponent data = component as Galaxy.Visual.AnimationComponent;
					Debug.Assert(data != null);
					return new Galaxy.Visual.AnimationComponentProxy(data, accessType);
				}
				case CompType.Visual:
				{
					Galaxy.Visual.VisualComponent data = component as Galaxy.Visual.VisualComponent;
					Debug.Assert(data != null);
					return new Galaxy.Visual.VisualComponentProxy(data, accessType);
				}
				default:
					throw new NotImplementedException();
			}
		}
	}
}

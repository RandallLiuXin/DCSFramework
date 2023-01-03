// auto generate file (mold)
using Galaxy.Data;
using Galaxy.Entities;
using Galaxy.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Entities
{
	public partial class EntityHelper
	{
		public static Entity CreateInstance(EntityType entityType, uint id, SystemType[] systems)
		{
			switch (entityType)
			{
				case EntityType.LocalPlayer:
					return new Galaxy.Entities.LocalPlayer(id, systems);
				case EntityType.AIEntity:
					return new Galaxy.Entities.AIEntity(id, systems);
				default:
					throw new NotImplementedException();
			}
		}
	}
}

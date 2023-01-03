// auto generate file (mold)
using Galaxy.Data;
using Galaxy.Entities;
using Galaxy.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Command
{
	public class CommandHelper
	{
		public static Dictionary<CommandType, CommandBase> GetAllCommands()
		{
			return new Dictionary<CommandType, CommandBase> {
			};
		}
	}
}

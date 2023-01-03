using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Entities
{
	public partial class EntityHelper
	{
		public static Dictionary<CompType, ComponentBase> GetEntityConfigData(uint configId)
        {
            //TODO Randall temp init
            switch (configId)
            {
                case 1:
                    return new Dictionary<CompType, ComponentBase> { 
                        { CompType.Visual, new Visual.VisualComponent { ModelPath = "Model/Boxer_Visual", VisualType = (uint)Visual.VisualType.Model } },
                        { CompType.Animation, new Visual.AnimationComponent { AnimatorType = (int)Visual.AnimatorType.Player } }
                    };
                case 2:
                    //TODO Randall aientity temp
                    return new Dictionary<CompType, ComponentBase> {
                        { CompType.Visual, new Visual.VisualComponent { ModelPath = "Model/Boxer_Visual", VisualType = (uint)Visual.VisualType.Model } },
                        { CompType.Animation, new Visual.AnimationComponent { AnimatorType = (int)Visual.AnimatorType.Player } }
                    };
                default:
                    return null;
            }
        }
	}
}

/*
 * Copyright (c) 2021 LG Electronics Inc.
 *
 * SPDX-License-Identifier: MIT
 */

using System;
using UE = UnityEngine;
using Debug = UnityEngine.Debug;

namespace SDF
{
	namespace Import
	{
		public partial class Loader : Base
		{
			protected override void ImportPlugin(in SDF.Plugin plugin, in System.Object parentObject)
			{
				var targetObject = (parentObject as UE.GameObject);

				// filtering plugin name
				var pluginLibraryName = plugin.LibraryName();
				// Debug.Log("plugin name = " + pluginName);

				var pluginType = Type.GetType(pluginLibraryName);
				if (pluginType != null)
				{
					if (targetObject == null)
					{
						Debug.LogError("[Plugin] targetObject is empty");
						return;
					}
				}
				else
				{
					Debug.LogWarningFormat("[Plugin] No plugin({0}) exist", plugin.Name);
				}
			}
		}
	}
}
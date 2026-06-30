using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Chumpware.Tools
{
#if UNITY_EDITOR
	public class ScreenshotToolSettings : ScriptableObject
	{
		public List<ScreenshotResolution> resolutions;
		public bool disableUI;
	}
#endif
}
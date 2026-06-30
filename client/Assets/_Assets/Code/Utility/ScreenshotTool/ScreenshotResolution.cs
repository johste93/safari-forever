using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chumpware.Tools
{
	[System.Serializable]
	public class ScreenshotResolution
	{
#if UNITY_EDITOR
		public int width = 100;
		public int height = 100;
		public string name = string.Empty;
#endif
	}
}
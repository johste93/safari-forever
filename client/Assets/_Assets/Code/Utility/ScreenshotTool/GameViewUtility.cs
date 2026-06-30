using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace Chumpware.Tools
{
#if UNITY_EDITOR
    public class GameViewUtility
    {
        static MethodInfo getGroup;
        static object gameViewSizesInstance;
        static string path {get{return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);}}

        static GameViewUtility()
		{
			var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
			var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
			var instanceProp = singleType.GetProperty("instance");
			getGroup = sizesType.GetMethod("GetGroup");
			gameViewSizesInstance = instanceProp.GetValue(null, null);
        }

        public static void SetSize(int index)
        {
            if(index == 0)
                return;
                
            var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var gvWnd = EditorWindow.GetWindow(gvWndType);
            selectedSizeIndexProp.SetValue(gvWnd, index, null);
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            return FindSize(GameViewSizeType.FixedResolution, sizeGroupType, width, height) != -1;
        }

        public static int FindSize(GameViewSizeType sizeType, GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];

            float aspect = (float)width / (float)height;
            
            for(int i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);

                if(sizeType == GameViewSizeType.FixedResolution)
                {	
                    if (sizeWidth == width && sizeHeight == height)
                        return i;
                }
                else if(sizeWidth != 0 && sizeHeight != 0 && (sizeWidth + sizeHeight < 100))
                {
                    
                    float sizeAspect = (float)sizeWidth / (float)sizeHeight;
                    if (MathIsHard.IsWithinRange(sizeAspect, aspect-0.01f, aspect+0.01f))
                    {
                        return i;
                    }
                        
                }
                
            }
            return -1;
        }

        private static object GetGroup(GameViewSizeGroupType type)
        {
            return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
        }

        public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
        {
            var group = GetGroup(sizeGroupType);
            var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
            var newSize = CreateGameViewSize(viewSizeType, width, height, text);
            addCustomSize.Invoke(group, new object[] { newSize });
        }

        public static void RemoveCustomSize(GameViewSizeGroupType sizeGroupType, int index)
        {
            if(index < 28)
                return;

            var group = GetGroup(sizeGroupType);
            var removeCustomSize = getGroup.ReturnType.GetMethod("RemoveCustomSize");
            removeCustomSize.Invoke(group, new object[] { index });
        }

        private static object CreateGameViewSize(GameViewSizeType viewSizeType, int width, int height, string text)
        {
            var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
            var ctor = gvsType.GetConstructor(new Type[] { typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType"), typeof(int), typeof(int), typeof(string) });
            var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
            return newSize;
        }
    
        public static void EditCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
        {
            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;

            var setGameViewSize = gvsType.GetMethod("Set");

            //Create a new GameViewSize Obj
            var asm = typeof(Editor).Assembly;
            //var gvsType = asm.GetType("UnityEditor.GameViewSize");
            //var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
            var ctor = gvsType.GetConstructor(new Type[] { typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize") });

            //var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
            setGameViewSize.Invoke(group, new object[] { CreateGameViewSize(viewSizeType, width, height, text) });
        }

        public enum GameViewSizeType
        {
            AspectRatio, FixedResolution
        }
    }
#endif
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

namespace Chumpware.Tools
{
    public class FileOperations
    {
        public static void Delete (string _filePath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
				Debug.LogError("File operations are not supported.");
			#else
				File.Delete(_filePath);
			#endif
		}

		public static void DeleteDirectory (string _filePath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
				Debug.LogError("File operations are not supported.");
			#else
				Directory.Delete(_filePath, true);
			#endif
		}

		public static void Move (string _sourcePath, string _destinationPath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
				Debug.LogError("File operations are not supported.");
			#elif UNITY_WINRT
				Debug.LogError("[Rename Unimplemeted on windows");
			#else
				File.Move(_sourcePath, _destinationPath);
			#endif
		}

		public static void MoveDirectory (string _sourcePath, string _destinationPath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
				Debug.LogError("File operations are not supported.");
			#elif UNITY_WINRT
				Debug.LogError("[Rename Unimplemeted on windows");
			#else
				Directory.Move(_sourcePath, _destinationPath);
			#endif
		}

		public static void Copy (string _sourcePath, string _destinationPath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
				Debug.LogError("File operations are not supported.");
			#elif UNITY_WINRT
				Debug.LogError("[Rename Unimplemeted on windows");
			#else
				File.Copy(_sourcePath, _destinationPath);
			#endif
		}

		public static bool Exists(string _filePath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
				Debug.LogError("File operations are not supported.");
				return false;
			#else
				return File.Exists(_filePath);
			#endif
		}

		public static bool DirectoryExists(string _filePath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
				Debug.LogError("File operations are not supported.");
				return false;
			#else
				return Directory.Exists(_filePath);
			#endif
		}

		public static byte[] ReadAllBytes (string _filePath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
				Debug.LogError("File operations are not supported");
				return null;
			#else
				return File.ReadAllBytes(_filePath);
			#endif
		}

		public static void WriteAllBytes (string _filePath, byte[] _bytes)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
				Debug.LogError("File operations are not supported");
				return;
			#else

			//if file path does not exsist create it.
			CreateDirectory(Path.GetDirectoryName(_filePath));
			File.WriteAllBytes(_filePath, _bytes);

			#endif
		}

		public static string ReadAllText (string _filePath)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
			Debug.LogError("File operations are not supported");
			return null;
			#elif UNITY_WINRT
			Debug.LogError("ReadAllText Un implemeted on windows");
			return null;
			#else
			return File.ReadAllText(_filePath);
			#endif
		}

		public static void WriteAllText (string _filePath, string _content)
		{
			#if (UNITY_WEBPLAYER || UNITY_WEBGL)
			Debug.LogError("File operations are not supported");
			return;
			#elif UNITY_WINRT
			Debug.LogError("ReadAllText Un implemeted on windows");
			return;
			#else

			//if file path does not exsist create it.
			CreateDirectory(Path.GetDirectoryName(_filePath));
            File.WriteAllText(_filePath, _content);

			#endif
		}

		public static void CreateDirectory(string directoryPath)
		{
			if(!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);
		}

		public static void DeleteAllFilesInDirectory(string directoryPath)
		{
			string[] files = Directory.GetFiles(directoryPath);

			foreach(string path in files)
			{
				Delete(path);
			}
		}

		public static string[] GetAllFilesInDirectory(string directoryPath)
		{
			return Directory.GetFiles(directoryPath);
		}

		public static string[] GetAllSubdirectoriesInDirectory(string directoryPath)
		{
			return Directory.GetDirectories(directoryPath);
		}

		public static string[] GetAllFilesAndSubDirectoryFilesInDirectory(string directoryPath)
		{
			return Directory.GetFileSystemEntries(directoryPath, "*", SearchOption.AllDirectories);
		}

		public static void CopyDirectory(string sourceDirectory, string targetDirectory, string[] excludeFileEndings = null)
		{
			DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
			DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

			CopyAllFilesToDirectory(diSource, diTarget, excludeFileEndings);
		}

		private static void CopyAllFilesToDirectory(DirectoryInfo source, DirectoryInfo target, string[] excludeFileEndings)
		{
			CreateDirectory(target.FullName);

			//make sure all endings are lowercase
			for(int i = 0; i < excludeFileEndings.Length; i++)
			{
				if(excludeFileEndings[i].Contains("."))
					excludeFileEndings[i] = excludeFileEndings[i].ToLower();
				else
					excludeFileEndings[i] = "." + excludeFileEndings[i].ToLower();
			}		

			// Copy each file into the new directory.
			foreach (FileInfo fi in source.GetFiles())
			{
				int index = Array.IndexOf(excludeFileEndings, fi.Extension.ToLower());
				if(index < 0)
					fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
			}

			// Copy each subdirectory using recursion.
			foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
				CopyAllFilesToDirectory(diSourceSubDir, nextTargetSubDir, excludeFileEndings);
			}
		}
    }
}

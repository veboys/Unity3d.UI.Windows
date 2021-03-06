﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ME {

	public partial class EditorUtilities {

		/*public static void ClearLayoutElement(UnityEngine.UI.Windows.WindowLayoutElement layoutElement) {
			
			if (layoutElement.editorLabel == null) return;

			var root = layoutElement.gameObject.transform.root.gameObject;

			if (EditorUtilities.IsPrefab(root) == false) {

				GameObject.DestroyImmediate(layoutElement.editorLabel);
				return;

			}

			var tag = layoutElement.tag;

			// Create prefab instance
			var instance = GameObject.Instantiate(root) as GameObject;
			var layout = instance.GetComponent<UnityEngine.UI.Windows.WindowLayout>();
			layout.hideFlags = HideFlags.HideAndDontSave;
			if (layout != null) {

				layoutElement = layout.GetRootByTag(tag);
				if (layoutElement != null) EditorUtilities.ClearLayoutElement(layoutElement);

				// Apply prefab
				PrefabUtility.ReplacePrefab(instance, root, ReplacePrefabOptions.ReplaceNameBased);

				// Clean up
				GameObject.DestroyImmediate(instance);

			}

		}*/
		/*
		public static void DestroyImmediateHidden<T>(this MonoBehaviour root) where T : Component {

			var components = root.GetComponents<T>();
			foreach (var component in components) {

				if (component.hideFlags == HideFlags.HideAndDontSave) {

					Component.DestroyImmediate(component);

				}

			}

		}*/

		public static bool IsPrefab(GameObject go) {

			if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab) {

				return true;

			}

			return false;

		}

		public static T CreateAsset<T>() where T : ScriptableObject {

			var asset = ScriptableObject.CreateInstance<T>();

			string path = AssetDatabase.GetAssetPath( Selection.activeObject );
			if ( path == "" ) {
				path = "Assets";
			} else if ( Path.GetExtension( path ) != "" ) {
				path = path.Replace( Path.GetFileName( AssetDatabase.GetAssetPath( Selection.activeObject ) ), "" );
			}

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath( path + "/New " + typeof( T ).ToString() + ".asset" );

			AssetDatabase.CreateAsset( asset, assetPathAndName );

			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;

			return asset;

		}

		public static T CreateAsset<T>(string filepath) where T : ScriptableObject {

			var asset = ScriptableObject.CreateInstance<T>();

			//string path = filepath;
			string assetPathAndName = filepath + ".asset";

			AssetDatabase.CreateAsset( asset, assetPathAndName );

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			
			return AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(T)) as T;
			
		}

		public static void CreateAsset( System.Type type ) {

			var asset = ScriptableObject.CreateInstance( type ) as Object;

			string path = AssetDatabase.GetAssetPath( Selection.activeObject );
			if ( path == "" ) {
				path = "Assets";
			} else if ( Path.GetExtension( path ) != "" ) {
				path = path.Replace( Path.GetFileName( AssetDatabase.GetAssetPath( Selection.activeObject ) ), "" );
			}

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath( path + "/New " + type.ToString() + ".asset" );

			AssetDatabase.CreateAsset( asset, assetPathAndName );

			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;

		}
		
		public static T[] GetPrefabsOfType<T>(bool strongType = true, string directory = null) where T : Component {
			
			return ME.EditorUtilities.GetPrefabsOfTypeRaw<T>(strongType, directory).Cast<T>().ToArray();
			
		}

		public static Component[] GetPrefabsOfTypeRaw<T>(bool strongType, string directory = null) where T : Component {
			
			return ME.Utilities.CacheComponentsArray<T>(() => {

				var folder = (directory == null) ? null : new string[] { directory };

				var objects = AssetDatabase.FindAssets("t:GameObject", folder);

				var output = new List<T>();
				foreach (var obj in objects) {

					if (obj == null) continue;

					var path = AssetDatabase.GUIDToAssetPath(obj);
					if (path == null) continue;

					var file = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
					if (file == null) continue;

					var comp = file.GetComponent<T>();
					if (comp == null) continue;

					if (strongType == true && comp.GetType().Name != typeof(T).Name) continue;

					output.Add(comp);

				}

				return output.ToArray();

			}, strongType, directory);

		}
		
		public static T[] GetAssetsOfType<T>(string directory = null, System.Func<T, bool> predicate = null) where T : ScriptableObject {

			return ME.EditorUtilities.GetAssetsOfTypeRaw<T>(directory, (p) => { if (predicate != null && p is T) { return predicate(p as T); } else { return true; } } ).Cast<T>().ToArray();

		}

		public static ScriptableObject[] GetAssetsOfTypeRaw<T>(string directory = null, System.Func<ScriptableObject, bool> predicate = null) where T : ScriptableObject {

			return ME.Utilities.CacheAssetsArray<T>(() => {
				
				var folder = (directory == null) ? null : new string[] { directory };

				var objects = AssetDatabase.FindAssets("t:ScriptableObject", folder);
				
				var output = new List<T>();
				foreach (var obj in objects) {
					
					if (obj == null) continue;
					
					var path = AssetDatabase.GUIDToAssetPath(obj);
					if (path == null) continue;

					var file = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject)) as ScriptableObject;
					if (file == null) continue;

					var comp = file as T;
					if (comp == null) continue;

					if (predicate != null && predicate(comp) == false) continue;

					output.Add(comp);

				}
				
				return output.ToArray();

			}, directory);

		}

	}

}
#endif
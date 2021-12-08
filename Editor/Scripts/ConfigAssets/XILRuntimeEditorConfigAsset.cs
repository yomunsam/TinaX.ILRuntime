using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TinaX.XILRuntime.Structs;
using TinaXEditor.Core.Consts;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor.XILRuntime.ConfigAssets
{
    public class XILRuntimeEditorConfigAsset : ScriptableObject
    {
        public XILRuntimeEditorConfigAsset()
        {
            EditorLoadAssemblies = new List<AssemblyLoadInfo>();
        }

        public string BindingCodeOutputPath = "Assets/ILRuntime/Generated";
        public List<AssemblyLoadInfo> EditorLoadAssemblies;



        //---------------------------------------------------------------------------
        private static string SavePath
            => Path.Combine(SavePathRootDir, "editorSettings.json");
        public static string SavePathRootDir 
            => Path.Combine(Directory.GetCurrentDirectory(), XEditorConsts.ProjectSettingsRootDirectory, "ILRuntime");

        private static XILRuntimeEditorConfigAsset _instance;

        public static XILRuntimeEditorConfigAsset GetInstance()
        {
            if(_instance == null)
                _instance = ScriptableObject.CreateInstance<XILRuntimeEditorConfigAsset>();

            var dir = SavePathRootDir;
            var path = SavePath;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                JsonUtility.FromJsonOverwrite(json, _instance);
            }

            return _instance;
        }

        public static void Save(XILRuntimeEditorConfigAsset instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var dir = SavePathRootDir;
            var path = SavePath;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonUtility.ToJson(instance);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

    }
}

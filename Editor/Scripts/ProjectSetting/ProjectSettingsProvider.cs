using System.IO;
using TinaX;
using TinaX.XILRuntime.ConfigAssets;
using TinaX.XILRuntime.Consts;
using TinaXEditor.Core;
using TinaXEditor.Core.Consts;
using TinaXEditor.XILRuntime.ConfigAssets;
using TinaXEditor.XILRuntime.Generator;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TinaXEditor.XILRuntime.ProjectSetting
{
    public class ProjectSettingsProvider
    {
        private static bool _refresh = false;
        private static XILRuntimeConfigAsset _configAsset;
        private static SerializedObject _configAssetSerializedObject;

        private static ReorderableList _list_assemblies;

        private static XILRuntimeEditorConfigAsset _editorConfigAsset;
        private static SerializedObject _editorConfigAssetSerializedObject;



        private static MyStyles myStyles
        {
            get
            {
                if(_mystyles == null)
                    _mystyles = new MyStyles();
                return _mystyles;
            }
        }
        private static MyStyles _mystyles;

        private static Localizer L
        {
            get
            {
                if(_localizer == null)
                    _localizer = new Localizer();
                return _localizer;
            }
        }
        private static Localizer _localizer;

        [SettingsProvider]
        public static SettingsProvider GetSettingsProvider()
            => new SettingsProvider($"{XEditorConsts.ProjectSettingsRootName}/ILRuntime", SettingsScope.Project, new string[] { "TinaX", "ILRuntime" })
            {
                label = "ILRuntime",
                guiHandler = (searchContent) =>
                {
                    if (!_refresh)
                        RefreshData();

                    EditorGUILayout.BeginVertical(myStyles.Body);
                    if(_configAsset == null)
                    {
                        GUILayout.Label(L.NoConfig);
                        if (GUILayout.Button(L.CreateConfigAsset , GUILayout.MaxWidth(120)))
                        {
                            EditorConfigAsset.CreateConfigIfNotExists<XILRuntimeConfigAsset>(XILConsts.DefaultConfigAssetName);
                            RefreshData();
                        }
                    }
                    else
                    {
                        Ensure_List_Assemblies();

                        GUILayout.Space(10);
                        _list_assemblies.DoLayoutList();

                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(_configAssetSerializedObject.FindProperty("EntryClass"), new GUIContent(L.EntryClass));
                        EditorGUILayout.PropertyField(_configAssetSerializedObject.FindProperty("EntryMethod"), new GUIContent(L.EntryMethodName));
                        GUILayout.Space(3);
                        EditorGUILayout.HelpBox(L.EntryTips, MessageType.None);

                        if (_editorConfigAsset != null)
                        {
                            GUILayout.Space(50);
                            EditorGUILayout.LabelField(L.EditorSettings, myStyles.Title);

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(_editorConfigAssetSerializedObject.FindProperty("BindingCodeOutputPath"),new GUIContent(L.BindingCodeOutputPath));
                            if(GUILayout.Button("Select", GUILayout.MaxWidth(65)))
                            {
                                var path = EditorUtility.OpenFolderPanel("Select CLR BindingOutputFolder", "Assets/", "");
                                if (!path.IsNullOrEmpty())
                                {
                                    var root_path = Directory.GetCurrentDirectory().Replace("\\", "/");
                                    if (path.StartsWith(root_path))
                                    {
                                        path = path.Substring(root_path.Length + 1, path.Length - root_path.Length - 1);
                                        path = path.Replace("\\", "/");
                                        _editorConfigAssetSerializedObject.FindProperty("BindingCodeOutputPath").stringValue = path;
                                    }
                                    else
                                        Debug.LogError("Invalid Path: " + path);
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.PropertyField(_editorConfigAssetSerializedObject.FindProperty("EditorLoadAssemblies"),new GUIContent(L.AssemblyLoadListEditor), true);

                            if (GUILayout.Button(L.SaveEditorConfiguration, GUILayout.MaxWidth(450)))
                            {
                                if (_editorConfigAssetSerializedObject != null)
                                    _editorConfigAssetSerializedObject.ApplyModifiedProperties();
                                XILRuntimeEditorConfigAsset.Save(_editorConfigAsset);
                                Debug.Log("Saved");
                            }

                            if (GUILayout.Button(L.GenerateBindingCode, GUILayout.MaxWidth(450)))
                            {
                                CLRBindingGenerator.GenerateByAnalysisFromConfiguration();
                            }
                        }
                    }

                    EditorGUILayout.EndVertical();

                    if (_configAssetSerializedObject != null)
                        _configAssetSerializedObject.ApplyModifiedProperties();
                    if (_editorConfigAssetSerializedObject != null)
                        _editorConfigAssetSerializedObject.ApplyModifiedProperties();
                },
                deactivateHandler = () =>
                {
                    if(_configAsset != null)
                    {
                        EditorUtility.SetDirty(_configAsset);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    if(_editorConfigAsset != null)
                    {
                        XILRuntimeEditorConfigAsset.Save(_editorConfigAsset);
                    }
                }
            };

        private static  void RefreshData()
        {
            _configAsset = EditorConfigAsset.GetConfig<XILRuntimeConfigAsset>(XILConsts.DefaultConfigAssetName);
            if(_configAsset != null)
            {
                _configAssetSerializedObject = new SerializedObject(_configAsset);
            }

            _editorConfigAsset = XILRuntimeEditorConfigAsset.GetInstance();
            if(_editorConfigAsset != null)
            {
                _editorConfigAssetSerializedObject = new SerializedObject(_editorConfigAsset);
            }

            _refresh = true;
        }

        private static void Ensure_List_Assemblies()
        {
            if(_list_assemblies == null)
            {
                _list_assemblies = new ReorderableList(_configAssetSerializedObject, _configAssetSerializedObject.FindProperty("LoadAssemblies"),
                    true, //draggable
                    true, //displayHeader
                    true, //displayAddButton
                    true //displayRemoveButton
                    );
                _list_assemblies.elementHeightCallback = (index) =>
                {
                    float single_line_height = EditorGUIUtility.singleLineHeight + 2;
                    return single_line_height * 2 + 2;
                };
                _list_assemblies.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    var singleLine = EditorGUIUtility.singleLineHeight + 2;

                    SerializedProperty itemData = _list_assemblies.serializedProperty.GetArrayElementAtIndex(index);
                    SerializedProperty item_assembly = itemData.FindPropertyRelative("AssemblyPath");
                    SerializedProperty item_symbol = itemData.FindPropertyRelative("SymbolPath");

                    var rect_assembly = rect;
                    EditorGUI.PropertyField(rect_assembly, item_assembly);

                    var rect_symbol = rect;
                    rect_symbol.y += singleLine;
                    EditorGUI.PropertyField(rect_symbol, item_symbol);

                };
                _list_assemblies.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, L.AssemblyLoadList);
                };
            }
        }

        class MyStyles
        {
            public MyStyles()
            {
                Body = new GUIStyle();
                Body.padding.left = 15;
                Body.padding.right = 15;

                Title = new GUIStyle(EditorStyles.largeLabel);
            }

            public GUIStyle Body;

            public GUIStyle Title;

        }

        class Localizer
        {
            bool IsHans;
            bool IsJp;

            public Localizer()
            {
                IsHans = Application.systemLanguage == SystemLanguage.Chinese 
                    || Application.systemLanguage == SystemLanguage.ChineseSimplified 
                    || Application.systemLanguage == SystemLanguage.ChineseTraditional;

                IsJp = Application.systemLanguage == SystemLanguage.Japanese;

#if TINAX_EDITOR_UI_ENGLISH
                IsHans = false;
                IsJp = false;
#endif
            }

            public string NoConfig
            {
                get
                {
                    if (IsHans)
                        return "请创建TinaX ILRuntime配置文件.";
                    if (IsJp)
                        return "TinaX.ILRuntime 構成ファイルを作成してください。";
                    return "Please create a TinaX ILRuntime configuration asset.";
                }
            }

            public string CreateConfigAsset
            {
                get
                {
                    if (IsHans)
                        return "创建配置资产";
                    if (IsJp)
                        return "構成アセットを作成する";
                    return "Create Configuration Asset.";
                }
            }

            public string AssemblyLoadList
            {
                get
                {
                    if (IsHans)
                        return "Assembly 加载列表";
                    if(IsJp)
                        return "アセンブリロードリスト";
                    return "Assembly Load List";
                }
            }

            public string EntryMethodName
            {
                get
                {
                    if (IsHans)
                        return "入口方法";
                    if (IsJp)
                        return "エントリー方法";
                    return "Entry Method";
                }
            }

            public string EntryClass
            {
                get
                {
                    if (IsHans)
                        return "入口方法所在类";
                    if (IsJp)
                        return "エントリー方法のクラス名";
                    return "Entry method class name";
                }
            }

            public string EntryTips
            {
                get
                {
                    if (IsHans)
                        return "入口方法应该是静态方法";
                    if (IsJp)
                        return "入力メソッドは静的メソッドである必要があります";
                    return "Entry method should be a static method";
                }
            }

            public string EditorSettings
            {
                get
                {
                    if (IsHans)
                        return "编辑器功能设置";
                    return "Editor Settings";
                }
            }

            public string BindingCodeOutputPath
            {
                get
                {
                    if (IsHans)
                        return "绑定代码输出路径";
                    return "Binding Code Output Path";
                }
            }

            public string AssemblyLoadListEditor
            {
                get
                {
                    if (IsHans)
                        return "Assembly 编辑器加载列表";
                    if (IsJp)
                        return "エディターのアセンブリロードリスト";
                    return "Assembly Load List For Editor";
                }
            }

            public string SaveEditorConfiguration
            {
                get
                {
                    if (IsHans)
                        return "保存编辑器配置";
                    return "Save editor configuration";
                }
            }

            public string GenerateBindingCode
            {
                get
                {
                    if (IsHans)
                        return "生成绑定代码";
                    if (IsJp)
                        return "バインディングコードを生成する";
                    return "Generate binding code";
                }
            }

        }
    }
}

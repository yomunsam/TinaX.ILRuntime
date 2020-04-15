using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaXEditor.Const;
using TinaXEditor.XILRuntime.Const;
using TinaX.XILRuntime.Internal;
using TinaX;
using TinaX.XILRuntime.Const;
using System.IO;
using System.Linq;
using UnityEditorInternal;

namespace TinaXEditor.XILRuntime
{
    public static class XRuntimeProjectSetting
    {
        private static bool mDataRefreshed = false;
        private static XRuntimeConfig mConfig;
        private static SerializedObject mConfig_SerializeObj;

        private static GUIStyle _style_label_large_bold;
        private static GUIStyle style_label_large_bold
        {
            get
            {
                if(_style_label_large_bold == null)
                {
                    _style_label_large_bold = new GUIStyle(EditorStyles.label);
                    _style_label_large_bold.fontStyle = FontStyle.Bold;
                    _style_label_large_bold.fontSize = 15;
                }
                return _style_label_large_bold;
            }
        }

        private static GUIStyle _style_btn_normal; //字体比原版稍微大一号
        private static GUIStyle style_btn_normal
        {
            get
            {
                if(_style_btn_normal == null)
                {
                    _style_btn_normal = new GUIStyle(GUI.skin.button);
                    _style_btn_normal.fontSize = 13;
                }
                return _style_btn_normal;
            }
        }

        private static GUIStyle _style_label_warning;
        private static GUIStyle style_label_warning
        {
            get
            {
                if(_style_label_warning == null)
                {
                    _style_label_warning = new GUIStyle(EditorStyles.label);
                    _style_label_warning.normal.textColor = TinaX.Internal.XEditorColorDefine.Color_Warning;
                }
                return _style_label_warning;
            }
        }

        private static GUIStyle _style_box;
        private static GUIStyle style_box
        {
            get
            {
                if (_style_box == null)
                {
                    _style_box = new GUIStyle(GUI.skin.box);
                    _style_box.margin.left = 10;
                    _style_box.margin.right = 10;
                    _style_box.margin.top = 5;
                }
                return _style_box;
            }
        }


        private static ReorderableList mList_AssemblyLoadPaths;

        [SettingsProvider]
        public static SettingsProvider XRuntimeSetting()
        {
            return new SettingsProvider(XRuntimeEditorConst.ProjectSetting_Node, SettingsScope.Project, new string[] { "Nekonya", "TinaX", "ILRuntime", "TinaX.ILRuntime" })
            {
                label = "X ILRuntime",
                activateHandler = (searconContent, uielementRoot) =>
                {
                    mDataRefreshed = false;
                },
                guiHandler = (searchContent) =>
                {
                    if (!mDataRefreshed) refreshData();
                    if (mConfig == null)
                    {
                        GUILayout.Space(20);
                        GUILayout.Label(I18Ns.NoConfig);
                        if (GUILayout.Button(I18Ns.BtnCreateConfigFile, style_btn_normal, GUILayout.MaxWidth(120)))
                        {
                            mConfig = XConfig.CreateConfigIfNotExists<XRuntimeConfig>(XRuntimeConst.ConfigPath_Resources, AssetLoadType.Resources);
                            refreshData();
                        }
                    }
                    else
                    {
                        if (mConfig_SerializeObj == null)
                            mConfig_SerializeObj = new SerializedObject(mConfig);
                        GUILayout.Space(20);

                        //Enable
                        EditorGUILayout.PropertyField(mConfig_SerializeObj.FindProperty("EnableILRuntime"));

                        //Entry Method
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.EntryMethod, GUILayout.MaxWidth(120));
                        EditorGUILayout.PropertyField(mConfig_SerializeObj.FindProperty("EntryMethod"), GUIContent.none);
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(3);
                        EditorGUILayout.HelpBox(I18Ns.Tips_EntryMethod, MessageType.None);
                        

                        GUILayout.Space(10);
                        
                        //Assembly Load Path
                        if(mList_AssemblyLoadPaths == null)
                        {
                            mList_AssemblyLoadPaths = new ReorderableList(mConfig_SerializeObj,
                                mConfig_SerializeObj.FindProperty("Assemblys"),
                                true, //drag
                                true, //header
                                true, //add
                                true); //remove
                            mList_AssemblyLoadPaths.elementHeightCallback = (index) =>
                            {
                                float single_line_height = EditorGUIUtility.singleLineHeight + 2;
                                return single_line_height * 2 + 2;
                            };
                            mList_AssemblyLoadPaths.drawElementCallback = (rect, index, isActive, isFocused) =>
                            {
                                rect.y += 2;
                                rect.height = EditorGUIUtility.singleLineHeight;
                                var singleLine = EditorGUIUtility.singleLineHeight + 2;

                                SerializedProperty itemData = mList_AssemblyLoadPaths.serializedProperty.GetArrayElementAtIndex(index);
                                SerializedProperty item_assembly = itemData.FindPropertyRelative("AssemblyLoadPath");
                                SerializedProperty item_symbol = itemData.FindPropertyRelative("SymbolFileLoadPath");

                                var rect_assembly = rect;
                                EditorGUI.PropertyField(rect_assembly, item_assembly);

                                var rect_symbol = rect;
                                rect_symbol.y += singleLine;
                                EditorGUI.PropertyField(rect_symbol, item_symbol);

                            };
                            mList_AssemblyLoadPaths.drawHeaderCallback = rect =>
                            {
                                EditorGUI.LabelField(rect, I18Ns.AssemblyLoadList);
                            };
                        }
                        mList_AssemblyLoadPaths.DoLayoutList();

                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(mConfig_SerializeObj.FindProperty("NotLoadSymbolInNonDevelopMode"), new GUIContent(I18Ns.NotLoadSymbolInNonDevelopMode,I18Ns.NotLoadSymbolInNonDevelopMode));

                        GUILayout.Space(20);
                        EditorGUILayout.HelpBox(I18Ns.Tips_ConnotHotFix, MessageType.Warning);

                        GUILayout.Space(35);
                        TinaXEditor.Utils.EditorGUIUtil.HorizontalLine();

                        //CLR OUTPUT
                        EditorGUILayout.LabelField(I18Ns.CLRBinding_Output_Folder);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(mConfig_SerializeObj.FindProperty("CLRBindingOutputFolder"));
                        if (GUILayout.Button("Select",style_btn_normal, GUILayout.Width(65)))
                        {
                            var path = EditorUtility.OpenFolderPanel("Select CLR BindingOutputFolder", "Assets/","");
                            if (!path.IsNullOrEmpty())
                            {
                                var root_path = Directory.GetCurrentDirectory().Replace("\\", "/");
                                if (path.StartsWith(root_path))
                                {
                                    path = path.Substring(root_path.Length + 1, path.Length - root_path.Length - 1);
                                    path = path.Replace("\\", "/");
                                    mConfig_SerializeObj.FindProperty("CLRBindingOutputFolder").stringValue = path;
                                }
                                else
                                    Debug.LogError("Invalid Path: " + path);
                            }
                        }
                        EditorGUILayout.EndHorizontal();


                        mConfig_SerializeObj.ApplyModifiedProperties();
                    }
                },
                deactivateHandler = () =>
                {
                    if(mConfig != null)
                    {
                        EditorUtility.SetDirty(mConfig);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    
                }
            };
        }


        private static void refreshData()
        {
            mConfig = XConfig.GetConfig<XRuntimeConfig>(XRuntimeConst.ConfigPath_Resources, AssetLoadType.Resources, false);

            

            mDataRefreshed = true;
        }


        private static class I18Ns
        {
            private static bool? _isChinese;
            private static bool IsChinese
            {
                get
                {
                    if (_isChinese == null)
                    {
                        _isChinese = (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified);
                    }
                    return _isChinese.Value;
                }
            }

            private static bool? _nihongo_desuka;
            private static bool NihongoDesuka
            {
                get
                {
                    if (_nihongo_desuka == null)
                        _nihongo_desuka = (Application.systemLanguage == SystemLanguage.Japanese);
                    return _nihongo_desuka.Value;
                }
            }

            public static string NoConfig
            {
                get
                {
                    if (IsChinese)
                        return "在首次使用TinaX ILRuntime的设置工具前，请先创建配置文件";
                    if (NihongoDesuka)
                        return "TinaX ILRuntimeセットアップツールを初めて使用する前に、構成ファイルを作成してください";
                    return "Before using the TinaX ILRuntime setup tool for the first time, please create a configuration file";
                }
            }

            public static string BtnCreateConfigFile
            {
                get
                {
                    if (IsChinese)
                        return "创建配置文件";
                    if (NihongoDesuka)
                        return "構成ファイルを作成する";
                    return "Create Configure File";
                }
            }

            
            public static string EntryMethod
            {
                get
                {
                    if (IsChinese)
                        return "入口方法";
                    if (NihongoDesuka)
                        return "エントリー方法";
                    return "Entry Method";
                }
            }

            public static string Tips_EntryMethod
            {
                get
                {
                    if (IsChinese)
                        return "入口方法是可热更新Assembly中的启动入口，当Framework启动完成后，入口方法会被自动调用。\n入口方法应该是一个无参数无返回值的静态方法，填写例如：MyNamespace.MyClass.Main";
                    if (NihongoDesuka)
                        return "エントリメソッドは、ホットアップデート可能なアセンブリの開始エントリであり、フレームワークが起動すると、エントリメソッドが自動的に呼び出されます。\nエントリメソッドは、パラメータと戻り値のない静的メソッドである必要があります\n例：MyNamespace.MyClass.Main"; ;
                    return "The entry method is a entry in the hot-startable Assembly. When the framework is started, the entry method is automatically called.\nThe entry method should be a static method with no parameters and no return value.\nFor example: MyNamespace.MyClass.Main";
                }
            }
            
            public static string Tips_ConnotHotFix
            {
                get
                {
                    if (IsChinese)
                        return "注意：此页面中的所有设置内容都无法被热更新。";
                    if (NihongoDesuka)
                        return "注：このページのすべての設定をホットアップデートすることはできません。";
                    return "Note: All settings on this page cannot be hot-updated.";
                }
            }

            public static string CLRBinding_Output_Folder
            {
                get
                {
                    if (IsChinese)
                        return "CLR绑定代码输出目录";
                    if (NihongoDesuka)
                        return "CLRバインディングコードの出力ディレクトリ";
                    return "CLR binding code output directory";
                }
            }


            public static string AssemblyLoadList
            {
                get
                {
                    if (IsChinese)
                        return "Assembly 加载列表";
                    if (NihongoDesuka)
                        return "アセンブリロードリスト";
                    return "Assembly Load List";
                }
            }

            public static string NotLoadSymbolInNonDevelopMode
            {
                get
                {
                    if (IsChinese)
                        return "在非开发模式下不加载Symbol调试文件";
                    if (NihongoDesuka)
                        return "非開発モードでSymbolデバッグファイルをロードしないでください。";
                    return "Do not load the Symbol debug file in non-development mode";
                }
            }
        }

    }


}


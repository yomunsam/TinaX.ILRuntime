using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX;
using TinaX.XILRuntime.Const;
using TinaX.XILRuntime.Internal;
using TinaXEditor.XILRuntime.Const;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TinaXEditor.XILRuntime.Internal
{
    public class XILRuntimeProjectSetting
    {
        private static XILRTConfig m_Config;
        private static SerializedObject m_Config_SerObj;

        private static bool m_DataRefreshed = false;

        private static ReorderableList m_list_assemblies;
        private static ReorderableList m_list_assemblies_editor;


        [SettingsProvider]
        public static SettingsProvider XILRuntimeSetting()
        {
            return new SettingsProvider(XILEditorConst.ProjectSetting_Node, SettingsScope.Project, new string[] { "Nekonya", "TinaX", "ILRuntime", "TinaX.ILRuntime", "XILRuntime" })
            {
                label = "X ILRuntime",
                guiHandler = (searchContent) =>
                {
                    if (!m_DataRefreshed) refreshData();

                    EditorGUILayout.BeginVertical(Styles.body);
                    if (m_Config == null)
                    {
                        GUILayout.Space(20);
                        GUILayout.Label(I18Ns.NoConfig);
                        if (GUILayout.Button(I18Ns.BtnCreateConfigFile, Styles.style_btn_normal, GUILayout.MaxWidth(120)))
                        {
                            m_Config = XConfig.CreateConfigIfNotExists<XILRTConfig>(XILConst.ConfigPath_Resources, AssetLoadType.Resources);
                            refreshData();
                        }
                    }
                    else
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("Enable"), new GUIContent(I18Ns.Enable));

                        GUILayout.Space(10);
                        if(m_list_assemblies == null)
                        {
                            m_list_assemblies = new ReorderableList(m_Config_SerObj,
                                m_Config_SerObj.FindProperty("LoadAssemblies"),
                                true, //drag
                                true,
                                true,
                                true);
                            m_list_assemblies.elementHeightCallback = (index) =>
                            {
                                float single_line_height = EditorGUIUtility.singleLineHeight + 2;
                                return single_line_height * 2 + 2;
                            };
                            m_list_assemblies.drawElementCallback = (rect, index, isActive, isFocused) =>
                            {
                                rect.y += 2;
                                rect.height = EditorGUIUtility.singleLineHeight;
                                var singleLine = EditorGUIUtility.singleLineHeight + 2;

                                SerializedProperty itemData = m_list_assemblies.serializedProperty.GetArrayElementAtIndex(index);
                                SerializedProperty item_assembly = itemData.FindPropertyRelative("AssemblyPath");
                                SerializedProperty item_symbol = itemData.FindPropertyRelative("SymbolPath");

                                var rect_assembly = rect;
                                EditorGUI.PropertyField(rect_assembly, item_assembly);

                                var rect_symbol = rect;
                                rect_symbol.y += singleLine;
                                EditorGUI.PropertyField(rect_symbol, item_symbol);

                            };
                            m_list_assemblies.drawHeaderCallback = rect =>
                            {
                                EditorGUI.LabelField(rect, I18Ns.AssemblyLoadList);
                            };
                        }
                        m_list_assemblies.DoLayoutList();

                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("EntryClass"), new GUIContent(I18Ns.EntryClass));
                        EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("EntryMethod"), new GUIContent(I18Ns.EntryMethod_MethodName));
                        GUILayout.Space(3);
                        EditorGUILayout.HelpBox(I18Ns.EntryTips, MessageType.None);


                        GUILayout.Space(20);
                        EditorGUILayout.HelpBox(I18Ns.Tips_ConnotHotFix, MessageType.Warning);

                        GUILayout.Space(35);
                        TinaXEditor.Utils.EditorGUIUtil.HorizontalLine();

                        //CLR生成代码输出
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField(I18Ns.GenCodeOutputPath);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("EditorCLRBindingCodeOutputPath"),GUIContent.none);
                        if (GUILayout.Button("Select", Styles.style_btn_normal, GUILayout.Width(65)))
                        {
                            var path = EditorUtility.OpenFolderPanel("Select CLR BindingOutputFolder", "Assets/", "");
                            if (!path.IsNullOrEmpty())
                            {
                                var root_path = Directory.GetCurrentDirectory().Replace("\\", "/");
                                if (path.StartsWith(root_path))
                                {
                                    path = path.Substring(root_path.Length + 1, path.Length - root_path.Length - 1);
                                    path = path.Replace("\\", "/");
                                    m_Config_SerObj.FindProperty("EditorCLRBindingCodeOutputPath").stringValue = path;
                                }
                                else
                                    Debug.LogError("Invalid Path: " + path);
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        if (m_list_assemblies_editor == null)
                        {
                            m_list_assemblies_editor = new ReorderableList(m_Config_SerObj,
                                m_Config_SerObj.FindProperty("EditorLoadAssemblyPaths"),
                                true, //drag
                                true,
                                true,
                                true);
                            m_list_assemblies_editor.elementHeightCallback = (index) =>
                            {
                                float single_line_height = EditorGUIUtility.singleLineHeight + 2;
                                return single_line_height * 2 + 2;
                            };
                            m_list_assemblies_editor.drawElementCallback = (rect, index, isActive, isFocused) =>
                            {
                                rect.y += 2;
                                rect.height = EditorGUIUtility.singleLineHeight;
                                var singleLine = EditorGUIUtility.singleLineHeight + 2;

                                SerializedProperty itemData = m_list_assemblies_editor.serializedProperty.GetArrayElementAtIndex(index);
                                SerializedProperty item_assembly = itemData.FindPropertyRelative("AssemblyPath");
                                SerializedProperty item_symbol = itemData.FindPropertyRelative("SymbolPath");

                                var rect_assembly = rect;
                                EditorGUI.PropertyField(rect_assembly, item_assembly);

                                var rect_symbol = rect;
                                rect_symbol.y += singleLine;
                                EditorGUI.PropertyField(rect_symbol, item_symbol);

                            };
                            m_list_assemblies_editor.drawHeaderCallback = rect =>
                            {
                                EditorGUI.LabelField(rect, I18Ns.AssemblyLoadListEditor);
                            };
                        }
                        m_list_assemblies_editor.DoLayoutList();
                        GUILayout.Space(5);
                        EditorGUILayout.HelpBox(I18Ns.AssemblyLoadListEditorTips, MessageType.Info);

                    }

                    
                    EditorGUILayout.EndVertical();

                    if (m_Config_SerObj != null)
                        m_Config_SerObj.ApplyModifiedProperties();
                },
                deactivateHandler = () =>
                {
                    if (m_Config != null)
                    {
                        EditorUtility.SetDirty(m_Config);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                },
            };
        }

        private static void refreshData()
        {
            m_Config = XConfig.GetConfig<XILRTConfig>(XILConst.ConfigPath_Resources, AssetLoadType.Resources, false);
            if (m_Config != null)
                m_Config_SerObj = new SerializedObject(m_Config);

            m_DataRefreshed = true;
        }

        private static class Styles
        {
            private static GUIStyle _style_btn_normal; //字体比原版稍微大一号
            public static GUIStyle style_btn_normal
            {
                get
                {
                    if (_style_btn_normal == null)
                    {
                        _style_btn_normal = new GUIStyle(GUI.skin.button);
                        _style_btn_normal.fontSize = 13;
                    }
                    return _style_btn_normal;
                }
            }


            private static GUIStyle _body;
            public static GUIStyle body
            {
                get
                {
                    if (_body == null)
                    {
                        _body = new GUIStyle();
                        _body.padding.left = 15;
                        _body.padding.right = 15;
                    }
                    return _body;
                }
            }
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
                        return "在首次使用TinaX ILRuntime 的设置工具前，请先创建配置文件";
                    if (NihongoDesuka)
                        return "TinaX ILRuntime セットアップツールを初めて使用する前に、構成ファイルを作成してください";
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

            public static string Enable
            {
                get
                {
                    if (IsChinese)
                        return "启用 X ILRuntime";
                    return "Enable X ILRuntime";
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

            public static string AssemblyLoadListEditor
            {
                get
                {
                    if (IsChinese)
                        return "Assembly 编辑器加载列表";
                    if (NihongoDesuka)
                        return "エディターのアセンブリロードリスト";
                    return "Assembly Load List For Editor";
                }
            }
            
            public static string AssemblyLoadListEditorTips
            {
                get
                {
                    if (IsChinese)
                        return "当框架中使用不同的资产管理服务来加载资产时，加载的路径可能是不一样的. \n所以我们需要额外定义Assembly在编辑器模式的加载路径。\n此列表的设置将用于自动生成代码。";
                    if (NihongoDesuka)
                        return "フレームワークで異なるアセット管理サービスを使用してアセットをロードすると、ロードパスが異なる場合があります。\nしたがって、エディターモードでアセンブリの読み込みパスを追加で定義する必要があります。\nこのリストの設定は、エディターでコードを自動的に生成するために使用されます。";
                    return "When different asset management services are used in the framework to load assets, the loading path may be different.\nSo we need to additionally define the loading path of Assembly in editor mode.\nThe settings in this list will be used to automatically generate code under the editor.";
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

            public static string EntryMethod_MethodName
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

            public static string EntryClass
            {
                get
                {
                    if (IsChinese)
                        return "入口方法所在类";
                    if (NihongoDesuka)
                        return "エントリー方法のクラス名";
                    return "Entry method class name";
                }
            }

            public static string EntryTips
            {
                get
                {
                    if (IsChinese)
                        return "入口方法应该是静态方法";
                    if (NihongoDesuka)
                        return "入力メソッドは静的メソッドである必要があります";
                    return "Entry method should be a static method";
                }
            }

            public static string GenCodeOutputPath
            {
                get
                {
                    if (IsChinese)
                        return "生成代码的存放目录";
                    return "Generated code dir: ";
                }
            }
        }
    }
}

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

namespace TinaXEditor.XILRuntime
{
    public static class XRuntimeProjectSetting
    {
        private static bool mDataRefreshed = false;
        private static XRuntimeConfig mConfig;


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
                        GUILayout.Label(XRuntimeProjectSettingI18N.NoConfig);
                        if (GUILayout.Button(XRuntimeProjectSettingI18N.BtnCreateConfigFile, style_btn_normal, GUILayout.MaxWidth(120)))
                        {
                            mConfig = XConfig.CreateConfigIfNotExists<XRuntimeConfig>(XRuntimeConst.ConfigPath_Resources, AssetLoadType.Resources);
                            refreshData();
                        }
                    }
                    else
                    {
                        GUILayout.Space(20);

                        //Entry Method
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(XRuntimeProjectSettingI18N.EntryMethod, GUILayout.MaxWidth(120));
                        mConfig.EntryMethod = EditorGUILayout.TextField(mConfig.EntryMethod);
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(3);
                        EditorGUILayout.HelpBox(XRuntimeProjectSettingI18N.Tips_EntryMethod, MessageType.None);

                        //Assembly Load Mode
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(XRuntimeProjectSettingI18N.AssemblyLoadMode, GUILayout.MaxWidth(180));
                        mConfig.AssemblyLoadMode = (AssemblyLoadingMethod)EditorGUILayout.EnumPopup(mConfig.AssemblyLoadMode);
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(5);
                        if (mConfig.AssemblyLoadMode == AssemblyLoadingMethod.FrameworkAssetsManager)
                            EditorGUILayout.HelpBox(XRuntimeProjectSettingI18N.Tips_AssemblyLoadMode_Framework, MessageType.None);
                        else if (mConfig.AssemblyLoadMode == AssemblyLoadingMethod.LoadFromSystemIO)
                            EditorGUILayout.HelpBox(XRuntimeProjectSettingI18N.Tips_AssemblyLoadMode_SystemIO, MessageType.None);

                        GUILayout.Space(10);
                        //Framework load mode only
                        if (mConfig.AssemblyLoadMode == AssemblyLoadingMethod.FrameworkAssetsManager)
                        {
                            //Load Path
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(XRuntimeProjectSettingI18N.LoadPath_FrameworkAssetsManager, GUILayout.MaxWidth(180));
                            mConfig.Dll_LoadPathByFrameworkAssetsManager = EditorGUILayout.TextField(mConfig.Dll_LoadPathByFrameworkAssetsManager);
                            EditorGUILayout.EndHorizontal();

                            //Symbol Load Path
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(XRuntimeProjectSettingI18N.Synbol_LoadPath_FrameworkAssetsManager, GUILayout.MaxWidth(180));
                            mConfig.Symbol_LoadPathByFrameworkAssetsManager = EditorGUILayout.TextField(mConfig.Symbol_LoadPathByFrameworkAssetsManager);
                            EditorGUILayout.EndHorizontal();
                        }


                        GUILayout.Space(20);
                        EditorGUILayout.HelpBox(XRuntimeProjectSettingI18N.Tips_ConnotHotFix, MessageType.Warning);

                        GUILayout.Space(35);
                        TinaXEditor.Utils.EditorGUIUtil.HorizontalLine();

                        //CLR OUTPUT
                        EditorGUILayout.LabelField(XRuntimeProjectSettingI18N.CLRBinding_Output_Folder);
                        EditorGUILayout.BeginHorizontal();
                        mConfig.CLRBindingOutputFolder = EditorGUILayout.TextField(mConfig.CLRBindingOutputFolder);
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
                                    mConfig.CLRBindingOutputFolder = path;
                                }
                                else
                                    Debug.LogError("Invalid Path: " + path);
                            }
                        }
                        EditorGUILayout.EndHorizontal();

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


        private static class XRuntimeProjectSettingI18N
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

            public static string AssemblyName
            {
                get
                {
                    if (IsChinese)
                        return "Assembly 名";
                    if (NihongoDesuka)
                        return "アセンブリ名";
                    return "Assembly Name";
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
            public static string AssemblyLoadMode
            {
                get
                {
                    if (IsChinese)
                        return "Assembly的加载模式";
                    if (NihongoDesuka)
                        return "アセンブリ読み込みモード";
                    return "Assembly Loading Mode";
                }
            }
            public static string Tips_AssemblyLoadMode_Framework
            {
                get
                {
                    if (IsChinese)
                        return "加载模式“Framework Assets Manager”：使用Framework中的资源管理来管理、更新和加载可热更新的Assembly文件。\n使用此模式需要Framework中有某个服务实现了内置接口\"TinaX.Services.IAssetService\"\n如\"TinaX.VFS\"服务包就实现了该接口。";
                    if (NihongoDesuka)
                        return "ロードモード「Framework Assets Manager」：フレームワークのアセット管理を使用して、ホット更新可能なアセンブリファイルを管理、更新、およびロードします。\nこのパターンを使用するには、組み込みインターフェイス「TinaX.Services.IAssetService」を実装するためのフレームワークのサービスが必要です\nたとえば、「TinaX.VFS」サービスパックはこのインターフェイスを実装しています。";
                    return "Loading mode \"Framework Assets Manager\": Use asset management in Framework to manage, update, and load hot-updatable Assembly files.\nUsing this pattern requires a service in Framework to implement the built-in interface \"TinaX.Services.IAssetService\"\nFor example, the \"TinaX.VFS\" service package implements this interface.";
                }
            }
            public static string Tips_AssemblyLoadMode_SystemIO
            {
                get
                {
                    if (IsChinese)
                        return "加载模式“Load From System IO”：使用DotNet的System.IO类直接加载可热更新的Assembly文件。\n使用该模式需要自行管理Assembly文件的更新。";
                    if (NihongoDesuka)
                        return "ロードモード「Load From System IO」：DotNetのSystem.IOクラスを使用して、ホット更新可能なアセンブリファイルを直接ロードします。 \n このモードを使用するには、アセンブリファイルの更新を自分で管理する必要があります。";
                    return "Load mode \"Load From System IO\": Use DotNet's System.IO class to directly load a hot-updateable Assembly file. \n Using this mode requires you to manage the update of the Assembly files yourself.";
                }
            }

            public static string LoadPath_FrameworkAssetsManager
            {
                get
                {
                    if (IsChinese)
                        return "加载Assembly的路径：";
                    if (NihongoDesuka)
                        return "アセンブリパスをロードします：";
                    return "Load the assembly path: ";
                }
            }

            public static string Synbol_LoadPath_FrameworkAssetsManager
            {
                get
                {
                    if (IsChinese)
                        return "加载调试符号文件的路径：";
                    if (NihongoDesuka)
                        return "シンボルパスをロードします：";
                    return "Load the symbol path: ";
                }
            }

            public static string SavePath_FrameworkAssetsManager
            {
                get
                {
                    if (IsChinese)
                        return "编辑器下将Assembly保存到项目\"Assets\"目录中的路径：";
                    if (NihongoDesuka)
                        return "エディターのプロジェクト\"Assets\"ディレクトリのパスにアセンブリを保存します";
                    return "Save the Assembly to the path in the project \"Assets\" directory in the editor:";
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

        }

    }


}


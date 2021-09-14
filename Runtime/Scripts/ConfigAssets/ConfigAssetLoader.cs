using TinaX.Core.Utils;
using TinaX.XILRuntime.Consts;
using TinaX.XILRuntime.Options;
using UnityEngine;

namespace TinaX.XILRuntime.ConfigAssets
{
    public static class ConfigAssetLoader
    {
        public static void ApplyOptionsFromConfigAsset(this XILRuntimeOptions options)
        {
            options.ApplyOptionsAsync += async options =>
            {
                var asset = await ConfigAssetUtil.GetConfigFromDefaultFolderAsync<XILRuntimeConfigAsset>(XILConsts.DefaultConfigAssetName);
                if(asset == null)
                {
                    Debug.LogError(LocalizationUtil.IsHans()
                        ? $"从默认路径加载 xILRuntime 配置资产失败，请检查资产文件是否有效:{ConfigAssetUtil.GetResourcesLoadPathFromDefaultConfigFolder(XILConsts.DefaultConfigAssetName)}"
                        : $"Failed to load the xILRuntime configuration asset from the default path. Please check whether the asset file is valid:{ConfigAssetUtil.GetResourcesLoadPathFromDefaultConfigFolder(XILConsts.DefaultConfigAssetName)}");
                    return;
                }
                MapXILConfigAssetToOptions(ref options, ref asset);
            };
        }

        public static void ApplyOptionsFromConfigAsset(this XILRuntimeOptions options, string loadPath)
        {
            options.ApplyOptionsAsync += async options =>
            {
                var asset = await ConfigAssetUtil.GetConfigAsync<XILRuntimeConfigAsset>(loadPath);
                MapXILConfigAssetToOptions(ref options, ref asset);
            };
        }


        private static void MapXILConfigAssetToOptions(ref XILRuntimeOptions options, ref XILRuntimeConfigAsset asset)
        {
            //Todo: 以后这里弄个类似AutoMapper的玩意
            options.Enable = asset.Enable;
            options.LoadAssemblies.AddRange(asset.LoadAssemblies);
            options.EntryClass = asset.EntryClass;
            options.EntryMethod = asset.EntryMethod;
        }

    }
}

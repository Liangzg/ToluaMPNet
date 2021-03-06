using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using com.gt.entities;
using com.gt.units;


/// <summary>
/// 
/// </summary>
public class PacjageAsset : Editor
{
    /// <summary>
    /// 
    /// </summary>
    internal static BuildTarget buildTarget
    {
        get
        {
            BuildTarget target;
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
#if UNITY_5
            target = BuildTarget.iOS;
#else
                target = BuildTarget.iPhone;
#endif
            }
            else
            {
                target = BuildTarget.Android;
            }
            return target;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static string buildExtension = ".assetbundle";
 



    /// <summary>
    /// 
    /// </summary>
    /// <param name="mainAsset"></param>
    /// <param name="assets"></param>
    /// <param name="pathName"></param>
    /// <param name="assetBundleOptions"></param>
    /// <returns></returns>
    internal static bool BuildAssetBundle(Object mainAsset, UnityEngine.Object[] assets, string dirName, string fileName, BuildAssetBundleOptions assetBundleOptions)
    {
        string pathName = GetPath(dirName, fileName, buildExtension);
        return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, assetBundleOptions, buildTarget);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="dirName"></param>
    /// <param name="fileName"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    internal static string GetPath(string dirName, string fileName, string extension)
    {
        string rootdir = Application.dataPath + "/StreamingAssets/" + buildTarget;
        string path = rootdir + "/";
        if (!string.IsNullOrEmpty(dirName))
        {
            path = path + dirName + "/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        string pathName = path + fileName + extension;
        return pathName;
    }


    /// <summary>
    /// 
    /// </summary>
    class AssetInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Object asset;
        /// <summary>
        /// 
        /// </summary>
        public string dir;
        /// <summary>
        /// 
        /// </summary>
        public string fileName;
    }

    /// <summary>
    /// 
    /// </summary>
    enum BuilderGuiType
    {
        Texture,
        Material,
        Shader,
        Mesh,
        AudioClip,
        Prefabs
    }


    /// <summary>
    /// 
    /// </summary>
    [MenuItem("Game/Bunlde GUI")]
    static void CreateAssetBunldesGui()
    {
        Caching.CleanCache();

        var options = (BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);

        string assetDir = GetPath("Prefabs", string.Empty, string.Empty);
        Object[] selectAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        
        if (Directory.Exists(assetDir))
        {
            Directory.Delete(assetDir, true);
           // File.Delete(assetDir + ".meta");
        }

        Dictionary<BuilderGuiType, Dictionary<string, AssetInfo>> assets = new Dictionary<BuilderGuiType, Dictionary<string, AssetInfo>>();
        IMPObject prefabdepensList = new MPObject();
        foreach (Object obj in selectAssets)
        {
            string path1 = AssetDatabase.GetAssetPath(obj);
            if (path1.IndexOf(".prefab") == -1) continue;
            Object[] tem = EditorUtility.CollectDependencies(new Object[] { obj });
            IMPArray prefabdepens = new MPArray();
            foreach (Object o in tem)
            {
                if (o != null)
                {
                    if (o is Texture2D)
                    {
                        AddBuilderAsset(assets, "Assets/GUIAtlas/", o, "Prefabs/Assets/Textures", BuilderGuiType.Texture, prefabdepens);
                    }
                    //if (o is Shader)
                    //{
                    //    AddBuilderAsset(assets, "Assets/GUIAtlas/", o, "Prefabs/Assets/Shaders", BuilderGuiType.Shader, prefabdepens);
                    //}
                    if (o is Material)
                    {
                        AddBuilderAsset(assets, "Assets/GUIAtlas/", o, "Prefabs/Assets/Materials", BuilderGuiType.Material, prefabdepens);
                    }
                    if (o is Mesh)
                    {
                        AddBuilderAsset(assets, "Assets/GUIAtlas/", o, "Prefabs/Assets/Meshes", BuilderGuiType.Mesh, prefabdepens);
                    }
                    if (o is AudioClip)
                    {
                        AddBuilderAsset(assets, "Assets/GUIAtlas/", o, "Prefabs/Assets/AudioClips", BuilderGuiType.AudioClip, prefabdepens);
                    }
                }
            }
            AssetInfo assetInfo = AddBuilderAsset(assets, "Assets/Resources/Prefabs/", obj, "Prefabs", BuilderGuiType.Prefabs, null);
            prefabdepensList.PutMPArray(obj.name, prefabdepens);
        }

        BuildPipeline.PushAssetDependencies();
        {
            BuilderAsset(assets, BuilderGuiType.Texture, options);
            BuilderAsset(assets, BuilderGuiType.Mesh, options);
            BuilderAsset(assets, BuilderGuiType.Material, options);
            BuilderAsset(assets, BuilderGuiType.AudioClip, options);

            BuildPipeline.PushAssetDependencies();
            {
                BuilderAsset(assets, BuilderGuiType.Prefabs, options);
            }
            BuildPipeline.PopAssetDependencies();
        }
        BuildPipeline.PopAssetDependencies();
        string path = GetPath("Prefabs", "GuiConfig", ".bytes");

        ByteArray bytes = prefabdepensList.ToBinary();
        byte[] byteData = bytes.Bytes;
        File.WriteAllBytes(path, byteData);

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assets"></param>
    /// <param name="asset"></param>
    /// <param name="rootDir"></param>
    /// <param name="type"></param>
    /// <param name="depens"></param>
    private static AssetInfo AddBuilderAsset(Dictionary<BuilderGuiType, Dictionary<string, AssetInfo>> assets, string assetRoot, Object asset, string rootDir, BuilderGuiType type, IMPArray depens)
    {
        string path = AssetDatabase.GetAssetPath(asset);
        string rootPath = path.Substring(assetRoot.Length);
        string dir = rootPath.LastIndexOf("/") != -1 ? rootPath.Substring(0, rootPath.LastIndexOf("/")) : null;
        if (dir == null)
        {
            dir = rootDir;
        }
        else
        {
            dir = rootDir + "/" + dir;
        }

        string fileName = asset.name;

        AssetInfo assetInfo = new AssetInfo()
        {
            asset = asset,
            dir = dir,
            fileName = fileName
        };
        Dictionary<string, AssetInfo> items = null;
        if (!assets.ContainsKey(type))
        {
            items = new Dictionary<string, AssetInfo>();
            assets.Add(type, items);
        }
        else
        {
            items = assets[type];
        }
        if (!items.ContainsKey(assetInfo.fileName))
        {
            items.Add(assetInfo.fileName, assetInfo);
        }

        if (depens != null)
            depens.AddUtfString(dir + "/" + fileName);
        return assetInfo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assets"></param>
    /// <param name="type"></param>
    /// <param name="options"></param>
    private static void BuilderAsset(Dictionary<BuilderGuiType, Dictionary<string, AssetInfo>> assets, BuilderGuiType type, BuildAssetBundleOptions options)
    {
        if (assets.ContainsKey(type))
        {
            Dictionary<string, AssetInfo> items = assets[type];
            ICollection tem = items.Values;
            foreach (AssetInfo assetInfo in tem)
            {
                BuildAssetBundle(assetInfo.asset, null, assetInfo.dir, assetInfo.fileName, options);
            }
        }
    }

}

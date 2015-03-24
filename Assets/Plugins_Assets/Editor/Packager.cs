using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Packager {
    public static string platform = string.Empty;
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    ///-----------------------------------------------------------
    static string[] exts = { ".txt", ".xml", ".lua", ".assetbundle" };
    static bool CanCopy(string ext) {   //�ܲ��ܸ���
        foreach (string e in exts) {
            if (ext.Equals(e)) return true;
        }
        return false;
    }

    /// <summary>
    /// �����ز�
    /// </summary>
    static UnityEngine.Object LoadAsset(string file) {
        if (file.EndsWith(".lua")) file += ".txt";
        return AssetDatabase.LoadMainAssetAtPath("Assets/Builds/" + file);
    }

    /// <summary>
    /// ���ɰ��ز�
    /// </summary>
    [MenuItem("Game/Build Bundle Resource")]
    public static void BuildAssetResource() {
        Object mainAsset = null;        //���ز���������
        Object[] addis = null;     //�����ز��������
        string assetfile = string.Empty;  //�ز��ļ���

        BuildAssetBundleOptions options = BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle;

        BuildTarget target;
        if (Application.platform == RuntimePlatform.OSXEditor) {
#if UNITY_5
            target = BuildTarget.iOS;
#else
            target = BuildTarget.iPhone;
#endif
        } else {
            target = BuildTarget.Android;
        }
        //string assetPath = (Application.dataPath + "/StreamingAssets/" + target + "/").ToLower();
        //if (!Directory.Exists(assetPath)) Directory.CreateDirectory(assetPath);

        /////-----------------------------���ɹ���Ĺ������زİ�-------------------------------------
        //BuildPipeline.PushAssetDependencies();

        //assetfile = assetPath + "shared.assetbundle";
        //mainAsset = LoadAsset("Shared/Atlas/Dialog.prefab");
        //BuildPipeline.BuildAssetBundle(mainAsset, null, assetfile, options, target);

        /////------------------------------����PromptPanel�زİ�-----------------------------------
        //BuildPipeline.PushAssetDependencies();
        //mainAsset = LoadAsset("Prompt/Prefabs/PromptPanel.prefab");
        //addis = new Object[1];
        //addis[0] = LoadAsset("Prompt/Prefabs/PromptItem.prefab");
        //assetfile = assetPath + "prompt.assetbundle";
        //BuildPipeline.BuildAssetBundle(mainAsset, addis, assetfile, options, target);
        //BuildPipeline.PopAssetDependencies();

        /////------------------------------����MessagePanel�زİ�-----------------------------------
        //BuildPipeline.PushAssetDependencies();
        //mainAsset = LoadAsset("Message/Prefabs/MessagePanel.prefab");
        //assetfile = assetPath + "message.assetbundle";
        //BuildPipeline.BuildAssetBundle(mainAsset, null, assetfile, options, target);
        //BuildPipeline.PopAssetDependencies();

        /////-------------------------------ˢ��---------------------------------------
        //BuildPipeline.PopAssetDependencies();

        HandleLuaFile();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// ����Lua�ļ�
    /// </summary>
    static void HandleLuaFile() {
        BuildTarget target;
        if (Application.platform == RuntimePlatform.OSXEditor) {
#if UNITY_5
            target = BuildTarget.iOS;
#else
            target = BuildTarget.iPhone;
#endif
        } else {
            target = BuildTarget.Android;
        }
        string resPath = (Application.dataPath + "/StreamingAssets/" + target).ToLower();
        string luaPath = resPath + "/lua/";

        //----------����Lua�ļ�----------------
        if (Directory.Exists(luaPath)) {
            Directory.Delete(luaPath, true);
        }
        Directory.CreateDirectory(luaPath);

        paths.Clear(); files.Clear();
        string luaDataPath = Application.dataPath + "/lua/".ToLower();
        Recursive(luaDataPath);
        foreach (string f in files) {
            if (!f.EndsWith(".lua")) continue;
            string newfile = f.Replace(luaDataPath, "");
            string newpath = luaPath + newfile;
            string path = Path.GetDirectoryName(newpath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            File.Copy(f, newpath, true);
        }

        ///----------------------�����ļ��б�-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear(); files.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++) {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (!CanCopy(ext)) continue;

            string value = file.Replace(resPath, string.Empty); 
            sw.WriteLine(value);
        }
        sw.Close(); fs.Close();

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// ����Ŀ¼
    /// </summary>
    static string AppDataPath {
        get { return Application.dataPath.ToLower(); }
    }

    /// <summary>
    /// ����Ŀ¼������Ŀ¼
    /// </summary>
    static void Recursive(string path) {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names) {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs) {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }
}
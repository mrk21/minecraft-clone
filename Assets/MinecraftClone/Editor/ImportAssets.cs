using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class ImportAssets
{
    [DataContract]
    class Asset
    {
        [DataMember(Name = "unitypackages")]
        public string[] UnityPackages { get; set; }

        [IgnoreDataMember]
        public string BasePath { get; set; } = "";

        [IgnoreDataMember]
        public List<string> UnityPackagePaths
        {
            get { return UnityPackages.Select(path => Path.Combine(BasePath, path)).ToList(); }
        }
    }

    public class Args
    {
        public string BasePath { get; }

        public Args(string basePath)
        {
            BasePath = basePath;
        }
    }

    [MenuItem("Window/ImportAssets")]
    public static void ImportForMenu()
    {
        Import();
    }

    public static void Import(Args args = null)
    {
        args = args ?? ArgsFromCommandLine();

        var rootPath = Path.GetDirectoryName(Application.dataPath);
        var jsonPath = Path.Combine(rootPath, "Packages", "asset.json");
        var jsonStream = new FileStream(jsonPath, FileMode.Open);
        var serializer = new DataContractJsonSerializer(typeof(Asset));
        var assets = serializer.ReadObject(jsonStream) as Asset;
        assets.BasePath = args.BasePath ?? DefaultBasePath();

        var type = typeof(AssetDatabase);
        var binding = BindingFlags.NonPublic | BindingFlags.Static;
        var importPackageImmediately = type.GetMethod("ImportPackageImmediately", binding);

        assets.UnityPackagePaths.ForEach(path => {
            importPackageImmediately.Invoke(type, new object[] { path });
        });
    }

    /// <summary>
    /// - Windows: "%APPDATA%\Unity\Asset Store-5.x"
    /// - Mac: "~/Library/Unity/Asset Store-5.x"
    /// - Linux: "~/.local/share/unity3d/Asset Store-5.x"
    /// </summary>
    ///
    /// <see href="https://answers.unity.com/questions/1450714/where-are-assets-downloaded-to-on-linux.html">Where are assets downloaded to on Linux? [solved] - Unity Answers</see>
    /// <see href="https://answers.unity.com/questions/1019767/how-to-download-an-unitypackage-asset-from-the-ass.html">How to download an .unitypackage asset from the Asset Store? - Unity Answers</see>
    private static string DefaultBasePath()
    {
#if UNITY_EDITOR_WIN
        var appData = System.Environment.GetEnvironmentVariable("APPDATA");
        return Path.Combine(appData, "Unity", "Asset Store-5.x");
#elif UNITY_EDITOR_OSX
        var home = System.Environment.GetEnvironmentVariable("HOME");
        return Path.Combine(home, "Library", "Unity", "Asset Store-5.x");
#else
        var home = System.Environment.GetEnvironmentVariable("HOME");
        return Path.Combine(home, ".local", "share", "unity3d", "Asset Store-5.x");
#endif
    }

    private static Args ArgsFromCommandLine()
    {
        var args = System.Environment.GetCommandLineArgs();
        string basePath = null;

        for (int i = 0; i < args.Length; ++i)
        {
            switch (args[i])
            {
                case "/basePath":
                    basePath = args[++i];
                    break;
            }
        }
        return new Args(basePath: basePath);
    }
}
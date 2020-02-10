using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Builder
{
    private static Dictionary<string, BuildTarget> Platforms { get; } = new Dictionary<string, BuildTarget>
    {
        { "webgl", BuildTarget.WebGL },
        { "osx", BuildTarget.StandaloneOSX },
    };

    private static Dictionary<BuildTarget, string> OutputNames { get; } = new Dictionary<BuildTarget, string>
    {
        { BuildTarget.WebGL, "minecraft-clone.webgl" },
        { BuildTarget.StandaloneOSX, "minecraft-clone.app" },
    };

    public class Args
    {
        public BuildTarget Platform { get; }

        public Args(BuildTarget platform)
        {
            Platform = platform;
        }
    }

    [MenuItem("Window/BuildWebGL")]
    public static void BuildWebGL()
    {
        Build(new Args(platform: BuildTarget.WebGL));
    }

    public static void Build(Args args = null)
    {
        args = args ?? ArgsFromCommandLine();

        var rootPath = Path.GetDirectoryName(Application.dataPath);
        var outputPath = Path.Combine(rootPath, "Build", OutputNames[args.Platform]);
        try
        {
            Directory.Delete(outputPath, true);
        }
        catch (DirectoryNotFoundException)
        {
            // noop
        }

        var levels = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        BuildPipeline.BuildPlayer(levels, outputPath, args.Platform, BuildOptions.None);
    }

    private static Args ArgsFromCommandLine()
    {
        var args = System.Environment.GetCommandLineArgs();
        string platform = null;

        for (int i = 0; i < args.Length; ++i)
        {
            switch (args[i])
            {
                case "/platform":
                    platform = args[++i];
                    break;
            }
        }
        return new Args(platform: Platforms[platform]);
    }
}

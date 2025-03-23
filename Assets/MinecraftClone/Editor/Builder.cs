using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Builder
{
    // Dictionary mapping command-line platform names to Unity BuildTargets
    private static Dictionary<string, BuildTarget> Platforms { get; } = new Dictionary<string, BuildTarget>
    {
        { "webgl", BuildTarget.WebGL },
        { "osx", BuildTarget.StandaloneOSX },
        { "windows", BuildTarget.StandaloneWindows64 },
        { "linux", BuildTarget.StandaloneLinux64 },
        { "android", BuildTarget.Android }
    };

    // Class to hold build arguments
    public class Args
    {
        public List<BuildTarget> Platforms { get; }
        public string BuildDirectory { get; }
        public BuildOptions BuildOptions { get; }
        public string Version { get; }
        public bool CleanBuild { get; }

        public Args(List<BuildTarget> platforms, string buildDirectory = "Build", BuildOptions buildOptions = BuildOptions.None, string version = null, bool cleanBuild = false)
        {
            Platforms = platforms;
            BuildDirectory = buildDirectory;
            BuildOptions = buildOptions;
            Version = version;
            CleanBuild = cleanBuild;
        }
    }

    // Menu item to build for WebGL
    [MenuItem("Window/BuildWebGL")]
    public static void BuildWebGL()
    {
        Build(new Args(new List<BuildTarget> { BuildTarget.WebGL }));
    }

    // Menu item to build for Windows
    [MenuItem("Window/BuildWindows")]
    public static void BuildWindows()
    {
        Build(new Args(new List<BuildTarget> { BuildTarget.StandaloneWindows64 }));
    }

    // Menu item to build for Linux
    [MenuItem("Window/BuildLinux")]
    public static void BuildLinux()
    {
        Build(new Args(new List<BuildTarget> { BuildTarget.StandaloneLinux64 }));
    }

    // Menu item to build for Android
    [MenuItem("Window/BuildAndroid")]
    public static void BuildAndroid()
    {
        Build(new Args(new List<BuildTarget> { BuildTarget.Android }));
    }

    // Menu item to build for all supported platforms
    [MenuItem("Window/BuildAll")]
    public static void BuildAll()
    {
        var allPlatforms = Platforms.Values.ToList();
        Build(new Args(allPlatforms));
    }

    /// <summary>
    /// Builds the project for the specified platforms with customizable options.
    /// </summary>
    /// <param name="args">Build arguments, including platforms, build directory, options, version, and clean build flag.</param>
    public static void Build(Args args = null)
    {
        // Use command-line args if none provided
        args = args ?? ArgsFromCommandLine();

        var rootPath = Path.GetDirectoryName(Application.dataPath);
        var buildPath = Path.Combine(rootPath, args.BuildDirectory);

        // Clean the build directory if requested
        if (args.CleanBuild)
        {
            try
            {
                Directory.Delete(buildPath, true);
            }
            catch (DirectoryNotFoundException)
            {
                // Ignore if directory doesn't exist
            }
        }

        // Add version subfolder if specified
        if (!string.IsNullOrEmpty(args.Version))
        {
            buildPath = Path.Combine(buildPath, args.Version);
        }

        // Get enabled scenes from EditorBuildSettings
        var levels = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        // Build for each platform
        foreach (var platform in args.Platforms)
        {
            var outputPath = Path.Combine(buildPath, GetOutputName(platform));
            BuildPipeline.BuildPlayer(levels, outputPath, platform, args.BuildOptions);
        }
    }

    // Generates platform-specific output names using the product name
    private static string GetOutputName(BuildTarget platform)
    {
        string productName = PlayerSettings.productName;
        switch (platform)
        {
            case BuildTarget.WebGL:
                return $"webgl/{productName}.webgl";
            case BuildTarget.StandaloneOSX:
                return $"osx/{productName}.app";
            case BuildTarget.StandaloneWindows64:
                return $"windows/{productName}.exe";
            case BuildTarget.StandaloneLinux64:
                return $"linux/{productName}";
            case BuildTarget.Android:
                return $"android/{productName}.apk";
            default:
                throw new System.ArgumentException("Unsupported platform");
        }
    }

    // Parses command-line arguments into an Args instance
    private static Args ArgsFromCommandLine()
    {
        var args = System.Environment.GetCommandLineArgs();
        List<string> platforms = new List<string>();
        string buildDirectory = "Build";
        string buildOptionsStr = null;
        string version = null;
        bool cleanBuild = false;

        for (int i = 0; i < args.Length; ++i)
        {
            switch (args[i])
            {
                case "/platforms":
                    var platformsStr = args[++i];
                    platforms.AddRange(platformsStr.Split(',').Select(p => p.Trim()));
                    break;
                case "/buildDirectory":
                    buildDirectory = args[++i];
                    break;
                case "/buildOptions":
                    buildOptionsStr = args[++i];
                    break;
                case "/version":
                    version = args[++i];
                    break;
                case "/cleanBuild":
                    bool.TryParse(args[++i], out cleanBuild);
                    break;
            }
        }

        var buildTargets = platforms.Select(p => Platforms[p]).ToList();
        var buildOptions = ParseBuildOptions(buildOptionsStr);

        return new Args(buildTargets, buildDirectory, buildOptions, version, cleanBuild);
    }

    // Parses a comma-separated string into BuildOptions flags
    private static BuildOptions ParseBuildOptions(string optionsString)
    {
        BuildOptions options = BuildOptions.None;
        if (string.IsNullOrEmpty(optionsString)) return options;

        var parts = optionsString.Split(',');
        foreach (var part in parts)
        {
            if (Enum.TryParse(part.Trim(), out BuildOptions option))
            {
                options |= option;
            }
            else
            {
                Debug.LogWarning($"Invalid build option: {part}");
            }
        }
        return options;
    }
}

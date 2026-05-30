#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace ValeDosCristais.EditorTools
{
    public static class BuildWindows
    {
        [MenuItem("Build/Vale dos Cristais/Windows 64-bit")]
        public static void BuildWindows64()
        {
            string outputDirectory = "Builds/Windows";
            Directory.CreateDirectory(outputDirectory);
            BuildPipeline.BuildPlayer(
                new[] { "Assets/Scenes/Main.unity" },
                Path.Combine(outputDirectory, "ValeDosCristais.exe"),
                BuildTarget.StandaloneWindows64,
                BuildOptions.None);
        }
    }
}
#endif

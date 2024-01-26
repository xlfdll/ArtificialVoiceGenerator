using System;
using System.IO;
using System.Reflection;

namespace ArtificialVoiceGenerator
{
    public static class ToolPaths
    {
        public static String ToolsPath
            => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Tools");
        public static String SoxPath
            => Path.Combine(ToolPaths.ToolsPath, "sox.exe");
    }
}
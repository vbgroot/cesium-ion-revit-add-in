using System.Diagnostics;
using System.IO;

namespace Cesium.Ion.Revit
{
    public static class Utils
    {
        public static void OpenBrowser(this string URL)
        {
            Process.Start(URL);
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}

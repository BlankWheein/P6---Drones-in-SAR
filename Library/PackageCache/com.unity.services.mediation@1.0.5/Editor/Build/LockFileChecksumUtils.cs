#if UNITY_IOS || UNITY_ANDROID

using System;
using System.IO;
using System.Security.Cryptography;

namespace Unity.Services.Mediation.Build.Editor
{
    static class LockFileChecksumUtils
    {
        internal static string GetHashFromFile(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hashBytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes);
                }
            }
        }

        internal static bool HasValidChecksum(string currentHash, string checksumFilePath)
        {
            if (File.Exists(checksumFilePath))
            {
                try
                {
                    var checksumFileContents = File.ReadAllText(checksumFilePath);
                    return currentHash == checksumFileContents;
                }
                catch (Exception)
                {
                    // Issues reading the file -> return false
                }
            }
            return false;
        }

        internal static void GenerateChecksumFile(string currentHash, string checksumFilePath)
        {
            var checksumDirectoryPath = Path.GetDirectoryName(checksumFilePath);
            if (checksumDirectoryPath == null)
            {
                //Invalid path, abort
                return;
            }
            Directory.CreateDirectory(checksumDirectoryPath);
            try
            {
                File.WriteAllText(checksumFilePath, currentHash);
            }
            catch (Exception)
            {
                //Issues writing to file, abort
            }
        }
    }
}

#endif

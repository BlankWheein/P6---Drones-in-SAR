#if UNITY_ANDROID
using System;
using System.IO;
using System.Text.RegularExpressions;
using Unity.Services.Mediation;
using UnityEngine;

namespace Unity.Mediation.Build.Editor
{
    static class FileContentAppender
    {
        const string k_ReadFileError = "Issue reading gradle properties file. Aborting UpdateDependencies patch.";
        const string k_WriteFileError = "Issue writing gradle properties file. Aborting UpdateDependencies patch.";

        internal static bool AppendContentToFile(string filePath, string appendContent, string regex)
        {
            var buildGradleContents = "";
            try
            {
                buildGradleContents = File.ReadAllText(filePath);
            }
            catch (Exception)
            {
                MediationLogger.Log(k_ReadFileError);
                return false;
            }

            var settingPresentRegex = new Regex(regex,
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!settingPresentRegex.IsMatch(buildGradleContents))
            {
                buildGradleContents += appendContent;
                try
                {
                    File.WriteAllText(filePath, buildGradleContents);
                }
                catch (Exception)
                {
                    MediationLogger.Log(k_WriteFileError);
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
#endif

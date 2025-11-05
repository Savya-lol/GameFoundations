using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Darkmatter.Core.Editor
{
    [InitializeOnLoad]
    public class DarkmatterPackageInstaller
    {
        private static ListRequest listRequest;
        private static Dictionary<string, string> requiredPackages;
        private static readonly string packageJsonGuid = "386cda33fd8b7453db02e90dcf77d120"; // Replace with actual GUID
        private static readonly string prefsKey = "DarkmatterCore_PackagesChecked";

        static DarkmatterPackageInstaller()
        {
            // Only check once per session to avoid annoying users
            if (SessionState.GetBool(prefsKey, false))
                return;

            SessionState.SetBool(prefsKey, true);
            EditorApplication.update += CheckPackages;
        }

        private static void CheckPackages()
        {
            EditorApplication.update -= CheckPackages;

            // Read package.json
            if (!LoadPackageJson())
            {
                Debug.LogWarning("Darkmatter Core: Could not read package.json");
                return;
            }

            // Get installed packages
            listRequest = Client.List();
            EditorApplication.update += WaitForList;
        }

        private static bool LoadPackageJson()
        {
            try
            {
                string packageJsonPath = AssetDatabase.GUIDToAssetPath(packageJsonGuid);
                
                if (string.IsNullOrEmpty(packageJsonPath))
                {
                    Debug.LogWarning($"Darkmatter Core: package.json not found. GUID: {packageJsonGuid}");
                    return false;
                }

                // For TextAsset in Packages folder, we need to read directly
                string fullPath = Path.GetFullPath(packageJsonPath);
                
                if (!File.Exists(fullPath))
                {
                    Debug.LogWarning($"Darkmatter Core: package.json file doesn't exist at {fullPath}");
                    return false;
                }

                string json = File.ReadAllText(fullPath);
                requiredPackages = ParseDependencies(json);

                if (requiredPackages == null || requiredPackages.Count == 0)
                {
                    Debug.Log("Darkmatter Core: No dependencies found in package.json");
                    return false;
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Darkmatter Core: Error reading package.json: {e.Message}");
                return false;
            }
        }

        private static Dictionary<string, string> ParseDependencies(string json)
        {
            var dict = new Dictionary<string, string>();
            
            // Find dependencies section
            int startIndex = json.IndexOf("\"dependencies\"");
            if (startIndex == -1) return dict;

            startIndex = json.IndexOf("{", startIndex);
            int endIndex = json.IndexOf("}", startIndex);
            
            if (startIndex == -1 || endIndex == -1) return dict;

            string depsJson = json.Substring(startIndex + 1, endIndex - startIndex - 1);
            
            // Split by commas but be careful with nested objects
            var lines = depsJson.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string line in lines)
            {
                if (line.Contains(":"))
                {
                    string[] parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim().Trim('"', ',', ' ');
                        string value = parts[1].Trim().Trim('"', ',', ' ');
                        
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            dict[key] = value;
                        }
                    }
                }
            }

            return dict;
        }

        private static void WaitForList()
        {
            if (!listRequest.IsCompleted) return;

            EditorApplication.update -= WaitForList;

            if (listRequest.Status == StatusCode.Success)
            {
                var installedPackages = new HashSet<string>();
                foreach (var package in listRequest.Result)
                {
                    installedPackages.Add(package.name);
                }

                var missingPackages = new List<KeyValuePair<string, string>>();
                foreach (var required in requiredPackages)
                {
                    if (!installedPackages.Contains(required.Key))
                    {
                        missingPackages.Add(required);
                    }
                }

                if (missingPackages.Count > 0)
                {
                    ShowInstallDialog(missingPackages);
                }
                else
                {
                    Debug.Log("Darkmatter Core: All required packages are installed.");
                }
            }
            else
            {
                Debug.LogError("Darkmatter Core: Failed to list packages");
            }
        }

        private static void ShowInstallDialog(List<KeyValuePair<string, string>> packages)
        {
            string message = "Darkmatter Core requires the following packages:\n\n";
            
            foreach (var package in packages)
            {
                message += $"• {package.Key} (v{package.Value})\n";
            }
            
            message += "\nWould you like to install them now?";

            if (EditorUtility.DisplayDialog("Darkmatter Core - Required Packages", message, "Install Now", "Later"))
            {
                InstallPackages(packages);
            }
            else
            {
                Debug.LogWarning("Darkmatter Core: Required packages were not installed. Some features may not work correctly.");
            }
        }

        private static void InstallPackages(List<KeyValuePair<string, string>> packages)
        {
            foreach (var package in packages)
            {
                string packageId = $"{package.Key}@{package.Value}";
                Debug.Log($"Installing {packageId}...");
                Client.Add(packageId);
            }

            EditorUtility.DisplayDialog(
                "Installing Packages",
                "Package installation started. Check the Package Manager window for progress.",
                "OK"
            );
        }

        [MenuItem("Darkmatter/Check Required Packages")]
        private static void ManualCheck()
        {
            SessionState.SetBool(prefsKey, false);
            CheckPackages();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

public class PackEditor : EditorWindow
{
    [MenuItem("Pack/GenerateHotUpDate(OnlyOne)", priority = 0)]
    public static void GenerateALL()
    {
        PrebuildCommand.GenerateAll();
    }

    [MenuItem("Pack/PackAotDLLs", priority = 1)]
    public static void CopyAllDllsTOAssets()
    {
        CopyAotDllToAssets();
        CopyDllToAssets();
    }

    [MenuItem("Pack/BuildPackage", priority = 2)]
    public static void BuildPackage()
    {
        string PackageName = "DefaultPackage";
        EBuildPipeline BuildPipeline = EBuildPipeline.BuiltinBuildPipeline;
        BuildTarget BuildTarget = EditorUserBuildSettings.activeBuildTarget;
        var buildMode = AssetBundleBuilderSetting.GetPackageBuildMode(PackageName, BuildPipeline);
        var fileNameStyle = AssetBundleBuilderSetting.GetPackageFileNameStyle(PackageName, BuildPipeline);
        var buildinFileCopyOption =
            AssetBundleBuilderSetting.GetPackageBuildinFileCopyOption(PackageName, BuildPipeline);
        var buildinFileCopyParams =
            AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(PackageName, BuildPipeline);
        var compressOption = AssetBundleBuilderSetting.GetPackageCompressOption(PackageName, BuildPipeline);

        BuiltinBuildParameters buildParameters = new BuiltinBuildParameters();
        buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
        buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
        buildParameters.BuildPipeline = BuildPipeline.ToString();
        buildParameters.BuildTarget = BuildTarget;
        buildParameters.BuildMode = buildMode;
        buildParameters.PackageName = PackageName;
        buildParameters.PackageVersion = GetPackageVersion();
        buildParameters.EnableSharePackRule = true;
        buildParameters.VerifyBuildingResult = true;
        buildParameters.FileNameStyle = fileNameStyle;
        buildParameters.BuildinFileCopyOption = buildinFileCopyOption;
        buildParameters.BuildinFileCopyParams = buildinFileCopyParams;
        //buildParameters.EncryptionServices = CreateEncryptionInstance();
        buildParameters.CompressOption = compressOption;

        BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
        var buildResult = pipeline.Run(buildParameters, true);
        if (buildResult.Success)
            EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
    }

    private static string GetPackageVersion()
    {
        int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
        return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
    }

    private static void CopyAotDllToAssets()
    {
        List<string> aotDlls = new List<string>();
        foreach (var dllName in AOTGenericReferences.PatchedAOTAssemblyList)
        {
            aotDlls.Add(dllName);
        }

        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        string buildDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        string folderPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + buildDir;
        Debug.Log(folderPath);
        string[] files = Directory.GetFiles(folderPath);
        string targetDirectory = Application.streamingAssetsPath;
        foreach (var file in files)
        {
            foreach (var aotFile in aotDlls)
            {
                if (file.Contains(aotFile))
                {
                    string newFileName = aotFile + ".bytes";
                    string destinationPath = Path.Combine(targetDirectory, newFileName);

                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }

                    File.Copy(file, destinationPath);
                    // 刷新资源，使其在 Unity 编辑器中可见
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    private static void CopyDllToAssets()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        string buildDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        // 项目配置的热更dll
        for (int i = 0; i < HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.Length; i++)
        {
            string fileName = HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions[i].name + ".dll";
            string sourcePath = Directory.GetFiles(buildDir).ToList().Find(hotPath => hotPath.Contains(fileName));
            if (string.IsNullOrEmpty(sourcePath))
            {
                Debug.Log($"热更程序集不存在: {buildDir} / {fileName}");
                Debug.LogError($"热更程序集不存在: {buildDir} / {fileName}");
                continue;
            }

            // 将程序集添加后缀 .bytes 并复制到AB包路径下
            string newFileName = fileName + ".bytes";
            //目标目录路径
            string targetDirectory = Application.dataPath + "/HotUpDateDll";

            Debug.Log($"目标目录路径:{targetDirectory} ");
            // 检查源文件是否存在
            if (File.Exists(sourcePath))
            {
                // 构建目标文件的完整路径
                string destinationPath = Path.Combine(targetDirectory, newFileName);
                // 检查目标目录是否存在，如果不存在则创建
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                // 如果目标文件已经存在，则删除
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                // 将源文件复制到目标目录下，并修改名称
                File.Copy(sourcePath, destinationPath);
                // 刷新资源，使其在 Unity 编辑器中可见
                AssetDatabase.Refresh();
                Debug.Log("File copied successfully!");
            }
            else
            {
                Debug.LogError("Source file does not exist!");
            }
        }

        Debug.Log("复制热更的DLL到资源目录 完成!!!");
    }
}
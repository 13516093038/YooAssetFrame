using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using HybridCLR;
using YooAsset;

namespace Aot
{
    public class HotUpdate : MonoBehaviour
    {
        public EPlayMode PlayMode = EPlayMode.HostPlayMode;

        // 资源系统运行模式
        private static ResourcePackage package;

        private string remoteAssetUrl = "http://192.168.10.71/";

        async void Start()
        {
            await UpdatePackageManifest();
            //补充元数据
            await LoadMetadataForAOTAssemblies();
#if !UNITY_EDITOR
            // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
            AssetHandle dllHandle = package.LoadAssetAsync<TextAsset>("Assets/HotUpDataDll/HotUpdate.dll.bytes");
            await dllHandle.Task;
            var dllAsset = dllHandle.AssetObject as TextAsset;
            Assembly hotUpdateAss = Assembly.Load(dllAsset.bytes);
#else
            // Editor下无需加载，直接查找获得HotUpdate程序集
            Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies()
                .First(a => a.GetName().Name == "HotUpdate");
#endif
            Type[] types = hotUpdateAss.GetTypes();
            foreach (var item in types)
            {
                if (item.Name == "GameEntry")
                {
                    Debug.Log(item.Name);
                    foreach (var method in item.GetMethods())
                    {
                        if (method.Name == "LoadGameRoot")
                        {
                            method.Invoke(null, null);
                            Debug.Log(method.Name);
                        }
                    }
                }
            }

            // Type type = hotUpdateAss.GetType("GameEntry");
            // type.GetMethod("LoadGameRoot").Invoke(null, null);

        }

        async Task LoadMetadataForAOTAssemblies()
        {
            List<string> aotDllList = new List<string>(AOTGenericReferences.PatchedAOTAssemblyList);
            foreach (var aotDllName in aotDllList)
            {
                
                AssetHandle dllHandle = package.LoadAssetAsync<TextAsset>("Assets/AOTDLL/" + aotDllName + ".bytes");
                await dllHandle.Task;
                byte[] dllBytes = (dllHandle.AssetObject as TextAsset).bytes;
                var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
            }
        }

        async Task UpdatePackageManifest()
        {
            YooAssets.Initialize();
            // 创建默认的资源包
            package = YooAssets.CreatePackage("DefaultPackage");
            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(package);

            //编辑器模拟模式
            if (PlayMode == EPlayMode.EditorSimulateMode)
            {
                var initParameters = new EditorSimulateModeParameters();
                var simulateManifestFilePath =
                    EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline,
                        "DefaultPackage");
                initParameters.SimulateManifestFilePath = simulateManifestFilePath;
                var operation = package.InitializeAsync(initParameters);
                await operation.Task;
            }

            //单机模式
            if (PlayMode == EPlayMode.OfflinePlayMode)
            {
                var initParameters = new OfflinePlayModeParameters();
                var packageTask = package.InitializeAsync(initParameters);
                await packageTask.Task;
            }

            //联机运行模式
            if (PlayMode == EPlayMode.HostPlayMode)
            {
                HostPlayModeParameters initParametersHostPlayMode = new HostPlayModeParameters();
                initParametersHostPlayMode.BuildinQueryServices = new GameQueryServices();
                initParametersHostPlayMode.RemoteServices = new RemoteServices(remoteAssetUrl, remoteAssetUrl);
                var operation = package.InitializeAsync(initParametersHostPlayMode);
                await operation.Task;
            }
#if !UNITY_EDITOR
            //更新
            string localPackageVersion = package.GetPackageVersion();
            var updateOperation = package.UpdatePackageVersionAsync();
            await updateOperation.Task;

            if (updateOperation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                string packageVersion = updateOperation.PackageVersion;
                if (localPackageVersion == packageVersion)
                {
                    Debug.Log("服务器版本与本地版本相同不需要更新");
                }
                else
                {
                    Debug.Log($"Updated package Version : {packageVersion}");
                    await UpdatePackageManifest(packageVersion);
                }
            }
            else
            {
                //更新失败
                Debug.LogWarning($"更新失败{updateOperation.Error}");
            }
        }
        
        /// <summary>
        /// 更新资源manifest
        /// </summary>
        /// <param name="packageVersion"></param>
        private async Task UpdatePackageManifest(string packageVersion)
        {
            // 更新成功后自动保存版本号，作为下次初始化的版本。
            // 也可以通过operation.SavePackageVersion()方法保存。
            bool savePackageVersion = true;
            var operation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion, 30);
            await operation.Task;
            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                Debug.Log("更新成功");
                await Download();
            }
            else
            {
                //更新失败
                Debug.LogError(operation.Error);
            }
        }
        
        /// <summary>
        /// 下载资源
        /// </summary>
        private async Task Download()
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
    
            //没有需要下载的资源
            if (downloader.TotalDownloadCount == 0)
            {        
                Debug.Log("没有需要下载的资源");
                return;
            }

            //需要下载的文件总数和总大小
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;    

            //注册回调方法
            downloader.OnDownloadErrorCallback = (fileName,error) =>
            {
                Debug.LogWarning($"资源下载失败,资源名{fileName} + error:{error}");
            };
            downloader.OnDownloadProgressCallback = (totalDownloadCount,currentDownloadCount,totalDownloadBytes,currentDownloadBytes) =>
            {
                Debug.Log($"资源下载总数量{totalDownloadCount},已经下载的数量{currentDownloadCount}，总bytes数{totalDownloadBytes}，已经下载的bytes数{currentDownloadBytes}");
            };
            downloader.OnDownloadOverCallback = (isSuccessful) =>
            {
                Debug.Log("是否下载成功：" + isSuccessful);
            };
            downloader.OnStartDownloadFileCallback = (fileName, sizeBytes) =>
            {
                Debug.Log($"开始下载{fileName}，资源大小{sizeBytes}");
            };

            //开启下载
            downloader.BeginDownload();
            await downloader.Task;

            //检测下载结果
            if (downloader.Status == EOperationStatus.Succeed)
            {
                //下载成功
                Debug.Log("资源下载成功");
            }
            else
            {
                //下载失败
                Debug.Log("资源下载失败：" + downloader.Error);
            }

#endif

        }
    }
}
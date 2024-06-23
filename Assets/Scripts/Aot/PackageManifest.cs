using System;

namespace Aot
{
    [Serializable]
    public class PackageManifest
    {
        public string FileVersion;
        public string PackageName;
        public string PackageVersion;

        public BundleData[] BundleList;
    }

    [Serializable]
    public class BundleData
    {
        public string BundleName;
        public string UnityCRC;
        public string FileHash;
        public string FileCRC;
        public int FileSize;
        public bool Encrypted;
        public string[] Tags;
        public string[] DependIDs;
    }
}
using UnityEngine;

namespace ResourcesSupport
{
    public class ResourcesLoaderSetting : ScriptableObject, IUsings
    {
        public string createClassName;

        public string createPath;

        public LoadParameter[] parameters;
        
        public string[] editUsings;
        public string[] usings
        {
            get
            {
                return editUsings;
            }
        }

        public bool isAuto;

        [Header("除外ファイル名")]
        public string[] ignoreFileNames;
        [Header("除外ファイルパスの正規表現 Assetsから始まる")]
        public string[] ignorePathPatterns;
    }
}

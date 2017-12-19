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

        public string[] ignoreFileNames;
    }
}

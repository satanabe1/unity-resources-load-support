using UnityEngine;

namespace ResourcesSupport
{
    public class LoadParameter : ScriptableObject, IUsings
    {
        public string typeName;

        public string[] targetExtensions;

        public string[] editUsings;
        public string[] usings
        {
            get
            {
                return editUsings;
            }
        }

    }
}
using UnityEngine;
using System;

namespace ResourcesSupport
{
    public class LoadParameter : ScriptableObject, IUsings
    {
        public string typeName;

        [EnumFlags]
        public LoadType editLoadType = LoadType.Load;

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
using UnityEditor;
using UnityEngine;

namespace ResourcesSupport
{
    public class ResourcesLoaderCreaterWindow : EditorWindow
    {
        public static void Open()
        {
            GetWindow<ResourcesLoaderCreaterWindow>(true);
        }

        private ResourcesLoaderSetting setting;

        void OnGUI()
        {
            setting = EditorGUILayout.ObjectField(setting, typeof(ResourcesLoaderSetting), false) as ResourcesLoaderSetting;
            if (setting == null)
            {
                return;
            }

            if (GUILayout.Button("生成"))
            {
                ResourcesLoaderCreater.Create(setting);
            }
        }
    }
}

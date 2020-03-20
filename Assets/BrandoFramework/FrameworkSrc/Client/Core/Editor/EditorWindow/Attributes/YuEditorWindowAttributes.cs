
using System;

namespace Common.EditorWindow
{
    [AttributeUsage(AttributeTargets.Class)]
    public class YuEditorWindowSizeAttribute : Attribute
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public YuEditorWindowSizeAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class YuEditorWindowTitleAttribute : Attribute
    {
        public string Title { get; private set; }
        public string IconIconPath { get; private set; }

        public YuEditorWindowTitleAttribute(string title, string iconPath = null)
        {
            Title = title;
            IconIconPath = iconPath;
        }
    }
}


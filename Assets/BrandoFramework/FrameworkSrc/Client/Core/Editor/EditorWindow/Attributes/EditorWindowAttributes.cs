
using System;

namespace Client.Core.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorWindowSizeAttribute : Attribute
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public EditorWindowSizeAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EditorWindowTitleAttribute : Attribute
    {
        public string Title { get; private set; }
        public string IconIconPath { get; private set; }

        public EditorWindowTitleAttribute(string title, string iconPath = null)
        {
            Title = title;
            IconIconPath = iconPath;
        }
    }
}


using UnityEngine;
using System;
using System.Collections;

namespace Anonym.Util
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        //The name of the bool field that will be in control
        public string ConditionalSourceField = "";
        public string ConditionalValueString = "";
        public string HeaderString = "";
        public char[] SplitString;

        public bool HidenInspector = false;
        public bool Inverse = false;
        public bool Split = false;
        public bool Contain = false;

        public ConditionalHideAttribute(string conditionalSourceField, string conditionalSourceValue = "True", string header = "", bool hideInInspector = false)
		{
            if (conditionalSourceField.Contains("!"))
            {
                Inverse = true;
                ConditionalSourceField = conditionalSourceField.Split('!')[1];
            }
            else
			    ConditionalSourceField = conditionalSourceField;
            
            ConditionalValueString = conditionalSourceValue;
            HidenInspector = hideInInspector;
            HeaderString = header;
        }
    }
}
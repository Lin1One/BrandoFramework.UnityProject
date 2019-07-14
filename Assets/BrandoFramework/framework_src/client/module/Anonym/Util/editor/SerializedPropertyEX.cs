using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using UnityEditor;

namespace Anonym.Util
{
    public static class SerializedPropertyEX
    {
        /// <summary>
        /// Is it an array property?
        /// </summary>
        public static bool hasArray(this SerializedProperty property)
        {
            int index = SerializedPropertyEX.GetIndex(property.propertyPath);
            return index >= 0;
        }

        /// <summary>
        /// find and return a index in the serializedPropertyPath
    	/// </summary>
        public static int GetIndex(string path){
			var arraytoken = ".Array.data[";
			var indexer_start = path.IndexOf(arraytoken);
			if (indexer_start != -1){
				var indexer_end = path.IndexOf(']');
				indexer_start = indexer_start + arraytoken.Length;
				var digitStr = path.Substring(indexer_start, indexer_end - indexer_start);
				try{
					return int.Parse(digitStr);
				}catch{}
			}
			return -1;
		}

        /// <summary>
        /// Is Array Support
    	/// </summary>
        public static bool IsArray_ByFirstPath(this SerializedProperty property){
            string[] variableName = property.propertyPath.Split('.');
            SerializedProperty p = property.serializedObject.FindProperty(variableName[0]);
            UnityEngine.Debug.Log(variableName[0] + p.isArray);
            return p.isArray;
        }

        public static T GetActualObjectForSerializedProperty<T>(this SerializedProperty property) where T : class
        {
            var serializedObject = property.serializedObject;
            if (serializedObject == null)
            {
                return null;
            }
            var targetObject = serializedObject.targetObject;
            var field = targetObject.GetType().GetField(property.name);
            var obj = field.GetValue(targetObject);
            if (obj == null)
            {
                return null;
            }
            T actualObject = null;
            if (obj.GetType().IsArray)
            {
                var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
                actualObject = ((T[])obj)[index];
            }
            else
            {
                actualObject = obj as T;
            }
            return actualObject;
        }

        public static object GetTargetPath(this SerializedProperty property, string targetPath, bool bGetParent = true)
		{
			if (property == null || property.propertyPath.IndexOf(targetPath) < 0)
				return null;

			object now = property.serializedObject.targetObject;
			var property_names = property.propertyPath.Replace(".Array.data", ".").Split('.');
			var binding_flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
			foreach (var property_name in property_names) {
				var last = now;
				now = last.GetType().GetField(property_name, binding_flags).GetValue(last);
				if (property_name.Equals(targetPath)){
					return bGetParent ? last : now;
				}
			}
			return null;			
		}
		
		public static object GetCurrent(this SerializedProperty property) {
			int index = 0;
			return GetCurrent(property, ref index);
		}
		public static object GetCurrent(this SerializedProperty property, ref int index) {
			return GetCurrent(GetParentOfCurrent(property), property.propertyPath, ref index);
		}
		
		public static object GetParentOfCurrent(this SerializedProperty property) {
			object result = property.serializedObject.targetObject;
			var property_names = property.propertyPath.Replace(".Array.data", ".").Split('.');
			foreach (var property_name in property_names) {
				var parent = result;
				var indexer_start = property_name.IndexOf('[');
				if (-1 == indexer_start) {
					var binding_flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
					result = parent.GetType().GetField(property_name, binding_flags).GetValue(parent);
				} else {
					return parent;
				}
			}
			return result;
		}

		public static object GetValue(object source, string name)
		{
			if(source == null)
				return null;
			var type = source.GetType();
			var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if(f == null)
			{
				var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if(p == null)
					return null;
				return p.GetValue(source, null);
			}
			return f.GetValue(source);
		}

		public static object GetValue(object source, string name, int index)
		{
			var enumerable = GetValue(source, name) as IEnumerable;
			var enm = enumerable.GetEnumerator();
			while(index-- >= 0)
				enm.MoveNext();
			return enm.Current;
		}	

        public static object GetCurrent(object parentArray, string propertyPath, ref int index){
            object result = parentArray;
            var property_names = propertyPath.Replace(".Array.data", ".").Split('.');
            foreach (var property_name in property_names){
                var indexer_start = property_name.IndexOf('[');
                if (-1 != indexer_start){
                    var indexer_end = property_name.IndexOf(']');
                    var index_string = property_name.Substring(indexer_start + 1, indexer_end - indexer_start - 1);
                    index = int.Parse(index_string);
                    var type = result.GetType();
                    if (type.IsArray){
                        var array = (System.Array)result;
                        if (index < array.Length){
                            result = array.GetValue(index);
                        }else{
                            result = null;
                            break;
                        }
                        // } else if (type.IsGenericType){
                        // 	var array = ((List<WeightedKey<T>>)parent).ToArray();
                        // 	if (index < array.Length) {
                        // 		result = array.ElementAt(index);
                        // } else {
                        // 		result = null;
                        // 		break;
                        // }
                        // } else if (type == typeof(List<T>)){
                        // } else if (type == typeof(IEnumerable<T>)){
                    }else{
                        throw new System.MissingFieldException();
                    }
                }
            }
			return result;
		}
    }
}
using System.Collections.Generic;
namespace GameWorld
{

    /// <summary>
    /// ���ʲ���
    /// </summary>
    [System.Serializable]
    public class ShaderTextureProperty
    {
        public string name;         //��������  
        public bool isNormalMap;    //�Ƿ�����ͼ


        public ShaderTextureProperty(string n,bool norm)
        {
            name = n;
            isNormalMap = norm;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ShaderTextureProperty))
            {
                return false;
            }
            ShaderTextureProperty b = (ShaderTextureProperty)obj;
            if (!name.Equals(b.name)) return false;
            if (isNormalMap != b.isNormalMap) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static string[] GetNames(List<ShaderTextureProperty> props)
        {
            string[] ss = new string[props.Count];
            for (int i = 0; i < ss.Length; i++)
            {
                ss[i] = props[i].name;
            }
            return ss;
        }
    }
}

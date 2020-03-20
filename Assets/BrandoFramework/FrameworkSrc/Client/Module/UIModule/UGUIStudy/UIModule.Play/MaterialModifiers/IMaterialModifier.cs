using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// �����޸Ľӿ�
    /// �� UI ͼ��������Ⱦǰ���ɶԸ����Ĳ��ʽ����Զ�����޸�
    /// </summary>
    public interface IMaterialModifier
    {
        /// <summary>
        /// ��ȡ�޸ĺ��ʵ�ʲ�����Ⱦ�Ĳ���
        /// </summary>
        /// <param name="baseMaterial"></param>
        /// <returns></returns>
        Material GetModifiedMaterial(Material baseMaterial);
    }
}

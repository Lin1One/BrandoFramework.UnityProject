using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// 材质修改接口
    /// 在 UI 图形重新渲染前，可对给定的材质进行自定义的修改
    /// </summary>
    public interface IMaterialModifier
    {
        /// <summary>
        /// 获取修改后的实际参与渲染的材质
        /// </summary>
        /// <param name="baseMaterial"></param>
        /// <returns></returns>
        Material GetModifiedMaterial(Material baseMaterial);
    }
}

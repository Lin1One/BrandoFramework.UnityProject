using UnityEngine;
using System.Collections;

//设置在编辑模式下也执行该脚本
[ExecuteInEditMode]
public class ScreenOilPaintEffect : MonoBehaviour
{
    #region Variables

    public Shader CurShader;
    private Material CurMaterial;

    [Range(0, 5), Tooltip("分辨率比例值")]
    public float ResolutionValue = 0.9f;
    [Range(0, 30), Tooltip("半径的值，决定了迭代的次数")]
    public int RadiusValue = 5;

    //两个用于调节参数的中间变量
    public static float ChangeValue;
    public static int ChangeValue2;
    #endregion

    Material material
    {
        get
        {
            if (CurMaterial == null)
            {
                CurMaterial = new Material(CurShader);
                CurMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return CurMaterial;
        }
    }

    void Start()
    {
        ChangeValue = ResolutionValue;
        ChangeValue2 = RadiusValue;


        CurShader = Shader.Find("GraphicsStudio/PostProcessing/OilPaintEffect");

        //判断当前设备是否支持屏幕特效
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if(RadiusValue == 0)
        {
            //直接拷贝源纹理到目标渲染纹理
            Graphics.Blit(sourceTexture, destTexture);
        }
        //着色器实例不为空，就进行参数设置
        if (CurShader != null)
        {
            //给Shader中的外部变量赋值
            material.SetFloat("_ResolutionValue", ResolutionValue);
            material.SetInt("_Radius", RadiusValue);
            material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0.0f, 0.0f));

            //拷贝源纹理到目标渲染纹理，加上我们的材质效果
            Graphics.Blit(sourceTexture, destTexture, material);
        }
        else
        {
            //直接拷贝源纹理到目标渲染纹理
            Graphics.Blit(sourceTexture, destTexture);
        }
    }


    void OnValidate()
    {
        //将编辑器中的值赋值回来，确保在编辑器中值的改变立刻让结果生效
        ChangeValue = ResolutionValue;
        ChangeValue2 = RadiusValue;
    }
    void Update()
    {
        //若程序在运行，进行赋值
        if (Application.isPlaying)
        {
            //赋值
            ResolutionValue = ChangeValue;
            RadiusValue = ChangeValue2;
        }
        //若程序没有在运行，去寻找对应的Shader文件
#if UNITY_EDITOR
        if (Application.isPlaying != true && CurShader == null)
        {
            CurShader = Shader.Find("GraphicsStudio/PostProcessing/OilPaintEffect");
        }
#endif

    }

    void OnDisable()
    {
        if (CurMaterial)
        {
            //立即销毁材质实例
            DestroyImmediate(CurMaterial);
        }

    }
}

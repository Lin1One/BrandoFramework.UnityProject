using UnityEngine;
using UnityEngine.EventSystems;

namespace Client.UI
{
    [RequireComponent(typeof(Canvas))]
    [ExecuteInEditMode]
    [AddComponentMenu("Layout/Canvas Scaler", 101)]
    public class CanvasScaler : UIBehaviour
    {
        protected CanvasScaler() { }

        public enum ScaleMode
        {
            ConstantPixelSize,
            ScaleWithScreenSize,
            ConstantPhysicalSize
        }

        // General variables

        private Canvas m_Canvas;
        [System.NonSerialized]
        private float m_PrevScaleFactor = 1;
        [System.NonSerialized]
        private float m_PrevReferencePixelsPerUnit = 100;

        [Tooltip("���������е�UIԪ�����ŷ�ʽ")]
        [SerializeField] private ScaleMode m_UiScaleMode = ScaleMode.ConstantPixelSize;
        public ScaleMode uiScaleMode
        {
            get
            {
                return m_UiScaleMode;
            }
            set
            {
                m_UiScaleMode = value;
            }
        }


        [Tooltip("���������С�Pixels Per Unit�����ã������е�һ�����ؽ�����UI�е�һ����λ��")]
        [SerializeField] protected float m_ReferencePixelsPerUnit = 100;
        public float referencePixelsPerUnit
        {
            get
            {
                return m_ReferencePixelsPerUnit;
            }
            set
            {
                m_ReferencePixelsPerUnit = value;
            }
        }

        // World Canvas settings

        [Tooltip("����UI�ж�̬������λͼ��ÿ��λ������������Text��")]
        [SerializeField] protected float m_DynamicPixelsPerUnit = 1;
        public float dynamicPixelsPerUnit
        {
            get
            {
                return m_DynamicPixelsPerUnit;
            }
            set
            {
                m_DynamicPixelsPerUnit = value;
            }
        }

        #region �㶨���ش�Сģʽ
        // Constant Pixel Size settings

        [Tooltip("Scales all UI elements in the Canvas by this factor.")]
        [SerializeField] protected float m_ScaleFactor = 1;
        public float scaleFactor
        {
            get
            {
                return m_ScaleFactor;
            }
            set
            {
                m_ScaleFactor = value;
            }
        }
        #endregion

        #region ������Ļ�ߴ�ģʽ
        // Scale With Screen Size settings

        public enum ScreenMatchMode
        {
            MatchWidthOrHeight = 0,
            Expand = 1,
            Shrink = 2
        }

        /// <summary>
        /// ��׼�ֱ���
        /// </summary>
        [Tooltip("UI������ƵĻ�׼�ֱ���.�����Ļ�ֱ��ʽϴ���UI���������Ŵ������Ļ�ֱ��ʽ�С����UI����������С,���Ǹ�����Ļƥ��ģʽ��ɵ�.")]
        [SerializeField] protected Vector2 m_ReferenceResolution = new Vector2(800, 600);
        public Vector2 referenceResolution
        {
            get
            {
                return m_ReferenceResolution;
            }
            set
            {
                m_ReferenceResolution = value;
            }
        }

        [Tooltip("�����ǰ�ֱ��ʵĿ�߱Ȳ����ϲο��ֱ��ʣ����������Ż��������ģʽ��")]
        [SerializeField] protected ScreenMatchMode m_ScreenMatchMode = ScreenMatchMode.MatchWidthOrHeight;
        public ScreenMatchMode screenMatchMode
        {
            get
            {
                return m_ScreenMatchMode;
            }
            set
            {
                m_ScreenMatchMode = value;
            }
        }

        [Tooltip("����������ʹ�ÿ�Ȼ��Ǹ߶���Ϊ�ο���������֮��Ļ�ϡ�")]
        [Range(0, 1)]
        [SerializeField] protected float m_MatchWidthOrHeight = 0;
        public float matchWidthOrHeight
        {
            get
            {
                return m_MatchWidthOrHeight;
            }
            set
            {
                m_MatchWidthOrHeight = value;
            }
        }

        // The log base doesn't have any influence on the results whatsoever, as long as the same base is used everywhere.
        private const float kLogBase = 2;

        #endregion

        #region �㶨����ߴ�ģʽ
        // Constant Physical Size settings

        public enum Unit { Centimeters, Millimeters, Inches, Points, Picas }

        [Tooltip("The physical unit to specify positions and sizes in.")]
        [SerializeField] protected Unit m_PhysicalUnit = Unit.Points;
        public Unit physicalUnit { get { return m_PhysicalUnit; } set { m_PhysicalUnit = value; } }

        [Tooltip("The DPI to assume if the screen DPI is not known.")]
        [SerializeField] protected float m_FallbackScreenDPI = 96;
        public float fallbackScreenDPI { get { return m_FallbackScreenDPI; } set { m_FallbackScreenDPI = value; } }

        [Tooltip("The pixels per inch to use for sprites that have a 'Pixels Per Unit' setting that matches the 'Reference Pixels Per Unit' setting.")]
        [SerializeField] protected float m_DefaultSpriteDPI = 96;
        public float defaultSpriteDPI { get { return m_DefaultSpriteDPI; } set { m_DefaultSpriteDPI = value; } }

        #endregion

        #region �������ں���
        protected override void OnEnable()
        {
            base.OnEnable();
            m_Canvas = GetComponent<Canvas>();
            Handle();
        }

        protected override void OnDisable()
        {
            SetScaleFactor(1);
            SetReferencePixelsPerUnit(100);
            base.OnDisable();
        }

        protected virtual void Update()
        {
            Handle();
        }
        protected virtual void Handle()
        {
            if (m_Canvas == null || !m_Canvas.isRootCanvas)
                return;

            if (m_Canvas.renderMode == RenderMode.WorldSpace)
            {
                HandleWorldCanvas();
                return;
            }

            switch (m_UiScaleMode)
            {
                case ScaleMode.ConstantPixelSize:
                    HandleConstantPixelSize();
                    break;
                case ScaleMode.ScaleWithScreenSize:
                    HandleScaleWithScreenSize();
                    break;
                case ScaleMode.ConstantPhysicalSize:
                    HandleConstantPhysicalSize();
                    break;
            }
        }
        #endregion

        #region Canvas ScaleMode ������

        protected virtual void HandleWorldCanvas()
        {
            SetScaleFactor(m_DynamicPixelsPerUnit);
            SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
        }

        protected void SetScaleFactor(float scaleFactor)
        {
            if (scaleFactor == m_PrevScaleFactor)
                return;

            m_Canvas.scaleFactor = scaleFactor;
            m_PrevScaleFactor = scaleFactor;
        }

        protected void SetReferencePixelsPerUnit(float referencePixelsPerUnit)
        {
            if (referencePixelsPerUnit == m_PrevReferencePixelsPerUnit)
                return;

            m_Canvas.referencePixelsPerUnit = referencePixelsPerUnit;
            m_PrevReferencePixelsPerUnit = referencePixelsPerUnit;
        }

        protected virtual void HandleConstantPixelSize()
        {
            SetScaleFactor(m_ScaleFactor);
            SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
        }

        protected virtual void HandleScaleWithScreenSize()
        {
            //��Ļ�ߴ�
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            float scaleFactor = 0;
            switch (m_ScreenMatchMode)
            {
                case ScreenMatchMode.MatchWidthOrHeight:
                {
                        // We take the log of the relative width and height before taking the average.
                        // Then we transform it back in the original space.
                        // the reason to transform in and out of logarithmic space is to have better behavior.
                        // If one axis has twice resolution and the other has half, it should even out if widthOrHeight value is at 0.5.
                        // In normal space the average would be (0.5 + 2) / 2 = 1.25
                        // In logarithmic space the average is (-1 + 1) / 2 = 0
                        //������ȡƽ��ֵ֮ǰ��¼��Կ�Ⱥ͸߶ȵĶ�����
                        float logWidth = Mathf.Log(screenSize.x / m_ReferenceResolution.x, kLogBase);
                        float logHeight = Mathf.Log(screenSize.y / m_ReferenceResolution.y, kLogBase);
                        //Ȼ�����ǽ���ת����ԭʼ�ռ䡣
                        float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, m_MatchWidthOrHeight);
                        //������˳������ռ��ԭ����Ҫ�и��õ���Ϊ��
                        //���һ�����������ķֱ��ʶ���һ������һ�룬���widthOrHeightֵΪ0.5����Ӧ�þ��ȡ�
                        //�������ռ��У�ƽ��ֵΪ��0.5 + 2��/ 2 = 1.25
                        //�ڶ����ռ��У�ƽ��ֵΪ��-1 + 1��/ 2 = 0


                    scaleFactor = Mathf.Pow(kLogBase, logWeightedAverage);
                    break;
                }
                case ScreenMatchMode.Expand:
                {
                    scaleFactor = Mathf.Min(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                    break;
                }
                case ScreenMatchMode.Shrink:
                {
                    scaleFactor = Mathf.Max(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                    break;
                }
            }

            SetScaleFactor(scaleFactor);
            SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
        }

        protected virtual void HandleConstantPhysicalSize()
        {
            float currentDpi = Screen.dpi;
            float dpi = (currentDpi == 0 ? m_FallbackScreenDPI : currentDpi);
            float targetDPI = 1;
            switch (m_PhysicalUnit)
            {
                case Unit.Centimeters: targetDPI = 2.54f; break;
                case Unit.Millimeters: targetDPI = 25.4f; break;
                case Unit.Inches:      targetDPI =     1; break;
                case Unit.Points:      targetDPI =    72; break;
                case Unit.Picas:       targetDPI =     6; break;
            }

            SetScaleFactor(dpi / targetDPI);
            SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit * targetDPI / m_DefaultSpriteDPI);
        }

        #endregion

    }
}

namespace Client.UI
{
    /// <summary>
    /// ˮƽ������
    /// </summary>
    public class HorizontalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        protected HorizontalLayoutGroup()
        {}

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, false);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, false);
        }

        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, false);
        }

        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, false);
        }
    }
}


public partial class Sort
{
    //冒泡排序
    //基本思想：依次比较相邻的两个元素，如果前面的数据大于后面的数据，就将两个数据进行交换
    private static void BubbleSort(int[] arr)
    {
        for (int i = 0; i < arr.Length - 1; i++)
        {
            for (int j = 1; j < arr.Length - i; j++)
            {
                if (arr[j] < arr[j - 1])
                {
                    Swap(ref arr[j - 1], ref arr[j]);
                }
            }
        }
    }

    public void BubbleSort2(int[] arr)
    {
        bool didSwap;
        for (int i = 0, len = arr.Length; i < len - 1; i++)
        {
            didSwap = false;
            for (int j = 0; j < len - i - 1; j++)
            {
                if (arr[j + 1] < arr[j])
                {
                    Swap(ref arr[j], ref arr[j + 1]);
                    didSwap = true;
                }
            }
            if (didSwap == false)
                return;
        }
    }



}

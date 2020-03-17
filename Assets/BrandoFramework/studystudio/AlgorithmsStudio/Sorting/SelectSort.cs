
public partial class Sort
{
    private static void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }

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

}

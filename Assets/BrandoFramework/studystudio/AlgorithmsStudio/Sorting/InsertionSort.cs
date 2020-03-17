
public partial class Sort
{

    //插入排序
    //基本思想：将原来的无序数列看成含有一个元素的有序序列和一个无序序列，将无序序列中的值依次插入到有序序列中，完成排序。
    private static void InsertionSort(int[] arr)
    {
        for (int i = 1; i < arr.Length - 1; i++)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                if (arr[j] < arr[j + 1])
                {
                    Swap(ref arr[j], ref arr[j + 1]);
                }
            }
        }
    }

}

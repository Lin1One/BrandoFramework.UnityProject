
public partial class Sort
{
    private static void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }


    //选择排序
    //基本思想：在要排序的一组数中，选出最小的一个数与当前的第i个位置的数交换，之后依次类推
    private static void SelectSort(int[] arr)
    {
        for (int i = 0; i < arr.Length - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < arr.Length - 1; j++)
            {
                if (arr[j] < arr[min])
                {
                    min = j;
                }
            }
            if (min != i)
            {
                Swap(ref arr[i], ref arr[min]);
            }
        }
    }

}

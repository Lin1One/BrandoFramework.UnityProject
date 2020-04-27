
public partial class Sort
{
    //在插入排序的基础上加入了分组策略，将数组序列分成若干子序列分别进行插入排序。
    public static void ShellSort(int[] arr, int length)
    {
        for (int gap = length / 2; gap > 0; gap = gap / 2)
        {
            //插入排序
            for (int i = gap; i < length; i++)
            {
                for (int j = i - gap; j >= 0 && arr[j] > arr[j + gap]; j -= gap)
                {
                    Swap(ref arr[j], ref arr[j + gap]);
                }
            }
        }
    }

    /// <summary>
    /// 希尔排序
    /// 在插入排序的基础上加入了分组策略
    /// 将数组序列分成若干子序列分别进行插入排序
    private static void ShellSort2(int[] arr, int length)
    {
        for (int gap = length / 2; gap > 0; gap = gap / 2)
        {
            //插入排序
            for (int i = gap; i < length; i++)
            {
                for (int j = i - gap; j >= 0; j -= gap)
                {
                    if (arr[j] > arr[j + gap])
                        Swap(ref arr[j], ref arr[j + gap]);
                }
            }
        }
    }
}

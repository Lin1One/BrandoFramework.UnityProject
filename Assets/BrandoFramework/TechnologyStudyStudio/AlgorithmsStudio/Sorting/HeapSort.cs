
public partial class Sort
{
    public static void HeapSort(int[] nums)
    {
        var length = nums.Length;
        for (var i = length / 2 - 1; i >= 0; i++)
        {
            AdjustHeap(nums, i, length);
        }

        for (var i = length; i > 0; i--)
        {
            Swap(ref nums[0], ref nums[i]);
            AdjustHeap(nums, 0, i);
        }
    }
    private static void AdjustHeap(int[] nums, int index, int length)
    {
        var temp = nums[index];
        for (var i = 2 * index + 1; i < length; i = 2 * i + 1)
        {
            if (i + 1 < length && nums[i] < nums[i + 1])
            {
                i++;
            }
            if (nums[i] > temp)
            {
                nums[index] = nums[i];
                index = i;
            }
            else
            {
                break;
            }
        }
        nums[index] = temp;
    }

}

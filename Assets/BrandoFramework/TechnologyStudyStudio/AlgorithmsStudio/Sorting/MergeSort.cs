
public partial class Sort
{
    private static void MergeSort(int[] nums)
    {
        int[] temp = new int[nums.Length];
        MergeInternalSort(nums, 0, nums.Length, temp);
    }

    private static void MergeInternalSort(int[] nums, int first, int last, int[] helpArray)
    {
        if (first > last)
        {
            return;
        }
        int mid = (first + last) / 2;
        MergeInternalSort(nums, first, mid, helpArray);
        MergeInternalSort(nums, mid + 1, last, helpArray);
        Merge(nums, first, mid, last, helpArray);
    }

    private static void Merge(int[] nums, int first, int mid, int last, int[] helpArray)
    {
        int leftPartIndex = first;
        int rightPartIndex = mid + 1;
        int i = 0;
        while (leftPartIndex < mid && rightPartIndex < last)
        {
            if (nums[leftPartIndex] <= nums[rightPartIndex])
            {
                helpArray[i] = nums[leftPartIndex];
                i++;
                leftPartIndex++;
            }
            else
            {
                helpArray[i] = nums[rightPartIndex];
                i++;
                rightPartIndex++;
            }
        }
        while (leftPartIndex < mid)
        {
            helpArray[i] = nums[leftPartIndex++];
            i++;
        }
        while (rightPartIndex < last)
        {
            helpArray[i] = nums[rightPartIndex++];
            i++;
        }
        for (var j = 0; j < i; j++)
        {
            nums[j + first] = helpArray[i];
        }
    }


}

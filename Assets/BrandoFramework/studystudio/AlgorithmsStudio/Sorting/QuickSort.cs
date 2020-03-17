
public partial class Sort
{

    //快速排序
    //基本思想：采用分治的思想，对数组进行排序，每次排序都使得操作的数组部分分成以某个元素为分界值的两部分，
    //          一部分小于分界值，另一部分大于分界值。分界值一般称为“轴”。
    //          一般是以第一个元素为轴，将数组分成左右两部分，然后对左右两部分递归操作，直至完成排序。

    private static void QuickSort(int[] arr, int left, int right)
    {
        if(left < right)
        {
            int L = left;
            int R = right;
            int pivot = arr[L];
            while(L < R)
            {
                while(L < R && arr[R] > pivot)
                {
                    R--;
                }
                if(L < R)
                {
                    arr[L] = arr[R];
                    L++;
                }
                while(L < R && arr[L] < pivot)
                {
                    L++;
                }
                if(L < R)
                {
                    arr[R] = arr[L];
                    R--;
                }
            }
            arr[L] = pivot;
            QuickSort(arr, left, L - 1);
            QuickSort(arr, L + 1, right);
        }
    }

}

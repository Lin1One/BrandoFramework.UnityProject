
public partial class Sort
{

    //快速排序
    //基本思想：采用分治的思想，对数组进行排序，每次排序都使得操作的数组部分分成以某个元素为分界值的两部分，
    //          一部分小于分界值，另一部分大于分界值。分界值一般称为“轴”。
    //          一般是以第一个元素为轴，将数组分成左右两部分，然后对左右两部分递归操作，直至完成排序。

    public static void QuickSort(int[] arr, int left, int right)
    {
        if (left < right)
        {
            int L = left;
            int R = right;
            int pivot = arr[L];
            while (L < R)
            {
                while (L < R && arr[R] > pivot)
                {
                    R--;
                }
                if (L < R)
                {
                    arr[L] = arr[R];
                    L++;
                }
                while (L < R && arr[L] < pivot)
                {
                    L++;
                }
                if (L < R)
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

    public static void QuickSort2(int left, int right, int[] nums)
    {
        int i, j, t, temp;
        if (left > right)
            return;
        temp = nums[left]; //temp中存的就是基准数
        i = left;
        j = right;
        while (i != j)
        { //顺序很重要，要先从右边开始找
            while (nums[j] >= temp && i < j)
                j--;
            while (nums[i] <= temp && i < j)//再找右边的
                i++;
            if (i < j)//交换两个数在数组中的位置
            {
                t = nums[i];
                nums[i] = nums[j];
                nums[j] = t;
            }
        }
        //最终将基准数归位
        nums[left] = nums[i];
        nums[i] = temp;
        QuickSort2(left, i - 1, nums);//继续处理左边的，这里是一个递归的过程
        QuickSort2(i + 1, right, nums);//继续处理右边的 ，这里是一个递归的过程
    }

    public static void QuickSort_Optimization(int[] arr, int left, int right)
    {
        //去除单个元素或者没有元素的情况
        if (left < right)
        {
            int lt = left;
            int i = left + 1;//第一个元素是切分元素，所以i从low+1开始
            int gt = right;
            int pivot = arr[lt];
            while (i <= gt)
            {
                //小于切分元素放在lt左边，指针lt和i整体右移
                if (arr[i] < pivot)
                {
                    Swap(ref arr[i], ref arr[lt]);
                    i++;
                    lt++;
                }
                //大于切分元素放在gt右边，指针gt左移
                else if (arr[i] > pivot)
                {
                    Swap(ref arr[i], ref arr[gt]);
                    gt--;
                }
                //等于切分元素，指针i++
                else
                {
                    i++;
                }
            }
            //lt-gt之间的元素已经排定，只需对lt左边和gt右边的元素进行递归排序
            //对比新的轴小的部分递归排序
            QuickSort_Optimization(arr, left, lt - 1);
            //对比新的轴大的部分递归排序
            QuickSort_Optimization(arr, gt + 1, right);
        }
    }
}

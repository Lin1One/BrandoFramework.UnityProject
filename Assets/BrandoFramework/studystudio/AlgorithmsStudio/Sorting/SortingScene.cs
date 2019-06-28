#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study.Algorithms
{
    /// <summary>
    /// �����㷨ʾ������
    /// </summary>
    public class SortingScene : MonoBehaviour
    {
        public int[] ArrayToSort;
        public List<GameObject> BoxToShowArray = new List<GameObject>();
        public List<Vector3> BoxTransVector3List = new List<Vector3>();


        public ISorting<Vector3> SortingWay;


        public void InitSceneSortItem()
        {
            var BoxRootTrans = GameObject.Find("BoxRoot").transform;
            for(int i = 0;i<ArrayToSort.Length;i++)
            {
                var box =  GameObject.CreatePrimitive(PrimitiveType.Cube);
                box.transform.SetParent(BoxRootTrans);
                box.transform.localPosition = new Vector3(i, 0, 0);
                box.transform.localScale = new Vector3(1, ArrayToSort[i], 1);
                BoxToShowArray.Add(box);
                BoxTransVector3List.Add(box.transform.localScale);
            }

        }

        public void ApplyIntArrayToBoxTrans()
        {

        }

        public void SortInSync()
        {
            //SortingWay.Sort(ArrayToSort);
        }

        public void SortInAsync()
        {
            for (var i = 0; i < ArrayToSort.Length; i++)
            {
                BoxToShowArray[i].transform.localScale = new Vector3(1, ArrayToSort[i], 1);
            }
        }
        private void Start()
        {
            SortingWay = new QuickSort<Vector3>();
            SortingWay.Compare = Vector3Compare;
            InitSceneSortItem();
        }

        private bool Vector3Compare(Vector3 left, Vector3 right)
        {
            return left.y <= right.y;
        }

        private void Update()
        {
            SortingWay.SortInUpdate(BoxTransVector3List);
        }


    }

    public interface ISorting
    {
        void Sort(int[] collections);

    }

    public interface ISorting<T>
    {
        void SortInUpdate(List<T> ItemArray);
        Func<T, T, bool> Compare { get; set; }
    }

    public class QuickSort<T> : ISorting<T>
    {
        private QuickSortingState<T> sortingState;
        private List<T> ToSort;
        private SortingStep step;
        public int SwapOperateTime;


        public void Sort(int[] collections)
        {
            Sort(collections, 0, collections.Length - 1);
        }

        private void Sort(int[] arr, int left, int right)
        {
            if (left < right)
            {
                int L = left;
                int R = right;
                int pivot = arr[L];
                while (L < R)
                {
                    //�Ӻ���ǰ�ҳ���pivotС����
                    while (L < R && arr[R] > pivot)
                    {
                        R--;
                    }
                    //�ҵ���pivotС��������ֵ��arr[L]
                    if (L < R)
                    {
                        arr[L] = arr[R];
                        L++;
                    }
                    //��ǰ����ҳ���pivot�����
                    while (L < R && arr[L] < pivot)
                    {
                        L++;
                    }
                    //�ҵ���pivot���������ֵ��arr[R], Ҳ����֮ǰ��pivotС������λ��
                    if (L < R)
                    {
                        arr[R] = arr[L];
                        R--;
                    }
                }
                //���µ�Lλ�������µ���
                arr[L] = pivot;
            }
        }

        public Func<T, T, bool> Compare { get; set; }

        public enum SortingStep
        {
            Init,
            FindSmallerThanPivot,
            FindBiggerThanPivot,
            SortNextChildArrays
        }
        public void SortInUpdate(List<T> ItemArray )
        {
            if(ToSort == null)
            {
                ToSort = ItemArray;
                sortingState = new QuickSortingState<T>();
                sortingState.ptrStack = new Stack<QuickSortingState<T>.QuickSortPtr>();
                sortingState.pivot = ToSort[0];
                sortingState.ptrStack.Push(new QuickSortingState<T>.QuickSortPtr(0, ToSort.Count - 1));
                step = SortingStep.Init;
            }
            int count = 0;
            while (sortingState.ptrStack.Peek().left < sortingState.ptrStack.Peek().right)
            {
                if(step != SortingStep.FindSmallerThanPivot)
                {
                    step = SortingStep.FindSmallerThanPivot;
                    //�Ӻ���ǰ�ҳ���pivotС����
                    while (sortingState.ptrStack.Peek().left < sortingState.ptrStack.Peek().right &&
                    Compare(ItemArray[sortingState.ptrStack.Peek().right], sortingState.pivot))
                    {
                        sortingState.ptrStack.Peek().right--;
                    }
                    //�ҵ���pivotС��������ֵ��arr[L]
                    if (sortingState.ptrStack.Peek().left < sortingState.ptrStack.Peek().right)
                    {
                        ItemArray[sortingState.ptrStack.Peek().left] = ItemArray[sortingState.ptrStack.Peek().right];
                        sortingState.ptrStack.Peek().left++;
                        return;
                    }
                }
                else if(step != SortingStep.FindBiggerThanPivot)
                {
                    step = SortingStep.FindBiggerThanPivot;
                    //��ǰ����ҳ���pivot�����
                    while (sortingState.ptrStack.Peek().left < sortingState.ptrStack.Peek().right &&
                        Compare(ItemArray[sortingState.ptrStack.Peek().left],sortingState.pivot))
                    {
                        sortingState.ptrStack.Peek().left++;
                    }
                    //�ҵ���pivot���������ֵ��arr[R], Ҳ����֮ǰ��pivotС������λ��
                    if (sortingState.ptrStack.Peek().left < sortingState.ptrStack.Peek().right)
                    {
                        ItemArray[sortingState.ptrStack.Peek().right] = ItemArray[sortingState.ptrStack.Peek().left];
                        sortingState.ptrStack.Peek().right--;
                    }
                }
                count++;
                if(count > 10)
                {
                    return;
                }
                //���µ�Lλ�������µ���
                //step = SortingStep.SortNextChildArrays;
                //ItemArray[sortingState.ptrStack.Peek().left] = sortingState.pivot;

                //sortingState.pivot = ItemArray[sortingState.ptrStack.Peek().left + 1];

                ////�µ���С�Ĳ�������
                //sortingState.ptrStack.Push(new QuickSortingState<T>.QuickSortPtr(
                //    sortingState.ptrStack.Peek().OriginalLeft,
                //    sortingState.ptrStack.Peek().left - 1));
                ////�µ����Ĳ�������
                //sortingState.ptrStack.Push(new QuickSortingState<T>.QuickSortPtr(
                //    sortingState.ptrStack.Peek().left + 1,
                //    sortingState.ptrStack.Peek().OriginlRight));

                //sortingState.ptrStack.Pop();
            }
        }


        public interface ISortingState
        {

        }
        public abstract class SortingState : ISortingState
        {

        }

        public class QuickSortingState<T> : SortingState
        {
            public Stack<QuickSortPtr> ptrStack;
            public T pivot;

            public class QuickSortPtr
            {
                public int OriginalLeft;
                public int OriginlRight;

                public int left;
                public int right;

                public QuickSortPtr(int l,int r)
                {
                    OriginalLeft = left = l;
                    OriginlRight = right = r;
                }
            }
        }
    }
}


#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion
using System.Collections.Generic;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //设计一个支持 push，pop，top 操作，并能在常数时间内检索到最小元素的栈。
        // push(x) -- 将元素 x 推入栈中。
        // pop() -- 删除栈顶的元素。
        // top() -- 获取栈顶元素。
        // getMin() -- 检索栈中的最小元素。
        // 示例:

        // MinStack minStack = new MinStack();
        // minStack.push(-2);
        // minStack.push(0);
        // minStack.push(-3);
        // minStack.getMin();   --> 返回 -3.
        // minStack.pop();
        // minStack.top();      --> 返回 0.
        // minStack.getMin();   --> 返回 -2.

        public class MinStack
        {
            /** initialize your data structure here. */
            public MinStack()
            {
            }
            public void Push(int x)
            {
            }
            public void Pop()
            {
            }
            public int Top()
            {
                return 0;
            }
            public int GetMin()
            {
                return 0;
            }
        }

        public class MinStack1
        {
            int minNum = 0;
            Stack<int> stack;
            /** initialize your data structure here. */
            public MinStack1()
            {
                stack = new Stack<int>();
            }
            public void Push(int x)
            {
                if(stack.Count == 0)
                {
                    minNum = x;
                    stack.Push(0);
                }
                else
                {
                    stack.Push(x - minNum);
                    if(x < minNum)
                    {
                        minNum = x;
                    }
                }
            }
            public void Pop()
            {
                var topNum = stack.Pop();
                if(topNum < 0)
                {
                    minNum = topNum - minNum;
                }

            }
            public int Top()
            {
                var topNum = stack.Peek();
                if (topNum < 0)
                {
                    return minNum;
                }
                else
                {
                    return topNum + minNum;
                }
            }
            public int GetMin()
            {
                return minNum;
            }
        }

        /**
          * Your MinStack object will be instantiated and called as such:
          * MinStack obj = new MinStack();
          * obj.Push(x);
          * obj.Pop();
          * int param_3 = obj.Top();
          * int param_4 = obj.GetMin();
          */
        public class MinStackTest 
        {
            private Stack<int> stack = new Stack<int>();
            private int min = 0;
            public MinStackTest() 
            {
            }

            public void Push(int x) 
            {
                if(stack.Count == 0)
                {
                    stack.Push(0);
                    min = x;
                }
                else
                {
                    stack.Push(x-min);
                    if(x-min<0)
                    {
                        min = x;
                    }
                }
                
            }
            public void Pop() 
            {    
                if(stack.Count == 0)
                return;
                if(stack.Peek() < 0)
                {
                    min = min - stack.Peek();
                }
                stack.Pop();
            }
    
            public int Top()
            {
                if(stack.Peek() < 0)
                {
                    return min ;
                }
                return stack.Peek()+ min;
            }
    
            public int GetMin() 
            {
                return min;
            }
        }

        public class MinStack3
        {   //辅助栈
            private Stack<int> mainStack;
            private Stack<int> minStack;
            public MinStack3() 
            {


                mainStack= new Stack<int>();
                minStack = new Stack<int>();
            }

            public void Push(int x) 
            {
                mainStack.Push(x);
                if(minStack.Count == 0||x <= minStack.Peek())
                {
                    minStack.Push(x);
                }
            }
            public void Pop() 
            {
                if( minStack.Peek() == mainStack.Peek())
                {
                    minStack.Pop();
                }
                mainStack.Pop();
        
            }
    
            public int Top()
            {
                return mainStack.Peek();
            }
    
            public int GetMin() 
            {
                return minStack.Peek();
            }
        }


        public class MinStack2
        {        
            //记录最小值方法
            //使用long防止int溢出
            private Stack<long> stack;
            private long min;
            public MinStack2() 
            {
                stack = new Stack<long>();
            }
            public void Push(int x) 
            {
                if(stack.Count == 0)
                {
                    //首次push，min=x，所以x-min=0，所以首个元素一定是0
                    min = (long)x;
                    stack.Push(0);
                }
                else
                {
                    stack.Push((long)x-min);
                    if(x<min)
                    {
                        //如果新的值小于min，需要更新min值
                        min = (long)x;
                    }
                }        
            }
            public void Pop() 
            {
                if(stack.Count == 0)
                return;
                long v = stack.Pop();
                if(v<0)
                {
                    //min(上一次的x)-lastMin=v(这一次的v)
                    //推算上一个min值：lastMin = min-v;
                    min = min-v;
                }    
            }

            public int Top() 
            {
                long top = stack.Peek();
                if(top<0)
                    return Convert.ToInt32(min);
                return Convert.ToInt32(top+min);
            }

            public int GetMin() 
            {
                return  Convert.ToInt32(min);
            }
        }


    }
}


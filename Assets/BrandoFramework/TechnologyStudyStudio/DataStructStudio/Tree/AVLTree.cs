namespace DataStructStudio
{
    public class AVLNode
    {
        public int value;
        public int balanceValue;
        public bool isDelete;
        public AVLNode parent;
        public AVLNode leftNode;
        public AVLNode rightNode;

        public AVLNode(int val)
        {
            value = val;
        }
    }

    /// <summary>
    /// 平衡二叉搜索树
    /// </summary>
    public class AVLTree
    {
        private AVLNode Head;

        private int count;

        public int Count { get { return count; } private set { count = value; } }


        /// <summary>
        /// AVL树插入方法
        /// </summary>
        /// <param name="data"></param>
        public void Insert(int data)
        {
            #region 找到插入点(这里和正常二叉搜索树查找方式相同不多做讲解)

            AVLNode tempNode = new AVLNode(data);
            if (Head == null)
            {
                Head = tempNode;
                return;
            }
            AVLNode parent = Head;
            AVLNode currentNode = Head;
            while (currentNode != null)
            {
                parent = currentNode;
                if (currentNode.value > data)
                {
                    currentNode = currentNode.leftNode;
                }
                else
                {
                    currentNode = currentNode.rightNode;
                }
            }
            currentNode = tempNode;
            currentNode.parent = parent;
            if (parent.value > data)
            {
                parent.leftNode = currentNode;
            }
            else
            {
                parent.rightNode = currentNode;
            }

            #endregion

            //插入每一个节点的时候都要计算平衡因子
            //当平衡因子等于2表示当前右子树比左子树高2(或者-2时右子树比左子树低2)
            while (parent != null)
            {
                //更新平衡因子
                if (parent.leftNode == currentNode)
                {
                    //当左边插入值的时候父节点由于左子树高度上升了1，平衡因子减1
                    parent.balanceValue--;
                }
                else
                {
                    //当右边插入值的时候父节点由于右子树高度上升了1,平衡因子加1
                    parent.balanceValue++;
                }

                //当父节点的平衡因子等于0的时候表示当前是平衡的不需要平衡树
                if (parent.balanceValue == 0)
                {
                    break;
                }

                //当父节点的平衡因子为1或者-1时同时父节点的平衡因子也会变更
                else if (parent.balanceValue == 1 || parent.balanceValue == -1)
                {
                    // 回溯上升 更新祖父节点的平衡因子并检验合法性
                    currentNode = parent;
                    parent = currentNode.parent;
                }

                else //平衡因子不合法需要重新平衡二叉树
                {
                    if (parent.balanceValue == 2)
                    {
                        if (currentNode.balanceValue == 1)
                        {
                            // 左旋转
                            RotateL(parent);
                        }
                        else
                        {
                            // 右左旋转
                            RotateRL(parent);
                        }
                    }
                    else if (parent.balanceValue == -2)
                    {
                        if (currentNode.balanceValue == -1)
                        {
                            //右旋转
                            RotateR(parent);
                        }
                        else
                        {
                            //左右旋转
                            RotateLR(parent);
                        }
                    }
                    break;
                }
            }
            Count++;
        }

        public void RotateL(AVLNode parent)
        {
            AVLNode childR = parent.rightNode;
            AVLNode childRL = childR.leftNode;
            AVLNode grandparent = parent.parent;

            //子树的左子树变成根节点的右子树
            parent.rightNode = childRL;
            if (childRL != null)
            {
                childRL.parent = parent;
            }

            //根节点变成子树的左子树
            childR.leftNode = parent;
            parent.parent = childR;

            //子树的根节点修改
            if (grandparent == null)
            {
                childR.parent = null;
                Head = childR;
            }
            else
            {
                if (grandparent.leftNode == parent)
                {
                    grandparent.leftNode = childR;
                }
                else
                {
                    grandparent.rightNode = childR;
                }
                childR.parent = grandparent;
            }
            parent.balanceValue = 0;
            childR.balanceValue = 0;
            parent = childR;
        }

        public void RotateR(AVLNode parent)
        {
            AVLNode childL = parent.leftNode;
            AVLNode childLR = childL.rightNode;
            AVLNode grandparent = parent.parent;

            childL.rightNode = parent;
            parent.parent = childL;

            parent.leftNode = childLR;
            if (childLR != null)
            {
                childLR.parent = parent;
            }

            if (grandparent == null)
            {
                childL.parent = null;
                Head = childL;
            }
            else
            {
                if (grandparent.leftNode == parent)
                {
                    grandparent.leftNode = childL;
                }
                else
                {
                    grandparent.rightNode = childL;
                }
            }
            parent.balanceValue = 0;
            childL.balanceValue = 0;
            parent = childL;
        }

        public void RotateRL(AVLNode parent)
        {
            AVLNode pNode = parent;
            AVLNode childR = parent.rightNode;
            AVLNode childRL = childR.leftNode;
            int childRLbv = childRL.balanceValue;

            RotateR(parent.rightNode);
            RotateL(parent);
            if (childRLbv == 1)
            {
                pNode.balanceValue = 0;
                childR.balanceValue = -1;
            }
            else if (childRLbv == -1)
            {
                pNode.balanceValue = 1;
                childR.balanceValue = 0;
            }
            else
            {
                pNode.balanceValue = 0;
                childR.balanceValue = 0;
            }
        }

        public void RotateLR(AVLNode parent)
        {
            AVLNode pNode = parent;
            AVLNode childL = parent.leftNode;
            AVLNode childLR = childL.rightNode;
            int childLRbv = childLR.balanceValue;
            RotateL(parent.leftNode);
            RotateR(parent);

            if (childLRbv == 1)
            {
                pNode.balanceValue = 0;
                childL.balanceValue = -1;
            }
            else if (childLRbv == -1)
            {
                pNode.balanceValue = 1;
                childL.balanceValue = 0;
            }
            else
            {
                pNode.balanceValue = 0;
                childL.balanceValue = 0;
            }
        }
    }
}

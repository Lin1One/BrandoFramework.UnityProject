

using System;
using System.Collections.Generic;

public class RedBlackTreeNode<T>
{
    //数据
    public T Value { get; set; }

    //左子节点
    public RedBlackTreeNode<T> LeftChild { get; set; }

    //右子节点
    public RedBlackTreeNode<T> RightChild { get; set; }

    //父节点
    public RedBlackTreeNode<T> Parent { get; set; }

    //该节点颜色
    public bool Color { get; set; }

    public RedBlackTreeNode(T value, bool color)
    {
        Value = value;
        LeftChild = null;
        RightChild = null;
        Color = color;
    }
}

/// 红黑树定义：
/// 性质1.节点是红色或黑色
/// 性质2.根是黑色
/// 性质3.所有叶子都是黑色（叶子是NIL节点）
/// 性质4.如果一个节点是红的，则它的两个子节点都是黑的(从每个叶子到根的所有路径上不能有两个连续的红色节点)
/// 性质5.从任一节点到其叶子的所有路径都包含相同数目的黑色节点。
public class RedBlackTree<T>
{
    //根节点
    private RedBlackTreeNode<T> root;
    //比较器
    private Comparer<T> mComparer;
    private const bool RED = true;
    private const bool BLACK = false;

    public RedBlackTree()
    {
        root = null;
        mComparer = Comparer<T>.Default;
    }

    public bool Contains(T value)
    {
        RedBlackTreeNode<T> node;
        return Contain(value, out node);
    }

    public bool Contain(T value, out RedBlackTreeNode<T> newNode)
    {
        if (value == null)
        {
            throw new ArgumentNullException();
        }
        newNode = null;
        RedBlackTreeNode<T> node = root;
        while (node != null)
        {
            int comparer = mComparer.Compare(value, node.Value);
            if (comparer > 0)
            {
                node = node.RightChild;
            }
            else if (comparer < 0)
            {
                node = node.LeftChild;
            }
            else
            {
                newNode = node;
                return true;
            }
        }
        return false;
    }

    #region 添加节点

    public void Add(T value)
    {
        if (root == null)
        {
            // 根节点是黑色的
            root = new RedBlackTreeNode<T>(value, BLACK);
        }
        else
        {
            // 新插入节点是红色的
            Insert(new RedBlackTreeNode<T>(value, RED), value);
        }
    }

    private void Insert(RedBlackTreeNode<T> newNode, T value)
    {
        //遍历找到插入位置
        RedBlackTreeNode<T> node = root;
        //插入节点的父节点
        RedBlackTreeNode<T> parent = null;
        while (node != null)
        {
            parent = node;
            int comparer = mComparer.Compare(value, node.Value);
            if (comparer > 0)
            {
                node = node.RightChild;
            }
            else if (comparer < 0)
            {
                node = node.LeftChild;
            }
            else
            {
                node.Value = value;
                return;
            }
        }
        //找到插入位置，设置新插入节点的父节点为current
        newNode.Parent = parent;
        //比较插入节点的值跟插入位置的值的大小, 插入新节点
        int comparer1 = mComparer.Compare(value, parent.Value);
        if (comparer1 > 0)
        {
            parent.RightChild = newNode;
        }
        else if (comparer1 < 0)
        {
            parent.LeftChild = newNode;
        }
        //将它重新修整为一颗红黑树
        InsertFixUp(newNode);
    }

    private void InsertFixUp(RedBlackTreeNode<T> newNode)
    {
        RedBlackTreeNode<T> parent = newNode.Parent; //插入节点的父节点
        RedBlackTreeNode<T> gParent = null; //插入节点的祖父节点
        //父节点的颜色是红色,并且不为空
        while (IsRed(parent) && parent != null)
        {
            //获取祖父节点，这里不用判空，
            //因为如果祖父节点为空，parent就是根节点，根节点是黑色，不会再次进入循环
            gParent = parent.Parent;
            //若父节点是祖父节点的左子节点 
            if (parent == gParent.LeftChild)
            {
                RedBlackTreeNode<T> uncle = gParent.RightChild; //获得叔叔节点  

                //case1: 叔叔节点也是红色  
                if (uncle != null && IsRed(uncle))
                {
                    //把父节点和叔叔节点涂黑,祖父节点涂红
                    parent.Color = BLACK;
                    uncle.Color = BLACK;
                    gParent.Color = RED;
                    //把祖父节点作为插入节点，向上继续遍历
                    newNode = gParent;
                    parent = newNode.Parent;
                    continue; //继续while，重新判断  
                }

                //case2: 叔叔节点是黑色，且当前节点是右子节点  
                if (newNode == parent.RightChild)
                {
                    //从父节点处左旋
                    //当这种情况时，只能左旋，因为父亲节点和祖父节点变色，无论左旋还是右旋，都会违背红黑树的基本性质
                    RotateLeft(parent);
                    //当左旋后，红黑树变成case3的情况，区别就是插入节点是父节点
                    //所以，将父节点和插入节点调换一下，为下面右旋做准备
                    RedBlackTreeNode<T> tmp = parent;
                    parent = newNode;
                    newNode = tmp;
                }

                //case3: 叔叔节点是黑色，且当前节点是左子节点
                // 父亲和祖父节点变色，从祖父节点处右旋
                parent.Color = BLACK;
                gParent.Color = RED;
                RotateRight(gParent);
            }
            else
            {
                //若父节点是祖父节点的右子节点,与上面的完全相反
                RedBlackTreeNode<T> uncle = gParent.LeftChild;

                //case1: 叔叔节点也是红色  
                if (uncle != null & IsRed(uncle))
                {
                    //把父节点和叔叔节点涂黑,祖父节点涂红
                    parent.Color = BLACK;
                    uncle.Color = BLACK;
                    gParent.Color = RED;
                    //把祖父节点作为插入节点，向上继续遍历
                    newNode = gParent;
                    parent = newNode.Parent;
                    continue;//继续while，重新判断
                }

                //case2: 叔叔节点是黑色的，且当前节点是左子节点  
                if (newNode == parent.LeftChild)
                {
                    //从父节点处右旋
                    //当这种情况时，只能右旋，因为父亲节点和祖父节点变色，无论左旋还是右旋，都会违背红黑树的基本性质
                    RotateRight(parent);
                    RedBlackTreeNode<T> tmp = parent;
                    parent = newNode;
                    newNode = tmp;
                }

                //case3: 叔叔节点是黑色的，且当前节点是右子节点  
                // 父亲和祖父节点变色，从祖父节点处右旋
                parent.Color = BLACK;
                gParent.Color = RED;
                RotateLeft(gParent);
            }
        }
        //将根节点设置为黑色
        root.Color = BLACK;
    }


    #endregion

    private static bool IsRed(RedBlackTreeNode<T> node)
    {
        if (node == null)
        {
            return false;
        }
        if (node.Color == RED)
        {
            return true;
        }
        return false;
    }

    private static bool IsBlack(RedBlackTreeNode<T> node)
    {
        if (node == null)
        {
            return false;
        }
        if (node.Color == BLACK)
        {
            return true;
        }
        return false;
    }

    // 左旋转,逆时针旋转
    /*************对红黑树节点x进行左旋操作 ******************/
    /* 
     * 左旋示意图：对节点x进行左旋 
     *     p                       p 
     *    /                       / 
     *   x                       y 
     *  / \                     / \ 
     * lx  y                   x  ry 
     *    / \                 / \ 
     *   ly ry               lx ly 
     * 左旋做了三件事： 
     * 1. 将y的左子节点赋给x的右子节点,并将x赋给y左子节点的父节点(y左子节点非空时) 
     * 2. 将x的父节点p(非空时)赋给y的父节点，同时更新p的子节点为y(左或右) 
     * 3. 将y的左子节点设为x，将x的父节点设为y 
     */
    private void RotateLeft(RedBlackTreeNode<T> x)
    {
        //1. 将y的左子节点赋给x的右子节点，并将x赋给y左子节点的父节点(y左子节点非空时)  
        RedBlackTreeNode<T> y = x.RightChild;
        x.RightChild = y.LeftChild;

        if (y.LeftChild != null)
        {
            y.LeftChild.Parent = x;
        }

        //2. 将x的父节点p(非空时)赋给y的父节点，同时更新p的子节点为y(左或右) 
        if (x.Parent != null)
        {
            y.Parent = x.Parent;
        }

        if (x.Parent == null)
        {
            root = y; //如果x的父节点为空，则将y设为父节点
            y.Parent = null;//设置y的父节点为空  
        }
        else
        {
            //如果x是左子节点  
            if (x == x.Parent.LeftChild)
            {
                //则也将y设为左子节点  
                x.Parent.LeftChild = y;
            }
            else
            {
                //否则将y设为右子节点 
                x.Parent.RightChild = y;
            }
        }

        //3. 将y的左子节点设为x，将x的父节点设为y  
        y.LeftChild = x;
        x.Parent = y;
    }


    // 右旋转,顺时针旋转
    /*************对红黑树节点y进行右旋操作 ******************/
    /* 
     * 左旋示意图：对节点y进行右旋 
     *        p                   p 
     *       /                   / 
     *      y                   x 
     *     / \                 / \ 
     *    x  ry               lx  y 
     *   / \                     / \ 
     * lx  rx                   rx ry 
     * 右旋做了三件事： 
     * 1. 将x的右子节点赋给y的左子节点,并将y赋给x右子节点的父节点(x右子节点非空时) 
     * 2. 将y的父节点p(非空时)赋给x的父节点，同时更新p的子节点为x(左或右) 
     * 3. 将x的右子节点设为y，将y的父节点设为x 
     */
    private void RotateRight(RedBlackTreeNode<T> y)
    {
        //1.将x的右子节点赋值给y的左子节点，同时将y赋值给x的右子节点的父节点(如果x的右子节点非空)
        RedBlackTreeNode<T> x = y.LeftChild;
        y.LeftChild = x.RightChild;

        if (x.RightChild != null)
        {
            x.RightChild.Parent = y;
        }

        //2.如果y的父节点非空时，将y的父节点赋值给x的父节点，同时更新p的子节点为x
        if (y.Parent != null)
        {
            x.Parent = y.Parent;
        }
        //如果y的父节点为空，则将x设为父节点
        if (y.Parent == null)
        {
            root = x;
            x.Parent = null;//设置x的父节点为空
        }
        else
        {
            //如果y是右子节点
            if (y == y.Parent.RightChild)
            {
                //则也将y设为右子节点  
                y.Parent.RightChild = x;
            }
            else
            {
                //否则将x设为左子节点
                y.Parent.LeftChild = x;
            }
        }

        //3.将x的右子节点设为y，y的父节点设置为x
        x.RightChild = y;
        y.Parent = x;
    }

    public int Count
    {
        get
        {
            return CountLeafNode(root);
        }
    }

    private int CountLeafNode(RedBlackTreeNode<T> root)
    {
        if (root == null)
        {
            return 0;
        }
        else
        {
            return CountLeafNode(root.LeftChild) + CountLeafNode(root.RightChild) + 1;
        }
    }

    public int Depth
    {
        get
        {
            return GetHeight(root);
        }
    }

    private int GetHeight(RedBlackTreeNode<T> root)
    {
        if (root == null)
        {
            return 0;
        }
        int leftHight = GetHeight(root.LeftChild);
        int rightHight = GetHeight(root.RightChild);
        return leftHight > rightHight ? leftHight + 1 : rightHight + 1;
    }


    public T Max
    {
        get
        {
            RedBlackTreeNode<T> node = root;
            while (node.RightChild != null)
            {
                node = node.RightChild;
            }
            return node.Value;
        }
    }

    public T Min
    {
        get
        {
            if (root != null)
            {
                RedBlackTreeNode<T> node = GetMinNode(root);
                return node.Value;
            }
            else
            {
                return default(T);
            }
        }
    }

    private RedBlackTreeNode<T> GetMinNode(RedBlackTreeNode<T> node)
    {
        while (node.LeftChild != null)
        {
            node = node.LeftChild;
        }
        return node;
    }


    public void DelMin()
    {
        root = DelMin(root);
    }

    private RedBlackTreeNode<T> DelMin(RedBlackTreeNode<T> node)
    {
        if (node.LeftChild == null)
        {
            return node.RightChild;
        }
        node.LeftChild = DelMin(node.LeftChild);
        return node;
    }

    public void Remove(T value)
    {
        root = Delete(root, value);
    }

    private RedBlackTreeNode<T> Delete(RedBlackTreeNode<T> node, T value)
    {
        if (node == null)
        {
            Console.WriteLine("没有找到要删除的节点: " + value);
            return null;
        }
        int comparer = mComparer.Compare(value, node.Value);
        if (comparer > 0)
        {
            node.RightChild = Delete(node.RightChild, value);
        }
        else if (comparer < 0)
        {
            node.LeftChild = Delete(node.LeftChild, value);
        }
        else
        {
            // a.如果删除节点没有子节点，直接返回null
            // b.如果只有一个子节点，返回其子节点代替删除节点即可
            if (node.LeftChild == null)
            {
                if (node.RightChild != null)
                {
                    node.RightChild.Parent = node.Parent;
                }
                return node.RightChild;
            }
            else if (node.RightChild == null)
            {
                if (node.LeftChild != null)
                {
                    node.LeftChild.Parent = node.Parent;
                }
                return node.LeftChild;
            }
            else
            {
                // c.被删除的节点“左右子节点都不为空”的情况  
                RedBlackTreeNode<T> child;
                RedBlackTreeNode<T> parent;
                bool color;
                // 1. 找到“删除节点的右子树中的最小节点”，用它来取代被删除节点的位置
                // 注意：这里也可以选择“删除节点的左子树中的最大节点”作为被删除节点的替换节点
                RedBlackTreeNode<T> replace = node;
                replace = GetMinNode(replace.RightChild);

                // 2. 更新删除父节点及其子节点
                // 要删除的节点不是根节点  
                if (node.Parent != null)
                {
                    // 要删除的节点是：删除节点的父节点的左子节点 
                    if (node == node.Parent.LeftChild)
                    {
                        // 把“删除节点的右子树中的最小节点”赋值给“删除节点的父节点的左子节点” 
                        node.Parent.LeftChild = replace;
                    }
                    else
                    {
                        // 把“删除节点的右子树中的最小节点”赋值给“删除节点的父节点的右子节点”
                        node.Parent.RightChild = replace;
                    }
                }
                else
                {
                    // 要删除的节点是根节点
                    // 如果只有一个根节点，把mRoot赋值为null，这时replace为null
                    // 如果不止一个节点，返回根节点的右子树中的最小节点
                    root = replace;
                }

                // 记录被删除节点的右子树中的最小节点的右子节点，父亲节点及颜色，没有左子节点
                child = replace.RightChild;
                parent = replace.Parent;
                color = replace.Color;

                // 3. 删除“被删除节点的右子树中的最小节点”，同时更新替换节点的左右子节点，父亲节点及颜色
                // 替换节点 也就是 最小节点
                if (parent == node)
                {
                    // 被删除节点的右子树中的最小节点是被删除节点的子节点
                    parent = replace;
                }
                else
                {
                    //如果最小节点的右子节点不为空，更新其父节点
                    if (child != null)
                    {
                        child.Parent = parent;
                    }
                    //更新最小节点的父节点的左子节点，指向最小节点的右子节点
                    parent.LeftChild = child;
                    //更新替换节点的右子节点
                    replace.RightChild = node.RightChild;
                    //更新删除节点的右子节点的父节点
                    node.RightChild.Parent = replace;
                }
                //更新替换节点的左右子节点，父亲节点及颜色
                replace.Parent = node.Parent;
                //保持原来位置的颜色
                replace.Color = node.Color;
                replace.LeftChild = node.LeftChild;
                //更新删除节点的左子节点的父节点
                node.LeftChild.Parent = replace;

                //红黑树平衡修复
                //如果删除的最小节点颜色是黑色，需要重新平衡红黑树
                //如果删除的最小节点颜色是红色，只需要替换删除节点后，涂黑即可
                //上面的保持原来位置的颜色已经处理了这种情况，这里只需要判断最小节点是黑色的情况
                if (color == BLACK)
                {
                    //将最小节点的child和parent传进去
                    RemoveFixUp(child, parent);
                }
                return replace;
            }
        }
        return node;
    }

    private void RemoveFixUp(RedBlackTreeNode<T> node, RedBlackTreeNode<T> parent)
    {
        RedBlackTreeNode<T> brother;
        // 被删除节点的右子树中的最小节点 不是 被删除节点的子节点的情况
        while ((node == null || IsBlack(node)) && (node != root))
        {
            if (parent.LeftChild == node)
            {
                //node是左子节点，下面else与这里的刚好相反  
                brother = parent.RightChild; //node的兄弟节点  
                if (IsRed(brother))
                {
                    //case1: node的兄弟节点brother是红色的 
                    brother.Color = BLACK;
                    parent.Color = RED;
                    RotateLeft(parent);
                    brother = parent.RightChild;
                }

                //case2: node的兄弟节点brother是黑色的，且brother的两个子节点也都是黑色的
                //继续向上遍历  
                if ((brother.LeftChild == null || IsBlack(brother.LeftChild)) &&
                    (brother.RightChild == null || IsBlack(brother.RightChild)))
                {
                    //把兄弟节点设置为黑色，平衡红黑树
                    brother.Color = RED;
                    node = parent;
                    parent = node.Parent;
                }
                else
                {
                    //case3: node的兄弟节点brother是黑色的，且brother的左子节点是红色，右子节点是黑色  
                    if (brother.RightChild == null || IsBlack(brother.RightChild))
                    {
                        brother.LeftChild.Color = BLACK;
                        brother.Color = RED;
                        RotateRight(brother);
                        brother = parent.RightChild;
                    }

                    //case4: node的兄弟节点brother是黑色的，且brother的右子节点是红色，左子节点任意颜色  
                    brother.Color = parent.Color;
                    parent.Color = BLACK;
                    brother.RightChild.Color = BLACK;
                    RotateLeft(parent);
                    node = root;
                    break;
                }
            }
            else
            {
                //与上面的对称  
                brother = parent.LeftChild;
                if (IsRed(brother))
                {
                    // Case 1: node的兄弟brother是红色的   
                    brother.Color = BLACK;
                    parent.Color = RED;
                    RotateRight(parent);
                    brother = parent.LeftChild;
                }
                // Case 2: node的兄弟brother是黑色，且brother的俩个子节点都是黑色的 
                if ((brother.LeftChild == null || IsBlack(brother.LeftChild)) &&
                    (brother.RightChild == null || IsBlack(brother.RightChild)))
                {
                    //把兄弟节点设置为黑色，平衡红黑树
                    brother.Color = RED;
                    node = parent;
                    parent = node.Parent;
                }
                else
                {
                    // Case 3: node的兄弟brother是黑色的，并且brother的左子节点是红色，右子节点为黑色。
                    if (brother.LeftChild == null || IsBlack(brother.LeftChild))
                    {
                        brother.RightChild.Color = BLACK;
                        brother.Color = RED;
                        RotateLeft(brother);
                        brother = parent.LeftChild;
                    }

                    // Case 4: node的兄弟brother是黑色的；并且brother的左子节点是红色的，右子节点任意颜色  
                    brother.Color = parent.Color;
                    parent.Color = BLACK;
                    brother.LeftChild.Color = BLACK;
                    RotateRight(parent);
                    node = root;
                    break;
                }
            }
        }
        //如果删除的最小节点的右子节点是红色，只需要替换最小节点后，涂黑即可
        if (node != null)
        {
            node.Color = BLACK;
        }
    }
}

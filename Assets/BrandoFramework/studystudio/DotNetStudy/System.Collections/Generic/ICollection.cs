
namespace Study.DotNet.System.Collections.Generic
{

    /// <summary>
    /// ���ͼ��Ͻӿ�
    /// </summary>
    public interface ICollection<T> : IEnumerable<T>
    {       
        int Count { get; }

        bool IsReadOnly { get; }

        void Add(T item);

        bool Remove(T item);

        void Clear();

        bool Contains(T item); 
                
        void CopyTo(T[] array, int arrayIndex);
                
        
    }

}

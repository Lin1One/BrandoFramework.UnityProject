namespace client_module_network
{
    public class ByteArray 
    {
        //缓冲区    
        public byte[] bytes;
        //读写位置    
        public int readIdx = 0;
        public int writeIdx = 0;

        public int remain;
        //数据长度    
        public int length
        {
            get
            {
                return writeIdx-readIdx;
            }
        }
        //构造函数    
        public ByteArray(byte[] defaultBytes)
        {
            bytes = defaultBytes;
            readIdx = 0;
            writeIdx = defaultBytes.Length;
        }
        public ByteArray() { }


        public void MoveBytes()
        {

        }
        public void ReSize(int size)
        {

        }

        public void CheckAndMoveBytes()
        {

        }
           
    }
}



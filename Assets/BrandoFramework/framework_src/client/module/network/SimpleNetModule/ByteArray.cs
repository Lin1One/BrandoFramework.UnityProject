namespace client_module_network
{
    public class ByteArray 
    {
        //������    
        public byte[] bytes;
        //��дλ��    
        public int readIdx = 0;
        public int writeIdx = 0;

        public int remain;
        //���ݳ���    
        public int length
        {
            get
            {
                return writeIdx-readIdx;
            }
        }
        //���캯��    
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



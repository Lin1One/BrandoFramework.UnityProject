namespace client_module_network
{
    public class U3DNetModuleTest
    {
        IU3DNetModule netModule;
        public void Test()
        {
            var netChannel = netModule.RegisterNetChannel("MyTest", NetServiceType.Socket, OnConnected);
            netChannel.AddDownHandler(new MessageDeCodeHandler());
            netChannel.AddUpHandler(new MessageUpCodeHandler());
            netChannel.BeginConnect("127.0.0.1", 8888);
        }

        private void OnConnected(IU3dNetChannel c)
        {
            netModule.SendMessage("MyTest", 10001, "hello");

        }
    }
}



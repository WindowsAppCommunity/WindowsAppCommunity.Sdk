using Ipfs;

namespace WindowsAppCommunity.Sdk.Tests;

[TestClass]
public partial class BasicTests
{
    [TestMethod]
    public void TestPeerIdToCid()
    {
        var keyId = "12D3KooWHTJs5AGsyVqosX15r2UwUXwU3XnDb6RFPka3EMeVhhJr";
            
        var peer = new Peer { Id = keyId };

        Cid peerCid = new Cid { Hash = keyId };
        
        Console.WriteLine("Success");
    }
}
using Unity.Netcode;
using UnityEngine;

public class rpcTest : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {   //only send RPC to the server on the client that owns the network object
        if (!IsServer && IsOwner)
        {
            TestServerRpc(0, NetworkObjectId);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TestClientRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"ClientReceived the RPC #{value} on Network object #{sourceNetworkObjectId}");
        //only send the RPC to the server on the client that owns the network object tgat iwbs tghus network behaviour.
        if (IsOwner)
        {
            TestServerRpc(value + 1, sourceNetworkObjectId);
        }
    }

    [Rpc(SendTo.Server)]
    void TestServerRpc(int value, ulong sourceNetworkObjectId) 
    {
        Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        TestClientRpc(value, sourceNetworkObjectId);
    }

}

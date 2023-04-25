using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf;

namespace PallyCon
{
    class SecretManagerClient
    {
        SecretManagerServiceClient Client { get; }
        string ProjectId { get; }
        public SecretManagerClient(string projectId) 
        { 
            ProjectId = projectId;
            Client = SecretManagerServiceClient.Create();
        }
        public Secret CreateSecret(string secretId)
        {
            ProjectName projectName = new ProjectName(ProjectId);

            Secret secret = new Secret
            {
                Replication = new Replication
                {
                    Automatic = new Replication.Types.Automatic(),
                },
            };

            // Make the request.
            return Client.CreateSecret(projectName, secretId, secret);
        }
        public Secret GetSecret(SecretName secretName)
        {
            return Client.GetSecret(secretName);
        }

        public SecretVersion AddSecretVersion(SecretName secretName, string payload)
        {
            SecretPayload secretPayload = new SecretPayload
            {
                // Set the payload data
                Data = ByteString.CopyFrom(payload, Encoding.UTF8),
            };

            // Make the request.
            return Client.AddSecretVersion(secretName, secretPayload);
        }
        public SecretVersion GetSecretVersion(SecretVersionName secretVersionName)
        {
            // Make the request.
            return Client.GetSecretVersion(secretVersionName);
        }

        public void DisableSecretVersion(SecretVersionName secretVersionName)
        {
            // Make the request.
            Client.DisableSecretVersion(secretVersionName);
        }
        public SecretVersion DestroySecretVersion(SecretVersionName secretVersionName)
        {
            // Make the request.
            return Client.DestroySecretVersion(secretVersionName);
        }

        public void DeleteSecret(string secretId)
        {
            try
            {
                SecretName secretName = new SecretName(ProjectId, secretId);
                // Make the request.
                Client.DeleteSecret(secretName);
            }
            catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                // Ignore error - secret was already deleted
            }
        }
    }
}

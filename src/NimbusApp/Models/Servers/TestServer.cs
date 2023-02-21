using System;
using System.Threading.Tasks;

namespace NimbusApp.Models.Servers
{
    /// <summary>
    /// Test implementation of the server class for testing purposes
    /// Should not be used as an actual server
    /// </summary>
    public class TestServer : Server
    {
        public override string Type => "TestName";

        public override string Host => "TestHost";

        public string TestProperty { get; private set; }

        protected override Task Build(params string[] args)
        {
            TestProperty = "Test";
            return Task.CompletedTask;
        }

        public override Task<string> GetLocalSaveHash(string archiveLocation)
        {
            return Task.FromResult("");
        }

        public override Task<bool> GetOnlineStatus()
        {
            return Task.FromResult(true);
        }

        public override Task<string> GetRemoteSaveHash(string name)
        {
            return Task.FromResult("");
        }

        public override Task GetSaveData(string name, string destination)
        {
            return Task.CompletedTask;
        }

        public override Task<string[]> SaveNames()
        {
            return Task.FromResult(Array.Empty<string>());
        }

        public override Task UploadSaveData(string name, string source)
        {
            return Task.CompletedTask;
        }
    }
}

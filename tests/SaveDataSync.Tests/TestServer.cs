﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataSync.Tests
{
    /// <summary>
    /// Test implementation of the server class for testing purposes
    /// </summary>
    internal class TestServer : Server
    {
        public override string Name => "TestName";

        public override string Host => "TestHost";

        public string? TestProperty { get; private set; }

        public override Task Build()
        {
            TestProperty = "Test";
            return Task.CompletedTask;
        }

        public override Task Deserialize(JObject json)
        {
            var value = json.GetValue("test_property");
            if (value is not null)
            {
                TestProperty = value.ToString();
            }
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

        public override Task<JObject> Serialize()
        {
            var json = new JObject
            {
                { "test_property", TestProperty }
            };

            return Task.FromResult(json);
        }

        public override Task UploadSaveData(string name, string source)
        {
            return Task.CompletedTask;
        }
    }
}
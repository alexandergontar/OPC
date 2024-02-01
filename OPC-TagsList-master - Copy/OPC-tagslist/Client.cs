﻿using System;
using System.Threading;
using System.Threading.Tasks;
using OPC.Common;
using OPC.Data.Interface;
using OPC.Data;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using OPC_tagslist;

namespace OPC_tagslist
{
    public class Client
    {
        public async Task<string> Send(string jsonString, string url)
        {
            StringContent data = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(url, data);
            string result = await response.Content.ReadAsStringAsync();
            client.Dispose();
            return result;

        }
    }
}
using System;
using ProviderBase.Framework.Entities;
using ProviderBase.Framework.Providers;
using ProviderBase.Data.Entities;
using ProviderBase.Data.Providers;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using ProviderBase.Framework.Forum.Entities;
using System.Reflection;
using ProviderBase.Data;
using System.Collections;
using ProviderBase.Framework;

namespace ProviderBaseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string pw = "12345";
            string encrypted = ProviderBase.Data.Entities.Encryption.Hash(pw);

            Console.ReadLine();
        }
    }
}

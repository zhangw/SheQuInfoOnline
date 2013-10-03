using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SheQuInfo.Models;

namespace SheQuInfo.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void DataBaseInit()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<SqInfoContext>());

            SqInfoContext context = new SqInfoContext("SqInfoContext");
            context.Database.Delete();
            context.Database.CreateIfNotExists();
        }

        [TestMethod]
        public void GetHashPass()
        {
            string hashpass = StringHelper.HashPassword("123456");
            Console.WriteLine(hashpass);
        }
    }
}
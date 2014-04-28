using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System.Collections.Generic;
using ProtoBuf;

namespace UnitTestRedis
{
    [TestClass]
    public class UnitTest1
    {
        private IDatabase db = RedisConnection.Instance.GetDatabase();
        [TestMethod]
        public void SetAndGet()
        {            
            string value = "abcdefg";
            db.StringSet("mykey", value);
            var val = db.StringGet("mykey");
            Assert.AreEqual(val, value);
        }

        [TestMethod]
        public void TestInteger()
        {
            int num = 5;
            db.StringSet("NewRedis_TestInteger", num);
            var val = db.StringGet("NewRedis_TestInteger");
            Assert.AreEqual(val, num);
        }

        [TestMethod]
        public void TestDouble()
        {
            double num = 5.34567;
            db.StringSet("NewRedis_TestDouble", num);
            var val = db.StringGet("NewRedis_TestDouble");
            Assert.AreEqual(val, num);
        }

        [TestMethod]
        public void TestBool()
        {
            bool b = true;
            db.StringSet("NewRedis_TestBoolT", b);
            var val = db.StringGet("NewRedis_TestBoolT");
            Assert.AreEqual(val, b);
        }

        [TestMethod]
        public void TestDate()
        {
            DateTime now = DateTime.Now;      
            db.StringSet("NewRedis_TestDate", now.ToString());
            var val = Convert.ToDateTime(db.StringGet("NewRedis_TestDate"));
            Console.WriteLine(now.ToString());
            Console.WriteLine(val);
            Assert.AreEqual(val, now);
        }

        [TestMethod]
        public void TestModel()
        {
            //List<People> ppl = new List<People>();
            var ppl = new People()
            {
                ID = 1,
                FirstName = "John",
                LastName = "Doe"
            };
            
            Serializer.Serialize<People>(sr, ppl);
        }
    }
}

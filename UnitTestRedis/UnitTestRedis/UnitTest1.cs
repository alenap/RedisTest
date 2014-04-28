using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;

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
        public void TestProtobufNet()
        {
            var ppl = new People()
            {
                ID = 1,
                FirstName = "John",
                LastName = "Doe",
                Address = new AddressModel() { 
                    AptNumber = 56, 
                    StreetAdress = "123 Main Street", 
                    City = "Toronto", 
                    State = "Ontario", 
                    Country = "Canada" 
                }
            };
            using (var file = File.Create("person.bin"))
            {
                Serializer.Serialize<People>(file, ppl);
            }

            People newPerson;
            using (var file = File.OpenRead("person.bin"))
            {
                newPerson = Serializer.Deserialize<People>(file);
            }
            Console.Write(newPerson.Address.StreetAdress);
            Assert.AreEqual(newPerson.Address.Country, ppl.Address.Country);
        }

        [TestMethod]
        public void TestProtobufAndRedis()
        {
            var ppl = new People()
            {
                ID = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Address = new AddressModel()
                {
                    AptNumber = 56,
                    StreetAdress = "123 Main Street",
                    City = "Toronto",
                    State = "Ontario",
                    Country = "Canada"
                }
            };
            db.StringSet("TestProtobufAndRedis", DataToBytes(ppl));
            byte[] val = db.StringGet("TestProtobufAndRedis");
            MemoryStream stream = new MemoryStream(val, false);
            var val2 = Serializer.Deserialize<People>(stream);
            Assert.AreEqual(ppl.Address.AptNumber, val2.Address.AptNumber);
        }

        [TestMethod]
        public void TestProtobufAndRedis_List()
        {
            List<People> ppl = new List<People>();
            var person1 = new People()
            {
                ID = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Address = new AddressModel() { AptNumber = 51, StreetAdress = "123 Main Street", City = "Toronto", State = "Ontario", Country = "Canada"}
            };
            var person2 = new People()
            {
                ID = 2,
                FirstName = "John",
                LastName = "Doe",
                Address = new AddressModel() { AptNumber = 52, StreetAdress = "678 Main Street", City = "Toronto1", State = "Ontario1", Country = "Canada1" }
            };
            ppl.Add(person1);
            ppl.Add(person2);
            db.StringSet("TestProtobufAndRedis", ListToBytes(ppl));
            byte[] val = db.StringGet("TestProtobufAndRedis");
            MemoryStream stream = new MemoryStream(val, false);
            var val2 = Serializer.Deserialize<List<People>>(stream);
            Assert.AreEqual(ppl[1].Address.StreetAdress, val2[1].Address.StreetAdress);
        }

        private static byte[] DataToBytes(People data)
        {
            MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream, data);
            byte[] bytes = stream.ToArray();
            stream.Close();
            return bytes;
        }

        private static byte[] ListToBytes(List<People> data)
        {
            MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream, data);
            byte[] bytes = stream.ToArray();
            stream.Close();
            return bytes;
        }
    }
}

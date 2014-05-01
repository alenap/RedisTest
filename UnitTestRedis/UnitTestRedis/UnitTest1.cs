using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using System.Linq;
using ProtoBuf.Meta;

namespace UnitTestRedis
{
    [TestClass]
    public class UnitTest1
    {
        private IDatabase db = RedisConnection.Instance.GetDatabase();
        //private RuntimeTypeModel model;
        //private IServer server = RedisConnection.Instance.GetServer("192.168.11.46:12112");

        public UnitTest1()
        {
            RuntimeTypeModel.Default.Add(typeof(PeopleNoAttr), true);
            RuntimeTypeModel.Default.Add(typeof(AddressNoAttr), true);
        }

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
            db.StringSet("StackExchangeRedis_TestInteger", num);
            var val = db.StringGet("StackExchangeRedis_TestInteger");
            Assert.AreEqual(val, num);
        }

        [TestMethod]
        public void TestDouble()
        {
            double num = 5.34567;
            db.StringSet("StackExchangeRedis_TestDouble", num);
            var val = db.StringGet("StackExchangeRedis_TestDouble");
            Assert.AreEqual(val, num);
        }

        [TestMethod]
        public void TestBool()
        {
            bool b = true;
            db.StringSet("StackExchangeRedis_TestBoolT", b);
            var val = db.StringGet("StackExchangeRedis_TestBoolT");
            Assert.AreEqual(val, b);
        }

        [TestMethod]
        public void TestSerializedDate()
        {
            DateTime now = DateTime.Now;
            SetCache<DateTime>("StackExchangeRedis_TestSerializedDate", now);
            var val = GetCache<DateTime>("StackExchangeRedis_TestSerializedDate");
            Console.WriteLine(now);
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
        public void TestSerializedArray()
        {
            int[] arr = new int[4] { 5, 7, 11, 17 };
            SetCache<int[]>("StackExchangeRedis_TestSerializedArray", arr);
            Console.WriteLine("Array length = " + arr.Length);
            arr = GetCache<int[]>("StackExchangeRedis_TestSerializedArray");
            Console.WriteLine("Deserialized array length = " + arr.Length);
            Assert.IsTrue(arr[2] == 11);
        }

        [TestMethod]
        public void TestProtobufAndRedis()
        {
            var ppl = new PeopleNoAttr()
            {
                ID = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Address = new AddressNoAttr()
                {
                    AptNumber = 56,
                    StreetAdress = "123 Main Street",
                    City = "Toronto",
                    State = "Ontario",
                    Country = "Canada"
                }
            };
            SetCacheNoAttr<PeopleNoAttr>("StackExchangeRedis_TestProtobufAndRedis_NoAttr", ppl);
            var val2 = GetCache<PeopleNoAttr>("StackExchangeRedis_TestProtobufAndRedis");
            //Assert.AreEqual(ppl.Address.AptNumber, val2.Address.AptNumber);
        }

        [TestMethod]
        public void TestProtobufAndRedis_List()
        {
            var cachekey = "StackExchangeRedis_TestProtobufAndRedisList";
            List<People> ppl = GenerateList();
            SetCache<List<People>>(cachekey, ppl);
            var val2 = GetCache<List<People>>(cachekey);
            Assert.AreEqual(ppl[1].Address.StreetAdress, val2[1].Address.StreetAdress);
        }
        
        [TestMethod]
        public void TestProtobufAndRedis_IEnumerable()
        {
            var cachekey = "StackExchangeRedis_TestProtobufAndRedisIEnumerable";
            List<People> ppl = GenerateList();
            IEnumerable<People> Ippl = (IEnumerable<People>)ppl;
            SetCache<IEnumerable<People>>(cachekey, ppl);
            var val2 = GetCache<IEnumerable<People>>(cachekey);
            var el = val2.ElementAt(1);            
            Assert.AreEqual(ppl[1].Address.StreetAdress, el.Address.StreetAdress);
        }

        [TestMethod]
        public void TestDeleteKey()
        {
            DeleteFromCache("StackExchangeRedis_TestProtobufAndRedis");
        }

        // TO DO:
        // =====
        // no attributes
        // twemproxy
        // compare to old redis: C:\workspace\CareerCruising_Core\CC.Data_Tests\RemoteCacheProvider_Test.cs


        //******************************
        [TestMethod]
        public void TestSync()
        {
            var aSync = db.StringGet("StackExchangeRedis_TestDouble");
            var bSync = db.StringGet("StackExchangeRedis_TestInteger");
        }
        [TestMethod]
        public void TestAsync()
        {            
            var aPending = db.StringGetAsync("StackExchangeRedis_TestDouble");
            var bPending = db.StringGetAsync("StackExchangeRedis_TestInteger");
            var a = db.Wait(aPending);
            var b = db.Wait(bPending);
        }
        //******************************

        /*[TestMethod]
        public void TestExpirationDate()
        {
            var cachekey = "StackExchangeRedis_TestExpirationDate";
            db.StringSet(cachekey, "testing expiration date");
            db.KeyExpire(cachekey, TimeSpan.FromMinutes(30));
            var ttl = db.KeyTimeToLive(cachekey);
            Console.Write(ttl);
        }

        [TestMethod]
        public void TestDeleteKeysByPartOfName()
        {
            DeleteKeysByPartOfName("StackExchangeRedis_");
        }*/

        /*[TestMethod]
        public void TestDeleteAllKeys()
        {
            ClearCache();
        }*/

        #region non-test methods

        private static byte[] DataToBytes<T>(T data)
        {
            MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream, data);
            byte[] bytes = stream.ToArray();
            stream.Close();
            return bytes;
        }

        private static List<People> GenerateList()
        {
            List<People> ppl = new List<People>();
            var person1 = new People()
            {
                ID = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Address = new AddressModel() { AptNumber = 51, StreetAdress = "123 Main Street", City = "Toronto", State = "Ontario", Country = "Canada" }
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
            return ppl;
        }

        // Serialization/deserialization and caching:
        public bool SetCache<T>(string key, T value)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                return db.StringSet(key, DataToBytes<T>(value));
            }
            return false;
        }

        public bool SetCacheNoAttr<T>(string key, T value)
        {            
            if (!string.IsNullOrWhiteSpace(key))
            {
                return db.StringSet(key, DataToBytes<T>(value));
            }
            return false;
        }



        public T GetCache<T>(string key)
        {
            byte[] val = db.StringGet(key);
            MemoryStream stream = new MemoryStream(val, false);
            return Serializer.Deserialize<T>(stream);          
        }

        public bool DeleteFromCache(string key)
        {
            return db.KeyDelete(key);
        }

        public bool DeleteKeysByPartOfName(string pattern)
        {
            bool result = true;
            var keysPattern = string.Format("*{0}*", pattern);            
            /*foreach (var key in server.Keys(pattern: keysPattern))
            {
                if (!db.KeyDelete(key))
                    result = false;
            }*/
            return result;
        }

        /*public void ClearCache()
        {
            server.FlushDatabase();
        }*/
      
        #endregion
    }
}

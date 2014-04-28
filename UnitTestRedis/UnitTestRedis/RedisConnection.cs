using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace UnitTestRedis
{
    public sealed class RedisConnection
    {
        private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("172.16.10.32");

        static RedisConnection()
        {

        }

        /// <summary>
        /// Get Redis connection instance
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                return redis;
            }
        }
    }    
}

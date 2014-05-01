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
        private static ConfigurationOptions configOptions = new ConfigurationOptions
        {
            EndPoints =
            {
                { "192.168.11.46", 12112 }
            },
            AllowAdmin = true,
            Proxy = Proxy.Twemproxy
        };
        private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configOptions);

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace UnitTestRedis
{
    [ProtoContract]
    public class People
    {
        [ProtoMember(1)]
        public int ID { get; set; }
        [ProtoMember(2)]
        public string FirstName { get; set; }
        [ProtoMember(3)]
        public string LastName { get; set; }
        [ProtoMember(4)]
        public DateTime BirthDate { get; set; }
        [ProtoMember(5)]
        public bool Active { get; set; }
        [ProtoMember(6)]
        public AddressModel Address { get; set; }
    }

    [ProtoContract]
    public class AddressModel
    {
        [ProtoMember(1)]
        public int AptNumber { get; set; }
        [ProtoMember(2)]
        public string StreetAdress { get; set; }
        [ProtoMember(3)]
        public string ZipCode { get; set; }
        [ProtoMember(4)]
        public string City { get; set; }
        [ProtoMember(5)]
        public string State { get; set; }
        [ProtoMember(6)]
        public string Country { get; set; }
    }
}

using System;
using LiteDB;

namespace HidCerberus.Core.Database
{
    public class CerberusDatabase : LiteDatabase
    {
        private static readonly Lazy<CerberusDatabase> LazyInstance = new Lazy<CerberusDatabase>(() => new CerberusDatabase("HidCerberus.db"));

        public static CerberusDatabase Instance => LazyInstance.Value;

        private CerberusDatabase(string connectionString, BsonMapper mapper = null, Logger log = null) : base(connectionString, mapper, log)
        {
        }
    }
}
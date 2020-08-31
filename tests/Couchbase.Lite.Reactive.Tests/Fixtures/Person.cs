using System;

namespace Couchbase.Lite.Reactive.Tests.Fixtures
{
    public class Person
    {
        private Guid _id;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int  Age { get; set; }

        public string Id
        {
            get => _id.ToString();
            set => _id = Guid.Parse(value);
        }
    }
}
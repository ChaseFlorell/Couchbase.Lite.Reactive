using System;
using System.IO;
using Couchbase.Lite.Reactive.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Couchbase.Lite.Reactive.Tests
{
    [SetUpFixture]
    public class SetUpAll
    {
        public static Database Database { get; private set; }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Couchbase_Lite_Reactive_Tests_Db");
            Console.WriteLine($"Creating DB at: {dbPath}");
            Database = new Database(dbPath);
            
            for (var i = 0; i < 100; i++)
            {
                var person = new Person()
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = i,
                    FirstName = "John",
                    LastName = "Smith"
                };

                var mutableDocument = person.ToMutableDocument();
                mutableDocument.SetString("$Type", nameof(Person));
                Database.Save(mutableDocument);
            }

            Database.Count.Should().Be(100);
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            Database.Delete();
        }
    }
}
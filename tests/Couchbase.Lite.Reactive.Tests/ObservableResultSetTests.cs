using System.Linq;
using System.Reactive.Disposables;
using Couchbase.Lite.Query;
using Couchbase.Lite.Reactive.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;
using ReactiveUI;

namespace Couchbase.Lite.Reactive.Tests
{
    [TestFixture]
    public class ObservableResultSetTests : ReactiveObject
    {
        private CompositeDisposable _disposable;

        [SetUp]
        public void SetUpEach()
        {
            _disposable = new CompositeDisposable();
        }

        [TearDown]
        public void TearDownEach()
        {
            _disposable.Dispose();
        }

        [Test]
        public void ShouldCreateBaselineTest()
        {
            // setup
            var query = QueryBuilder.Select(SelectResult.All())
                .From(DataSource.Database(SetUpAll.Database))
                .Where(Expression.Property("$Type")
                    .EqualTo(Expression.String(nameof(Person)))
                    .And(Expression.Property("age").Is(Expression.Int(5))));

            // execute
            var result = query.Execute();

            // assert
            result.Count().Should().Be(1);
        }

        [Test]
        public void ShouldConnectToDatabase()
        {
            // setup
            var query = QueryBuilder.Select(SelectResult.All())
                .From(DataSource.Database(SetUpAll.Database))
                .Where(Expression.Property("$Type")
                    .EqualTo(Expression.String(nameof(Person)))
                    .And(Expression.Property("age").Is(Expression.Int(5))));

            // execute
            var resultSet = ObservableResultSet.Create(query)
                .ToProperty(this, "")
                .DisposeWith(_disposable);
            
            var allResults = resultSet.Value.AllResults();
            var connectionCount = ObservableResultSet.ConnectionCount.ToProperty(this, "").DisposeWith(_disposable);
            
            // assert
            allResults.Count.Should().Be(1);
            connectionCount.Value.Should().Be(1);
        }
    }
}
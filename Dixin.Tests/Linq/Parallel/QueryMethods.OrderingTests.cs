﻿namespace Dixin.Tests.Linq.Parallel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Dixin.Linq.Parallel;
    using Dixin.TestTools.UnitTesting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public partial class QueryMethodsTests
    {
        [TestMethod]
        public void PartitionerTest()
        {
            try
            {
                Partitioning.PartitionerAsOrdered();
                Assert.Fail();
            }
            catch (InvalidOperationException exception)
            {
                Trace.WriteLine(exception);
            }
        }

        [TestMethod]
        public void OrderablePartitionerTest()
        {
            int partitionCount = Environment.ProcessorCount * 2;
            int valueCount = partitionCount * 10000;
            IEnumerable<int> source = Enumerable.Range(0, valueCount);
            IEnumerable<KeyValuePair<long, int>> partitionsSource = new IxOrderablePartitioner<int>(source).GetOrderableDynamicPartitions();
            IEnumerable<KeyValuePair<long, int>> result = Partitioning.GetPartitions(partitionsSource, partitionCount).Concat();
            IOrderedEnumerable<int> indexes = result.Select(value => Convert.ToInt32(value.Key)).OrderBy(index => index);
            EnumerableAssert.AreSequentialEqual(source, indexes);
            IOrderedEnumerable<int> values = result.Select(value => value.Value).OrderBy(value => value);
            EnumerableAssert.AreSequentialEqual(source, values);
        }

        [TestMethod]
        public void OrderingTest()
        {
            QueryMethods.Select();
            QueryMethods.ElementAt();
            QueryMethods.Take();
            QueryMethods.Reverse();
            QueryMethods.Join();
        }
    }
}

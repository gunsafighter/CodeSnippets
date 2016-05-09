﻿namespace Dixin.Tests.Linq.Parallel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dixin.Linq.Parallel;
    using Dixin.TestTools.UnitTesting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartitioningTests
    {
        [TestMethod]
        public void BuiltInPartitioningTest()
        {
            Partitioning.Range();
            Partitioning.Strip();
            Partitioning.StripLoadBalance();
            Partitioning.StripForArray();
            Partitioning.HashInGroupBy();
            Partitioning.HashInJoin();
            Partitioning.Chunk();
        }

        [TestMethod]
        public void StaticPartitionerTest()
        {
            Partitioning.StaticPartitioner();

            int partitionCount = Environment.ProcessorCount * 2;
            int valueCount = partitionCount * 10000;
            IEnumerable<int> source = Enumerable.Range(1, valueCount);
            IEnumerable<int> partitionsSource = new StaticPartitioner<int>(source).GetDynamicPartitions();
            IEnumerable<int> values = Partitioning.GetPartitions(partitionsSource, partitionCount).Concat().OrderBy(value => value);
            EnumerableAssert.AreSequentialEqual(source, values);
        }

        [TestMethod]
        public void DynamicPartitionerTest()
        {
            Partitioning.DynamicPartitioner();
            Partitioning.VisualizeDynamicPartitioner();

            int partitionCount = Environment.ProcessorCount * 2;
            int valueCount = partitionCount * 10000;
            IEnumerable<int> source = Enumerable.Range(1, valueCount);
            IEnumerable<int> partitionsSource = new DynamicPartitioner<int>(source).GetDynamicPartitions();
            IEnumerable<int> values = Partitioning.GetPartitions(partitionsSource, partitionCount).Concat().OrderBy(value => value);
            EnumerableAssert.AreSequentialEqual(source, values);
        }
    }
}
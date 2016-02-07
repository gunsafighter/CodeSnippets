﻿namespace Dixin.Tests.Linq.LinqToSql
{
    using System.Linq;

    using Dixin.Linq.LinqToSql;
    using Dixin.TestTools.UnitTesting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public partial class AdventureWorksTests
    {
        [TestMethod]
        public void CompiedQuery()
        {
            using (AdventureWorks adventureWorks = new AdventureWorks())
            {
                string[] productNames = adventureWorks.GetProductNames(100).ToArray();
                EnumerableAssert.Any(productNames);
            }
        }
    }
}

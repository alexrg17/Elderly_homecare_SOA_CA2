using Microsoft.EntityFrameworkCore;
using CA2_SOA.Data;

namespace CA2_SOA.Tests.Helpers;

/// <summary>
/// Helper class to create in-memory database contexts for testing
/// </summary>
public static class DbContextHelper
{
    public static CareHomeDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CareHomeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CareHomeDbContext(options);
    }
}


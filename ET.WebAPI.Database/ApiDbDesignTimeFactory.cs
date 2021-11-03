using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace ET.WebAPI.Database
{
    internal class ApiDbDesignTimeFactory : IDesignTimeDbContextFactory<ApiDbContext>
    {
        public ApiDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>()
                    .UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING"));
            return new ApiDbContext(optionsBuilder.Options);
        }
    }
}
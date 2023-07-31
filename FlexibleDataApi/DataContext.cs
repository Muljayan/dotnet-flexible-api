using FlexibleDataApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlexibleDataApi
{
	public class DataContext: DbContext
	{
		public DataContext(DbContextOptions<DataContext> options): base(options)
		{
		}

		// setup FlexibleData table
		public DbSet<FlexibleData> FlexibleDatas => Set<FlexibleData>();

        // setup Statistics table
        public DbSet<Statistics> Statistics => Set<Statistics>();

    }
}


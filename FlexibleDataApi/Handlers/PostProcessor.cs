using FlexibleDataApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlexibleDataApi.Handlers
{
    public class ProcessAndStoreEvent : INotification
    {
        public Dictionary<string, string> Data { get; set; }
    }

    public class PostProcessor : INotificationHandler<ProcessAndStoreEvent>
    {
        private readonly DataContext _context;

        public PostProcessor(DataContext context)
        {
            _context = context;
        }

        public async Task Handle(ProcessAndStoreEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("PostProcessor triggered");
            foreach (var kvp in notification.Data)
            {

                var key = kvp.Key;
                var value = kvp.Value;

                var count = 1;
                var uniqueValues = value;

                var existingStat = await _context.Statistics.FirstOrDefaultAsync(s => s.Key == key);
    
                if (existingStat != null)
                {
                   existingStat.Count += count;
                   if (!existingStat.UniqueValues.Contains(uniqueValues))
                   {
                       existingStat.UniqueValues += $",{uniqueValues}";
                   }
                }
                else
                {
                   var stat = new Statistics
                   {
                       Key = key,
                       Count = count,
                       UniqueValues = uniqueValues
                   };
                   _context.Statistics.Add(stat);
                }
            }

            Console.WriteLine("Post processing complete");

            await _context.SaveChangesAsync();

            Console.WriteLine("Processed data stored to database");
        }
    }


}


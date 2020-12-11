using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public class SQLMemorieRepository : IMemorieRepository
    {
        private readonly AppDbContext context;

        public SQLMemorieRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Memories AddMemories(Memories memories)
        {

            context.DbMemories.Add(memories);

            context.SaveChanges();
            return memories;
        }

        public async Task<Memories> DeleteMemorie(Memories Memory)
        {
            if(Memory!=null)
            {
                context.DbMemories.Remove(Memory);
               await context.SaveChangesAsync();
            }
            return Memory;
        }

        public IEnumerable<Memories> GetAllMemories()
        {
            return context.DbMemories;
        }

        public Memories GetMemorieById(int id)
        {
            return context.DbMemories.Find(id);
        }

        public IEnumerable<Memories> UserSharedMemories(string id)
        {
            var memories = context.DbMemories;
            var query = from Memory in memories
                        where Memory.UserId.Equals(id)
                        select Memory;                 
            return query.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public class SQLPendingGameRepository : IPendingGameRepository
    {
        private readonly AppDbContext context;

        public SQLPendingGameRepository(AppDbContext context)
        {
            this.context = context;
        }
        public PendingGame AddGame(PendingGame model)
        {
            context.DbPendingGames.Add(model);
            context.SaveChanges();
            return model;
        }

        public PendingGame Delete(int id)
        {
            var pendingGame = context.DbPendingGames.Find(id);
            if (pendingGame != null)
            {
                context.DbPendingGames.Remove(pendingGame);
                context.SaveChanges();
            }

            return pendingGame;
        }

        public IEnumerable<PendingGame> GetAllGames()
        {
            return context.DbPendingGames.ToList();
        }

        public PendingGame GetById(int id)
        {
            return context.DbPendingGames.Find(id);
        }

        public PendingGame Update(PendingGame model)
        {
            var newPendingGame = context.DbPendingGames.Attach(model);
            newPendingGame.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return model;
        }
    }
}

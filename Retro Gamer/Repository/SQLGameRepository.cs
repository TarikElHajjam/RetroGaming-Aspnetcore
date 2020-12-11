using Retro_Gamer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public class SQLGameRepository : IGameRepository
    {
      
        private readonly AppDbContext context;

        public SQLGameRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Game AddNewGame(Game game)
        {
            context.DbGame.Add(game);
            context.SaveChanges();
            return game;
        }

        public Game Delete(int id)
        {
          var game= context.DbGame.Find(id);
            if(game!=null)
            {
                context.DbGame.Remove(game);
               
            }
            context.SaveChanges();
            return game;
        }

        public IEnumerable<Game> GetAllGames()
        {
            return context.DbGame;
        }

        public Game GetById(int id)
        {
            return context.DbGame.Find(id);
        }
        public IQueryable<Game> Search(string searchString)
        {
            var query = from game in context.DbGame
                        orderby game.Name          
                        select game;
            
            if (!String.IsNullOrEmpty(searchString) )
            {
                query = query.Where(g => g.Name.StartsWith(searchString)).OrderBy(g=>g.Name);

            }
            return query;
        }

        public Game Update(Game game)
        {
            var newGame = context.DbGame.Attach(game);
            newGame.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return game;
        }

    }
}


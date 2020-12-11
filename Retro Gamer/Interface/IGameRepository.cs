using Retro_Gamer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
   public interface IGameRepository
    {
        Game GetById(int id);
        IEnumerable<Game> GetAllGames();     
        Game AddNewGame(Game game);
        Game Update(Game game);
        Game Delete(int id);
        IQueryable<Game> Search(string search);

    }
}

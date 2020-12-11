using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public interface IPendingGameRepository
    {
        IEnumerable<PendingGame> GetAllGames();
        PendingGame AddGame(PendingGame model);
        PendingGame Update(PendingGame model);
        PendingGame GetById(int id);
        PendingGame Delete(int id);
    }
}

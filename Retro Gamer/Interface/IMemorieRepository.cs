using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public interface IMemorieRepository
    {
        IEnumerable<Memories> GetAllMemories();
        Memories AddMemories(Memories memories);
        Memories GetMemorieById(int id);
        Task <Memories> DeleteMemorie(Memories memorie);
        IEnumerable<Memories> UserSharedMemories(string id);
    }
}

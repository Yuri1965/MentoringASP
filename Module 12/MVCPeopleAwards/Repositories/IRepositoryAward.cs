using MVCPeopleAwards.Models;
using System.Collections.Generic;

namespace MVCPeopleAwards.Repositories
{
    public interface IRepositoryAward
    {
        IEnumerable<AwardViewModel> GetListAward(string nameAward = "");
        AwardViewModel GetAwardById(int id);
        AwardViewModel GetAwardByName(string nameAward);
        int SaveAward(AwardViewModel item);
        void DeleteAward(int id);
        bool CheckNameAward(string nameAward, int id = 0);
    }
}

using MVCPeopleAwards.Models;
using System.Collections.Generic;

namespace MVCPeopleAwards.Repositories
{
    public interface IRepositoryAward
    {
        IEnumerable<AwardViewModel> GetListAward();
        AwardViewModel GetAward(int id);
        void SaveAward(AwardViewModel item);
        void DeleteAward(int id);
        void AwardMapToAwardModel(Awards source, ref AwardViewModel dest);
        void AwardModelMapToAward(AwardViewModel source, ref Awards dest);
        bool CheckNameAward(string nameAward, int id = 0);
    }
}

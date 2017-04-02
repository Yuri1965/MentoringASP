using MVCPeopleAwards.Enums;
using MVCPeopleAwards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPeopleAwards.Repositories
{
    public interface IRepositoryAward
    {
        IEnumerable<AwardViewModel> GetListAward();
        AwardViewModel GetAward(int id);
        void SaveAward(AwardViewModel item, Operation operation);
        void DeleteAward(int id);
        void AwardMapToAwardModel(Awards source, ref AwardViewModel dest);
        void AwardModelMapToAward(AwardViewModel source, ref Awards dest);
        bool CheckNameAward(string nameAward, int id = 0);
    }
}

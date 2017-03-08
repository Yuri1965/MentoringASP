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
        IEnumerable<AwardModel> GetListAward();
        AwardModel GetAward(int id);
        void SaveAward(AwardModel item, Operation operation);
        void DeleteAward(int id);
        void AwardMapToAwardModel(Awards source, ref AwardModel dest);
        void AwardModelMapToAward(AwardModel source, ref Awards dest);
        bool CheckNameAward(string nameAward, int id = 0);
    }
}

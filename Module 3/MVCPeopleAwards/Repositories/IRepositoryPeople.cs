using MVCPeopleAwards.Enums;
using MVCPeopleAwards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPeopleAwards.Repositories
{
    public interface IRepositoryPeople
    {
        IEnumerable<PeopleModel> GetListPeople();
        PeopleModel GetPeople(int id);
        void SavePeople(PeopleModel item, Operation operation);
        void DeletePeople(int id);
        void PeopleMapToPeopleModel(People source, ref PeopleModel dest);
        void PeopleModelMapToPeoples(PeopleModel source, ref People dest, bool isPeoplePart = false);
        List<AwardModel> GetAwards();
        void SavePeopleAward(int peopleID, int awardID, Operation operation);
        void DeletePeopleAward(int id);

    }
}

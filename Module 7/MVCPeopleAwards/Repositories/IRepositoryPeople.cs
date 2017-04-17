using MVCPeopleAwards.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVCPeopleAwards.Repositories
{
    public interface IRepositoryPeople
    {
        IEnumerable<PeopleViewModel> GetListPeople();
        PeopleViewModel GetPeople(int id);
        void SavePeople(PeopleViewModel item);
        void DeletePeople(int id);
        void PeopleMapToPeopleModel(People source, ref PeopleViewModel dest);
        void PeopleModelMapToPeoples(PeopleViewModel source, ref People dest, bool isPeoplePart = false);
        IEnumerable<SelectListItem> GetAwards();
        //List<AwardModel> GetAwards();
        void SavePeopleAward(int peopleID, int awardID);
        void DeletePeopleAward(int id);
        bool CheckPeopleAward(int idAward, int idPeople);
    }
}

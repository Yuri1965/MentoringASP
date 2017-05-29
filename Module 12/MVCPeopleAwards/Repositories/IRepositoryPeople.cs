using MVCPeopleAwards.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVCPeopleAwards.Repositories
{
    public interface IRepositoryPeople
    {
        IEnumerable<PeopleViewModel> GetListPeople(string namePeople = "");
        PeopleViewModel GetPeople(int id);
        PeopleViewModel GetPeopleByFullName(string fullNamePeople);
        int SavePeople(PeopleViewModel item);
        void DeletePeople(int id);
        IEnumerable<SelectListItem> GetAwards();
        int SavePeopleAward(int peopleID, int awardID);
        void DeletePeopleAward(int id);
        bool CheckPeopleAward(int idAward, int idPeople);
    }
}

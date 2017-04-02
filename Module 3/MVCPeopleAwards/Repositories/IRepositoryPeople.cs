﻿using MVCPeopleAwards.Enums;
using MVCPeopleAwards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVCPeopleAwards.Repositories
{
    public interface IRepositoryPeople
    {
        IEnumerable<PeopleViewModel> GetListPeople();
        PeopleViewModel GetPeople(int id);
        void SavePeople(PeopleViewModel item, Operation operation);
        void DeletePeople(int id);
        void PeopleMapToPeopleModel(People source, ref PeopleViewModel dest);
        void PeopleModelMapToPeoples(PeopleViewModel source, ref People dest, bool isPeoplePart = false);
        IEnumerable<SelectListItem> GetAwards();
        //List<AwardModel> GetAwards();
        void SavePeopleAward(int peopleID, int awardID);
        void DeletePeopleAward(int id);

    }
}

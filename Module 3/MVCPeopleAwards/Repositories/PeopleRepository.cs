using MVCPeopleAwards.Models;
using MVCPeopleAwards.Models.DataDBContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards.Repositories
{
    public class PeopleRepository : IDisposable
    {
        private PeopleAwardsDBContext db = new PeopleAwardsDBContext();

        // получает список награжденных
        public List<PeopleModel> GetListPeoples()
        {
            List<PeopleModel> lst = new List<PeopleModel>();

            List<Peoples> entList = db.ListPeoples.ToList();

            PeopleModel peopleModel;
            foreach (var item in entList)
            {
                peopleModel = new PeopleModel();
                PeopleMapToPeopleModel(item, ref peopleModel);

                lst.Add(peopleModel);
            }

            return lst;
        }

        // маппит из Entity в Model
        public void PeopleMapToPeopleModel(Peoples source, ref PeopleModel dest)
        {
            dest.Id = source.Id;
            dest.FirstName = source.FirstName;
            dest.LastName = source.LastName;
            dest.BirthDate = source.BirthDate;

            List<PeopleAwardsModel> lst = new List<PeopleAwardsModel>();

            List<PeopleAwards> entList = source.PeopleAwards.ToList();

            PeopleAwardsModel peopleAwardModel;
            foreach (var item in entList)
            {
                peopleAwardModel = new PeopleAwardsModel();

                peopleAwardModel.Id = item.Id;
                peopleAwardModel.PeopleID = item.PeopleID;
                peopleAwardModel.AwardID = item.AwardID;

                AwardModel award = new AwardModel();
                award.Id = item.Award.Id;
                award.NameAward = item.Award.NameAward;
                award.DescriptionAward = item.Award.DescriptionAward;
                peopleAwardModel.Award = award;

                lst.Add(peopleAwardModel);
            }

            dest.PeopleAwards = lst;
        }

        // маппит из Model в Entity
        public void PeopleModelMapToPeoples(PeopleModel source, ref Peoples dest, bool isPeoplePart = false)
        {
            dest.Id = source.Id;
            dest.FirstName = source.FirstName;
            dest.LastName = source.LastName;
            dest.BirthDate = source.BirthDate;

            List<PeopleAwards> lst = new List<PeopleAwards>();

            if (isPeoplePart)
            {
                dest.PeopleAwards = lst;
                return;
            }

            List<PeopleAwardsModel> entList = source.PeopleAwards.ToList();

            PeopleAwards peopleAward;
            foreach (var item in entList)
            {
                peopleAward = new PeopleAwards();

                peopleAward.Id = item.Id;
                peopleAward.PeopleID = item.PeopleID;
                peopleAward.AwardID = item.AwardID;

                Awards award = new Awards();
                award.Id = item.Award.Id;
                award.NameAward = item.Award.NameAward;
                award.DescriptionAward = item.Award.DescriptionAward;
                peopleAward.Award = award;

                lst.Add(peopleAward);
            }

            dest.PeopleAwards = lst;
        }

        // получает человека, вместе с наградами
        public PeopleModel GetPeople(int? id)
        {
            if (id == null)
                return null;

            PeopleModel peopleModel = new PeopleModel();
            PeopleMapToPeopleModel(db.ListPeoples.Find(id), ref peopleModel);
            return peopleModel;
        }

        // для создания нового человека
        public PeopleModel GetNewPeople()
        {
            PeopleModel peopleModel = new PeopleModel() { Id = 0, FirstName = "", LastName = "", BirthDate = null, PeopleAwards = new List<PeopleAwardsModel>() };
            return peopleModel;
        }

        // получает Справочник наград
        public List<AwardModel> GetAwards()
        {
            AwardsRepository repositoryAwards = new AwardsRepository();
            return repositoryAwards.GetListAwards();
        }

        // сохраняет человека
        public void SavePeople(PeopleModel peopleModel, bool isAdd)
        {
            Peoples savePeople = new Peoples();
            PeopleModelMapToPeoples(peopleModel, ref savePeople, true);

            if (isAdd)
                db.ListPeoples.Add(savePeople);
            else
                db.Entry(savePeople).State = EntityState.Modified;

            db.SaveChanges();
        }

        // сохраняет награду человека
        public void SavePeopleAward(int peopleID, int awardID, bool isAdd)
        {
            PeopleAwards savePeopleAward = new PeopleAwards();
            savePeopleAward.PeopleID = peopleID;
            savePeopleAward.AwardID = awardID;

            if (isAdd)
                db.ListPeopleAwards.Add(savePeopleAward);
            else
                db.Entry(savePeopleAward).State = EntityState.Modified;

            db.SaveChanges();
        }

        // удаляет человека вместе с наградами
        public void DeletePeople(int? id)
        {
            if (id != null)
            {
                Peoples people = db.ListPeoples.Find(id);

                // сначала удалим награды человека
                var lstAwards = people.PeopleAwards.ToList();
                foreach (var item in lstAwards)
                {
                    db.ListAwards.Remove(item.Award);
                    db.SaveChanges();
                }

                // удалим человека
                db.ListPeoples.Remove(people);
                db.SaveChanges();
            }
        }

        // удаляет награду человека
        public void DeletePeopleAward(int? id)
        {
            if (id != null)
            {
                PeopleAwards award = db.ListPeopleAwards.Find(id);

                db.ListPeopleAwards.Remove(award);
                db.SaveChanges();
            }
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)db).Dispose();
        }
    }
}
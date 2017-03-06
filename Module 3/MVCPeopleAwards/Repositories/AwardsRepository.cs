using MVCPeopleAwards.Models;
using MVCPeopleAwards.Models.DataDBContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MVCPeopleAwards.Repositories
{
    public class AwardsRepository : IDisposable
    {
        private PeopleAwardsDBContext db = new PeopleAwardsDBContext();

        public List<AwardModel> GetListAwards()
        {
            List<AwardModel> lst = new List<AwardModel>();

            List<Awards> entList = db.ListAwards.ToList();

            AwardModel awardModel;
            foreach (var item in entList)
            {
                awardModel = new AwardModel();
                AwardMapToAwardModel(item, ref awardModel);

                lst.Add(awardModel);
            }

            return lst;
        }

        public void AwardMapToAwardModel(Awards source, ref AwardModel dest)
        {
            dest.Id = source.Id;
            dest.NameAward = source.NameAward;
            dest.DescriptionAward = source.DescriptionAward;
        }

        public void AwardModelMapToAward(AwardModel source, ref Awards dest)
        {
            dest.Id = source.Id;
            dest.NameAward = source.NameAward;
            dest.DescriptionAward = source.DescriptionAward;
        }

        public AwardModel GetAward(int? id)
        {
            if (id == null)
                return null;

            AwardModel awardModel = new AwardModel();
            AwardMapToAwardModel(db.ListAwards.Find(id), ref awardModel);
            return awardModel;
        }

        public AwardModel GetNewAward()
        {
            AwardModel awardModel = new AwardModel() { Id = 0, NameAward = "", DescriptionAward = "" };
            return awardModel;
        }

        public void SaveAward(AwardModel awardModel, bool isAdd)
        {
            Awards saveAward = new Awards();
            AwardModelMapToAward(awardModel, ref saveAward);

            if (isAdd)
                db.ListAwards.Add(saveAward);
            else
                db.Entry(saveAward).State = EntityState.Modified;

            db.SaveChanges();
        }

        public void DeleteAward(int? id)
        {
            if (id != null)
            {
                Awards award = db.ListAwards.Find(id);
                db.ListAwards.Remove(award);
                db.SaveChanges();
            }
        }

        public bool CheckNameAward(string nameAward, int id = 0)
        {
            if (db.ListAwards.ToList().FindAll(c => c.NameAward == nameAward && c.Id != id).Count() > 0)
                return true;
            else
                return false;
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)db).Dispose();
        }
    }
}
using MVCPeopleAwards.Enums;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Models.DataDBContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MVCPeopleAwards.Repositories
{
    public class AwardsRepository : IDisposable, IRepositoryAward
    {
        private MainDBContext dbContext;

        public AwardsRepository()
        {
            dbContext = new MainDBContext();
        }

        public AwardsRepository(MainDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // получает Справочник наград
        public IEnumerable<AwardViewModel> GetListAward()
        {
            List<AwardViewModel> lst = new List<AwardViewModel>();

            try
            {
                List<Awards> entList = dbContext.ListAwards.ToList();

                AwardViewModel awardModel;
                foreach (var item in entList)
                {
                    awardModel = new AwardViewModel();
                    AwardMapToAwardModel(item, ref awardModel);

                    lst.Add(awardModel);
                }
                return lst;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        // маппит Entity в Model
        public void AwardMapToAwardModel(Awards source, ref AwardViewModel dest)
        {
            dest.Id = source.Id;
            dest.NameAward = source.NameAward;
            dest.DescriptionAward = source.DescriptionAward;
            dest.PhotoAward = source.PhotoAward;
            if (source.PhotoMIMEType == null || source.PhotoAward == null)
            {
                dest.PhotoMIMEType = "";
                dest.ImageIsEmpty = true;
            }
            else
            {
                dest.PhotoMIMEType = source.PhotoMIMEType;
                dest.ImageIsEmpty = false;
            }
        }

        // маппит Model в Entity
        public void AwardModelMapToAward(AwardViewModel source, ref Awards dest)
        {
            dest.Id = source.Id;
            dest.NameAward = source.NameAward;
            dest.DescriptionAward = source.DescriptionAward;
            dest.PhotoAward = source.PhotoAward;
            if (source.PhotoMIMEType == null || source.PhotoAward == null)
                dest.PhotoMIMEType = "";
            else
                dest.PhotoMIMEType = source.PhotoMIMEType;
        }

        //получает запись
        public AwardViewModel GetAward(int id)
        {
            AwardViewModel awardModel = new AwardViewModel();
            try
            {
                AwardMapToAwardModel(dbContext.ListAwards.Find(id), ref awardModel);
                return awardModel;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        //сохраняет запись - награду
        public void SaveAward(AwardViewModel awardModel, Operation operation)
        {
            Awards saveAward = new Awards();
            AwardModelMapToAward(awardModel, ref saveAward);

            if (operation == Operation.Add)
                dbContext.ListAwards.Add(saveAward);
            else
                dbContext.Entry(saveAward).State = EntityState.Modified;

            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        //удаляет запись - награду
        public void DeleteAward(int id)
        {
            try
            {
                Awards award = dbContext.ListAwards.Find(id);
                dbContext.ListAwards.Remove(award);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        //проверяет наименование награды на уникальность
        public bool CheckNameAward(string nameAward, int id = 0)
        {
            try
            {
                if (dbContext.ListAwards.ToList().FindAll(c => c.NameAward == nameAward && c.Id != id).Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                    dbContext = null;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
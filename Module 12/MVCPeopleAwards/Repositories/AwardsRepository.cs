﻿using MVCPeopleAwards.Helpers;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Models.DataDBContext;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

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
        public IEnumerable<AwardViewModel> GetListAward(string nameAward = "")
        {
            List<AwardViewModel> lst = new List<AwardViewModel>();
            try
            {
                List<Awards> entList;
                if (nameAward.Trim().Equals(""))
                    entList = dbContext.ListAwards.ToList();
                else
                    entList = dbContext.ListAwards.ToList().FindAll(x => x.NameAward.Contains(nameAward));

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
                throw ex;
            }
        }

        // маппит Entity в Model
        private void AwardMapToAwardModel(Awards source, ref AwardViewModel dest)
        {
            dest.Id = source.Id;
            dest.NameAward = source.NameAward;

            if (source.DescriptionAward == null)
                dest.DescriptionAward = "";
            else
                dest.DescriptionAward = source.DescriptionAward;

            if (source.PhotoAward == null)
                dest.PhotoAward = null;
            else
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
        private void AwardModelMapToAward(AwardViewModel source, ref Awards dest)
        {
            dest.Id = source.Id;
            dest.NameAward = source.NameAward;

            if (source.DescriptionAward == null)
                dest.DescriptionAward = "";
            else
                dest.DescriptionAward = source.DescriptionAward;

            if (source.PhotoAward == null)
                dest.PhotoAward = null;
            else
                dest.PhotoAward = source.PhotoAward;

            if (source.PhotoMIMEType == null || source.PhotoAward == null)
                dest.PhotoMIMEType = "";
            else
                dest.PhotoMIMEType = source.PhotoMIMEType;
        }

        //получает запись по идентификатору
        public AwardViewModel GetAwardById(int id)
        {
            try
            {
                AwardViewModel awardModel = new AwardViewModel();
                var sourceAward = dbContext.ListAwards.Find(id);

                if (sourceAward == null)
                    return null;

                AwardMapToAwardModel(sourceAward, ref awardModel);
                return awardModel;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw ex;
            }
        }

        //получает запись по наименованию
        public AwardViewModel GetAwardByName(string nameAward)
        {
            if (nameAward == null || nameAward.Trim().Equals(""))
                return null;

            AwardViewModel awardModel = new AwardViewModel();
            try
            {
                var award = dbContext.ListAwards.FirstOrDefault(x => x.NameAward == nameAward);
                if (award != null)
                {
                    AwardMapToAwardModel(award, ref awardModel);
                    return awardModel;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw ex;
            }
        }

        //сохраняет запись - награду
        public int SaveAward(AwardViewModel awardModel)
        {
            Awards saveAward = new Awards();
            AwardModelMapToAward(awardModel, ref saveAward);
            try
            {
                dbContext.Set<Awards>().AddOrUpdate(saveAward);
                dbContext.SaveChanges();
                return saveAward.Id;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw ex;
            }
        }

        //удаляет запись - награду
        public void DeleteAward(int id)
        {
            try
            {
                Awards award = dbContext.ListAwards.Find(id);

                if (award != null)
                {
                    dbContext.ListAwards.Remove(award);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw ex;
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
                throw ex;
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
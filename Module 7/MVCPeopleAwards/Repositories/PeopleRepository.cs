﻿using MVCPeopleAwards.Helpers;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Models.DataDBContext;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Repositories
{
    public class PeopleRepository : IDisposable, IRepositoryPeople
    {
        private MainDBContext dbContext;

        public PeopleRepository()
        {
            dbContext = new MainDBContext();
        }

        public PeopleRepository(MainDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // получает список награжденных
        public IEnumerable<PeopleViewModel> GetListPeople()
        {
            try
            {
                List<PeopleViewModel> lst = new List<PeopleViewModel>();

                List<People> entList = dbContext.ListPeoples.ToList();

                PeopleViewModel peopleModel;
                foreach (var item in entList)
                {
                    peopleModel = new PeopleViewModel();
                    PeopleMapToPeopleModel(item, ref peopleModel);

                    lst.Add(peopleModel);
                }
                return lst;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        // маппит из Entity в Model
        public void PeopleMapToPeopleModel(People source, ref PeopleViewModel dest)
        {
            try
            {
                dest.Id = source.Id;
                dest.FirstName = source.FirstName;
                dest.LastName = source.LastName;
                dest.BirthDate = source.BirthDate;

                if (source.PhotoPeople == null)
                    dest.PhotoPeople = null;
                else
                    dest.PhotoPeople = (HttpPostedFileBase)new ExtHttpPostedFileBase(source.PhotoPeople);

                if (source.PhotoMIMEType == null || source.PhotoPeople == null)
                {
                    dest.PhotoMIMEType = "";
                    dest.ImageIsEmpty = true;
                }
                else
                {
                    dest.PhotoMIMEType = source.PhotoMIMEType;
                    dest.ImageIsEmpty = false;
                }

                List<ListPeopleAwardsViewModel> lst = new List<ListPeopleAwardsViewModel>();

                List<PeopleAwards> entList = source.PeopleAwards.ToList();

                ListPeopleAwardsViewModel peopleAwardModel;
                foreach (var item in entList)
                {
                    peopleAwardModel = new ListPeopleAwardsViewModel();

                    peopleAwardModel.Id = item.Id;
                    peopleAwardModel.PeopleID = item.PeopleID;
                    peopleAwardModel.AwardID = item.AwardID;

                    AwardViewModel award = new AwardViewModel();
                    award.Id = item.Award.Id;
                    award.NameAward = item.Award.NameAward;
                    award.DescriptionAward = item.Award.DescriptionAward;

                    if (item.Award.PhotoAward != null)
                    {
                        award.PhotoAward = (HttpPostedFileBase)new ExtHttpPostedFileBase(item.Award.PhotoAward);
                        award.ImageIsEmpty = false;
                    }
                    else award.ImageIsEmpty = true;

                    award.PhotoMIMEType = item.Award.PhotoMIMEType;

                    peopleAwardModel.Award = award;
                    lst.Add(peopleAwardModel);
                }
                dest.PeopleAwards = lst;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        // маппит из Model в Entity
        public void PeopleModelMapToPeoples(PeopleViewModel source, ref People dest, bool isPeoplePart = false)
        {
            try
            {
                dest.Id = source.Id;
                dest.FirstName = source.FirstName;
                dest.LastName = source.LastName;
                dest.BirthDate = source.BirthDate;

                if (source.PhotoPeople == null)
                    dest.PhotoPeople = null;
                else
                    dest.PhotoPeople = UtilHelper.HttpPostedFileBaseToByte(source.PhotoPeople);

                if (source.PhotoMIMEType == null || source.PhotoPeople == null)
                    dest.PhotoMIMEType = "";
                else
                    dest.PhotoMIMEType = source.PhotoMIMEType;

                List<PeopleAwards> lst = new List<PeopleAwards>();

                if (isPeoplePart)
                {
                    dest.PeopleAwards = lst;
                    return;
                }

                List<ListPeopleAwardsViewModel> entList = source.PeopleAwards.ToList();

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

                    if (item.Award.PhotoAward != null)
                        award.PhotoAward = UtilHelper.HttpPostedFileBaseToByte(item.Award.PhotoAward);

                    award.PhotoMIMEType = item.Award.PhotoMIMEType;

                    peopleAward.Award = award;

                    lst.Add(peopleAward);
                }
                dest.PeopleAwards = lst;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        // получает человека, вместе с наградами
        public PeopleViewModel GetPeople(int id)
        {
            try
            {
                PeopleViewModel peopleModel = new PeopleViewModel();
                PeopleMapToPeopleModel(dbContext.ListPeoples.Find(id), ref peopleModel);
                return peopleModel;
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        // получает Справочник наград
        public IEnumerable<SelectListItem> GetAwards()
        {
            try
            {
                AwardsRepository repositoryAwards = new AwardsRepository(dbContext);
                return repositoryAwards.GetListAward().Select(x =>
                        new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.NameAward
                        });
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        // сохраняет человека
        public void SavePeople(PeopleViewModel peopleModel)
        {
            People savePeople = new People();
            PeopleModelMapToPeoples(peopleModel, ref savePeople, true);
            try
            {
                dbContext.Set<People>().AddOrUpdate(savePeople);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        // сохраняет награду человека
        public void SavePeopleAward(int peopleID, int awardID)
        {
            PeopleAwards savePeopleAward = new PeopleAwards();
            savePeopleAward.PeopleID = peopleID;
            savePeopleAward.AwardID = awardID;

            dbContext.ListPeopleAwards.Add(savePeopleAward);

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

        // удаляет человека вместе с наградами
        public void DeletePeople(int id)
        {
            try
            {
                People people = dbContext.ListPeoples.Find(id);

                // сначала удалим награды человека
                var lstAwards = people.PeopleAwards.ToList();
                foreach (var item in lstAwards)
                {
                    dbContext.ListPeopleAwards.Remove(item);
                    dbContext.SaveChanges();
                }

                // удалим человека
                dbContext.ListPeoples.Remove(people);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        // удаляет награду человека
        public void DeletePeopleAward(int id)
        {
            try
            {
                PeopleAwards award = dbContext.ListPeopleAwards.Find(id);

                dbContext.ListPeopleAwards.Remove(award);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", ex.Message, ex.StackTrace, ex.InnerException.StackTrace)));
                throw;
            }
        }

        //проверяет Награду на уникальность в списке Наград человека
        public bool CheckPeopleAward(int idAward, int idPeople)
        {
            try
            {
                if (dbContext.ListPeopleAwards.ToList().FindAll(c => c.AwardID == idAward && c.PeopleID == idPeople).Count() > 0)
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
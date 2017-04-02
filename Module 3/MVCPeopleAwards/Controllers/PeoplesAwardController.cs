﻿using MVCPeopleAwards.Enums;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Controllers
{
    public class PeoplesAwardController : Controller
    {
        private IRepositoryPeople repository;

        public PeoplesAwardController(IRepositoryPeople rep)
        {
            this.repository = rep;
        }

        public ActionResult Index()
        {
            ListPeopleViewModel peopleModel = new ListPeopleViewModel();

            try
            {
                peopleModel.ListPeople = repository.GetListPeople().ToList();
            }
            catch
            {
                // создаем пустой список в случае неудачи и заполняем текст ошибки
                peopleModel.ListPeople = new List<PeopleViewModel>();
                peopleModel.Error = "Не удалось получить список награжденных из БД";
            }

            ViewBag.Title = "Список награжденных";
            return View(peopleModel);
        }

        #region People part
        public ActionResult CreateEditPeople(int id)
        {
            PeopleViewModel peopleModel;

            // если переход в режим Новая запись
            if (id <= 0)
            {
                peopleModel = new PeopleViewModel()
                {
                    Id = 0,
                    FirstName = "",
                    LastName = "",
                    BirthDate = DateTime.Now.Date.AddYears(-16),
                    ImageIsEmpty = true,
                    PhotoMIMEType = "",
                    PhotoPeople = null,
                    PeopleAwards = new List<ListPeopleAwardsViewModel>()
                };
                ViewBag.Title = "Добавление записи";
            }
            else
            {
                try
                {
                    peopleModel = repository.GetPeople(id);
                    if (peopleModel == null)
                        return HttpNotFound("Не найден человек с таким идентификатором");
                }
                catch
                {
                    return HttpNotFound("Ошибка на сервере");
                }
                ViewBag.Title = "Изменение записи";
            }

            SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
            return View(peopleModel);
        }

        public ActionResult DeletePeople(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleViewModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                if (peopleModel == null)
                    return HttpNotFound("Не найден человек с таким идентификатором");
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }

            ViewBag.Title = "Удаление записи";
            return View(peopleModel);
        }

        // получает список награжденных в виде файла txt (отчет)
        public ActionResult GetPeopleListReport()
        {
            ListPeopleViewModel peopleModel = new ListPeopleViewModel();

            try
            {
                peopleModel.ListPeople = repository.GetListPeople().ToList();

                var result = peopleModel.GeListPeopleToMemory();
                if (result != null)
                {
                    var memoryStream = new MemoryStream(result);
                    return new FileStreamResult(memoryStream, "text/plain") { FileDownloadName = "ReportPeople.txt" };
                }
                else
                    peopleModel.Error = "Не удалось сформировать список награжденных из БД";
            }
            catch
            {
                // создаем пустой список в случае неудачи и заполняем текст ошибки
                peopleModel.ListPeople = new List<PeopleViewModel>();
                peopleModel.Error = "Не удалось получить список награжденных из БД";
            }

            return View(peopleModel);
        }

        public ActionResult GetPhotoPeople(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            PeopleViewModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                if (peopleModel == null)
                    return null;

                return File(peopleModel.PhotoPeople, peopleModel.PhotoMIMEType);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePeople([Bind(Include = "Id,LastName,FirstName,BirthDate,PhotoPeople,PhotoMIMEType,ImageIsEmpty")] PeopleViewModel peopleModel, HttpPostedFileBase photo = null)
        {
            bool saveCreateMode = (peopleModel.Id == 0) ? true : false;

            if (ModelState.IsValid)
            {
                try
                {
                    // проверка на ввод Даты рождения (возраст от 5 до 120 лет)
                    if (!UtilHelper.CheckBirthDate(peopleModel.BirthDate))
                    {
                        peopleModel.Error = "Возраст может быть от 5 до 120 лет! Введите корректную дату рождения";
                        return View("CreateEditPeople", peopleModel);
                    }

                    // если добавляем запись
                    if (saveCreateMode)
                        peopleModel.PhotoMIMEType = "";

                    if (photo != null)
                    {
                        peopleModel.PhotoMIMEType = photo.ContentType;
                        peopleModel.PhotoPeople = new byte[photo.ContentLength];
                        photo.InputStream.Read(peopleModel.PhotoPeople, 0, photo.ContentLength);
                    }

                    // если изменяем запись
                    if (!saveCreateMode)
                        // если фото было удалено пользователем
                        if (peopleModel.ImageIsEmpty)
                        {
                            peopleModel.PhotoMIMEType = "";
                            peopleModel.PhotoPeople = null;
                        }

                    // если добавляем запись
                    if (saveCreateMode)
                    {
                        repository.SavePeople(peopleModel, Operation.Add);
                        Logger.logger.Info(String.Format("Добавлен человек:\n LastName={0}, FirstName={1}, BirthDate={2}",
                                peopleModel.LastName, peopleModel.FirstName, peopleModel.BirthDateStr));
                    }
                    else
                    {
                        repository.SavePeople(peopleModel, Operation.Update);
                        Logger.logger.Info(String.Format("Изменен человек:\n Id={0}, LastName={1}, FirstName={2}, BirthDate={3}",
                                peopleModel.Id, peopleModel.LastName, peopleModel.FirstName, peopleModel.BirthDateStr));
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                }
            }
            return RedirectToAction("CreateEditPeople", peopleModel.Id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDeletePeople(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                repository.DeletePeople(id);
                Logger.logger.Info(String.Format("Удален человек:\n Id={0}", id));
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }
            return RedirectToAction("Index");
        }

        #endregion

        private PeopleViewModel GetPeopleModelForEdit(int id)
        {
            PeopleViewModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                peopleModel.Awards = repository.GetAwards();

                return peopleModel;
            }
            catch
            {
            }

            return null;
        }

        public ActionResult EditPeopleAwards(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleViewModel peopleModel;
            try
            {
                peopleModel = GetPeopleModelForEdit(id);
                if (peopleModel == null)
                    return HttpNotFound("Не найден человек с таким идентификатором");
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }

            ViewBag.Title = "Список наград человека";
            return View(peopleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePeopleAward(int id, int peopleID)
        {
            if (id <= 0 || peopleID <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleViewModel peopleModel;
            try
            {
                peopleModel = GetPeopleModelForEdit(peopleID);
                if (peopleModel == null)
                    return HttpNotFound("Не найден человек с таким идентификатором");

                try
                {
                    repository.DeletePeopleAward(id);
                    Logger.logger.Info(String.Format("Удалена награда человека:\n Id={0}, PeopleID={1}", id, peopleID));

                    peopleModel = GetPeopleModelForEdit(peopleID);
                    if (peopleModel == null)
                        return HttpNotFound("Не найден человек с таким идентификатором");
                }
                catch
                {
                    peopleModel.Error = "Запись не удалена! Ошибка на сервере";
                }

                return RedirectToAction("EditPeopleAwards", new { id = peopleID });
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePeopleAward(int peopleID, int SelectedAwardID)
        {
            if (peopleID <= 0 || SelectedAwardID <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleViewModel peopleModel;
            try
            {
                peopleModel = GetPeopleModelForEdit(peopleID);
                if (peopleModel == null)
                    return HttpNotFound("Не найден человек с таким идентификатором");

                try
                {
                    repository.SavePeopleAward(peopleID, SelectedAwardID);
                    Logger.logger.Info(String.Format("Добавлена награда человека:\n PeopleID={0}, AwardID={1}", peopleID, SelectedAwardID));

                    peopleModel = GetPeopleModelForEdit(peopleID);
                    if (peopleModel == null)
                        return HttpNotFound("Не найден человек с таким идентификатором");
                }
                catch
                {
                    peopleModel.Error = "Запись не добавлена! Ошибка на сервере";
                }

                return RedirectToAction("EditPeopleAwards", new { id = peopleID });
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)repository).Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
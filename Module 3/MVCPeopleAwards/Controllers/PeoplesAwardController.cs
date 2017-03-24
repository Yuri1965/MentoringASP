using MVCPeopleAwards.Enums;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
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
            PeopleModelView peopleModel = new PeopleModelView();

            try
            {
                peopleModel.ListPeople = repository.GetListPeople().ToList();
            }
            catch
            {
                // создаем пустой список в случае неудачи и заполняем текст ошибки
                peopleModel.ListPeople = new List<PeopleModel>();
                peopleModel.Error = "Не удалось получить список награжденных из БД";
            }

            return View(peopleModel);
        }

        #region People part
        public ActionResult CreatePeople()
        {
            PeopleModel peopleModel = new PeopleModel()
            {
                Id = 0,
                FirstName = "",
                LastName = "",
                BirthDate = DateTime.Now.Date.AddYears(-16),
                PeopleAwards = new List<PeopleAwardsModel>()
            };
            return View(peopleModel);
        }

        public ActionResult EditPeople(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleModel peopleModel;
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

            return View(peopleModel);
        }

        public ActionResult DeletePeople(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleModel peopleModel;
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

            return View(peopleModel);
        }

        // получает список награжденных в виде файла txt (отчет)
        public ActionResult GetPeopleListReport()
        {
            PeopleModelView peopleModel = new PeopleModelView();

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
                peopleModel.ListPeople = new List<PeopleModel>();
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

            PeopleModel peopleModel;
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
        public ActionResult SaveCreatePeople([Bind(Include = "LastName,FirstName,BirthDate")] PeopleModel peopleModel, HttpPostedFileBase photo = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // проверка на ввод Даты рождения (возраст от 5 до 120 лет)
                    if (!UtilHelper.CheckBirthDate(peopleModel.BirthDate))
                    {
                        peopleModel.Error = "Возраст может быть от 5 до 120 лет! Введите корректную дату рождения";
                        return View("CreatePeople", peopleModel);
                    }

                    peopleModel.PhotoMIMEType = "";
                    if (photo != null)
                    {
                        peopleModel.PhotoMIMEType = photo.ContentType;
                        peopleModel.PhotoPeople = new byte[photo.ContentLength];
                        photo.InputStream.Read(peopleModel.PhotoPeople, 0, photo.ContentLength);
                    }

                    repository.SavePeople(peopleModel, Operation.Add);
                    Logger.logger.Info(String.Format("Добавлен человек:\n Id={0}, LastName={1}, FirstName={2}, BirthDate={3}",
                            peopleModel.Id, peopleModel.LastName, peopleModel.FirstName, peopleModel.BirthDateStr));
                    return RedirectToAction("Index");
                }
                catch
                {
                }
            }
            return RedirectToAction("CreatePeople", peopleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEditPeople([Bind(Include = "Id,LastName,FirstName,BirthDate,PhotoPeople,PhotoMIMEType")] PeopleModel peopleModel, HttpPostedFileBase photo = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // проверка на ввод Даты рождения (возраст от 5 до 120 лет)
                    if (!UtilHelper.CheckBirthDate(peopleModel.BirthDate))
                    {
                        peopleModel.Error = "Возраст может быть от 5 до 120 лет! Введите корректную дату рождения";
                        return View("EditPeople", peopleModel);
                    }

                    if (photo != null)
                    {
                        peopleModel.PhotoMIMEType = photo.ContentType;
                        peopleModel.PhotoPeople = new byte[photo.ContentLength];
                        photo.InputStream.Read(peopleModel.PhotoPeople, 0, photo.ContentLength);
                    }

                    repository.SavePeople(peopleModel, Operation.Update);
                    Logger.logger.Info(String.Format("Изменен человек:\n Id={0}, LastName={1}, FirstName={2}, BirthDate={3}",
                            peopleModel.Id, peopleModel.LastName, peopleModel.FirstName, peopleModel.BirthDateStr));
                    return RedirectToAction("Index");
                }
                catch
                {
                }
            }
            return RedirectToAction("EditPeople", peopleModel);
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

        private PeopleModel GetPeopleModelForEdit(int id)
        {
            PeopleModel peopleModel;
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

            PeopleModel peopleModel;
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

            PeopleModel peopleModel;
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

            PeopleModel peopleModel;
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
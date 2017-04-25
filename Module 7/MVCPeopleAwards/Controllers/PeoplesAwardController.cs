using MVCPeopleAwards.Helpers;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            catch (Exception e)
            {
                Logger.LogException(e);

                // создаем пустой список в случае неудачи и заполняем текст ошибки
                peopleModel.ListPeople = new List<PeopleViewModel>();
                peopleModel.Error = "Не удалось получить список награжденных из БД";
            }

            ViewBag.Title = "Список награжденных";
            return View("Index", peopleModel);
        }

        public ActionResult GetPeoplesByName(string namePeople)
        {
            ListPeopleViewModel peopleModel = new ListPeopleViewModel();
            try
            {
                peopleModel.ListPeople = (List<PeopleViewModel>)repository.GetListPeople(namePeople);
            }
            catch (Exception e)
            {
                Logger.LogException(e);

                // создаем пустой список в случае неудачи и заполняем текст ошибки
                peopleModel.ListPeople = new List<PeopleViewModel>();
                peopleModel.Error = "Не удалось получить список награжденных из БД";
            }

            ViewBag.Title = "Список награжденных";
            return View("Index", peopleModel);
        }

        public ActionResult GetPeopleByFullName(string fullNamePeople)
        {
            PeopleViewModel peopleModel;
            try
            {
                peopleModel = repository.GetPeopleByFullName(fullNamePeople);
                if (peopleModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найден человек с такими параметрами", "",
                        ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));

                peopleModel.Awards = repository.GetAwards();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                throw e;
            }

            ViewBag.Title = "Список наград человека";
            return View("EditPeopleAwards", peopleModel);
        }

        #region People part
        public ActionResult CreatePeople()
        {
            PeopleViewModel peopleModel = new PeopleViewModel()
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
            SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
            return View("CreateEditPeople", peopleModel);
        }

        public ActionResult EditPeople(int id)
        {
            PeopleViewModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                if (peopleModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "",
                        ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace,
                    ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
            }

            ViewBag.Title = "Изменение записи";
            SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
            return View("CreateEditPeople", peopleModel);
        }

        public ActionResult DeletePeople(int id)
        {
            PeopleViewModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                if (peopleModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "",
                        ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace,
                    ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
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
            catch (Exception e)
            {
                Logger.LogException(e);
                // создаем пустой список в случае неудачи и заполняем текст ошибки
                peopleModel.ListPeople = new List<PeopleViewModel>();
                peopleModel.Error = "Не удалось получить список награжденных из БД";
            }

            return View(peopleModel);
        }

        public ActionResult GetPhotoPeople(int id)
        {
            if (id <= 0)
                return null;

            PeopleViewModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                if (peopleModel == null)
                    return null;

                return File(UtilHelper.HttpPostedFileBaseToByte(peopleModel.PhotoPeople), peopleModel.PhotoMIMEType);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePeople([Bind(Include = "Id,LastName,FirstName,BirthDate,PhotoPeople,ImageIsEmpty")] PeopleViewModel peopleModel)
        {
            bool saveCreateMode = (peopleModel.Id == 0) ? true : false;

            if (ModelState.IsValid)
            {
                try
                {
                    // проверка на ввод Даты рождения (возраст от 5 до 150 лет)
                    if (!UtilHelper.CheckBirthDate(peopleModel.BirthDate))
                    {
                        ModelState.AddModelError("BirthDate", "Возраст может быть от 5 до 150 лет! Введите корректную дату рождения");
                        return View("CreateEditPeople", peopleModel);
                    }

                    // если изменяем запись
                    if (!saveCreateMode && peopleModel.PhotoPeople == null)
                        // если фото было удалено пользователем
                        if (peopleModel.ImageIsEmpty)
                        {
                            peopleModel.PhotoPeople = null;
                            peopleModel.PhotoMIMEType = "";
                        }
                        else
                        {
                            PeopleViewModel tmpPeopleModel = repository.GetPeople(peopleModel.Id);
                            peopleModel.PhotoPeople = tmpPeopleModel.PhotoPeople;
                            peopleModel.PhotoMIMEType = tmpPeopleModel.PhotoMIMEType;
                        }
                    else
                    {
                        if (peopleModel.PhotoPeople != null && peopleModel.PhotoPeople.ContentLength > 0)
                            peopleModel.PhotoMIMEType = peopleModel.PhotoPeople.ContentType;
                        else
                            peopleModel.PhotoMIMEType = "";
                    }

                    repository.SavePeople(peopleModel);
                    if (saveCreateMode)
                        Logger.logger.Trace(String.Format("Добавлен человек:\n LastName={0}, FirstName={1}, BirthDate={2}",
                                peopleModel.LastName, peopleModel.FirstName, peopleModel.BirthDateStr));
                    else
                        Logger.logger.Trace(String.Format("Изменен человек:\n Id={0}, LastName={1}, FirstName={2}, BirthDate={3}",
                                peopleModel.Id, peopleModel.LastName, peopleModel.FirstName, peopleModel.BirthDateStr));

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                    return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace,
                        ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
                }
            }
            else return View("CreateEditPeople", peopleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDeletePeople(int id)
        {
            try
            {
                repository.DeletePeople(id);
                Logger.logger.Trace(String.Format("Удален человек:\n Id={0}", id));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace,
                    ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region People info with Award part
        private PeopleViewModel GetPeopleModelForEdit(int id)
        {
            PeopleViewModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                peopleModel.Awards = repository.GetAwards();

                return peopleModel;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }

            return null;
        }

        public ActionResult EditPeopleAwards(int id)
        {
            PeopleViewModel peopleModel = GetPeopleModelForEdit(id);
            if (peopleModel == null)
                return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "",
                    ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));

            ViewBag.Title = "Список наград человека";
            return View("EditPeopleAwards", peopleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePeopleAward(int id, int peopleID)
        {
            if (id <= 0 || peopleID <= 0)
            {
                return View("Error", ErrorHelper.GetErrorModel("Переданы некорректные параметры", "",
                    ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
            }

            PeopleViewModel peopleModel;
            peopleModel = GetPeopleModelForEdit(peopleID);
            if (peopleModel == null)
                return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "",
                    ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));

            try
            {
                repository.DeletePeopleAward(id);
                Logger.logger.Trace(String.Format("Удалена награда человека:\n Id={0}, PeopleID={1}", id, peopleID));

                peopleModel = GetPeopleModelForEdit(peopleID);
                if (peopleModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "",
                        ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                peopleModel.Error = "Запись не удалена! Ошибка на сервере";
            }

            return RedirectToAction("EditPeopleAwards", new { id = peopleID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePeopleAward(int peopleId, int SelectedAwardID)
        {
            if (peopleId <= 0 || SelectedAwardID <= 0)
            {
                return View("Error", ErrorHelper.GetErrorModel("Переданы некорректные параметры", "",
                    ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
            }

            if (ModelState.IsValid)
            {
                PeopleViewModel peopleModel;
                peopleModel = GetPeopleModelForEdit(peopleId);
                if (peopleModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "",
                        ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));

                try
                {
                    repository.SavePeopleAward(peopleId, SelectedAwardID);
                    Logger.logger.Trace(String.Format("Добавлена награда человека:\n PeopleID={0}, AwardID={1}", peopleId, SelectedAwardID));

                    peopleModel = GetPeopleModelForEdit(peopleId);
                    if (peopleModel == null)
                        return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "",
                            ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    peopleModel.Error = "Запись не добавлена! Ошибка на сервере";
                }

                return RedirectToAction("EditPeopleAwards", new { id = peopleId });
            }
            else
            {
                PeopleViewModel peopleModel = GetPeopleModelForEdit(peopleId);
                if (peopleModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "",
                        ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));

                return View("EditPeopleAwards", peopleModel);
            }
        }

        // проверка Награды на уникальность в списке Наград человека
        [HttpPost]
        public ActionResult CheckPeopleAward(int selectedAwardID, int peopleId)
        {
            try
            {
                if (repository.CheckPeopleAward(selectedAwardID, peopleId))
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace,
                    ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion People info with Award part

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
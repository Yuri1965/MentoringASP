using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using System;
using MvcSiteMapProvider;

namespace MVCPeopleAwards.Controllers
{
    public class AwardsController : Controller
    {
        private IRepositoryAward repository;

        public AwardsController(IRepositoryAward rep)
        {
            this.repository = rep;
        }

        public ActionResult Index()
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();

            try
            {
                awardsModel.ListAwards = (List<AwardViewModel>)repository.GetListAward();
            }
            catch
            {
                // создаем пустой список в случае неудачи и заполняем текст ошибки
                awardsModel.ListAwards = new List<AwardViewModel>();
                awardsModel.Error = "Не удалось получить список наград из БД";
            }

            ViewBag.Title = "Справочник Награды";
            return View(awardsModel);
        }

        public ActionResult CreateEditAward(int id)
        {
            AwardViewModel awardModel;

            // если переход в режим Новая запись
            if (id <= 0)
            {
                awardModel = new AwardViewModel()
                {
                    Id = 0,
                    NameAward = "",
                    DescriptionAward = "",
                    PhotoAward = null,
                    ImageIsEmpty = true,
                    PhotoMIMEType = ""
                };
                ViewBag.Title = "Добавление записи";
            }
            else
            {
                try
                {
                    awardModel = repository.GetAward(id);
                    if (awardModel == null)
                        return HttpNotFound("Не найдена награда с таким идентификатором");
                }
                catch
                {
                    return HttpNotFound("Ошибка на сервере");
                }

                ViewBag.Title = "Изменение записи";
            }

            SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
            return View(awardModel);
        }

        public ActionResult DeleteAward(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAward(id);
                if (awardModel == null)
                    return HttpNotFound("Не найдена награда с таким идентификатором");
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }

            ViewBag.Title = "Удаление записи";
            return View(awardModel);
        }

        public ActionResult GetPhotoAward(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAward(id);
                if (awardModel == null)
                    return null;

                return File(UtilHelper.HttpPostedFileBaseToByte(awardModel.PhotoAward), awardModel.PhotoMIMEType);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAward([Bind(Include = "Id,NameAward,DescriptionAward,PhotoAward,ImageIsEmpty")] AwardViewModel awardModel)
        {
            bool saveCreateMode = (awardModel.Id == 0) ? true : false;

            if (ModelState.IsValid)
            {
                try
                {
                    // если изменяем запись
                    if (!saveCreateMode && awardModel.PhotoAward == null)
                        // если фото было удалено пользователем
                        if (awardModel.ImageIsEmpty)
                        {
                            awardModel.PhotoAward = null;
                            awardModel.PhotoMIMEType = "";
                        }
                        else
                        {
                            AwardViewModel tmpAwardModel = repository.GetAward(awardModel.Id);
                            awardModel.PhotoAward = tmpAwardModel.PhotoAward;
                            awardModel.PhotoMIMEType = tmpAwardModel.PhotoMIMEType;
                        }
                    else
                    {
                        if (awardModel.PhotoAward != null && awardModel.PhotoAward.ContentLength > 0)
                            awardModel.PhotoMIMEType = awardModel.PhotoAward.ContentType;
                        else
                            awardModel.PhotoMIMEType = "";
                    }

                    // проверка на обязательный ввод Фото награды
                    if (awardModel.PhotoAward == null || awardModel.PhotoAward.ContentLength <= 0)
                    {
                        ModelState.AddModelError("PhotoAward", "Это поле должно быть заполнено");
                        return View("CreateEditAward", awardModel);
                    }

                    repository.SaveAward(awardModel);
                    if (saveCreateMode)
                        Logger.logger.Info(String.Format("Добавлена награда:\n NameAward = {0}, DescriptionAward = {1}",
                                awardModel.NameAward, awardModel.DescriptionAward));
                    else
                        Logger.logger.Info(String.Format("Изменена награда:\n Id={0}, NameAward = {1}, DescriptionAward = {2}",
                                awardModel.Id, awardModel.NameAward, awardModel.DescriptionAward));

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                }
            }
            else return View("CreateEditAward", awardModel);

            return RedirectToAction("CreateEditAward", awardModel.Id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDeleteAward(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                repository.DeleteAward(id);
                Logger.logger.Info(String.Format("Удалена награда:\n Id={0}", id));
            }
            catch
            {
                AwardViewModel awardModel;
                try
                {
                    awardModel = repository.GetAward(id);
                    if (awardModel == null)
                        return HttpNotFound("Не найдена награда с таким идентификатором");
                    else
                    {
                        awardModel.Error = "Запись не удалена! Возможно на нее есть ссылки в списке награжденных";
                        return View("DeleteAward", awardModel);
                    }
                }
                catch
                {
                }
                return HttpNotFound("Ошибка на сервере");
            }
            return RedirectToAction("Index");
        }

        // проверка на уникальность наименования
        [HttpPost]
        public ActionResult CheckNameAward(string nameAward, int id = 0)
        {
            try
            {
                if (repository.CheckNameAward(nameAward, id))
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }
            return Json(true, JsonRequestBehavior.AllowGet);
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

using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using System;
using MVCPeopleAwards.Enums;
using System.Web;

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
            AwardsModelView awardsModel = new AwardsModelView();

            try
            {
                awardsModel.ListAwards = (List<AwardModel>)repository.GetListAward();
            }
            catch
            {
                // создаем пустой список в случае неудачи и заполняем текст ошибки
                awardsModel.ListAwards = new List<AwardModel>();
                awardsModel.Error = "Не удалось получить список наград из БД";
            }

            return View(awardsModel);
        }

        public ActionResult EditAward(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AwardModel awardModel;
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

            return View(awardModel);
        }

        public ActionResult CreateAward()
        {
            AwardModel awardModel = new AwardModel()
            {
                Id = 0,
                NameAward = "",
                DescriptionAward = "",
                PhotoAward = null,
                PhotoMIMEType = ""
            };
            return View(awardModel);
        }

        public ActionResult DeleteAward(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AwardModel awardModel;
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

            return View(awardModel);
        }

        public ActionResult GetPhotoAward(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            AwardModel awardModel;
            try
            {
                awardModel = repository.GetAward(id);
                if (awardModel == null)
                    return null;

                return File(awardModel.PhotoAward, awardModel.PhotoMIMEType);
            }
            catch
            {
                return null;
            }
        }

        public ActionResult GetPhoto(HttpPostedFileBase photo)
        {
            return File(photo.InputStream, photo.ContentType);
            //try
            //{
            //}
            //catch
            //{
            //    return null;
            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCreateAward([Bind(Include = "NameAward,DescriptionAward,PhotoAward,PhotoMIMEType")] AwardModel awardModel, HttpPostedFileBase photo = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    awardModel.PhotoMIMEType = "";
                    if (photo != null)
                    {
                        awardModel.PhotoMIMEType = photo.ContentType;
                        awardModel.PhotoAward = new byte[photo.ContentLength];
                        photo.InputStream.Read(awardModel.PhotoAward, 0, photo.ContentLength);
                    }

                    repository.SaveAward(awardModel, Operation.Add);
                    Logger.logger.Info(String.Format("Добавлена награда:\n Id={0}, NameAward = {1}, DescriptionAward = {2}",
                            awardModel.Id, awardModel.NameAward, awardModel.DescriptionAward));
                    return RedirectToAction("Index");
                }
                catch
                {
                }
            }
            return RedirectToAction("CreateAward", awardModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEditAward([Bind(Include = "Id,NameAward,DescriptionAward,PhotoAward,PhotoMIMEType")] AwardModel awardModel, HttpPostedFileBase photo = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null)
                    {
                        awardModel.PhotoMIMEType = photo.ContentType;
                        awardModel.PhotoAward = new byte[photo.ContentLength];
                        photo.InputStream.Read(awardModel.PhotoAward, 0, photo.ContentLength);
                    }

                    repository.SaveAward(awardModel, Operation.Update);
                    Logger.logger.Info(String.Format("Изменена награда:\n Id={0}, NameAward = {1}, DescriptionAward = {2}",
                            awardModel.Id, awardModel.NameAward, awardModel.DescriptionAward));
                    return RedirectToAction("Index");
                }
                catch
                {
                }
            }
            return RedirectToAction("EditAward", awardModel);
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
                AwardModel awardModel;
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
                return HttpNotFound();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}

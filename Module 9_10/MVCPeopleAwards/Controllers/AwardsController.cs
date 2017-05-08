using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using System;
using MvcSiteMapProvider;
using MVCPeopleAwards.Filters;
using MVCPeopleAwards.Helpers;

namespace MVCPeopleAwards.Controllers
{
    [Authorize(Roles = "Admin,CandidateAdmin")]
    public class AwardsController : Controller
    {
        private const string DEFAULT_BACK_ERROR_URL = "/awards";

        private IRepositoryAward repository;

        public AwardsController()
        {
            this.repository = new AwardsRepository();
        }

        public AwardsController(IRepositoryAward rep)
        {
            this.repository = rep;
        }

        [Route("awards")]
        public ActionResult Index()
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            try
            {
                awardsModel.ListAwards = (List<AwardViewModel>)repository.GetListAward();
            }
            catch (Exception e)
            {
                Logger.LogException(e);

                // создаем пустой список в случае неудачи и заполняем текст ошибки
                awardsModel.ListAwards = new List<AwardViewModel>();
                awardsModel.Error = "Не удалось получить список наград из БД";
            }

            ViewBag.Title = "Справочник Награды";

            if (Request.IsAjaxRequest())
            {
                return PartialView("ListAwardsPartial", awardsModel);
            }
            return View("Index", awardsModel);
        }

        [Route("awards/{nameAward:regex(^([a-zA-Zа-яА-Я0-9 -]+)$)}")]
        public ActionResult GetAwardsByName(string nameAward)
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            try
            {
                awardsModel.ListAwards = (List<AwardViewModel>)repository.GetListAward(nameAward);
            }
            catch (Exception e)
            {
                Logger.LogException(e);

                // создаем пустой список в случае неудачи и заполняем текст ошибки
                awardsModel.ListAwards = new List<AwardViewModel>();
                awardsModel.Error = "Не удалось получить список наград из БД";
            }

            ViewBag.Title = "Справочник Награды";
            return View("Index", awardsModel);
        }

        [Route("award/{id:int:min(1)}", Order = 1)]
        public ActionResult GetAwardById(int id)
        {
            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAwardById(id);
                if (awardModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }

            ViewBag.Title = "Информация о записи";
            SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
            return View("AwardDetail", awardModel);
        }

        [Route("award/{nameAward:regex(^([a-zA-Zа-яА-Я0-9 -]+)$)}", Order = 2)]
        public ActionResult GetAwardByName(string nameAward)
        {
            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAwardByName(nameAward);
                if (awardModel == null)
                    return View("Error", ErrorHelper.GetErrorModel(String.Format("Не найдена награда с наименованием = {0}", nameAward), "", DEFAULT_BACK_ERROR_URL));

                ViewBag.Title = "Информация о записи";
                SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
                return View("AwardDetail", awardModel);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }
        }

        [AllowAnonymous]
        public ActionResult GetAwardInfo(int id)
        {
            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAwardById(id);
                if (awardModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }

            return PartialView("ModalAwardDetail", awardModel);
        }

        [Route("create-award")]
        public ActionResult CreateAward()
        {
            AwardViewModel awardModel = new AwardViewModel()
            {
                Id = 0,
                NameAward = "",
                DescriptionAward = "",
                PhotoAward = null,
                ImageIsEmpty = true,
                PhotoMIMEType = ""
            };

            ViewBag.Title = "Добавление записи";
            SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
            return View("CreateEditAward", awardModel);
            
            //ViewBag.Title = "Добавление записи";
            //return PartialView("ModalCreateEditAward", awardModel);
        }

        [Route("award/{id:int:min(1)}/edit")]
        public ActionResult EditAward(int id)
        {
            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAwardById(id);
                if (awardModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.InnerException + e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }

            ViewBag.Title = "Изменение записи";
            SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
            return View("CreateEditAward", awardModel);
            //ViewBag.Title = "Изменение записи";
            //return PartialView("ModalCreateEditAward", awardModel);
        }

        //[Route("award/{id:int:min(1)}/delete")]
        //public ActionResult DeleteAward(int id)
        //{
        //    AwardViewModel awardModel;
        //    try
        //    {
        //        awardModel = repository.GetAwardById(id);
        //        if (awardModel == null)
        //            return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.LogException(e);
        //        return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace, DEFAULT_BACK_ERROR_URL));
        //    }

        //    return PartialView("ModalDeleteAward", awardModel);
        //}

        [AllowAnonymous]
        public ActionResult GetPhotoAward(int id)
        {
            if (id <= 0)
                return null;

            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAwardById(id);
                if (awardModel == null)
                    return null;

                return File(UtilHelper.HttpPostedFileBaseToByte(awardModel.PhotoAward), awardModel.PhotoMIMEType);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
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
                            AwardViewModel tmpAwardModel = repository.GetAwardById(awardModel.Id);
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
                        Logger.logger.Trace(String.Format("Добавлена награда:\n NameAward = {0}, DescriptionAward = {1}",
                                awardModel.NameAward, awardModel.DescriptionAward));
                    else
                        Logger.logger.Trace(String.Format("Изменена награда:\n Id={0}, NameAward = {1}, DescriptionAward = {2}",
                                awardModel.Id, awardModel.NameAward, awardModel.DescriptionAward));

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                    return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace, DEFAULT_BACK_ERROR_URL));
                }
            }
            else return View("CreateEditAward", awardModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAward(int id)
        {
            if (id <= 0)
                if (Request.IsAjaxRequest())
                    return Json(new { error = "Не найдена награда с таким идентификатором" });
                else
                    return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

            try
            {
                repository.DeleteAward(id);
                Logger.logger.Trace(String.Format("Удалена награда:\n Id={0}", id));

                if (Request.IsAjaxRequest())
                    return Json(new { id });
                else
                    return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.LogException(e);

                if (Request.IsAjaxRequest())
                    return Json(new { error = e.InnerException.Message });
                else
                    return View("Error", ErrorHelper.GetErrorModel(e.Message, e.InnerException + e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }
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
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return View("Error", ErrorHelper.GetErrorModel(ex.Message, ex.StackTrace, DEFAULT_BACK_ERROR_URL));
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

using System.Collections.Generic;
using System.Web.Mvc;
using MVCPeopleAwards.Models;
using System;
using MvcSiteMapProvider;
using MVCPeopleAwards.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.IO;

namespace MVCPeopleAwards.Controllers
{
    [Authorize(Roles = "Admin,CandidateAdmin")]
    public class AwardsController : Controller
    {
        private const string DEFAULT_BACK_ERROR_URL = "/awards";

        public AwardsController()
        {
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            string baseAddress = Request.Url.Scheme + "://" + Request.Url.Authority;
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        [Route("awards")]
        public ActionResult Index()
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            try
            {
                using (var client = GetHttpClient())
                {
                    var response = client.GetAsync("api/awards").Result;
                    awardsModel.ListAwards = (List<AwardViewModel>)response.Content.ReadAsAsync<IEnumerable<AwardViewModel>>().Result;
                }
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

        [Route("awardsByName/{nameAward:regex(^([a-zA-Zа-яА-Я0-9]+)$)}")]
        public ActionResult GetAwardsByName(string nameAward)
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            try
            {
                using (var client = GetHttpClient())
                {
                    var response = client.GetAsync(string.Format("api/awards/{0}", nameAward)).Result;
                    awardsModel.ListAwards = (List<AwardViewModel>)response.Content.ReadAsAsync<IEnumerable<AwardViewModel>>().Result;
                }
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
                using (var client = GetHttpClient())
                {
                    var response = client.GetAsync(string.Format("api/award/{0}", id)).Result;

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                    awardModel = (AwardViewModel)response.Content.ReadAsAsync<AwardViewModel>().Result;
                    if (awardModel == null)
                        return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                    ViewBag.Title = "Информация о записи";
                    SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
                    return View("AwardDetail", awardModel);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }
        }

        [Route("award/{nameAward:regex(^([a-zA-Zа-яА-Я0-9 -]+)$)}", Order = 2)]
        public ActionResult GetAwardByName(string nameAward)
        {
            AwardViewModel awardModel;
            try
            {
                using (var client = GetHttpClient())
                {
                    var response = client.GetAsync(string.Format("api/award/{0}", nameAward)).Result;

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return View("Error", ErrorHelper.GetErrorModel(String.Format("Не найдена награда с наименованием = {0}", nameAward), "", DEFAULT_BACK_ERROR_URL));

                    awardModel = (AwardViewModel)response.Content.ReadAsAsync<AwardViewModel>().Result;
                    if (awardModel == null)
                        return View("Error", ErrorHelper.GetErrorModel(String.Format("Не найдена награда с наименованием = {0}", nameAward), "", DEFAULT_BACK_ERROR_URL));

                    ViewBag.Title = "Информация о записи";
                    SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
                    return View("AwardDetail", awardModel);
                }
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
                using (var client = GetHttpClient())
                {
                    var response = client.GetAsync(string.Format("api/award/{0}", id)).Result;

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                    awardModel = (AwardViewModel)response.Content.ReadAsAsync<AwardViewModel>().Result;
                    if (awardModel == null)
                        return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                    return PartialView("ModalAwardDetail", awardModel);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }
        }

        [Route("award/{id:int:min(1)}/edit")]
        public ActionResult EditAward(int id)
        {
            AwardViewModel awardModel;
            try
            {
                using (var client = GetHttpClient())
                {
                    var response = client.GetAsync(string.Format("api/award/{0}", id)).Result;

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                    awardModel = (AwardViewModel)response.Content.ReadAsAsync<AwardViewModel>().Result;
                    if (awardModel == null)
                        return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                    ViewBag.Title = "Изменение записи";
                    SiteMaps.Current.CurrentNode.Title = ViewBag.Title;
                    return View("CreateEditAward", awardModel);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }
        }

        [AllowAnonymous]
        public ActionResult GetPhotoAward(int id)
        {
            if (id <= 0)
                return null;

            AwardViewModel awardModel;
            try
            {
                using (var client = GetHttpClient())
                {
                    var response = client.GetAsync(string.Format("api/award/{0}", id)).Result;

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    awardModel = (AwardViewModel)response.Content.ReadAsAsync<AwardViewModel>().Result;
                    if (awardModel == null)
                        return null;

                    return File(awardModel.PhotoAward, awardModel.PhotoMIMEType);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAward([Bind(Include = "Id,NameAward,DescriptionAward,ImageIsEmpty")] AwardViewModel awardModel)
        {
            bool saveCreateMode = (awardModel.Id == 0) ? true : false;

            if (ModelState.IsValid)
            {
                try
                {
                    if (!CheckNameAward(awardModel.NameAward, awardModel.Id))
                    {
                        ModelState.AddModelError("NameAward", "Такое название награды уже имеется");
                        if (saveCreateMode)
                            return PartialView("CreateAwardPartial", awardModel);
                        else
                            return View("CreateEditAward", awardModel);
                    }

                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase file = Request.Files["PhotoAward"];
                        using (var bin = new BinaryReader(file.InputStream))
                        {
                            if (file.ContentLength >= 10)
                            {
                                awardModel.PhotoAward = bin.ReadBytes(file.ContentLength);
                                awardModel.PhotoMIMEType = file.ContentType;
                            }
                            else
                            {
                                awardModel.PhotoAward = null;
                                awardModel.PhotoMIMEType = "";
                            }
                        }
                    }

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
                            using (var client = GetHttpClient())
                            {
                                var response = client.GetAsync(string.Format("api/award/{0}", awardModel.Id)).Result;

                                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                                    return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                                AwardViewModel tmpAwardModel = (AwardViewModel)response.Content.ReadAsAsync<AwardViewModel>().Result;
                                if (tmpAwardModel == null)
                                    return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                                awardModel.PhotoAward = tmpAwardModel.PhotoAward;
                                awardModel.PhotoMIMEType = tmpAwardModel.PhotoMIMEType;
                            }
                        }

                    // проверка на обязательный ввод Фото награды
                    if (awardModel.PhotoAward == null || awardModel.PhotoAward.Length <= 0)
                    {
                        if (!saveCreateMode)
                        {
                            ModelState.AddModelError("PhotoAward", "Это поле должно быть заполнено");
                            return View("CreateEditAward", awardModel);
                        }
                    }

                    int saveID = 0;
                    using (var client = GetHttpClient())
                    {
                        HttpResponseMessage response;
                        if (saveCreateMode)
                            response = client.PostAsJsonAsync(string.Format("api/award/create"), awardModel).Result;
                        else
                            response = client.PutAsJsonAsync(string.Format("api/award/update"), awardModel).Result;

                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            var errorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            return View("Error", ErrorHelper.GetErrorModel(errorMessage, "", DEFAULT_BACK_ERROR_URL));
                        }

                        saveID = (int)response.Content.ReadAsAsync<int>().Result;
                    }

                    if (saveCreateMode)
                    {
                        using (var client = GetHttpClient())
                        {
                            var response = client.GetAsync(string.Format("api/award/{0}", saveID)).Result;

                            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                                return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                            awardModel = (AwardViewModel)response.Content.ReadAsAsync<AwardViewModel>().Result;
                            if (awardModel == null)
                                return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                            return PartialView("AwardSinglePartial", awardModel);
                        }
                    }
                    else
                        return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                    return View("Error", ErrorHelper.GetErrorModel(e.Message, e.InnerException + e.StackTrace, DEFAULT_BACK_ERROR_URL));
                }
            }
            else
            {
                if (saveCreateMode)
                    return PartialView("CreateAwardPartial", awardModel);
                else
                    return View("CreateEditAward", awardModel);
            }
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
                using (var client = GetHttpClient())
                {
                    var response = client.DeleteAsync(string.Format("api/award/delete/{0}", id)).Result;

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return View("Error", ErrorHelper.GetErrorModel("Не найдена награда с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                    if (Request.IsAjaxRequest())
                        return Json(new { id });
                    else
                        return RedirectToAction("Index");
                }
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
        public bool CheckNameAward(string nameAward, int id)
        {
            bool result = false;
            using (var client = GetHttpClient())
            {
                var response = client.GetAsync(string.Format("api/award/{0}/checkName/{1}", id, nameAward)).Result;
                result = (bool)response.Content.ReadAsAsync<bool>().Result;
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
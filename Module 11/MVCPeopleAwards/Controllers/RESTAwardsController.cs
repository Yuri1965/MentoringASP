using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using RazorEngine;
using RazorEngine.Templating;
using System.Net.Http.Headers;

namespace MVCPeopleAwards.Controllers
{
    [Authorize(Roles = "Admin,CandidateAdmin")]
    public class RESTAwardsController : ApiController
    {
        private IRepositoryAward repository;

        public RESTAwardsController()
        {
            this.repository = new AwardsRepository();
        }

        public RESTAwardsController(IRepositoryAward rep)
        {
            this.repository = rep;
        }

        [HttpGet]
        [EnableQuery]
        //[GET("documents/checkForDocuments/{test}")]
        public IQueryable<AwardViewModel> Get()
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            try
            {
                awardsModel.ListAwards = (List<AwardViewModel>)repository.GetListAward();
            }
            catch (Exception e)
            {
                Logger.LogException(e);

                // создаем пустой список в случае неудачи
                awardsModel.ListAwards = new List<AwardViewModel>();
            }
            return new EnumerableQuery<AwardViewModel>(awardsModel.ListAwards);
        }

        [HttpGet]
        public IHttpActionResult GetListAwards()
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

            //ViewBag.Title = "Справочник Награды";

            if (Request.IsAjaxRequest())
            {
                return PartialView("ListAwardsPartial", awardsModel);
            }
            return View("Index", awardsModel);



            //ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            //try
            //{
            //    awardsModel.ListAwards = (List<AwardViewModel>)repository.GetListAward();
            //}
            //catch (Exception e)
            //{
            //    Logger.LogException(e);

            //    // создаем пустой список в случае неудачи и заполняем текст ошибки
            //    awardsModel.ListAwards = new List<AwardViewModel>();
            //    awardsModel.Error = "Не удалось получить список наград из БД";
            //}

            //string viewPath = HttpContext.Current.Server.MapPath(@"~\Views\Awards\ListAwardsPartial.cshtml");
            //var view = File.ReadAllText(viewPath);
            //string result = Engine.Razor.RunCompile(view, "ListAwardsPartial", null, awardsModel, new DynamicViewBag( ));

            //var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            //response.Content = new StringContent(result);
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            //return response;
        }

        [Route("api/awards/{nameAward:regex(^([a-zA-Zа-яА-Я0-9 -]+)$)}")]
        [EnableQuery]
        [HttpGet]
        public IQueryable<AwardViewModel> GetAwardsByName(string nameAward)
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            try
            {
                awardsModel.ListAwards = (List<AwardViewModel>)repository.GetListAward(nameAward);
            }
            catch (Exception e)
            {
                Logger.LogException(e);

                // создаем пустой список в случае неудачи
                awardsModel.ListAwards = new List<AwardViewModel>();
            }
            return new EnumerableQuery<AwardViewModel>(awardsModel.ListAwards);
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

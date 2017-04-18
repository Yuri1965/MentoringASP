using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;

namespace MVCPeopleAwards
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //регистрация атрибутов Route, прописанных в методах контроллеров
            routes.MapMvcAttributeRoutes();

            #region People Routes

            //Список Награжденных - Главная страница
            routes.MapRoute(
                name: "GetListPeople",
                url: "peoples",
                defaults: new { controller = "PeoplesAward", action = "Index" }
            );

            //Список Награжденных с указанным именем
            routes.MapRoute(
                name: "GetListPeoplesByName",
                url: "peoples/{namePeople}",
                defaults: new { controller = "PeoplesAward", action = "GetPeoplesByName" },
                constraints: 
                    new { namePeople = new CompoundRouteConstraint(new IRouteConstraint[] 
                    { new RegexRouteConstraint("^([a-zA-Zа-яА-Я -]+)$"), new MinLengthRouteConstraint(2), new MaxLengthRouteConstraint(50) }) }
            );

            //Добавить человека
            routes.MapRoute(
                name: "GetAddPeople",
                url: "create-people",
                defaults: new { controller = "PeoplesAward", action = "CreatePeople" }
            );

            //Изменить человека
            routes.MapRoute(
                name: "GetUpdatePeople",
                url: "people/{id}/edit",
                defaults: new { controller = "PeoplesAward", action = "EditPeople" },
                constraints: new { id = new CompoundRouteConstraint(new IRouteConstraint[] { new IntRouteConstraint(), new MinRouteConstraint(1) }) }
            );

            //Удалить человека
            routes.MapRoute(
                name: "GetDeletePeople",
                url: "people/{id}/delete",
                defaults: new { controller = "PeoplesAward", action = "DeletePeople" },
                constraints: new { id = new CompoundRouteConstraint(new IRouteConstraint[] { new IntRouteConstraint(), new MinRouteConstraint(1) }) }
            );

            //Информация о человеке с наградами
            routes.MapRoute(
                name: "GetPeopleAwards",
                url: "people/{id}",
                defaults: new { controller = "PeoplesAward", action = "EditPeopleAwards" },
                constraints: new { id = new CompoundRouteConstraint(new IRouteConstraint[] { new IntRouteConstraint(), new MinRouteConstraint(1) }) }
            );

            #endregion People Routes

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "PeoplesAward", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

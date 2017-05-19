using System.Web.Http;
using System.Web.Http.Routing.Constraints;

namespace MVCPeopleAwards
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Filters.Add(new AuthorizeAttribute());
            // НЕ работает
            //config.MapHttpAttributeRoutes();

            //Список Наград
            config.Routes.MapHttpRoute(
                name: "APIAwards",
                routeTemplate: "api/awards",
                defaults: new { controller = "RESTAwards", action = "Get" }
            );

            //Список Наград, которые начинаются с какой-то буковки
            config.Routes.MapHttpRoute(
                name: "APIAwardsByName",
                routeTemplate: "api/awards/{nameAward}",
                defaults: new { controller = "RESTAwards", action = "Get", nameAward = RouteParameter.Optional },
                constraints:
                    new
                    {
                        nameAward = new CompoundRouteConstraint(new IRouteConstraint[]
                        { new RegexRouteConstraint("^([a-zA-Zа-яА-Я -]+)$"), new MinLengthRouteConstraint(2), new MaxLengthRouteConstraint(50) })
                    }

            );

            //config.Routes.MapHttpRoute(
            //    name: "GetListAward",
            //    routeTemplate: "api/listawards",
            //    defaults: new { controller = "RESTAwards", action = "GetListAwards" }
            //);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

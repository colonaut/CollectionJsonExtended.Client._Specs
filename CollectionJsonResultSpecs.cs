using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.WebPages;
using CollectionJsonExtended.Client.Attributes;
using CollectionJsonExtended.Core;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
namespace CollectionJsonExtended.Client._Specs
{
    public class FakeEntityWithId
    {
        public int Id { get; set; }
        public string SomeString { get; set; }
    }

    public class FakeController : Controller
    {
        public CollectionJsonResult<FakeEntityWithId> GetMethodWithIntIdParam(int id)
        {
            return new CollectionJsonResult<FakeEntityWithId>(
                new FakeEntityWithId
                {
                    Id = 1,
                    SomeString = "Some string"
                });
        }
        public CollectionJsonResult<FakeEntityWithId> GetMethodWithStringIdParam(string id)
        {
            return new CollectionJsonResult<FakeEntityWithId>(
                new FakeEntityWithId
                {
                    Id = 1,
                    SomeString = "Some string"
                });
        }
    }

    public abstract class CollectionJsonAttributeContext
    {
        protected static ControllerDescriptor ControllerDescriptor;
        protected static ActionDescriptor[] GetMethodWithIntIdParamActionDescriptors;
        protected static ActionDescriptor[] GetMethodWithStringIdParamActionDescriptors;
        protected static DirectRouteProviderContext DirectRouteProviderContext;
        protected static DirectRouteBuilder DirectRouteBuilder;
        protected static CollectionJsonRouteAttribute CollectionJsonAttribute;

        Establish context =
            () =>
            {
                ControllerDescriptor =
                    new ReflectedControllerDescriptor(typeof(FakeController));

                GetMethodWithStringIdParamActionDescriptors =
                    new ActionDescriptor[]
                    {
                        new ReflectedActionDescriptor(typeof (FakeController).GetMethod("GetMethodWithStringIdParam"),
                            "GetMethodWithStringIdParam",
                            ControllerDescriptor)
                    };

                GetMethodWithIntIdParamActionDescriptors =
                    new ActionDescriptor[]
                    {
                        new ReflectedActionDescriptor(typeof (FakeController).GetMethod("GetMethodWithIntIdParam"),
                            "GetMethodWithIntIdParam",
                            ControllerDescriptor),                            
                    };
                
            };
    }


    [Subject(typeof(CollectionJsonRouteAttribute), "RouteInfo creation in CollectionJsonRouteAttribute instance")]
    public class When_the_CollectionJsonAttribute_is_instanciated_wo_CreateRoute_but_fakes
        : CollectionJsonAttributeContext
    {
        //TODO do we want to get the token to replace from the route data constraints?
        
        Establish context =
            () =>
            {
                CollectionJsonAttribute =
                    new CollectionJsonRouteAttribute(Is.Item, "some/path/{myPrimaryKeyIsId}");

                DirectRouteProviderContext =
                    new DirectRouteProviderContext("", "",
                        GetMethodWithIntIdParamActionDescriptors,
                        new DefaultInlineConstraintResolver(), true);

                DirectRouteBuilder =
                    DirectRouteProviderContext.CreateBuilder("some/path/{myPrimaryKeyIsId}");

                    new DirectRouteBuilder(GetMethodWithIntIdParamActionDescriptors,
                        true);
                
            };

        //static IList<RouteInfo> TheUrlInfoCollection;
        static RouteInfo TheRouteInfo;

        Because of =
            () =>
            {
                //var routeData = attr.CreateRoute(DirectRouteProviderContext);
                //TheUrlInfoCollection = Singleton<UrlInfoCollection>.Instance
                //    .Find<RouteInfo>(typeof(FakeEntityWithId))
                //    .ToList();

                TheRouteInfo = CollectionJsonAttribute.CreateRouteInfo(DirectRouteBuilder);
            };

        //Flicker because create route generates a new instance,
        //with ncrunch every time you write in a test... bad bad bad
        //we should find a solution... because if we call CreateRoute, the RouteInfo is also created multiple...
        //It should_foo =
        //    () => urlInfoCollection.Count.ShouldEqual(1);
        //It should_foo1 =
        //    () => urlInfoCollection[0].ShouldBeOfType<RouteInfo>();

        It should_foo2 =
            () => TheRouteInfo.Kind.ShouldEqual(Is.Item);

        private It shouuld_foo3 =
            () => TheRouteInfo.PrimaryKeyTemplate.ShouldEqual("{myPrimaryKeyIsId}");


    }


}

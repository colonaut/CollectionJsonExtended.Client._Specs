using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using CollectionJsonExtended.Client.Attributes;
using CollectionJsonExtended.Core;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
namespace CollectionJsonExtended.Client._Specs
{
    internal class FakeEntityWithIntId
    {
        public int Id { get; set; }
        public string SomeString { get; set; }
    }

    internal class FakeEntityWithStringId
    {
        public string Id { get; set; }
        public string SomeString { get; set; }
    }

    internal class FakeController : Controller
    {
        public CollectionJsonResult<FakeEntityWithIntId>
            GetMethodWithIntIdParam(int id)
        {
            return new CollectionJsonResult<FakeEntityWithIntId>(
                new FakeEntityWithIntId
                {
                    Id = 1,
                    SomeString = "Some string"
                });
        }
        
        public CollectionJsonResult<FakeEntityWithStringId>
            GetMethodWithStringIdParam(string id)
        {
            return new CollectionJsonResult<FakeEntityWithStringId>(
                new FakeEntityWithStringId
                {
                    Id = "myStringId",
                    SomeString = "Some string"
                });
        }
        
        public CollectionJsonResult<FakeEntityWithStringId>
            GetMethodWithStringIdEntityButMethodInt(int id)
        {
            return new CollectionJsonResult<FakeEntityWithStringId>(
                new FakeEntityWithStringId
                {
                    Id = "myStringId",
                    SomeString = "Some string"
                });
        }
        
    }


    internal abstract class CollectionJsonRouteAttributeContext
    {
        protected static ControllerDescriptor ControllerDescriptor;
        protected static ActionDescriptor[] GetMethodWithIntIdParamActionDescriptors;
        protected static ActionDescriptor[] GetMethodWithStringIdParamActionDescriptors;
        protected static ActionDescriptor[] GetMethodWithStringIdParamButMethodIntActionDescriptors;
        protected static DirectRouteProviderContext DirectRouteProviderContext;
        protected static DirectRouteBuilder DirectRouteBuilder;
        
        protected static RouteInfo TheRouteInfo;
        protected static RouteEntry TheRouteEntry;
        protected static CollectionJsonRouteAttribute TheAttribute;
        protected static IList<RouteInfo> TheUrlInfoCollection;
        
        Establish context  =
            () =>
            {

                ControllerDescriptor =
                    new ReflectedControllerDescriptor(typeof (FakeController));

                GetMethodWithStringIdParamActionDescriptors =
                    new ActionDescriptor[]
                    {
                        new ReflectedActionDescriptor(
                            typeof (FakeController).GetMethod("GetMethodWithStringIdParam"),
                            "GetMethodWithStringIdParam",
                            ControllerDescriptor)
                    };

                GetMethodWithIntIdParamActionDescriptors =
                    new ActionDescriptor[]
                    {
                        new ReflectedActionDescriptor(typeof (FakeController).GetMethod("GetMethodWithIntIdParam"),
                            "GetMethodWithIntIdParam",
                            ControllerDescriptor)
                    };

                GetMethodWithStringIdParamButMethodIntActionDescriptors =
                    new ActionDescriptor[]
                    {
                        new ReflectedActionDescriptor(typeof (FakeController)
                            .GetMethod("GetMethodWithStringIdEntityButMethodInt"),
                            "GetMethodWithStringIdParamButMethodInt",
                            ControllerDescriptor)
                    };
            
                

            };
        
    }


    [Subject(typeof(CollectionJsonRouteAttribute),
        "CollectionJsonAttribute.CreateRouteInfo")]
    internal class When_the_Attribute_has_input_for_controller_string_method_but_int_entity
        : CollectionJsonRouteAttributeContext
    {
        Establish context =
            () =>
            {
                const string template = "some/path/{id}";
                TheAttribute =
                    new CollectionJsonRouteAttribute(Is.Item, template);
                DirectRouteBuilder =
                    new DirectRouteBuilder(GetMethodWithStringIdParamButMethodIntActionDescriptors,
                        true);
                DirectRouteBuilder.Template = template;

            };
        
        static Exception TheException;
        Because of = () => TheException = Catch.Exception(
            () =>
            {
                TheAttribute.CreateRouteInfo(DirectRouteBuilder);
            });

        It should_throw_a_ty = () => TheException.ShouldBeOfType(typeof(TypeAccessException));

    }


    [Subject(typeof(CollectionJsonRouteAttribute),
        "CollectionJsonAttribute.CreateRoute")]
    internal class When_the_Attribute_has_valid_input_for_controller_int_method
        : CollectionJsonRouteAttributeContext
    {
        Establish context =
            () =>
            {
                var singletonFactory =
                    new SingletonFactory<UrlInfoCollection>(() => new UrlInfoCollection());
                
                TheAttribute =
                    new CollectionJsonRouteAttribute(Is.Item, "some/path/{myPrimaryKeyIsId}");

                DirectRouteProviderContext =
                    new DirectRouteProviderContext("", "",
                        GetMethodWithIntIdParamActionDescriptors,
                        new DefaultInlineConstraintResolver(), true);

                TheRouteEntry =
                    TheAttribute.CreateRoute(DirectRouteProviderContext);

                TheUrlInfoCollection =
                    singletonFactory.GetInstance()
                        .Find<RouteInfo>(typeof (FakeEntityWithIntId))
                        .ToList();
            };

        Because of =
            () =>
            {
                TheRouteInfo =
                    TheUrlInfoCollection[0];
                //TheAttribute.CreateRouteInfo(DirectRouteBuilder);
            };

        private It should_foo =
            () => TheUrlInfoCollection.Count.ShouldEqual(1);

        private It should_foo1 =
            () => TheUrlInfoCollection[0].ShouldBeOfType<RouteInfo>();

        private It should_foo2 =
            () => TheRouteInfo.Kind.ShouldEqual(Is.Item);

        private It shouuld_foo3 =
            () => TheRouteInfo.PrimaryKeyTemplate.ShouldEqual("{myPrimaryKeyIsId}");
    }


    [Subject(typeof(CollectionJsonRouteAttribute),
        "RouteInfo creation in CollectionJsonRouteAttribute instance (string emthod)")]
    internal class When_the_Attribute_has_valid_input_for_controller_string_method
        : CollectionJsonRouteAttributeContext
    {
        Establish context =
            () =>
            {
                new SingletonFactory<UrlInfoCollection>(() => new UrlInfoCollection());

                TheAttribute =
                    new CollectionJsonRouteAttribute(Is.Item, "some/path/{myPrimaryKeyIsIdAndAString}");

                DirectRouteProviderContext =
                    new DirectRouteProviderContext("", "",
                        GetMethodWithStringIdParamActionDescriptors,
                        new DefaultInlineConstraintResolver(), true);

                TheRouteEntry =
                    TheAttribute.CreateRoute(DirectRouteProviderContext);

                TheUrlInfoCollection =
                    new SingletonFactory<UrlInfoCollection>().GetInstance()
                        .Find<RouteInfo>(typeof(FakeEntityWithStringId))
                        .ToList();
            };

        Because of =
            () =>
            {
                TheRouteInfo =
                    TheUrlInfoCollection[0];
                //TheAttribute.CreateRouteInfo(DirectRouteBuilder);
            };

        It should_foo =
            () => TheUrlInfoCollection.Count.ShouldEqual(1);

        It should_foo1 =
            () => TheUrlInfoCollection[0].ShouldBeOfType<RouteInfo>();

        It should_foo2 =
            () => TheRouteInfo.Kind.ShouldEqual(Is.Item);

        It shouuld_foo3 =
            () => TheRouteInfo.PrimaryKeyTemplate.ShouldEqual("{myPrimaryKeyIsIdAndAString}");


    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using CollectionJsonExtended.Client.Attributes;
using CollectionJsonExtended.Core;
using Machine.Fakes;
using Machine.Specifications;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace CollectionJsonExtended.Client._Specs
{
    internal class FakeEntityWithIntId
    {
        public int Id { get; set; }
        public string SomeString { get; set; }
    }

    internal class FakeEntityWithPrivateIntId
    {
        int Id { get { return 1; } }
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
            GetMethodWithIntIdParamButEntityInt(int id)
        {
            return new CollectionJsonResult<FakeEntityWithStringId>(
                new FakeEntityWithStringId
                {
                    Id = "myStringId",
                    SomeString = "Some string"
                });
        }

        public CollectionJsonResult<FakeEntityWithPrivateIntId>
            GetMethodWithIntIdParamAndEntityIdPrivate(int id)
        {
            return new CollectionJsonResult<FakeEntityWithPrivateIntId>(
                new FakeEntityWithPrivateIntId
                {
                    SomeString = "Some string"
                });
        }

        public CollectionJsonResult<FakeEntityWithIntId>
            GetMethodWithNoParam()
        {
            return new CollectionJsonResult<FakeEntityWithIntId>(
                new FakeEntityWithIntId
                {
                    Id = 1,
                    SomeString = "Some string"
                });
        }
    }


    internal abstract class CollectionJsonRouteAttributeContext
        : WithFakes
    {
        protected static ControllerDescriptor ControllerDescriptor;
        protected static ActionDescriptor[] GetMethodWithIntIdParamActionDescriptors;
        protected static ActionDescriptor[] GetMethodWithStringIdParamActionDescriptors;
        protected static ActionDescriptor[] GetMethodWithStringIdParamButMethodIntActionDescriptors;
        protected static ActionDescriptor[] GetMethodWithPrivateIntIdEntityAndMethodIntActionDescriptors;
        protected static ActionDescriptor[] GetMethodWithNoParamActionDescriptors;
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
                            .GetMethod("GetMethodWithIntIdParamButEntityInt"),
                            "GetMethodWithIntIdParamButEntityInt",
                            ControllerDescriptor)
                    };

                GetMethodWithPrivateIntIdEntityAndMethodIntActionDescriptors =
                    new ActionDescriptor[]
                    {
                        new ReflectedActionDescriptor(typeof (FakeController)
                            .GetMethod("GetMethodWithIntIdParamAndEntityIdPrivate"),
                            "GetMethodWithIntIdParamAndEntityIdPrivate",
                            ControllerDescriptor)
                    };

                GetMethodWithNoParamActionDescriptors =
                    new ActionDescriptor[]
                    {
                        new ReflectedActionDescriptor(typeof (FakeController)
                            .GetMethod("GetMethodWithNoParam"),
                            "GetMethodWithNoParam",
                            ControllerDescriptor)
                    };

            };
        
    }


    [Subject(typeof(CollectionJsonRouteAttribute),
        "CollectionJsonAttribute.CreateRouteInfo")]
    internal class When_the_Attribute_has_input_for_Is_Item_and_controller_has_string_method_but_entity_has_int_primary_key
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
                        true) {Template = template};
            };
        
        static Exception TheException;
        Because of = () => TheException = Catch.Exception(
            () =>
            {
                TheAttribute.CreateRouteInfo(DirectRouteBuilder);
            });

        It should_throw_a_type_access_exception = () => TheException.ShouldBeOfType(typeof(TypeAccessException));

    }

    [Subject(typeof(CollectionJsonRouteAttribute),
        "CollectionJsonAttribute.CreateRouteInfo")]
    internal class When_the_Attribute_has_input_for_Is_Item_and_controller_has_int_method_but_entity_has_private_primary_key
        : CollectionJsonRouteAttributeContext
    {
        Establish context =
            () =>
            {
                const string template = "some/path/{id}";
                TheAttribute =
                    new CollectionJsonRouteAttribute(Is.Item, template);
                DirectRouteBuilder =
                    new DirectRouteBuilder(GetMethodWithPrivateIntIdEntityAndMethodIntActionDescriptors,
                        true) { Template = template };
            };

        static Exception TheException;
        Because of = () => TheException = Catch.Exception(
            () =>
            {
                TheAttribute.CreateRouteInfo(DirectRouteBuilder);
            });

        It should_throw_a_null_reference_exception = () => TheException.ShouldBeOfType(typeof(NullReferenceException));

    }

    [Subject(typeof(CollectionJsonRouteAttribute),
    "CollectionJsonAttribute.CreateRouteInfo")]
    internal class When_the_Attribute_has_input_for_Is_Item_and_controller_method_has_no_param
        : CollectionJsonRouteAttributeContext
    {
        Establish context =
            () =>
            {
                const string template = "some/path";
                TheAttribute =
                    new CollectionJsonRouteAttribute(Is.Item, template);
                DirectRouteBuilder =
                    new DirectRouteBuilder(GetMethodWithNoParamActionDescriptors,
                        true) { Template = template };
            };

        static Exception TheException;
        Because of = () => TheException = Catch.Exception(
            () =>
            {
                TheAttribute.CreateRouteInfo(DirectRouteBuilder);
            });

        It should_throw_a_null_reference_exception = () => TheException.ShouldBeOfType(typeof(ArgumentNullException));

    }


    [Subject(typeof(CollectionJsonRouteAttribute),
        "CollectionJsonAttribute.CreateRoute")]
    internal class When_the_Attribute_has_valid_input_for_controller_int_method
        : CollectionJsonRouteAttributeContext
    {
        Establish context =
            () =>
            {
                //Configure<SingletonFactory<UrlInfoCollection>>()
                //    .WhenToldTo(x => x.GetInstance()).Return(new UrlInfoCollection());

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
                    SingletonFactory<UrlInfoCollection>.Instance
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

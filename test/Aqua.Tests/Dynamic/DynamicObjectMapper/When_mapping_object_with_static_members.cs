﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.Tests.Dynamic.DynamicObjectMapper
{
    using Aqua.Dynamic;
    using Shouldly;
    using Xunit;

    public class When_mapping_object_with_static_members
    {
        private class CustomType
        {
#pragma warning disable CS0414 // Private filed is assigned but never used

            private const string PrivateConstString = "PrivateConstStringValue";

            public const string ConstString = "ConstStringValue";

            private static readonly string PrivateStaticReadonlyField = "PrivateStaticReadonlyFieldValue";

            public static readonly string StaticReadonlyField = "StaticReadonlyFieldValue";

#pragma warning disable SA1401 // Fields should be private

            private static string PrivateStaticField = "PrivateStaticFieldValue";

            public static string StaticField = "StaticFieldValue";

            private readonly string PrivateReadonlyField = "PrivateReadonlyFieldValue";

            public readonly string ReadonlyField = "ReadonlyFieldValue";

            private string PrivateField = "PrivateFieldValue";

            public string Field = "FieldValue";

#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CS0414 // Private filed is assigned but never used

            private static string PrivateStaticProperty { get; set; } = "PrivateStaticPropertyValue";

            public static string StaticProperty { get; set; } = "StaticPropertyValue";

            private string PrivateProperty { get; set; } = "PrivatePropertyValue";

            public string Property { get; set; } = "PropertyValue";

            public string ReadonlyProperty { get; } = "ReadonlyPropertyValue";
        }

        private readonly DynamicObject dynamicObject;

        public When_mapping_object_with_static_members()
        {
            dynamicObject = new DynamicObjectMapper().MapObject(new CustomType());
        }

        [Fact]
        public void Dynamic_object_type_should_contain_non_readonly_instance_members()
        {
            dynamicObject.Properties.Count.ShouldBe(4);
        }

        [Fact]
        public void Dynamic_object_type_should_contain_readable_instance_properties()
        {
            dynamicObject["Property"].ShouldBe("PropertyValue");
            dynamicObject["ReadonlyProperty"].ShouldBe("ReadonlyPropertyValue");
        }

        [Fact]
        public void Dynamic_object_type_should_contain_readable_instance_fields()
        {
            dynamicObject["Field"].ShouldBe("FieldValue");
            dynamicObject["ReadonlyField"].ShouldBe("ReadonlyFieldValue");
        }
    }
}

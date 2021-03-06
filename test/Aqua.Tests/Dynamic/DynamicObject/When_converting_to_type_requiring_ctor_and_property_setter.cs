﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.Tests.Dynamic.DynamicObject
{
    using Aqua.Dynamic;
    using Shouldly;
    using Xunit;

    public class When_converting_to_type_requiring_ctor_and_property_setter
    {
        private class Data
        {
            private readonly int _id;

            public Data(int id)
            {
                _id = id;
            }

            public int Id => _id;

            public double DoubleValue { get; set; }

            public string StringValue { get; set; }
        }

        private const int Int32Value = 11;
        private const double DoubleValue = 1.234567891;
        private const string StringValue = "eleven";

        private readonly Data obj;

        public When_converting_to_type_requiring_ctor_and_property_setter()
        {
            var dynamicObject = new DynamicObject()
            {
                Properties = new PropertySet
                {
                    { "Id", Int32Value },
                    { "DoubleValue", DoubleValue },
                    { "StringValue", StringValue },
                },
            };

            obj = dynamicObject.CreateObject<Data>();
        }

        [Fact]
        public void Should_create_instance()
        {
            obj.ShouldNotBeNull();
        }

        [Fact]
        public void Should_populate_id_property()
        {
            obj.Id.ShouldBe(Int32Value);
        }

        [Fact]
        public void Should_populate_double_property()
        {
            obj.DoubleValue.ShouldBe(DoubleValue);
        }

        [Fact]
        public void Should_populate_string_property()
        {
            obj.StringValue.ShouldBe(StringValue);
        }
    }
}
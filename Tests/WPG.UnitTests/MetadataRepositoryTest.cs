﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using tainicom.WpfPropertyGrid;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WpfPropertyGrid.UnitTests
{
    [TestClass]
    public class MetadataRepositoryTest
    {
        [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
        public class MyCustomAttribute : Attribute
        {
            public string Name { get; set; }
        }

        public class MyEmptyClass
        {
        }

        [MyCustom(Name = "MyEntity")]
        public class MyEntity
        {
            [Category("General")]
            [DisplayName("First Name")]
            public string FirstName { get; set; }

            [Category("General")]
            [DisplayName("Last Name")]
            public string LastName { get; set; }
        }

        public class MyEntity2
        {
            public string FirstName { get; set; }

            [MergableProperty(false)]
            public string LastName { get; set; }
        }

        MetadataRepository metadataRepository;

        [TestInitialize]
        public void Initialize()
        {
            metadataRepository = new MetadataRepository();
            metadataRepository.Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldFailCollectingProperties_1()
        {
            metadataRepository.GetProperties(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldFailCollectingProperties_2()
        {
            metadataRepository.GetProperty(null, "property");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldFailCollectingProperties_3()
        {
            metadataRepository.GetProperty(typeof(MyEntity), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldFailCollectingTypeAttributes()
        {
            metadataRepository.GetAttributes(null);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldFailCollectingPropertyAttributes_1()
        {
            metadataRepository.GetAttributes(null, "property");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldFailCollectingPropertyAttributes_2()
        {
            metadataRepository.GetAttributes(typeof(MyEntity), null);
        }

        [TestMethod]
        public void ShouldCollectProperties()
        {
            List<PropertyData> properties = metadataRepository.GetProperties(new MyEntity()).ToList();
            Assert.AreEqual<int>(2, properties.Count);

            PropertyData data = properties[0];

            Assert.AreEqual<string>("FirstName", data.Name);
            Assert.AreEqual<string>("First Name", data.DisplayName);
            Assert.AreEqual<string>("General", data.Category);

            data = properties[1];

            Assert.AreEqual<string>("LastName", data.Name);
            Assert.AreEqual<string>("Last Name", data.DisplayName);
            Assert.AreEqual<string>("General", data.Category);
        }

        [TestMethod]
        public void ShouldFetchPropertyByName()
        {
            PropertyData property = metadataRepository.GetProperty(new MyEntity(), "FirstName");
            Assert.IsNotNull(property);
            Assert.AreEqual<string>("FirstName", property.Name);
        }

        [TestMethod]
        public void ShouldReturnNullForMissingProperty()
        {
            Assert.IsNull(metadataRepository.GetProperty(new MyEntity(), "MISSING"));
        }

        [TestMethod]
        public void ShouldCollectTypeAttributes()
        {
            List<Attribute> attributes = metadataRepository.GetAttributes(new MyEntity()).ToList();
            Assert.AreEqual<int>(1, attributes.Count);

            MyCustomAttribute attr = attributes[0] as MyCustomAttribute;
            Assert.IsNotNull(attr);
            Assert.AreEqual<string>("MyEntity", attr.Name);
        }

        [TestMethod]
        public void ShouldCollectCommonProperties()
        {
            List<PropertyData> properties = metadataRepository.GetCommonProperties(new object[] { new MyEntity(), new MyEntity2() }).ToList();
            Assert.AreEqual<int>(1, properties.Count);
            Assert.AreEqual<string>("FirstName", properties[0].Name);
        }

        [TestMethod]
        public void ShouldReturnEmptyPropertyCollection()
        {
            IEnumerable<PropertyData> properties = metadataRepository.GetCommonProperties(null);
            Assert.IsNotNull(properties);
            Assert.AreEqual<int>(0, properties.Count());

            properties = metadataRepository.GetCommonProperties(new object[] { new MyEmptyClass() });
            Assert.IsNotNull(properties);
            Assert.AreEqual<int>(0, properties.Count());

            properties = metadataRepository.GetCommonProperties(Enumerable.Empty<object>());
            Assert.IsNotNull(properties);
            Assert.AreEqual<int>(0, properties.Count());
        }

        [TestMethod]
        public void ShouldReturnEmptyCollectionForMissingProperty()
        {
            IEnumerable<Attribute> properties = metadataRepository.GetAttributes(new MyEntity(), "Missing");
            Assert.IsNotNull(properties);
            Assert.AreEqual<int>(0, properties.Count());
        }

        [TestMethod]
        public void ShouldReturnPropertyAttributes()
        {
            List<Attribute> attributes = metadataRepository.GetAttributes(new MyEntity(), "FirstName").ToList();
            Assert.AreEqual<int>(7, attributes.Count);

            metadataRepository.Clear();

            metadataRepository.GetProperties(new MyEntity());
            attributes = metadataRepository.GetAttributes(new MyEntity(), "FirstName").ToList();
            Assert.AreEqual<int>(7, attributes.Count);
        }
    }
}

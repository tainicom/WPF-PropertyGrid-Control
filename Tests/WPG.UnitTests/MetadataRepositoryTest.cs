using System;
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

    [TestInitialize]
    public void Initialize()
    {
      MetadataRepository.Instance.Clear();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ShouldFailCollectingProperties_1()
    {
        MetadataRepository.Instance.GetProperties(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ShouldFailCollectingProperties_2()
    {
        MetadataRepository.Instance.GetProperty(null, "property");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ShouldFailCollectingProperties_3()
    {
        MetadataRepository.Instance.GetProperty(typeof(MyEntity), null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ShouldFailCollectingTypeAttributes()
    {
        MetadataRepository.Instance.GetAttributes(null);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ShouldFailCollectingPropertyAttributes_1()
    {
        MetadataRepository.Instance.GetAttributes(null, "property");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ShouldFailCollectingPropertyAttributes_2()
    {
        MetadataRepository.Instance.GetAttributes(typeof(MyEntity), null);
    }

    [TestMethod]
    public void ShouldCollectProperties()
    {
      List<PropertyData> properties = MetadataRepository.Instance.GetProperties(new MyEntity()).ToList();
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
      PropertyData property = MetadataRepository.Instance.GetProperty(new MyEntity(), "FirstName");
      Assert.IsNotNull(property);
      Assert.AreEqual<string>("FirstName", property.Name);
    }

    [TestMethod]
    public void ShouldReturnNullForMissingProperty()
    {
      Assert.IsNull(MetadataRepository.Instance.GetProperty(new MyEntity(), "MISSING"));
    }

    [TestMethod]
    public void ShouldCollectTypeAttributes()
    {
      List<Attribute> attributes = MetadataRepository.Instance.GetAttributes(new MyEntity()).ToList();
      Assert.AreEqual<int>(1, attributes.Count);

      MyCustomAttribute attr = attributes[0] as MyCustomAttribute;
      Assert.IsNotNull(attr);
      Assert.AreEqual<string>("MyEntity", attr.Name);
    }

    [TestMethod]
    public void ShouldCollectCommonProperties()
    {
      List<PropertyData> properties = MetadataRepository.Instance.GetCommonProperties(new object[] { new MyEntity(), new MyEntity2() }).ToList();
      Assert.AreEqual<int>(1, properties.Count);
      Assert.AreEqual<string>("FirstName", properties[0].Name);
    }

    [TestMethod]
    public void ShouldReturnEmptyPropertyCollection()
    {
      IEnumerable<PropertyData> properties = MetadataRepository.Instance.GetCommonProperties(null);
      Assert.IsNotNull(properties);
      Assert.AreEqual<int>(0, properties.Count());

      properties = MetadataRepository.Instance.GetCommonProperties(new object[] { new MyEmptyClass() });
      Assert.IsNotNull(properties);
      Assert.AreEqual<int>(0, properties.Count());

      properties = MetadataRepository.Instance.GetCommonProperties(Enumerable.Empty<object>());
      Assert.IsNotNull(properties);
      Assert.AreEqual<int>(0, properties.Count());
    }

    [TestMethod]
    public void ShouldReturnEmptyCollectionForMissingProperty()
    {
      IEnumerable<Attribute> properties = MetadataRepository.Instance.GetAttributes(new MyEntity(), "Missing");
      Assert.IsNotNull(properties);
      Assert.AreEqual<int>(0, properties.Count());
    }

    [TestMethod]
    public void ShouldReturnPropertyAttributes()
    {
      List<Attribute> attributes = MetadataRepository.Instance.GetAttributes(new MyEntity(), "FirstName").ToList();
      Assert.AreEqual<int>(7, attributes.Count);

      MetadataRepository.Instance.Clear();

      MetadataRepository.Instance.GetProperties(new MyEntity());
      attributes = MetadataRepository.Instance.GetAttributes(new MyEntity(), "FirstName").ToList();
      Assert.AreEqual<int>(7, attributes.Count);
    }
  }
}

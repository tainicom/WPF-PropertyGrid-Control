﻿/*
 * Copyright © 2010, Denys Vuika
 * Copyright © 2014-2016, Kastellanos Nikolaos
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System;

namespace tainicom.WpfPropertyGrid
{
    [DebuggerDisplay("{Name}")]
    // For the moment this class is a wrapper around PropertyDescriptor. Later on it will be migrated into a separate independent unit.
    // It will be able in future creating dynamic objects without using reflection
    public class PropertyData : IEquatable<PropertyData>
    {
        #region Fields

        private static readonly List<Type> CultureInvariantTypes = new List<Type> 
    { 
      KnownTypes.Wpf.CornerRadius,
      KnownTypes.Wpf.Point3D, 
      KnownTypes.Wpf.Point4D, 
      KnownTypes.Wpf.Point3DCollection, 
      KnownTypes.Wpf.Matrix3D, 
      KnownTypes.Wpf.Quaternion, 
      KnownTypes.Wpf.Rect3D, 
      KnownTypes.Wpf.Size3D, 
      KnownTypes.Wpf.Vector3D, 
      KnownTypes.Wpf.Vector3DCollection, 
      KnownTypes.Wpf.PointCollection, 
      KnownTypes.Wpf.VectorCollection, 
      KnownTypes.Wpf.Point, 
      KnownTypes.Wpf.Rect, 
      KnownTypes.Wpf.Size, 
      KnownTypes.Wpf.Thickness, 
      KnownTypes.Wpf.Vector     
    };

        private static readonly string[] StringConverterMembers = { "Content", "Header", "ToolTip", "Tag" };

        #endregion

        public PropertyDescriptor Descriptor { get; private set; }

        public string Name
        {
            get { return Descriptor.Name; }
        }

        public string DisplayName
        {
            get { return Descriptor.DisplayName; }
        }

        public string Description
        {
            get { return Descriptor.Description; }
        }

        public string Category
        {
            get { return Descriptor.Category; }
        }

        public Type PropertyType
        {
            get { return Descriptor.PropertyType; }
        }

        public Type ComponentType
        {
            get { return Descriptor.ComponentType; }
        }

        public bool IsBrowsable
        {
            get { return Descriptor.IsBrowsable; }
        }

        public bool IsReadOnly
        {
            get { return Descriptor.IsReadOnly; }
        }

        // TODO: Cache value?
        public bool IsMergable
        {
            get { return MergablePropertyAttribute.Yes.Equals(Descriptor.Attributes[KnownTypes.Attributes.MergablePropertyAttribute]); }
        }

        // TODO: Cache value?
        public bool IsAdvanced
        {
            get
            {
                var attr = Descriptor.Attributes[KnownTypes.Attributes.EditorBrowsableAttribute] as EditorBrowsableAttribute;
                return attr != null && attr.State == EditorBrowsableState.Advanced;
            }
        }

        public bool IsLocalizable
        {
            get { return Descriptor.IsLocalizable; }
        }

        public bool IsCollection
        {
            get { return KnownTypes.Collections.IList.IsAssignableFrom(this.PropertyType); }
        }

        public DesignerSerializationVisibility SerializationVisibility
        {
            get { return Descriptor.SerializationVisibility; }
        }

        private CultureInfo _SerializationCulture;
        public CultureInfo SerializationCulture
        {
            get
            {
                if (_SerializationCulture == null)
                {
                    _SerializationCulture = (CultureInvariantTypes.Contains(PropertyType) || KnownTypes.Wpf.Geometry.IsAssignableFrom(PropertyType))
                      ? CultureInfo.InvariantCulture
                      : CultureInfo.CurrentCulture;
                }

                return _SerializationCulture;
            }
        }

        public PropertyData(PropertyDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        #region System.Object overrides

        public override int GetHashCode()
        {
            return Descriptor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PropertyData data = obj as PropertyData;
            return (data != null) ? Descriptor.Equals(data.Descriptor) : false;
        }

        #endregion

        #region IEquatable<PropertyData> Members

        public bool Equals(PropertyData other)
        {
            return Descriptor.Equals(other.Descriptor);
        }

        #endregion
    }

    public class MetadataRepository
    {
        private class PropertySet : Dictionary<string, PropertyData> { }
        private class AttributeSet : Dictionary<string, HashSet<Attribute>> { }

        private readonly Dictionary<object, PropertySet> Properties = new Dictionary<object, PropertySet>();
        private readonly Dictionary<object, AttributeSet> PropertyAttributes = new Dictionary<object, AttributeSet>();
        private readonly Dictionary<object, HashSet<Attribute>> TypeAttributes = new Dictionary<object, HashSet<Attribute>>();

        private static readonly Attribute[] PropertyFilter = new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.SetValues | PropertyFilterOptions.UnsetValues | PropertyFilterOptions.Valid) };

        internal MetadataRepository()
        {
        }

        public void Clear()
        {
            Properties.Clear();
            PropertyAttributes.Clear();
            TypeAttributes.Clear();
        }

        #region Property Management

        public IEnumerable<PropertyData> GetProperties(object target)
        {
            return DoGetProperties(target).ToList().AsReadOnly();
        }

        private IEnumerable<PropertyData> DoGetProperties(object target)
        {
            if (target == null) throw new ArgumentNullException("target");

            PropertySet result;
            if (!Properties.TryGetValue(target, out result))
                result = CollectProperties(target);

            return result.Values;
        }

        public IEnumerable<PropertyData> GetCommonProperties(IEnumerable<object> targets)
        {
            if (targets == null) return Enumerable.Empty<PropertyData>();

            IEnumerable<PropertyData> result = null;

            foreach (object target in targets)
            {
                var properties = DoGetProperties(target).Where(prop => prop.IsBrowsable && prop.IsMergable);
                result = (result == null) ? properties : result.Intersect(properties);
            }

            return (result != null) ? result : Enumerable.Empty<PropertyData>();
        }

        public PropertyData GetProperty(object target, string propertyName)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

            PropertySet propertySet = null;

            if (!Properties.TryGetValue(target, out propertySet))
                propertySet = CollectProperties(target);

            PropertyData property;

            if (propertySet.TryGetValue(propertyName, out property))
                return property;

            return null;
        }

        private PropertySet CollectProperties(object target)
        {
            PropertySet result;

            if (!Properties.TryGetValue(target, out result))
            {
                result = new PropertySet();

                // testing custom properties for objects with TypeConverter
                //var typeConverter = TypeDescriptor.GetConverter(target.GetType());
                //if (typeConverter.GetPropertiesSupported(null)) // has custom Properties?
                //{
                //  foreach (PropertyDescriptor descriptor in typeConverter.GetProperties(null, target, PropertyFilter))
                //  {
                //    result.Add(descriptor.Name, new PropertyData(descriptor));
                //    CollectAttributes(target, descriptor);
                //  }
                //}
                //else
                {
                    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(target, PropertyFilter))
                    {
                        result.Add(descriptor.Name, new PropertyData(descriptor));
                        CollectAttributes(target, descriptor);
                    }

                    // get non-public browsable properties
                    foreach (var propertyInfo in target.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))
                    {
                        var browsable = false;
                        foreach (var attribute in propertyInfo.GetCustomAttributes(true))
                        {
                            if (attribute is BrowsableAttribute && ((BrowsableAttribute)attribute).Browsable)
                                browsable = true;
                            if (attribute is EditorBrowsableAttribute && ((EditorBrowsableAttribute)attribute).State != EditorBrowsableState.Never)
                                browsable = true;
                        }
                        if (!browsable) continue;
                        var descriptor = new tainicom.WpfPropertyGrid.Internal.NonPublicPropertyDescriptor(propertyInfo);
                        result.Add(descriptor.Name, new PropertyData(descriptor));
                        CollectAttributes(target, descriptor);
                    }
                }


                Properties.Add(target, result);
            }

            return result;
        }

        #endregion Property Management

        #region Attribute Management

        public IEnumerable<Attribute> GetAttributes(object target)
        {
            if (target == null) throw new ArgumentNullException("target");

            return CollectAttributes(target).ToList().AsReadOnly();
        }

        private HashSet<Attribute> CollectAttributes(object target)
        {
            HashSet<Attribute> attributes;

            if (!TypeAttributes.TryGetValue(target, out attributes))
            {
                attributes = new HashSet<Attribute>();

                foreach (Attribute attribute in TypeDescriptor.GetAttributes(target))
                    attributes.Add(attribute);

                TypeAttributes.Add(target, attributes);
            }

            return attributes;
        }

        private HashSet<Attribute> CollectAttributes(object target, PropertyDescriptor descriptor)
        {
            AttributeSet attributeSet;

            if (!PropertyAttributes.TryGetValue(target, out attributeSet))
            {
                // Create an empty attribute sequence
                attributeSet = new AttributeSet();
                PropertyAttributes.Add(target, attributeSet);
            }

            HashSet<Attribute> attributes;

            if (!attributeSet.TryGetValue(descriptor.Name, out attributes))
            {
                attributes = new HashSet<Attribute>();

                foreach (Attribute attribute in descriptor.Attributes)
                    attributes.Add(attribute);

                attributeSet.Add(descriptor.Name, attributes);
            }

            return attributes;
        }

        public IEnumerable<Attribute> GetAttributes(object target, string propertyName)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

            if (!PropertyAttributes.ContainsKey(target))
                CollectProperties(target);

            AttributeSet attributeSet;

            if (PropertyAttributes.TryGetValue(target, out attributeSet))
            {
                HashSet<Attribute> result;
                if (attributeSet.TryGetValue(propertyName, out result))
                    return result.ToList().AsReadOnly();
            }

            return Enumerable.Empty<Attribute>();
        }

        #endregion Attribute Management
    }
}

/*
 * Copyright © 2016, Kastellanos Nikolaos
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

using System;
using System.ComponentModel;
using System.Reflection;

namespace tainicom.WpfPropertyGrid.Internal
{
    class NonPublicPropertyDescriptor : PropertyDescriptor
    {
        PropertyInfo propertyInfo;
        public NonPublicPropertyDescriptor(PropertyInfo propertyInfo)
            : base(
                propertyInfo.Name.Substring(propertyInfo.Name.LastIndexOf('.') + 1) // remove full namespace from explicit property
                , Array.ConvertAll(propertyInfo.GetCustomAttributes(true), o => (Attribute)o))
        {
            this.propertyInfo = propertyInfo;
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get
            {
                return this.propertyInfo.DeclaringType;
            }
        }

        public override object GetValue(object component)
        {
            return this.propertyInfo.GetValue(component, null);
        }

        public override bool IsReadOnly
        {
            get
            {
                return !this.propertyInfo.CanWrite;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return this.propertyInfo.PropertyType;
            }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            this.propertyInfo.SetValue(component, value, null);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}

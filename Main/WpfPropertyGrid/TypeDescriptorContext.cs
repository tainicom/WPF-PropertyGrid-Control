using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace tainicom.WpfPropertyGrid
{
    public class TypeDescriptorContext : ITypeDescriptorContext
    {
        public IContainer Container { get; private set; }
        public object Instance { get; private set; }
        public PropertyDescriptor PropertyDescriptor { get; private set; }


        public TypeDescriptorContext(PropertyItem propertyItem)
        {
            this.Instance = propertyItem.Component;
        }

        public void OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        public bool OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}

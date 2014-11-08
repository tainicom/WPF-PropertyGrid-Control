/*
 * Copyright © 2010, Denys Vuika
 * Copyright © 2014, Kastellanos Nikolaos
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

using System.Windows;
using System.Windows.Controls;
namespace tainicom.WpfPropertyGrid.Design
{
  public class TabbedLayoutTemplateSelector : DataTemplateSelector
  {
    private readonly ResourceLocator _resourceLocator = new ResourceLocator();

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var category = item as CategoryItem;
      if (category != null)
      {
        var template = FindEditorTemplate(category);
        if (template != null) return template;
      }

      return base.SelectTemplate(item, container);
    }

    protected virtual DataTemplate FindEditorTemplate(CategoryItem category)
    {
      if (category == null) return null;

      var editor = category.Editor;

      if (editor == null) return null;

      var template = editor.InlineTemplate as DataTemplate;
      if (template != null) return template;

      return _resourceLocator.GetResource(editor.InlineTemplate) as DataTemplate;
    }
  }
}

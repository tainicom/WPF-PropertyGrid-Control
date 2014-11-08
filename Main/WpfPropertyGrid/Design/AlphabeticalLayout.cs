﻿/*
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

using System.Windows.Controls;
using System.Windows;
namespace tainicom.WpfPropertyGrid.Design
{
  /// <summary>
  /// The default alphabetical view for properties.
  /// </summary>
  public class AlphabeticalLayout : Control
  {
    static AlphabeticalLayout()
    {      
      DefaultStyleKeyProperty.OverrideMetadata(typeof(AlphabeticalLayout), new FrameworkPropertyMetadata(typeof(AlphabeticalLayout)));
    }
  }
}

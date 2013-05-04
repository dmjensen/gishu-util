/*   Copyright 2010 Gishu Pillai

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Gishu.WPF.Utilities
{
    /// <summary>
    /// A class to visualize the logical/visual tree under a certain dependency object
    /// </summary>
    public class UITreeNode
    {
        private object _obj;
        private DependencyObject _depObj;
        
        /// <summary>
        /// Create a TreeNode for an object in the visual tree 
        /// </summary>
        /// <param name="obj">A Dependency Object whose logical/visual children you want to drill into</param>
        public UITreeNode(object obj):this(obj, false)
        {        }
        
        /// <summary>
        /// Create a TreeNode for an object in the visual tree 
        /// </summary>
        /// <param name="obj">A Dependency Object whose logical/visual children you want to drill into</param>
        /// <param name="isLogicalChild">True if the above object is a logical child of some parent in the tree</param>
        public UITreeNode(object obj, bool isLogicalChild)
        {
            _obj = obj;
            _depObj = obj as DependencyObject;
            this.IsLogicalChild = isLogicalChild;
        }
        /// <summary>
        /// Returns true if the current object is a Logical child of some node in the hierarchy
        /// </summary>
        public bool IsLogicalChild { get; private set; }
        /// <summary>
        /// A descriptive name to identify the node type/value
        /// </summary>
        public string Title
        {
            get
            {
                var typeName = _obj.GetType().FullName;
                var asString = _obj.ToString();
                if (asString.Contains(typeName))
                    return _obj.GetType().Name;
                else
                    return string.Format("{0} [{1}]", asString, _obj.GetType().Name);
            }
        }
        
        /// <summary>
        /// Number of logical children in the hierarchy
        /// </summary>
        public int LogicalChildCount
        { 
            get
            {
                if (_depObj == null)
                    return 0;
                return LogicalTreeHelper.GetChildren(_depObj).Cast<object>().Count();
            } 
        }
        /// <summary>
        /// Number of visual children in the hierarchy
        /// </summary>
        public int VisualChildCount 
        {
            get 
            {
                if (_depObj == null)
                    return 0;
                return VisualTreeHelper.GetChildrenCount(_depObj);
            }
        }

        /// <summary>
        /// A list of UITreeNodes, one for each unique visual/logical child of _obj
        /// </summary>
        public IEnumerable<UITreeNode> Children
        {
            get
            {
                var children = new List<UITreeNode>();

                if (_depObj == null)
                    return children;
                for (int childPos = 0; childPos < VisualTreeHelper.GetChildrenCount(_depObj); childPos++)
                    children.Add(new UITreeNode(VisualTreeHelper.GetChild(_depObj, childPos)));
                
                foreach (var logicalChild in LogicalTreeHelper.GetChildren(_depObj))
                {
                    var child = new UITreeNode(logicalChild, true);
                    if (!children.Contains(child))
                        children.Add(child);
                }
                    
                return children;
            }
        }

        /// <summary>
        /// Delegates to the cached object
        /// </summary>
        /// <returns>The numeric hashcode</returns>
        public override int GetHashCode()
        {
            return this._obj.GetHashCode();
        }

        /// <summary>
        /// Equality comparer
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>Equality comparer returns true if the parameter is same as the cached object
        /// else false</returns>
        public override bool Equals(object obj)
        {
            var treenode = obj as UITreeNode;
            if (treenode == null)
                return false;
            return Object.ReferenceEquals(this._obj, treenode._obj);
        }
    }
}

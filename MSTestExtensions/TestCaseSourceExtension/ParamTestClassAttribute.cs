/*
 * Copyright (c) 2013 Gishu Pillai (gishu AT hotmail DOT com)
 * 
 * You should have received a copy of the license - see license.txt
 * If not, please refer to it online at http://opensource.org/licenses/MIT
*/
using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCaseSourceExtension.UI;

namespace TestCaseSourceExtension
{
    [Serializable]
    public class ParamTestClassAttribute : TestClassExtensionAttribute
    {
        private readonly Uri _uri = new Uri("urn:ParamTestClassAttribute");

        public override TestExtensionExecution GetExecution()
        {
            return new ParamTestExecution();
        }

        public override Uri ExtensionId
        {
            get { return _uri; }
        }

        public override object GetClientSide()
        {
            return new ExtensionUIGrandparent();
        }
    }

    
}
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.Common;

namespace TestCaseSourceExtension.UI
{
    public class ExtensionUIGrandparent : TestTypeExtensionClientSide
    {
        public override string ExtensionName
        {
            get { return "Parameterized Test Extension"; }
        }
        public override object GetUI()
        {

            MessageBox.Show("Grandparent.GetUI");
            return new ExtensionUIParent();
        }

        public override void Initialize(ITmi tmi)
        {
            base.Initialize(tmi);
        }
    }
}
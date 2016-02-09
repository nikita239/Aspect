using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using N2;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Persistence.Serialization;
using AspectDotNet;
using N2.Edit.Web;
using N2.Engine;
using N2.Web;
using N2.Edit.Versioning;

namespace Aspect1
{
    public class Class2
    {
        private static void Method(ContentItem item, ReadingJournal journal, object id, dynamic child)
        {

            if (child != null)
                child.AddTo(item);
            else
            {
                if (id is string)
                {
                    journal.Register(id.ToString(), (ci) => ci.AddTo(item), isChild: true);
                }
                else if (id is int)
                {
                    journal.Register(int.Parse(id.ToString()), (ci) => ci.AddTo(item), isChild: true);
                }
            }
        }

        [AspectAction("%instead %call *ChildXmlReader.Handle(ContentItem, ReadingJournal, int) && args(..)")]
        public static void Handle(ContentItem item, ReadingJournal journal, int id)
        {
            var child = journal.Find(id);
            Method(item, journal, id, child);
        }

        [AspectAction("%instead %call *ChildXmlReader.Handle(ContentItem, ReadingJournal, string) && args(..)")]
        public static void Handle(ContentItem item, ReadingJournal journal, string id)
        {
            var child = journal.Find(id);
            Method(item, journal, id, child);
        }

    }
}

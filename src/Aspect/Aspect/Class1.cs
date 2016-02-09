using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N2.Edit.Installation;
using AspectDotNet;
using NHibernate;

namespace Aspect1
{
    public abstract class Class1
    {
        public const string InstallationAppPath = "Installation.AppPath";
        public const string installationHost = "Installation.Host";
        public const string installationAssemblyVersion = "Installation.AssemblyVersion";
        public const string installationFileVersion = "Installation.FileVersion";
        public const string installationFeatures = "Installation.Features";
        public const string installationImageSizes = "Installation.ImageSizes";

        [AspectAction("%instead %call *InstallationManager.UpdateRecordedValues(DatabaseStatus) && args(..)")]
        public static void UpdateRecordedValues(DatabaseStatus status)
        {
            try
            {
                if (status.RootItem == null)
                    return;

                status.AppPath = status.RootItem[InstallationAppPath] as string;
                status.NeedsRebase = !string.IsNullOrEmpty(status.AppPath) && !string.Equals(status.AppPath, N2.Web.Url.ToAbsolute("~/"));

                Version v;
                if (status.RootItem[installationAssemblyVersion] != null && Version.TryParse(status.RootItem[installationAssemblyVersion].ToString(), out v))
                    status.RecordedAssemblyVersion = v;
                if (status.RootItem[installationFileVersion] != null && Version.TryParse(status.RootItem[installationFileVersion].ToString(), out v))
                    status.RecordedFileVersion = v;

                status.RecordedFeatures = status.RootItem.GetInstalledFeatures();

                status.RecordedImageSizes = status.RootItem.GetInstalledImageSizes().ToArray();
            }
            catch (Exception ex)
            {
                status.ItemsError = ex.Message;
            }
        }

      /* [AspectAction("%instead %call *InstallationManager.UpdateSchema(DatabaseStatus) && args(..)")]
        public static bool UpdateSchema(DatabaseStatus status)
        {
            try
            {
                ISession session = sessionProvider.OpenSession.Session;

                session.CreateQuery("from ContentItem").SetMaxResults(1).List();
                session.CreateQuery("from ContentDetail").SetMaxResults(1).List();
                session.CreateQuery("from AuthorizedRole").SetMaxResults(1).List();
                session.CreateQuery("from DetailCollection").SetMaxResults(1).List();
                //session.CreateQuery("from ContentVersion").SetMaxResults(1).List();

                status.HasSchema = true;

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                status.HasSchema = false;
                status.SchemaError = ex.Message;
                status.SchemaException = ex;

                return false;
            }
        }
         }
}

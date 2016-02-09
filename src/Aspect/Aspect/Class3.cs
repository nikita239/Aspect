using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using N2.Edit.FileSystem;
using AspectDotNet;


namespace Aspect1
{
    class Class3
    {
        [AspectAction("%before %call *EventHandler<FileEventArgs>.Invoke(object, FileEventArgs) && %withincode(*VirtualPathFileSystem.MoveDirectory(string, string")]
        public static void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
        {
            string fromPath = MapPath(fromVirtualPath);
            string toPath = MapPath(destinationVirtualPath);

            try
            {
                Directory.Move(fromPath, toPath);
            }
            catch (IOException)
            {
                // retry once
                Thread.Sleep(10);
                Directory.Move(fromPath, toPath);
            }
        }
        protected static string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }

    }


}

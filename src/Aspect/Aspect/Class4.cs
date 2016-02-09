using System;
using System.Security.Principal;
using N2;
using AspectDotNet;
using N2.Engine;
using N2.Persistence;
using N2.Security;

namespace Aspect1
{
    public class Class4
    {

        public static event EventHandler<CancellableItemEventArgs> AuthorizationFailed;
        public static Logger<SecurityEnforcer> logger;
        private static ISecurityManager _security;
        private static N2.Web.IWebContext _webContext;
        private static int _permissionDeniedHttpCode;
        [AspectAction("%after %call *SecurityEnforcer.ctor(IPersister, ISecurityManager, ContentActivator, Web.IUrlParser, Web.IWebContext, Configuration.HostSection) && args(..)")]
        public static void Initialize(N2.Persistence.IPersister persister, ISecurityManager security, ContentActivator activator, N2.Web.IUrlParser urlParser, N2.Web.IWebContext webContext, N2.Configuration.HostSection config)
        {
            _webContext = webContext;
            _security = security;
            _permissionDeniedHttpCode = config.Web.PermissionDeniedHttpCode;
        }

        [AspectAction("%instead %call *SecurityEnforcer.OnItemSaving(ContentItem) && args(..)")]
        public static void OnItemSaving(ContentItem item)
        {
            if (!_security.IsAuthorized(item, _webContext.User))
            {
                logger.InfoFormat("OnItemSaving: User {0} not authorized for {1}.", _webContext.User, item);
                throw new PermissionDeniedException(item, _webContext.User);
            }
            IPrincipal user = _webContext.User;
            if (user != null)
                item.SavedBy = user.Identity.Name;
            else
                item.SavedBy = null;
        }

        [AspectAction("%instead %call *SecurityEnforcer.OnItemCopying(ContentItem, ContentItem) && args(..)")]
        public static void OnItemCopying(ContentItem source, ContentItem destination)
        {
            if (!_security.IsAuthorized(source, _webContext.User) || !_security.IsAuthorized(destination, _webContext.User))
            {
                logger.InfoFormat("OnItemCopying: User {0} not authorized for {1} to {2}.", _webContext.User, source, destination);
                throw new PermissionDeniedException(source, _webContext.User);
            }
        }

        [AspectAction("%instead %call *SecurityEnforcer.OnItemMoving(ContentItem, ContentItem) && args(..)")]
        public static void OnItemMoving(ContentItem source, ContentItem destination)
        {
            if (!_security.IsAuthorized(source, _webContext.User) || !_security.IsAuthorized(destination, _webContext.User))
            {
                logger.InfoFormat("OnItemMoving: User {0} not authorized for {1} to {2}.", _webContext.User, source, destination);
                throw new PermissionDeniedException(source, _webContext.User);
            }
        }

        [AspectAction("%instead %call *SecurityEnforcer.OnItemDeleting(ContentItem) && args(..)")]
        public static void OnItemDeleting(ContentItem item)
        {
            IPrincipal user = _webContext.User;
            if (!_security.IsAuthorized(item, user))
            {
                logger.InfoFormat("OnItemDeleting: User {0} not authorized for {1}.", _webContext.User, item);
                throw new PermissionDeniedException(item, user);
            }
        }


    }
}
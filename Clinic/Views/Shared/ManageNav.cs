using Microsoft.AspNetCore.Mvc.Rendering;

namespace Clinic.Views.Shared
{
    public static class ManageNav
    {
        public static string Home => "Home";
        public static string HomeNavClass(ViewContext viewContext) => PageNavClass(viewContext, Home);

        public static string Profile => "Profile";
        public static string ProfileNavClass(ViewContext viewContext) => PageNavClass(viewContext, Profile);

        public static string Users => "Users";
        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : "";
        }
    }
}

using LogSystem.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LogSystem.Helpers
{
    class AsynExceptionFilterHelper : IMyLog
    {
        private readonly ExceptionContext context;

        public AsynExceptionFilterHelper(ExceptionContext context)
        {
            this.context = context;
        }

        public string ContextId => context.ActionDescriptor.Id;

        public string Controller => context.RouteData.Values.ContainsKey("controller") ? context.RouteData.Values["controller"].ToString() : null;

        public string Action => context.RouteData.Values.ContainsKey("action") ? context.RouteData.Values["action"].ToString() : null;
    }
}
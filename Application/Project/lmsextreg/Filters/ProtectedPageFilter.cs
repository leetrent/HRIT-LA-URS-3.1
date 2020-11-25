using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace lmsextreg.Filters
{
    public class ProtectedPageFilter : IPageFilter
    {
        private readonly ILogger _logger;

        // string expiredCookieValue = "CfDJ8AjxBSBZ8mVMmJNAhbDkfxRgO4Yle7MlubMm0VsMxNFigMOja5B_4Sx3QUIsV3Ux-Uxnp4vde1fkR4m0jNd-C97Ar_CCgfSeqxjReo08pxEzSKaQWVVhMv3WQVN_LLx3-i2bc9SmHsP_FgWDXS7Fk92IlUzc9KDz5wcKFTI0GgIxl04v1n1yfXwr4mcB4auwSYqBXWQWYf_A-EHDqIb1_Z4hP7_j2WNae7o_xrIYPF-Digiun9k5HZOfP39kVU6b_J_J0k7p6Y5vxEMGKZXL5oZf2cDdzEkJv-B2HDIeFkLuouNqXQ6h_LXJSX4CcSGvQNTsbKFM5TBZ5EQKZreC4k8amAm-1RKLUxkAcVUgW0tnPJ72THSuYV7vb5UEqO0CJQyxkVfgRZBx92eMTEUgPclgeYRioJe_bLKbPfvR1S6v3tM0FxJcjpLj1dBAMj1OxPZWmPrBIvV1EpQZ1TBPFN29h1VdmHfroIyyvhm04e3FGyRFwTCOGgUgGgMsPDdr9PoJqBQtUNdafIVbeXOkKDBF4ywNbG7A34oNXMYfYyEJZS_iti1U98zmCPGnL13JdZUa-WRAUWkUWb7V_efpQfDaVbHUQqhwlU8F7EFwDo5Pyvd3TrjKVRAeNpuiZwqJtg80n66iHmL_0qt3YKLdqPcHz68YNlRDsZjpx_ivXnuXDVvf2tkGRXu8qVd7q2-5ng";


        public ProtectedPageFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            _logger.LogDebug("\nGlobal sync OnPageHandlerSelected called =>");
  
            //_logger.LogDebug("\nCookie has expired - redirecting to Login page");
            // context.HttpContext.Response.Redirect("/Account/Login");
            
            if ( context.HttpContext.Request.Headers != null )
            {
                foreach ( var headerKey in context.HttpContext.Request.Headers.Keys )
                {
                    _logger.LogDebug("\nHeader " + headerKey + ":");
                    _logger.LogDebug("\n'" + context.HttpContext.Request.Headers[headerKey] + "'");
                }
            }

            if ( context.HttpContext.Request.Cookies != null )
            {
                foreach (var cookieKey in context.HttpContext.Request.Cookies.Keys)
                {
                    _logger.LogDebug("\nCookie " + cookieKey + ":");
                    _logger.LogDebug("\n'" + context.HttpContext.Request.Cookies[cookieKey] + "'");

                    var cookie = context.HttpContext.Request.Cookies[cookieKey];


                    // if ( expiredCookieValue.Equals(context.HttpContext.Request.Cookies[cookieKey]) )
                    // {
                    //      _logger.LogDebug("\nCookie has expired - redirecting to Login page");
                    //     context.HttpContext.Response.Redirect("/Account/Login");
                    // }
                }
            }

            _logger.LogDebug("\n<= Global sync OnPageHandlerSelected called");
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            _logger.LogDebug("Global sync PageHandlerExecutingContext called.");
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            _logger.LogDebug("Global sync OnPageHandlerExecuted called.");
        }
    }
}
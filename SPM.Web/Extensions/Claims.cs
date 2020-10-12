using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ZDC.Web.Extensions
{
    public static class Claims
    {
        public static string GetSpecificClaim(this ClaimsIdentity claimsIdentity, string claimType)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);

            return (claim != null) ? claim.Value : string.Empty;
        }

        public static bool IsTrainingStaff(this ClaimsIdentity claimsIdentity)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == "Training");

            return claim != null && (claim.Equals("MTR") || claim.Equals("INS"));
        }

        public static bool IsStaff(this ClaimsIdentity claimsIdentity)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == "ZdcRole");

            return claim != null && (claim.Equals("ATM") || claim.Equals("DATM")
                                    || claim.Equals("TA") || claim.Equals("ATA")
                                    || claim.Equals("WM") || claim.Equals("AWM")
                                    || claim.Equals("EC") || claim.Equals("AEC")
                                    || claim.Equals("FE") || claim.Equals("AFE"));
        }

        public static bool IsSrStaff(this ClaimsIdentity claimsIdentity)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == "ZdcRole");

            return claim != null && (claim.Equals("ATM") || claim.Equals("DATM")
                                    || claim.Equals("TA") || claim.Equals("WM"));
        }
    }
}

using System;
namespace PurchaseShared.Models
{
  public static class Devise
  {
    public static string XOF = "XOF";
    public static string EUR = "EUR";
    public static string USD = "USD";
    public static string GBP = "GBP";
    public static string INR = "INR";

        public const decimal USD_CFA_Ratio = 1 / 630;
        public const decimal USD_EURO_Ration = (decimal)0.98618097;

    public static decimal ConvertCfaToUsd(decimal target)
        {
            return target * USD_CFA_Ratio;
        }

    public static decimal ConvertEuroToUsd(decimal target)
        {
            return target * USD_EURO_Ration;
        }

  }
}